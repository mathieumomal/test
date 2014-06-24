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
        /// Check for first two lines of fixed title as below
        ///     3rd Generation Partnership Project;
        ///     Technical Specification Group [TSG Title];
        /// </summary>
        /// <param name="tsgTitle">Technical Specification Group Title</param>
        /// <returns>True/False</returns>
        bool IsFirstTwoLinesOfTitleCorrect(string tsgTitle);

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

        /// <summary>
        /// Check for Annexure Headings having correct styles or not
        /// If TS, Style should be 'Heading 8'
        ///      , text '(normative)/(informative)' allowed after Annexure heading
        /// If TR, Style should be 'Heading 9'
        ///      , no text allowed after Annexure heading
        /// </summary>
        /// <param name="isTechnicalSpecification">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>True/False</returns>
        bool IsAnnexureStylesCorrect(bool isTechnicalSpecification);
    }
}
