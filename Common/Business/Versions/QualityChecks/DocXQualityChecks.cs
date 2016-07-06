using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Etsi.Ultimate.Business.Versions.Interfaces;

namespace Etsi.Ultimate.Business.Versions.QualityChecks
{
    /// <summary>
    /// Quality checks for .docX files
    /// </summary>
    public class DocXQualityChecks : IQualityChecks
    {
        #region Variables

        private readonly Type[] _trackedRevisionsElements =
        {
            typeof(CellDeletion),
            typeof(CellInsertion),
            typeof(CellMerge),
            typeof(CustomXmlDelRangeEnd),
            typeof(CustomXmlDelRangeStart),
            typeof(CustomXmlInsRangeEnd),
            typeof(CustomXmlInsRangeStart),
            typeof(Deleted),
            typeof(DeletedFieldCode),
            typeof(DeletedMathControl),
            typeof(DeletedRun),
            typeof(DeletedText),
            typeof(Inserted),
            typeof(InsertedMathControl),
            typeof(InsertedMathControl),
            typeof(InsertedRun),
            typeof(MoveFrom),
            typeof(MoveFromRangeEnd),
            typeof(MoveFromRangeStart),
            typeof(MoveTo),
            typeof(MoveToRangeEnd),
            typeof(MoveToRangeStart),
            typeof(MoveToRun),
            typeof(NumberingChange),
            typeof(ParagraphMarkRunPropertiesChange),
            typeof(ParagraphPropertiesChange),
            typeof(RunPropertiesChange),
            typeof(SectionPropertiesChange),
            typeof(TableCellPropertiesChange),
            typeof(TableGridChange),
            typeof(TablePropertiesChange),
            typeof(TablePropertyExceptionsChange),
            typeof(TableRowPropertiesChange)
        };

        private readonly WordprocessingDocument _wordProcessingDocument;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to create Word processing document by using given memory stream
        /// </summary>
        /// <param name="memoryStream">Memory Stream</param>
        public DocXQualityChecks(MemoryStream memoryStream)
        {
            _wordProcessingDocument = WordprocessingDocument.Open(memoryStream, false);
        }

        #endregion

        #region IQualityChecks Members

        /// <summary>
        /// Check the document having any tracking revisions
        /// </summary>
        /// <returns>True/False</returns>
        public bool HasTrackedRevisions()
        {
            try
            {
                if (PartHasTrackedRevisions(_wordProcessingDocument.MainDocumentPart)) //Check document body for tracking revisions
                    return true;
                if (_wordProcessingDocument.MainDocumentPart.HeaderParts.Any(PartHasTrackedRevisions))
                {
                    return true;
                }
                if (_wordProcessingDocument.MainDocumentPart.FooterParts.Any(PartHasTrackedRevisions))
                {
                    return true;
                }
                if (_wordProcessingDocument.MainDocumentPart.EndnotesPart != null)
                    if (PartHasTrackedRevisions(_wordProcessingDocument.MainDocumentPart.EndnotesPart))
                        return true;
                if (_wordProcessingDocument.MainDocumentPart.FootnotesPart != null)
                    if (PartHasTrackedRevisions(_wordProcessingDocument.MainDocumentPart.FootnotesPart))
                        return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check the version in change history table
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsHistoryVersionCorrect(string versionToCheck)
        {
            bool isHistoryVersionCorrect = false;
            IEnumerable<Table> tables = _wordProcessingDocument.MainDocumentPart.Document.Body.OfType<Table>();
            foreach (var table in tables)
            {
                var headerRow = table.OfType<TableRow>().FirstOrDefault();
                if (headerRow != null)
                {
                    var headerText = GetPlainText(headerRow).Replace("\r\n", String.Empty).Replace(" ", String.Empty);
                    if (headerText.Equals("Changehistory", StringComparison.InvariantCultureIgnoreCase)) //Change History table found
                    {
                        var secondRow = table.OfType<TableRow>().ToList()[1];
                        if (secondRow != null)
                        {
                            var secondRowColumns = secondRow.OfType<TableCell>().ToList();
                            int indexOfNewColumn = -1;
                            for (int i = 0; i < secondRowColumns.Count; i++)
                            {
                                var columnText = GetPlainText(secondRowColumns[i]).Replace("\r\n", String.Empty).Replace(" ", String.Empty);
                                if (columnText.Equals("New", StringComparison.InvariantCultureIgnoreCase) || columnText.Equals("NewVersion", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    indexOfNewColumn = i;
                                    break;
                                }
                            }

                            if (indexOfNewColumn >= 0)
                            {
                                var lastRow = table.OfType<TableRow>().LastOrDefault();  //Last row found in change history table
                                if (lastRow != null)
                                {
                                    var newColumnCell = lastRow.OfType<TableCell>().ToList()[indexOfNewColumn];
                                    if (newColumnCell != null)
                                    {
                                        var historyVersionNumber = GetPlainText(newColumnCell).Replace("\r\n", String.Empty).Replace(" ", String.Empty);
                                        if (historyVersionNumber.Equals(versionToCheck, StringComparison.InvariantCultureIgnoreCase)) //Version matching on last row last cell of change history table
                                        {
                                            isHistoryVersionCorrect = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isHistoryVersionCorrect;
        }

        /// <summary>
        /// Check the version in cover page
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageVersionCorrect(string versionToCheck)
        { 
            bool isCoverPageVersionCorrect = false;

            const string title = "3rd generation partnership project;"; //Search version on cover page till we found fixed 3GPP title

            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                var paragraphText = GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower();
                if (String.IsNullOrEmpty(paragraphText))
                    continue;
                else if (paragraphText.Contains(title))
                {
                    break;
                }
                else
                {
                    string versionToFind = "v" + versionToCheck;
                    if (paragraphText.Contains(versionToFind))
                    {
                        isCoverPageVersionCorrect = true;
                        break;
                    }
                }
            }

            return isCoverPageVersionCorrect;
        }

        /// <summary>
        /// Check the date in cover page
        /// </summary>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageDateCorrect(DateTime meetingDate)
        {
            bool isCoverPageDateCorrect = false;

            string title = "3rd generation partnership project;"; //Search meeting date on cover page till we found fixed 3GPP title

            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                var paragraphText = GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower();
                if (String.IsNullOrEmpty(paragraphText))
                    continue;
                else if (paragraphText.Contains(title))
                {
                    break;
                }
                else
                {
                    const string regularExpressionPattern = @"\((\d{4}-\d{2})\)";
                    var regex = new Regex(regularExpressionPattern);

                    foreach (Match match in regex.Matches(paragraphText))
                    {
                        var coverPageDate = match.Value.TrimStart('(').TrimEnd(')');
                        double coverPageYear = Convert.ToDouble(coverPageDate.Split('-')[0]);
                        double coverPageMonth = Convert.ToDouble(coverPageDate.Split('-')[1]);
                        double coverPageDateInMonths = (coverPageYear * 12) + coverPageMonth;
                        double meetingDateInMonths = (meetingDate.Year * 12) + meetingDate.Month;
                        if (((meetingDateInMonths - 2) <= coverPageDateInMonths) && (coverPageDateInMonths <= (meetingDateInMonths + 2)))
                        {
                            isCoverPageDateCorrect = true;
                            break;
                        }
                    }
                }
            }

            return isCoverPageDateCorrect;
        }

        /// <summary>
        /// Check the year in copyright statement
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsCopyRightYearCorrect()
        {
            bool isCopyRightYearCorrect = false;

            //Find the copyright year, based on the 'copyrightaddon' bookmark
            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
            foreach (var paragraph in paragraphs)
            {
                var bookMarks = paragraph.Descendants().OfType<BookmarkStart>();
                bool isCopyRightAddOnBookmarkFound = false;
                foreach (var bookMark in bookMarks)
                {
                    if (bookMark.Name.ToString().ToLower().Equals("copyrightaddon"))
                    {
                        isCopyRightAddOnBookmarkFound = true;
                        break;
                    }
                }

                if (isCopyRightAddOnBookmarkFound)
                {
                    var paragraphText = GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower();
                    if (DateTime.UtcNow.Month == 1)
                    {
                        if (paragraphText.Contains("© " + (DateTime.UtcNow.Year - 1)))
                        {
                            isCopyRightYearCorrect = true;
                            break;
                        }
                    }

                    if (paragraphText.Contains("© " + DateTime.UtcNow.Year))
                    {
                        isCopyRightYearCorrect = true;
                        break;
                    }
                }                
            }

            return isCopyRightYearCorrect;
        }

        /// <summary>
        /// Check for first two lines of fixed title as below
        ///     3rd Generation Partnership Project;
        ///     Technical Specification Group [TSG Title];
        /// </summary>
        /// <param name="tsgTitle">Technical Specification Group Title</param>
        /// <returns>True/False</returns>
        public bool IsFirstTwoLinesOfTitleCorrect(string tsgTitle)
        {
            string firstTwoLinesOfTitle = String.Format("3rd Generation Partnership Project;{0};", tsgTitle);
            bool isFirstTwoLinesOfTitleCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

            bool page1BookmarkStart = false;
            bool page2BookmarkStart = false;
            StringBuilder page1Text = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                var bookMarkStart = paragraph.Descendants().OfType<BookmarkStart>();

                foreach (var bookMark in bookMarkStart)
                {
                    if (bookMark.Name.ToString().ToLower().Equals("page1"))
                        page1BookmarkStart = true;
                    if (bookMark.Name.ToString().ToLower().Equals("page2"))
                        page2BookmarkStart = true;
                }

                if (page1BookmarkStart)
                    page1Text.AppendLine(GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower());

                if (page2BookmarkStart)
                    break;
            }

            string formattedPage = page1Text.ToString().Replace("\r\n", String.Empty).Replace(" ", String.Empty);
            if (formattedPage.Contains(firstTwoLinesOfTitle.Replace(" ", String.Empty).ToLower()))
                isFirstTwoLinesOfTitleCorrect = true;

            return isFirstTwoLinesOfTitleCorrect;
        }

        /// <summary>
        /// Check the title & release correct in cover page
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>True/False</returns>
        public bool IsTitleCorrect(string title)
        {
            bool isTitleCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

            bool page1BookmarkStart = false;
            bool page2BookmarkStart = false;
            StringBuilder page1Text = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                var bookMarkStart = paragraph.Descendants().OfType<BookmarkStart>();

                foreach (var bookMark in bookMarkStart)
                {
                    if (bookMark.Name.ToString().ToLower().Equals("page1"))
                        page1BookmarkStart = true;
                    if (bookMark.Name.ToString().ToLower().Equals("page2"))
                        page2BookmarkStart = true;
                }

                if (page1BookmarkStart)
                    page1Text.AppendLine(GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower());

                if (page2BookmarkStart)
                    break;
            }

            string formattedPage = page1Text.ToString().Replace("\r\n", String.Empty).Replace(" ", String.Empty);
            if (formattedPage.Contains(title.Replace(" ", String.Empty).ToLower()))
                isTitleCorrect = true;

            return isTitleCorrect;
        }

        /// <summary>
        /// Check the release is correct in cover page
        /// </summary>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        public bool IsReleaseCorrect(string release)
        {
            bool isReleaseCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

            bool page1BookmarkStart = false;
            bool page2BookmarkStart = false;
            StringBuilder page1Text = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                var bookMarkStart = paragraph.Descendants().OfType<BookmarkStart>();

                foreach (var bookMark in bookMarkStart)
                {
                    if (bookMark.Name.ToString().ToLower().Equals("page1"))
                        page1BookmarkStart = true;
                    if (bookMark.Name.ToString().ToLower().Equals("page2"))
                        page2BookmarkStart = true;
                }

                if (page1BookmarkStart)
                    page1Text.AppendLine(GetPlainText(paragraph).Replace("\r\n", String.Empty).Trim().ToLower());

                if (page2BookmarkStart)
                    break;
            }

            string formattedPage = page1Text.ToString().Replace("\r\n", String.Empty).Replace(" ", String.Empty);
            string title = "3rd generation partnership project;";
            string releaseText = "(" + release + ")";
            if (formattedPage.Contains(title.Replace(" ", String.Empty).ToLower()))
            {
                int indexOfTitle = formattedPage.IndexOf(title.Replace(" ", String.Empty).ToLower());
                int indexOfRelease = formattedPage.IndexOf(releaseText.Replace(" ", String.Empty).ToLower());
                if (indexOfTitle < indexOfRelease)
                    isReleaseCorrect = true;

            }
            else if (formattedPage.Contains(releaseText.Replace(" ", String.Empty).ToLower()))
                isReleaseCorrect = true;

            return isReleaseCorrect;
        }

        /// <summary>
        /// Check the release style for ZGSM
        /// </summary>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        public bool IsReleaseStyleCorrect(string release)
        {
            bool isReleaseStyleCorrect = false;
            StyleDefinitionsPart styleDefinitionPart = _wordProcessingDocument.MainDocumentPart.StyleDefinitionsPart;
            bool isReleaseStyleExist = styleDefinitionPart.RootElement.Elements<Style>().Any(x => x.StyleId.Value.Equals("ZGSM", StringComparison.InvariantCultureIgnoreCase));
            if (isReleaseStyleExist)
            {
                var paragraphs = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
                StringBuilder zgsmText = new StringBuilder();
                foreach (var paragraph in paragraphs)
                {
                    paragraph.Descendants().OfType<Run>().Where(x => x.Descendants().OfType<RunStyle>().Any(y => y.Val == "ZGSM")).ToList().ForEach(z => zgsmText.Append(GetPlainText(z)));
                    if (!String.IsNullOrWhiteSpace(zgsmText.ToString()))
                        break;
                }

                if (zgsmText.ToString().Equals(release, StringComparison.InvariantCultureIgnoreCase))
                    isReleaseStyleCorrect = true;
            }

            return isReleaseStyleCorrect;
        }

        /// <summary>
        /// Check for Auto Numbering values present in the document (only Decimal, LowerLetter, LowerRoman lists)
        /// Remark: a docx file could be unzip and contains multiple xml files. Two of them are document.xml and numbering.xml, used here. 
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsAutomaticNumberingPresent()
        {
            //--- DOCUMENT.XML file

            //Find all lists inside document and the related list style id : NUMBERING ID (ex: <w:numId w:val="3"/>)
            var numberedIdsUsed = _wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<NumberingId>().Where(x => x.Val != "0").Select(x => x.Val.ToString()).Distinct().ToList();
            if (numberedIdsUsed.Count == 0)
                return false;


            //--- NUMBERING.XML file

            //Get access to the numbering.xml content
            var numberingDefinitionsPart = _wordProcessingDocument.MainDocumentPart.GetPartsOfType<NumberingDefinitionsPart>().FirstOrDefault();
            if (numberingDefinitionsPart == null)
                return false;

            //INFO : A list inside document.xml have a kind of id (NumberingId, ex: <w:numId w:val="3"/>) 
            //and this id is related to another id inside numbering.xml file (AbstractNumId, ex: <w:abstractNumId w:val="0"/>).
            //Relation is done inside a specific tag inside numbering.xml file (NumberingInstance, ex: <w:num w:numId="3">...</w:num>). 
            //Please find an example of an numbering instance (num)
            //<w:num w:numId="3">
            //  <w:abstractNumId w:val="0"/>
            //</w:num>
            //Finally style is define inside a last tag : AbstractNum identified thanks to the AbstractNumberId.
            //Please find an example of AbstractNum:
            //<w:abstractNum w:abstractNumId="0">
            //  <w:nsid w:val="52887D4E"/>
            //  <w:multiLevelType w:val="hybridMultilevel"/>
            //  <w:tmpl w:val="3522A828"/>
            //  <w:lvl w:tplc="D1BCAD1A" w:ilvl="0">
            //      <w:start w:val="1"/>
            //      <w:numFmt w:val="bullet"/> --------------- HERE YOU CAN FIND THE TYPE OF LIST FOR LVL 1
            //      <w:lvlText w:val="-"/>
            //...
            //At the end, if numFmt <=> NumberingFormat is not Decimal, LowerLetter or LowerRoman we can consider that this is not a numbered list but a bullet list 
            //(be careful, this is most common numbered lis type, but other exist)

            //Get Numbering instance (ex: <w:num w:numId="3">) related to numberedIds found before
            var nums = numberingDefinitionsPart.RootElement.Descendants()
                .OfType<NumberingInstance>()
                .Where(x => numberedIdsUsed.Contains(x.NumberID.ToString()))
                .ToList();

            //Get AbstractNumId found inside Numbering Instance (ex: <w:abstractNumId w:val="0"/>)
            var abstractNumIds = nums.Select(ani => ani.Descendants().OfType<AbstractNumId>().First().Val.ToString()).ToList();

            //Check if abstract num define numbered list... return true if yes, else false
            var abstractNums = numberingDefinitionsPart.Numbering.Descendants()
                .OfType<AbstractNum>().Where(x => abstractNumIds.Contains(x.AbstractNumberId.ToString())).ToList();
            foreach (var an in abstractNums)
            {
                var levels = an.ChildElements.OfType<Level>().ToList();
                foreach (var level in levels)
                {
                    var numFmt = level.ChildElements.OfType<NumberingFormat>().FirstOrDefault();
                    if(numFmt == null)
                        continue;
                    if (numFmt.Val == NumberFormatValues.Decimal || numFmt.Val == NumberFormatValues.LowerLetter ||
                        numFmt.Val == NumberFormatValues.LowerRoman)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check for Annexure Headings having correct styles or not
        /// If TS, Style should be 'Heading 8'
        ///      , text '(normative)/(informative)' allowed after Annexure heading
        /// If TR, Style should be 'Heading 9'
        ///      , no text allowed after Annexure heading
        /// </summary>
        /// <param name="isTechnicalSpecification">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>True/False</returns>
        public bool IsAnnexureStylesCorrect(bool isTechnicalSpecification)
        {
            return true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose word application from memory
        /// </summary>
        public void Dispose()
        {
            // close word and dispose reference
            if (_wordProcessingDocument != null)
            {
                _wordProcessingDocument.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check for tracked revisions
        /// </summary>
        /// <param name="part">XmlElement in document</param>
        /// <returns>True/False</returns>
        private bool PartHasTrackedRevisions(OpenXmlPart part)
        {
            return part.RootElement.Descendants().Any(e => _trackedRevisionsElements.Contains(e.GetType()));
        }

        /// <summary> 
        /// Read Plain Text in all XmlElements of word document 
        /// </summary> 
        /// <param name="element">XmlElement in document</param> 
        /// <returns>Plain Text in XmlElement</returns> 
        private string GetPlainText(OpenXmlElement element)
        {
            var plainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    case "t":                           // Text 
                        plainTextInWord.Append(section.InnerText);
                        break;
                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        plainTextInWord.Append(Environment.NewLine);
                        break;
                    case "tab":                         // Tab
                        plainTextInWord.Append("\t");
                        break;
                    case "p":                           // Paragraph   
                        plainTextInWord.Append(GetPlainText(section));
                        plainTextInWord.AppendLine(Environment.NewLine);
                        break;
                    default:
                        plainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }
            return plainTextInWord.ToString();
        }

        #endregion
    }
}
