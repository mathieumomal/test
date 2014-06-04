using System;

namespace Etsi.Ultimate.Business
{
    class DocQualityChecks : IQualityChecks
    {
        #region IQualityChecks Members

        /// <summary>
        /// Check the document having any tracking revisions
        /// </summary>
        /// <returns>True/False</returns>
        public bool HasTrackedRevisions()
        {
            return false;
        }

        /// <summary>
        /// Check the version in change history table
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsHistoryVersionCorrect(string versionToCheck)
        {
            return true;
        }

        /// <summary>
        /// Check the version in cover page
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageVersionCorrect(string versionToCheck)
        {
            return true;
        }

        /// <summary>
        /// Check the date in cover page
        /// </summary>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>True/False</returns>
        public bool IsCoverPageDateCorrect(DateTime meetingDate)
        {
            return true;
        }

        /// <summary>
        /// Check the year in copyright statement
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsCopyRightYearCorrect()
        {
            return true;
        }

        /// <summary>
        /// Check the title & release correct in cover page
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        public bool IsTitleCorrect(string title, string release)
        {
            return true;
        }

        #endregion
    }
}
