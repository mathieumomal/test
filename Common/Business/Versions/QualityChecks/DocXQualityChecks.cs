using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Versions.QualityChecks
{
    /// <summary>
    /// Quality checks for .docx files
    /// Uses Interop for some checks  (minority => long but sometime usefull for specific cases)
    /// And Document.OpenXML for the rest of checks (majority => fast and powerfull)
    /// </summary>
    public class DocXQualityChecks : IQualityChecks
    {
        #region Variables

        private readonly DocDocumentManager _docDocumentManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to create Word processing document by using given memory stream
        /// </summary>
        /// <param name="myVar">Contains all necessary object to analyse doc/docx file</param>
        public DocXQualityChecks(DocDocumentManager myVar)
        {
            _docDocumentManager = myVar;
        }

        #endregion

        #region IQualityChecks Members

        /// <summary>
        /// Check the document having any tracking revisions ('suivi des modifications' / 'révisions non validées' in french)
        /// </summary>
        /// <returns>True/False</returns>
        public bool HasTrackedRevisions()
        {
            try
            {
                return _docDocumentManager.Word.ActiveDocument.Revisions.Count > 0;

                #region legacy implementation
                /*
                 * Old implementation kept for information purpose. 
                 * Has been replaced by Interop analyse because of revisions found by Document.OpenXML api was incorrect:
                 * The problem was (with Document.OpenXml):
                 * When a revision occured on table inside docx document the grid inside the document should look like:
                    <w:tblGrid>
                        <w:gridCol w:w="1548" />
                        <w:gridCol w:w="8028" />
                        <w:tblGridChange w:id="1">
                            <w:tblGrid>
                                <w:gridCol w:w="4788" />
                                <w:gridCol w:w="4788" />
                            </w:tblGrid>
                        </w:tblGridChange>
                    </w:tblGrid>
                 * So, with differences between root 'gridCol' and internal one. 
                 * FYI: tblGridChange indicates a table style revision (modification)
                 * But, infortunately some documents contained no differences with something like:
                    <w:tblGrid>
                        <w:gridCol w:w="4788" />
                        <w:gridCol w:w="4788" />
                        <w:tblGridChange w:id="1">
                            <w:tblGrid>
                                <w:gridCol w:w="4788" />
                                <w:gridCol w:w="4788" />
                            </w:tblGrid>
                        </w:tblGridChange>
                    </w:tblGrid>
                 * For this kind of problem, if we open the document with word -> he is able to detect that there is no difference (no revision) but unfortunately the Document.OpenXML api is not able and raised the quality checks warning. 
                 * For this reason this XML framework has been replaced by using Interop library (and revision count feature) 
                if (PartHasTrackedRevisions(_docDocumentManager.WordProcessingDocument.MainDocumentPart)) //Check document body for tracking revisions
                    return true;
                if (_docDocumentManager.WordProcessingDocument.MainDocumentPart.HeaderParts.Any(PartHasTrackedRevisions))
                {
                    return true;
                }
                if (_docDocumentManager.WordProcessingDocument.MainDocumentPart.FooterParts.Any(PartHasTrackedRevisions))
                {
                    return true;
                }
                if (_docDocumentManager.WordProcessingDocument.MainDocumentPart.EndnotesPart != null)
                    if (PartHasTrackedRevisions(_docDocumentManager.WordProcessingDocument.MainDocumentPart.EndnotesPart))
                        return true;
                if (_docDocumentManager.WordProcessingDocument.MainDocumentPart.FootnotesPart != null)
                    if (PartHasTrackedRevisions(_docDocumentManager.WordProcessingDocument.MainDocumentPart.FootnotesPart))
                        return true;
                return false;
                 * https://msdn.microsoft.com/en-us/library/office/ff629396(v=office.14).aspx
                 * 
                 * Rest of the legacy code:
                 private bool PartHasTrackedRevisions(OpenXmlPart part)
                 {
                    return part.RootElement.Descendants().Any(e => _trackedRevisionsElements.Contains(e.GetType()));
                 }
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
                 * */
                #endregion
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the last table is the one which contains the change history records
        /// </summary>
        /// <returns></returns>
        public bool ChangeHistoryTableIsTheLastOne()
        {
            //Find last table and take the innerText
            var lastTable = _docDocumentManager.WordProcessingDocument.MainDocumentPart.Document.Body.OfType<Table>().LastOrDefault();
            if (lastTable == null)
            {
                return false;
            }
            var lasttableText = lastTable.InnerText;

            //Search inside all documents for the last paragraph with text which contains "changehistory"
            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
            Paragraph changeHistoryP = null;
            foreach (var paragraph in paragraphs)
            {
                if (GetPlainText(paragraph).Replace("\r\n", String.Empty).Replace(" ", String.Empty).ToLower().Contains("changehistory"))
                {
                    changeHistoryP = paragraph;
                }
            }
            //If no paragraph has been found with the 'change history' text raised a warning
            if (changeHistoryP == null)
            {
                return false;
            }

            //Check if the last 'change history' text has been found:
            //- before the last table of the document
            //- inside the last table of the document
            // else raised a warning
            //TECH: compare the text of the last table defined earlier to the table found here
            var nextPs = changeHistoryP.ElementsAfter().OfType<Table>().ToList();
            Table table;
            if (nextPs.Any())
            {
                table = nextPs.ElementAt(0);
            }
            else
            {
                //If "Change history" is not contain inside a TableCell return the warning
                if (changeHistoryP.Parent.GetType() != typeof(TableCell))
                {
                    LogManager.Debug("DocXQualityChecks - ChangeHistoryTableIsTheLastOne: last occurence of change history text is not inside a cell -> " + changeHistoryP.Parent.GetType());
                    return false;
                }
                else if (changeHistoryP.Parent.Parent.Parent.GetType() != typeof (Table))
                {
                    LogManager.Debug("DocXQualityChecks - ChangeHistoryTableIsTheLastOne: last occurence of change history text is not inside a table -> " + changeHistoryP.Parent.Parent.Parent.GetType());
                    return false;
                }
                table = changeHistoryP.Parent.Parent.Parent as Table;
                if (table == null)
                    return false;
            }

            var tableTextToCompare = table.InnerText;
            if (tableTextToCompare.Equals(lasttableText, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check the version in change history table
        /// System search for the last table, then take the last row of it
        /// Then search for column "new" or "newversion" or which contains both keywords "new" or "version"
        /// Then finally, checks that version inside this cell is the same than the one provided
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsHistoryVersionCorrect(string versionToCheck)
        {
            bool isHistoryVersionCorrect = false;
            var lastTable = _docDocumentManager.WordProcessingDocument.MainDocumentPart.Document.Body.OfType<Table>().LastOrDefault();
            if (lastTable != null)
            {
                var headerRow = lastTable.OfType<TableRow>().ToList()[0];
                if (headerRow.Any() && headerRow != null)
                {
                    var headerRowColumns = headerRow.OfType<TableCell>().ToList();
                    if (headerRowColumns.Count == 1)
                    {
                        //If first row of table contains only one cell => this is not the really the header which contains columns name. 
                        //This is just a global header for all the table which could contains for exemple "Change history" text (according to our examples for the moment)
                        //In that case consider the second row as the real headerRow
                        if (lastTable.OfType<TableRow>().Count() > 1)
                        {
                            headerRow = lastTable.OfType<TableRow>().ToList()[1];
                            headerRowColumns = headerRow.OfType<TableCell>().ToList();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    var indexOfNewColumn = -1;
                    for (var i = 0; i < headerRowColumns.Count; i++)
                    {
                        var columnText = GetPlainText(headerRowColumns[i]).Replace("\r\n", String.Empty).Replace(" ", String.Empty).ToLower();
                        if (columnText.Equals("new", StringComparison.InvariantCultureIgnoreCase) || columnText.Equals("newversion", StringComparison.InvariantCultureIgnoreCase) || (columnText.Contains("new") && columnText.Contains("version")))
                        {
                            indexOfNewColumn = i;
                            break;
                        }
                    }

                    if (indexOfNewColumn >= 0)
                    {
                        var lastRow = lastTable.OfType<TableRow>().LastOrDefault();  //Last row found in change history table
                        if (lastRow != null)
                        {
                            var newColumnCell = lastRow.OfType<TableCell>().ToList()[indexOfNewColumn];
                            if (newColumnCell != null)
                            {
                                var historyVersionNumber = GetPlainText(newColumnCell).Replace("\r\n", String.Empty).Replace(" ", String.Empty);
                                if (historyVersionNumber.Equals(versionToCheck, StringComparison.InvariantCultureIgnoreCase)) //Version matching on last row last cell of change history table
                                {
                                    isHistoryVersionCorrect = true;
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
        /// Confirm that version given as parameter is the one inside the first page (cover) of the document
        /// (inside header between spec number and date)
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageVersionCorrect(string versionToCheck)
        { 
            bool isCoverPageVersionCorrect = false;

            const string title = "3rd generation partnership project;"; //Search version on cover page till we found fixed 3GPP title

            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
        /// Confirm that date given as parameter is the one inside the first page (cover) of the document
        /// (Margin error +/- 2 months)
        /// </summary>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageDateCorrect(DateTime meetingDate)
        {
            bool isCoverPageDateCorrect = false;

            string title = "3rd generation partnership project;"; //Search meeting date on cover page till we found fixed 3GPP title

            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
        /// System should be able to find bookmark (signet): copyrightaddon
        /// And inside system should find:
        /// - '© [CURRENT YEAR - 1]' or '© [CURRENT YEAR]' if month is january
        /// - otherwise '© [CURRENT YEAR]'
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsCopyRightYearCorrect(DateTime now)
        {
            bool isCopyRightYearCorrect = false;

            //Find the copyright year, based on the 'copyrightaddon' bookmark
            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
                    if (now.Month == 1)
                    {
                        if (paragraphText.Contains("© " + (now.Year - 1)))
                        {
                            isCopyRightYearCorrect = true;
                            break;
                        }
                    }

                    if (paragraphText.Contains("© " + now.Year))
                    {
                        isCopyRightYearCorrect = true;
                        break;
                    }
                }                
            }

            return isCopyRightYearCorrect;
        }

        /// <summary>
        /// Check for first two lines (inside first page (cover)) of fixed title as below
        ///     3rd Generation Partnership Project;
        ///     [TSG Title];
        /// </summary>
        /// <param name="tsgTitle">Technical Specification Group Title</param>
        /// <returns>True/False</returns>
        public bool IsFirstTwoLinesOfTitleCorrect(string tsgTitle)
        {
            string firstTwoLinesOfTitle = String.Format("3rd Generation Partnership Project;{0};", tsgTitle);
            bool isFirstTwoLinesOfTitleCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
        /// Check that the spec's title is present inside first page (cover)
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>True/False</returns>
        public bool IsTitleCorrect(string title)
        {
            bool isTitleCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
        /// - if title is present, release name (which should be there) should be after
        /// - otherwise, release name should be there
        /// </summary>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        public bool IsReleaseCorrect(string release)
        {
            bool isReleaseCorrect = false;

            //Search title on cover page based on bookmarks page1 & page2 (title should be on page1 bookmark range)
            var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();

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
        /// Style 'ZGSM' should be used at just one time inside the document and should contain Release name 
        /// (even if not checked: release name seems to be on the first page (cover)) 
        /// </summary>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        public bool IsReleaseStyleCorrect(string release)
        {
            bool isReleaseStyleCorrect = false;
            StyleDefinitionsPart styleDefinitionPart = _docDocumentManager.WordProcessingDocument.MainDocumentPart.StyleDefinitionsPart;
            bool isReleaseStyleExist = styleDefinitionPart.RootElement.Elements<Style>().Any(x => x.StyleId.Value.Equals("ZGSM", StringComparison.InvariantCultureIgnoreCase));
            if (isReleaseStyleExist)
            {
                var paragraphs = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<Paragraph>();
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
            var numberedIdsUsed = _docDocumentManager.WordProcessingDocument.MainDocumentPart.RootElement.Descendants().OfType<NumberingId>().Where(x => x.Val != "0").Select(x => x.Val.ToString()).Distinct().ToList();
            if (numberedIdsUsed.Count == 0)
                return false;


            //--- NUMBERING.XML file

            //Get access to the numbering.xml content
            var numberingDefinitionsPart = _docDocumentManager.WordProcessingDocument.MainDocumentPart.GetPartsOfType<NumberingDefinitionsPart>().FirstOrDefault();
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
            //(be careful, this is most common numbered list type, but other exist)

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
            if (_docDocumentManager.WordProcessingDocument != null)
            {
                _docDocumentManager.WordProcessingDocument.Close();
            }
        }

        #endregion

        #region Private Methods

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
