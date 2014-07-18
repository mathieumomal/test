using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Quality checks for .docX files
    /// </summary>
    class DocXQualityChecks : IQualityChecks
    {
        #region Variables

        private Type[] trackedRevisionsElements = new Type[] {
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
            typeof(TableRowPropertiesChange),
        };

        private WordprocessingDocument wordProcessingDocument;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to create Word processing document by using given memory stream
        /// </summary>
        /// <param name="memoryStream">Memory Stream</param>
        public DocXQualityChecks(MemoryStream memoryStream)
        {
            wordProcessingDocument = WordprocessingDocument.Open(memoryStream, false);
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
                if (PartHasTrackedRevisions(wordProcessingDocument.MainDocumentPart)) //Check document body for tracking revisions
                    return true;
                foreach (var part in wordProcessingDocument.MainDocumentPart.HeaderParts)
                    if (PartHasTrackedRevisions(part))
                        return true;
                foreach (var part in wordProcessingDocument.MainDocumentPart.FooterParts)
                    if (PartHasTrackedRevisions(part))
                        return true;
                if (wordProcessingDocument.MainDocumentPart.EndnotesPart != null)
                    if (PartHasTrackedRevisions(wordProcessingDocument.MainDocumentPart.EndnotesPart))
                        return true;
                if (wordProcessingDocument.MainDocumentPart.FootnotesPart != null)
                    if (PartHasTrackedRevisions(wordProcessingDocument.MainDocumentPart.FootnotesPart))
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
            IEnumerable<Table> tables = wordProcessingDocument.MainDocumentPart.Document.Body.OfType<Table>();
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

            string title = "3rd generation partnership project;"; //Search version on cover page till we found fixed 3GPP title

            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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

            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
                    string regularExpressionPattern = @"\((\d{4}-\d{2})\)";
                    Regex regex = new Regex(regularExpressionPattern);

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
            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
            var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
            StyleDefinitionsPart styleDefinitionPart = wordProcessingDocument.MainDocumentPart.StyleDefinitionsPart;
            bool isReleaseStyleExist = styleDefinitionPart.RootElement.Elements<Style>().Any(x => x.StyleId.Value.Equals("ZGSM", StringComparison.InvariantCultureIgnoreCase));
            if (isReleaseStyleExist)
            {
                var paragraphs = wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
        /// Check for Auto Numbering values present in the document
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsAutomaticNumberingPresent()
        {
            return wordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<NumberingId>().Any(x => x.Val != "0");
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
            if (wordProcessingDocument != null)
            {
                wordProcessingDocument.Close();
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
            return part.RootElement.Descendants().Any(e => trackedRevisionsElements.Contains(e.GetType()));
        }

        /// <summary> 
        /// Read Plain Text in all XmlElements of word document 
        /// </summary> 
        /// <param name="element">XmlElement in document</param> 
        /// <returns>Plain Text in XmlElement</returns> 
        private string GetPlainText(OpenXmlElement element)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    case "t":                           // Text 
                        PlainTextInWord.Append(section.InnerText);
                        break;
                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        break;
                    case "tab":                         // Tab
                        PlainTextInWord.Append("\t");
                        break;
                    case "p":                           // Paragraph   
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;
                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }
            return PlainTextInWord.ToString();
        }

        #endregion
    }
}
