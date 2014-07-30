using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionUploadAction
    {

        private const string CACHE_KEY = "VERSION_UPLOAD";

        private const string CONST_VALID_FILENAME = "{0}-{1}{2}{3}";
        private const string CONST_FTP_ARCHIVE_PATH = "{0}\\Specs\\archive\\{1}_series\\{2}\\";
        private const string CONST_FTP_LATEST_PATH = "{0}\\Specs\\latest\\{1}\\{2}_series\\";
        private const string CONST_FTP_LATEST_DRAFTS_PATH = "{0}\\Specs\\latest-drafts\\";
        private const string CONST_FTP_VERSIONS_PATH = "{0}\\Specs\\{1}\\{2}\\{3}_series\\";

        private const string CONST_QUALITY_CHECK_REVISIONMARK = "Some revision marks left unaccepted";
        private const string CONST_QUALITY_CHECK_VERSION_HISTORY = "Invalid/missing version in history";
        private const string CONST_QUALITY_CHECK_VERSION_COVERPAGE = "Invalid/missing version in cover page";
        private const string CONST_QUALITY_CHECK_DATE_COVERPAGE = "Invalid/missing date in cover page";
        private const string CONST_QUALITY_CHECK_YEAR_COPYRIGHT = "Year not valid/missing in copyright statement";
        private const string CONST_QUALITY_CHECK_TITLE_COVERPAGE = "Incorrect/missing title in cover page";
        private const string CONST_QUALITY_CHECK_RELEASE_STYLE = "Release style should be 'ZGSM' in cover page";
        private const string CONST_QUALITY_CHECK_AUTO_NUMBERING = "Automatic numbering (of clauses, figures, tables, notes, examples etc…) should be disabled in the document";
        private const string CONST_QUALITY_CHECK_FIRST_TWO_LINES_TITLE = "The first two lines of the title must be correct, according to the TSG responsible for the specification";
        private const string CONST_QUALITY_CHECK_ANNEXURE_STYLE = "Annexes should be correctly styled as Heading 8(TS) or Heading 9(TR). In case of TS, (normative) or (informative) should appear immediately after annexure heading";
        private const string CONST_QUALITY_CHECK_RELEASE = "Invalid/missing release in cover page";

        public IUltimateUnitOfWork UoW;

        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path)
        {
            var svcResponse = new ServiceResponse<string>();

            GetRelatedSpecAndRelease(version);

            svcResponse.Report = CheckPersonRightToUploadVersion(svcResponse.Report, personId);
            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                try
                {
                    Report validationReport = svcResponse.Report;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    var fileExtension = String.Empty;
                    Stream fileStream = null; 

                    //[1] Check the file name
                    string validFileName = GetValidFileName(version);
                    if (!fileNameWithoutExtension.Equals(validFileName, StringComparison.InvariantCultureIgnoreCase))
                        validationReport.LogWarning(String.Format("Invalid file name. System will change this to '{0}'", validFileName));

                    bool allowToRunQualityChecks = false;

                    //[2] If file is in .zip format, check that .zip file and internal word file must have same name.
                    if (Path.GetExtension(path).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var zipContent = Zip.Extract(path, true);
                        List<string> zipContentShortPath = new List<string>();
                        zipContent.ForEach(x => zipContentShortPath.Add(x.Split('\\').Last()));


                        //1 - File match found
                        if (zipContentShortPath.Find(x => x.Equals(fileNameWithoutExtension + ".doc", StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".doc";
                            fileStream = new FileStream(Path.GetDirectoryName(zipContent.First()) + "\\" + fileNameWithoutExtension + ".doc", FileMode.Open);
                        }
                        else if (zipContentShortPath.Find(x => x.Equals(fileNameWithoutExtension + ".docx", StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".docx";
                            fileStream = new FileStream(Path.GetDirectoryName(zipContent.First()) + "\\" + fileNameWithoutExtension + ".docx", FileMode.Open);
                        }
                        else
                        {
                            if (zipContent.Count == 1 && (zipContent.First().ToLower().EndsWith(".doc")))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".doc";
                                fileStream = new FileStream(zipContent.First(), FileMode.Open);
                                validationReport.LogWarning("Zip file and internal word file must have same name");
                            }
                            else if (zipContent.Count == 1 && zipContent.First().ToLower().EndsWith(".docx"))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".docx";
                                fileStream = new FileStream(zipContent.First(), FileMode.Open);
                                validationReport.LogWarning("Zip file and internal word file must have same name");
                            }
                            else
                            {
                                validationReport.LogWarning("No matching files inside zip. Quality checks cannot be executed");
                            }
                        }
                    }
                    else //Copy to stream from .doc / .docx
                    {
                        if (!Path.GetExtension(path).Equals("doc", StringComparison.InvariantCultureIgnoreCase) &&
                            !Path.GetExtension(path).Equals("docx", StringComparison.InvariantCultureIgnoreCase))
                            throw new InvalidOperationException("Invalid file format provided");
                        allowToRunQualityChecks = true;
                    }

                    //If we have valid file & spec is under change control, run quality checks
                    if (allowToRunQualityChecks && version.Specification.IsUnderChangeControl.GetValueOrDefault())
                    {
                        string versionStr = String.Empty;
                        DateTime meetingDate = DateTime.MinValue;
                        string tsgTitle = String.Empty;

                        ICommunityManager communityMgr = ManagerFactory.Resolve<ICommunityManager>();
                        var communities = communityMgr.GetCommunities();
                        var community = communities.Where(x => x.TbId == version.Specification.PrimeResponsibleGroup.Fk_commityId).FirstOrDefault();
                        if (community != null)
                            tsgTitle = GetParentTSG(community, communities).TbTitle.Replace("Technical Specification Group -", "Technical Specification Group");

                        versionStr = String.Format("{0}.{1}.{2}", version.MajorVersion.Value, version.TechnicalVersion.Value, version.EditorialVersion.Value);

                        if (version.Source.HasValue)
                        {
                            MeetingManager meetingMgr = new MeetingManager();
                            meetingMgr.UoW = UoW;

                            meetingDate = meetingMgr.GetMeetingById(version.Source.Value).END_DATE.Value;
                        }

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);

                            var businessValidationReport = ValidateVersionDocument(fileExtension, 
                                                                                       memoryStream, 
                                                                                       Path.GetDirectoryName(path), 
                                                                                       versionStr, 
                                                                                       version.Specification.Title, 
                                                                                       version.Release.Name, 
                                                                                       meetingDate, tsgTitle, 
                                                                                       version.Specification.IsTS.GetValueOrDefault());

                            validationReport.ErrorList.AddRange(businessValidationReport.ErrorList);
                            validationReport.WarningList.AddRange(businessValidationReport.WarningList);

                            fileStream.Close();
                        }

                        svcResponse.Result = Guid.NewGuid().ToString();
                        CacheManager.InsertForLimitedTime(CACHE_KEY + svcResponse.Result, new KeyValuePair<SpecVersion, string>(version, path), 10);
                    }
                }
                catch (Exception exc)
                {
                    svcResponse.Report.LogError("An error occured: " + exc.Message);
                }
            }
            return svcResponse;
        }

        public ServiceResponse<string> UploadVersion(int personId, SpecVersion version, string token)
        {
            var svcResponse = new ServiceResponse<string>();

            svcResponse.Report = CheckPersonRightToUploadVersion(svcResponse.Report, personId);
            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                //...
            }
            return svcResponse;
        }

        private Report CheckPersonRightToUploadVersion(Report report, int personId)
        {
            IRightsManager rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);

            if (!rights.HasRight(Enum_UserRights.Versions_Upload))
                report.LogError(Utils.Localization.RightError);
            return report;
        }

        private string GetValidFileName(SpecVersion specVersion)
        {
            var specNumber = specVersion.Specification.Number;

            var utilsMgr = new UtilsManager();

            string majorVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.MajorVersion);
            string technicalVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.TechnicalVersion);
            string editorialVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.EditorialVersion);

            string validFileName = String.Format(CONST_VALID_FILENAME, specNumber, majorVersionBase36, technicalVersionBase36, editorialVersionBase36);
            return validFileName;
        }

        private void GetRelatedSpecAndRelease(SpecVersion specVersion)
        {
            //spec
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            if (!specVersion.Fk_SpecificationId.HasValue)
                throw new InvalidOperationException("Specification id is not provided");
            var spec = specMgr.GetSpecificationById(0, specVersion.Fk_SpecificationId.Value).Key;

            if (spec == null)
                throw new InvalidOperationException("Specification is not found");

            specVersion.Specification = spec;

            //Release
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            if (!specVersion.Fk_ReleaseId.HasValue)
                throw new InvalidOperationException("Release id is not provided");
            var release = releaseMgr.GetReleaseById(0, specVersion.Fk_ReleaseId.Value).Key;

            if (release == null)
                throw new InvalidOperationException("Release is not found");

            specVersion.Release = release;
        }

        private Community GetParentTSG(Community community, List<Community> communities)
        {
            if (community.TbType == "TSG")
                return community;
            else
            {
                var parentCommunity = communities.Where(x => x.TbId == community.ParentCommunityId).FirstOrDefault();
                if (parentCommunity != null)
                    return GetParentTSG(parentCommunity, communities);
                else
                    return community;
            }
        }

        private Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string temporaryFolder, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
        {
            Report validationReport;

            if (fileExtension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
            {
                using (IQualityChecks qualityChecks = new DocXQualityChecks(memoryStream))
                {
                    validationReport = ValidateDocument(qualityChecks, version, title, release, meetingDate, tsgTitle, isTS);
                }
            }
            else
            {
                using (IQualityChecks qualityChecks = new DocQualityChecks(memoryStream, temporaryFolder))
                {
                    validationReport = ValidateDocument(qualityChecks, version, title, release, meetingDate, tsgTitle, isTS);
                }
            }

            return validationReport;
        }

        /// <summary>
        /// Validate Word Document
        /// </summary>
        /// <param name="qualityChecks">Quality Checks Interface</param>
        /// <param name="version">Version</param>
        /// <param name="title">Title</param>
        /// <param name="release">Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <param name="tsgTitle">Technical Specificaion Group Title</param>
        /// <param name="isTS">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>Validation Summary</returns>
        private Report ValidateDocument(IQualityChecks qualityChecks, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
        {
            Report validationReport = new Report();

            if (qualityChecks.HasTrackedRevisions())
                validationReport.LogWarning(CONST_QUALITY_CHECK_REVISIONMARK);

            if (!qualityChecks.IsHistoryVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_HISTORY);

            if (!qualityChecks.IsCoverPageVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_COVERPAGE);

            if (!qualityChecks.IsCoverPageDateCorrect(meetingDate))
                validationReport.LogWarning(CONST_QUALITY_CHECK_DATE_COVERPAGE);

            if (!qualityChecks.IsCopyRightYearCorrect())
                validationReport.LogWarning(CONST_QUALITY_CHECK_YEAR_COPYRIGHT);

            if (!qualityChecks.IsTitleCorrect(title))
                validationReport.LogWarning(CONST_QUALITY_CHECK_TITLE_COVERPAGE);

            if (!qualityChecks.IsReleaseCorrect(release))
                validationReport.LogWarning(CONST_QUALITY_CHECK_RELEASE);

            if (!qualityChecks.IsReleaseStyleCorrect(release))
                validationReport.LogWarning(CONST_QUALITY_CHECK_RELEASE_STYLE);

            if (qualityChecks.IsAutomaticNumberingPresent())
                validationReport.LogWarning(CONST_QUALITY_CHECK_AUTO_NUMBERING);

            if (!qualityChecks.IsFirstTwoLinesOfTitleCorrect(tsgTitle))
                validationReport.LogWarning(CONST_QUALITY_CHECK_FIRST_TWO_LINES_TITLE);

            if (!qualityChecks.IsAnnexureStylesCorrect(isTS))
                validationReport.LogWarning(CONST_QUALITY_CHECK_ANNEXURE_STYLE);

            return validationReport;
        }
    }
}
