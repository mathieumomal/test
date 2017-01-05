namespace Etsi.Ultimate.Business.Versions.QualityChecks
{
    /*
     * /// <summary>
    /// Doc Quality checks which used NetOffice library. 
    /// Removed and replace by docX quality checks (convert doc to docx) for performance reasons
    /// CODE just kept for information !
    /// </summary>
    public class DocQualityChecks : IQualityChecks
    {
        #region Variables

        private Application wordApplication;
        private Document wordDocument;
        private string documentName = String.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to create Word document by using given memory stream
        /// </summary>
        /// <param name="memoryStream">Memory Stream</param>
        /// <param name="temporaryFolder">Temporary Folder</param>
        public DocQualityChecks(MemoryStream memoryStream, string temporaryFolder)
        {
            //Save stream in temporary location
            byte[] bytes = memoryStream.ToArray();
            var tempId = Guid.NewGuid().ToString().Substring(0,8);
            documentName = Path.Combine(temporaryFolder, "tempword_" + tempId + ".doc");
            if (File.Exists(documentName))
                File.Delete(documentName);
            var fs = new FileStream(documentName, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            wordApplication = new Application {DisplayAlerts = WdAlertLevel.wdAlertsNone};
            wordDocument = wordApplication.Documents.Open(documentName, false, true);
        }

        #endregion

        #region IQualityChecks Members

        /// <summary>
        /// Check the document having any tracking revisions
        /// </summary>
        /// <returns>True/False</returns>
        public bool HasTrackedRevisions()
        {
            bool hasTrackedRevisions = false;

            //Check revisions on all doc sections
            foreach (var section in wordDocument.Sections)
            {
                //If nothing found in main document, search in headers & footers 
                if (section.Range.Revisions.Count > 0)
                {
                    hasTrackedRevisions = true;
                    break;
                }
                if (section.Headers.Any(header => header.Range.Revisions.Count > 0))
                {
                    hasTrackedRevisions = true;
                }
                if (section.Footers.Any(footer => footer.Range.Revisions.Count > 0))
                {
                    hasTrackedRevisions = true;
                }
            }

            return hasTrackedRevisions;
        }

        /// <summary>
        /// Check the version in change history table
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsHistoryVersionCorrect(string versionToCheck)
        {
            bool isHistoryVersionCorrect = false;

            //Check history table in last section
            var section = wordDocument.Sections.Last;
            if (section.Range.Tables.Count > 0)
            {
                //Change history should be last table
                var table = section.Range.Tables[section.Range.Tables.Count];

                Range range = table.Range;
                if (range.Cells.Count > 0)
                {
                    var headerText = ReplaceSpecialCharacters(range.Cells[1].Range.Text, true);
                    if (headerText.Equals("Changehistory", StringComparison.InvariantCultureIgnoreCase)) //Change History table found
                    {
                        int indexOfNewVersionCell = 1;
                        for (int i = 1; i <= range.Cells.Count; i++)
                        {
                            if (range.Cells[i].RowIndex == 2)
                            {
                                if (ReplaceSpecialCharacters(range.Cells[i].Range.Text, true).Equals("New", StringComparison.InvariantCultureIgnoreCase)
                                    || ReplaceSpecialCharacters(range.Cells[i].Range.Text, true).Equals("NewVersion", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    indexOfNewVersionCell = range.Cells[i].ColumnIndex;
                                }
                            }
                            if ((range.Cells[i].RowIndex == table.Rows.Count) && (range.Cells[i].ColumnIndex == indexOfNewVersionCell))
                            {
                                var historyVersionNumber = ReplaceSpecialCharacters(range.Cells[i].Range.Text, true);
                                if (historyVersionNumber.Equals(versionToCheck, StringComparison.InvariantCultureIgnoreCase))
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
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageVersionCorrect(string versionToCheck)
        {
            bool isCoverPageVersionCorrect = false;
            string title = "3rd generation partnership project;"; //Search version on cover page till we found fixed 3GPP title
            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string sectionText = section.Range.Text.ToLower();
                string versionToFind = "v" + versionToCheck;
                int titleIndex = sectionText.IndexOf(title);

                if (sectionText.Contains(versionToFind))
                {
                    int versionIndex = sectionText.IndexOf(versionToFind);
                    isCoverPageVersionCorrect = (versionIndex < titleIndex);
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
            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string sectionText = section.Range.Text.ToLower();
                int titleIndex = sectionText.IndexOf(title);

                const string regularExpressionPattern = @"\((\d{4}-\d{2})\)";
                var regex = new Regex(regularExpressionPattern);
                
                foreach (Match match in regex.Matches(sectionText))
                {
                    int matchIndex = sectionText.IndexOf(match.Value);
                    if (matchIndex < titleIndex)
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
                    else
                        break;
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

            bool isCopyRightAddOnBookmarkFound = false;
            int textLength = 0;
            foreach (var section in wordDocument.Sections)
            {
                foreach (var bookmark in section.Range.Bookmarks)
                {
                    if (bookmark.Name.ToLower().Equals("copyrightaddon"))
                    {
                        isCopyRightAddOnBookmarkFound = true;
                        int copyRightNotificationIndex = section.Range.Text.IndexOf("copyright notification", StringComparison.InvariantCultureIgnoreCase);
                        if (copyRightNotificationIndex > 0 && (copyRightNotificationIndex + textLength) < bookmark.Start) 
                        {
                            string copyRightText = section.Range.Text.Substring(copyRightNotificationIndex, bookmark.Start - (copyRightNotificationIndex + textLength));
                            if (DateTime.UtcNow.Month == 1)
                            {
                                if (copyRightText.Contains("© " + (DateTime.UtcNow.Year - 1)))
                                {
                                    isCopyRightYearCorrect = true;
                                    break;
                                }
                            }

                            if (copyRightText.Contains("© " + DateTime.UtcNow.Year))
                            {
                                isCopyRightYearCorrect = true;
                            }
                        }
                        break;
                    }
                }
                if (isCopyRightAddOnBookmarkFound)
                    break;
                textLength += section.Range.Text.Length;
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

            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string sectionText = ReplaceSpecialCharacters(section.Range.Text, true);
                if (sectionText.ToLower().Contains(firstTwoLinesOfTitle.Replace(" ", String.Empty).ToLower()))
                    isFirstTwoLinesOfTitleCorrect = true;
            }

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

            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string sectionText = ReplaceSpecialCharacters(section.Range.Text, true);
                if (sectionText.ToLower().Contains(title.Replace(" ", String.Empty).ToLower()))
                    isTitleCorrect = true;
            }

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

            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string releaseText = "(" + release + ")";
                int indexOf3GppTitle = section.Range.Text.IndexOf("3rd generation partnership project;", StringComparison.InvariantCultureIgnoreCase);
                int indexOfRelease = section.Range.Text.IndexOf(releaseText, StringComparison.InvariantCultureIgnoreCase);

                if (indexOf3GppTitle < indexOfRelease)
                {
                    Range releaseRange = wordDocument.Range(indexOfRelease + 1, release.Length + indexOfRelease + 1);
                    if ((releaseRange != null) && releaseRange.Text.Equals(release, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isReleaseCorrect = true;
                    }
                }
            }

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

            var section = wordDocument.Sections.First;
            if (section != null)
            {
                string releaseText = "(" + release + ")";
                int indexOf3GppTitle = section.Range.Text.IndexOf("3rd generation partnership project;", StringComparison.InvariantCultureIgnoreCase);
                int indexOfRelease = section.Range.Text.IndexOf(releaseText, StringComparison.InvariantCultureIgnoreCase);

                if (indexOf3GppTitle < indexOfRelease)
                {
                    Range releaseRange = wordDocument.Range(indexOfRelease + 1, release.Length + indexOfRelease + 1);
                    if ((releaseRange != null) && releaseRange.Text.Equals(release, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Style releaseStyle = (Style)releaseRange.Style;

                        if ((releaseStyle != null) && (releaseStyle.NameLocal == "ZGSM"))
                        {
                            isReleaseStyleCorrect = true;
                        }
                    }
                }
            }

            return isReleaseStyleCorrect;
        }

        /// <summary>
        /// Check for Auto Numbering values present in the document
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsAutomaticNumberingPresent()
        {
            return wordDocument.Paragraphs.Any(p => p.Range.ListFormat.ListType == WdListType.wdListSimpleNumbering);
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
            bool isAnnexureStyleCorrect = true;

            if (wordDocument.TablesOfContents.Count > 0)
            {
                var tocRange = wordDocument.TablesOfContents[1].Range;
                foreach (var paragraph in tocRange.Paragraphs)
                {
                    string paragraphText = ReplaceSpecialCharacters(paragraph.Range.Text, true);
                    if (paragraphText.StartsWith("Annex"))
                    {
                        if (isTechnicalSpecification)
                        {
                            Style style = (Style)paragraph.Range.Style;
                            Regex tsRegex = new Regex(@"(annex\w{1}\(normative\):)|(annex\w{1}\(informative\):)");
                            if (!(style != null && style.NameLocal == "TOC 8" && tsRegex.IsMatch(paragraphText.ToLower())))
                            {
                                isAnnexureStyleCorrect = false;
                                break;
                            }
                        }
                        else
                        {
                            Style style = (Style)paragraph.Range.Style;
                            Regex trRegex = new Regex(@"annex\w{1}:");
                            if (!(style != null && style.NameLocal == "TOC 9" && trRegex.IsMatch(paragraphText.ToLower())))
                            {
                                isAnnexureStyleCorrect = false;
                                break;
                            }
                        }
                    }
                }
            }
            return isAnnexureStyleCorrect;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose word application from memory
        /// </summary>
        public void Dispose()
        {
            // close word and dispose reference
            if (wordApplication != null)
            {
                wordDocument.Close();
                wordApplication.Quit();
                wordApplication.Dispose();
            }

            if (File.Exists(documentName))
                File.Delete(documentName);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Provide plain text by removing special characters ( \r, \n, \a ) 
        /// </summary>
        /// <param name="inputText">Input Text</param>
        /// <param name="removeEmptySpace">Remove Empty Space</param>
        /// <returns>Output Text</returns>
        private string ReplaceSpecialCharacters(string inputText, bool removeEmptySpace)
        {
            string outputText = inputText.Replace("\r", String.Empty)
                                         .Replace("\n", String.Empty)
                                         .Replace("\t", String.Empty)
                                         .Replace("\a", String.Empty)
                                         .Replace("", String.Empty)
                                         .Replace("", String.Empty);
            if(removeEmptySpace)
                outputText = outputText.Replace(" ", String.Empty);

            return outputText;
        }

        #endregion
    }
     * */
}
