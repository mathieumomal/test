using System;

namespace Etsi.Ultimate.Business
{
    interface IQualityChecks : IDisposable
    {
        /// <summary>
        /// Check the document having any tracking revisions
        /// </summary>
        /// <returns>True/False</returns>
        bool HasTrackedRevisions();

        /// <summary>
        /// Check the version in change history table
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        bool IsHistoryVersionCorrect(string versionToCheck);

        /// <summary>
        /// Check the version in cover page
        /// </summary>
        /// <param name="versionToCheck">version</param>
        /// <returns>True/False</returns>
        bool IsCoverPageVersionCorrect(string versionToCheck);

        /// <summary>
        /// Check the date in cover page
        /// </summary>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>True/False</returns>
        bool IsCoverPageDateCorrect(DateTime meetingDate);

        /// <summary>
        /// Check the year in copyright statement
        /// </summary>
        /// <returns>True/False</returns>
        bool IsCopyRightYearCorrect();

        /// <summary>
        /// Check the title & release correct in cover page
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>True/False</returns>
        bool IsTitleCorrect(string title);

        /// <summary>
        /// Check the release style for ZGSM
        /// </summary>
        /// <param name="release">Release</param>
        /// <returns>True/False</returns>
        bool IsReleaseStyleCorrect(string release);

        /// <summary>
        /// Check for Auto Numbering values present in the document
        /// </summary>
        /// <returns>True/False</returns>
        bool IsAutomaticNumberingPresent();
    }
}
