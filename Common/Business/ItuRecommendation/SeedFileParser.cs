using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Standard implementation of the ISeedFileParser
    /// </summary>
    public class SeedFileParser : ISeedFileParser
    {
        #region ISeedFileParser

        /// <summary>
        /// Checks that file is a docx, then loops through all headers to find TS/TR numbers.
        /// Valid formats: {Clause#}TS{SpecNumber#}
        ///              : {Clause#}TR{SpecNumber#}
        ///              : {Clause#}{tab}TS{SpecNumber#}
        ///              : {Clause#}{tab}TR{SpecNumber#}               
        ///  *Space allowed before & after TS/TR
        /// </summary>
        /// <param name="filePath">Path where the file can be found.</param>
        /// <returns>
        /// A list of keyvalue pairs (clause#, spec#) strings representing all the specification numbers extracted if the parsing went well.
        /// Else, the Report should contain errors.
        /// </returns>
        public ServiceResponse<List<KeyValuePair<string, string>>> ExtractSpecificationNumbersFromSeedFile(string filePath)
        {
            var serviceResponse = new ServiceResponse<List<KeyValuePair<string, string>>>();
            var specList = new List<KeyValuePair<string, string>>();

            if (!File.Exists(filePath))
            {
                serviceResponse.Result = new List<KeyValuePair<string, string>>();
                serviceResponse.Report.LogError(String.Format("Seed file '{0}' not present", Path.GetFileName(filePath)));
                return serviceResponse;
            }

            if (!Path.GetExtension(filePath).Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
            {
                serviceResponse.Result = new List<KeyValuePair<string, string>>();
                serviceResponse.Report.LogError("Seed file should be in 'docx' format");
                return serviceResponse;
            }

            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        var wordProcessingDocument = WordprocessingDocument.Open(memoryStream, false);

                        AnalyseSeedFile(wordProcessingDocument, specList);

                        serviceResponse.Result = specList;

                        fileStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("Exception while reading seed file: " + ex.Message);
                LogManager.Error(ex.StackTrace);

                serviceResponse.Result = new List<KeyValuePair<string, string>>();
                serviceResponse.Report.LogError(String.Format("Exception occured while reading seed file '{0}'", Path.GetFileName(filePath)));
            }

            return serviceResponse;
        }

        

        #endregion

        #region Private Methods

        /// <summary>
        /// Goes through the WordProcessing document to extract the clauses and specification numbers.
        /// </summary>
        /// <param name="wordProcessingDocument"></param>
        /// <param name="specList"></param>
        private void AnalyseSeedFile(WordprocessingDocument wordProcessingDocument, List<KeyValuePair<string, string>> specList)
        {
            //Process only when document contains valid heading styles
            if (
                wordProcessingDocument.MainDocumentPart.StyleDefinitionsPart.RootElement.Elements<Style>()
                    .Any(y => y.StyleId.Value.ToLower().StartsWith("titre") || y.StyleId.Value.ToLower().StartsWith("heading")))
            {
                //[1] Get all paragraphs to process
                var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

                foreach (var paragraph in paragraphs)
                {
                    //[2] Process only heading style paragraphs
                    if (
                        paragraph.Descendants()
                            .OfType<ParagraphStyleId>()
                            .Any(
                                x =>
                                    x.Val.ToString().ToLower().StartsWith("titre") ||
                                    x.Val.ToString().ToLower().StartsWith("heading")))
                    {
                        //[3] Proceed only if the paragraph contains TR/TS
                        if (paragraph.InnerText.ToLower().Contains("tr") || paragraph.InnerText.ToLower().Contains("ts"))
                        {
                            var header = paragraph.InnerText.ToLower().Split(new[] { "tr", "ts" }, StringSplitOptions.None);
                            //[4] Don't proceed if any paragraph contains more than one TR/TS
                            if (header.Count() == 2)
                            {
                                var clauseText = header[0].Trim();
                                var specText = header[1].Trim();

                                //[5] Check for valid spec number
                                var match = Regex.Match(specText, @"^[0-9]{2}\.(\w|\-)*$");
                                if (match.Success)
                                {
                                    //[6] Check for valid clause
                                    bool isValidClause = true;
                                    if (paragraph.Descendants().Any(x => x.LocalName == "tab"))
                                    {
                                        //[7] Check the text "before first tab" matching the clause number
                                        var paragraphTextWithTabCharacter = GetPlainTextWithTabCharacter(paragraph);
                                        if (clauseText.ToLower() !=
                                            paragraphTextWithTabCharacter.ToLower().Split('\t')[0].Trim())
                                            isValidClause = false;
                                    }

                                    if (isValidClause && !String.IsNullOrEmpty(clauseText) && !String.IsNullOrEmpty(specText))
                                        specList.Add(new KeyValuePair<string, string>(clauseText, specText));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the plain text with tab character as "\t".
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Plain text with tab character as "\t"</returns>
        private string GetPlainTextWithTabCharacter(OpenXmlElement element)
        {
            StringBuilder plainTextWithTabCharacter = new StringBuilder();

            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    case "t":                           // Text 
                        plainTextWithTabCharacter.Append(section.InnerText);
                        break;
                    case "tab":                         // Tab
                        plainTextWithTabCharacter.Append("\t");
                        break;
                    default:
                        plainTextWithTabCharacter.Append(GetPlainTextWithTabCharacter(section));
                        break;
                }
            }
            return plainTextWithTabCharacter.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Interface to isolate the algorithm to parse the seed file.
    /// </summary>
    public interface ISeedFileParser
    {
        /// <summary>
        /// Opens the file and parses it to extract all the specifications numbers from the headers.
        /// </summary>
        /// <param name="filePath">Path where the file can be found.</param>
        /// <returns>A list of keyvalue pairs (clause#, spec#) strings representing all the specification numbers extracted if the parsing went well.
        /// Else, the Report should contain errors.</returns>
        ServiceResponse<List<KeyValuePair<string, string>>> ExtractSpecificationNumbersFromSeedFile(string filePath);
    }
}
