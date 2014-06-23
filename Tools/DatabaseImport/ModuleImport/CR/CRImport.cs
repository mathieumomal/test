using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.ModuleImport
{
    public class CRImport : IModuleImport
    {
        public const string RefImportForLog = "[CR]";
        List<Enum_CRCategory> enumCategory;
        List<Enum_TDocStatus> enumTDocStatus;
        List<Specification> specs;
        List<Release> releases;
        List<Specification_Release> specRelease;
        List<SpecVersion> specVersions;
        List<Meeting> meetings;
        List<WorkItem> wis;

        /// <summary>
        /// Old table(s) : 
        /// List_of_GSM_&_3G_CRs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase(){}

        public void FillDatabase()
        {
            //Initialization
            enumCategory = NewContext.Enum_CRCategory.ToList();
            enumTDocStatus = NewContext.Enum_TDocStatus.ToList();
            specs = NewContext.Specifications.ToList();
            releases = NewContext.Releases.ToList();
            specRelease = NewContext.Specification_Release.ToList();
            specVersions = NewContext.SpecVersions.ToList();
            meetings = NewContext.Meetings.ToList();
            wis = NewContext.WorkItems.ToList();

            CreateDatas();
            
            try
            {
                NewContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var test = ex;
                Console.WriteLine(ex.InnerException);
                Console.ReadLine();
            }
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            NewContext.SetAutoDetectChanges(false);
            var total = LegacyContext.List_of_GSM___3G_CRs.Count();
            var count = 0;
            foreach (var legacyCR in LegacyContext.List_of_GSM___3G_CRs)
            {
                var logID = new StringBuilder()
                    .Append(legacyCR.Spec)
                    .Append("#")
                    .Append(legacyCR.CR)
                    .Append("#")
                    .Append(legacyCR.Rev)
                    .ToString();

                var newCR = new Domain.ChangeRequest();

                newCR.CRNumber = Utils.CheckString(legacyCR.CR, 10, RefImportForLog + " CRNumber", logID, Report);
                newCR.Revision = Utils.CheckStringToInt(legacyCR.Rev, null, RefImportForLog + " Revision ", logID, Report);
                newCR.Subject = Utils.CheckString(legacyCR.Subject, 300, RefImportForLog + " Subject", logID, Report);
                newCR.CreationDate = legacyCR.created;

                SpecReleaseAndVersionCase(newCR, legacyCR, logID);

                RemarksCase(newCR, legacyCR, logID);

                MeetingsCase(newCR, legacyCR, logID);

                CategoryCase(newCR, legacyCR, logID);

                StatusCase(newCR, legacyCR, logID);

                WICase(newCR, legacyCR, logID);

                newCR.TSGTDoc = Utils.CheckString(legacyCR.Doc_1st_Level, 50, RefImportForLog + " TSG Tdoc ", logID, Report);
                newCR.WGTDoc = Utils.CheckString(legacyCR.Doc_2nd_Level, 50, RefImportForLog + " WG Tdoc ", logID, Report);

                newCR.TSGSourceOrganizations = Utils.CheckString(legacyCR.Source_1st_Level, 100, RefImportForLog + " TSGSourceorganization ", logID, Report);
                newCR.WGSourceOrganizations = Utils.CheckString(legacyCR.Source_2nd_Level, 100, RefImportForLog + " WGSourceorganization ", logID, Report);

                TSGWGTargetCase(newCR, legacyCR, logID);
                
                #region no take in consideration for the moment...
                //Impact
                #endregion

                NewContext.ChangeRequests.Add(newCR);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
                if (count % 10000 == 0)
                {
                    Console.Write("\r" + RefImportForLog + " Intermediary Recording  ");
                    NewContext.SetValidateOnSave(false);
                    NewContext.SaveChanges();
                    foreach (var elt in NewContext.ChangeRequests)
                    {
                        NewContext.SetDetached(elt);
                    }
                    NewContext.SetValidateOnSave(true);
                }
            }
        }

        private void StatusCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var legacyWGStatus = Utils.CheckString(legacyCR.Status_2nd_Level, 20, RefImportForLog + " WG status ", logID, Report).ToLower();
            var legacyTSGStatus = Utils.CheckString(legacyCR.Status_1st_Level, 20, RefImportForLog + " TSG status ", logID, Report).ToLower();
            var WGStatus = enumTDocStatus.Where(x => x.Status == legacyWGStatus && x.WGUsable).FirstOrDefault();
            var TSGStatus = enumTDocStatus.Where(x => x.Status == legacyTSGStatus && x.TSGUsable).FirstOrDefault();

            if (WGStatus != null)
            {
                newCR.Enum_TDocStatusWG = WGStatus;
                newCR.Fk_WGStatus = WGStatus.Pk_EnumTDocStatus;
            }
            else
            {
                if (!legacyWGStatus.Equals("-"))
                {
                    Report.LogWarning(RefImportForLog + "WG Status not found : " + legacyWGStatus + " for CR : " + logID);
                }
            }

            if (TSGStatus != null)
            {
                newCR.Enum_TDocStatusTSG = TSGStatus;
                newCR.Fk_TSGStatus = TSGStatus.Pk_EnumTDocStatus;
            }
            else
            {
                if (!legacyTSGStatus.Equals("-"))
                {
                    Report.LogWarning(RefImportForLog + "TSG Status not found : " + legacyTSGStatus + " for CR : " + logID);
                }
            }
        }

        private void CategoryCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var legacyCRCategory = Utils.CheckString(legacyCR.Cat, 5, RefImportForLog + " category ", logID, Report);
            var categoryAssiocated = enumCategory.Where(x => x.Category == legacyCRCategory).FirstOrDefault();

            if (categoryAssiocated != null)
            {
                newCR.Enum_CRCategory = categoryAssiocated;
                newCR.Fk_Enum_CRCategory = categoryAssiocated.Pk_EnumCRCategory;
            }
            else
            {
                Report.LogWarning(RefImportForLog + "Category not found : " + legacyCRCategory + " for CR : " + logID);
            }
        }

        private void SpecReleaseAndVersionCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var legacyCRSpecNumber = Utils.CheckString(legacyCR.Spec, 10, RefImportForLog + " Spec Number ", logID, Report);
            var legacyCRReleaseCode = Utils.CheckString(legacyCR.Phase, 10, RefImportForLog + " Release Code ", logID, Report);
            var specAssociated = specs.Where(x => x.Number == legacyCRSpecNumber).FirstOrDefault();
            var releaseAssociated = releases.Where(x => x.Code == legacyCRReleaseCode).FirstOrDefault();

            var legacyCRNewVersion = Utils.CheckString(legacyCR.Version_New, 10, RefImportForLog + " Version-new ", logID, Report);
            var legacyCRTargetVersion = Utils.CheckString(legacyCR.Version_Current, 10, RefImportForLog + " Version-current ", logID, Report);

            //Spec 
            if (specAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Spec not found : " + legacyCRSpecNumber + ", for CR : " + logID);
            }
            else
            {
                newCR.Specification = specAssociated;
                newCR.Fk_Specification = specAssociated.Pk_SpecificationId;
            }

            //Release
            if (releaseAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Release not found : " + legacyCRReleaseCode + ", for CR : " + logID);
            }
            else
            {
                newCR.Release = releaseAssociated;
                newCR.Fk_Release = releaseAssociated.Pk_ReleaseId;
            }

            //Version NEW
            if (releaseAssociated == null || specAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Versions (target and current) undefined because spec or release undefined, for CR : " + logID);
            }
            else
            {
                var newVersionExploded = legacyCRNewVersion.Split('.');

                if (newVersionExploded.Count() != 3)
                {
                    Report.LogWarning(RefImportForLog + "Target version invalid format : " + legacyCRNewVersion + ", for CR : " + logID);
                }
                else
                {
                    var mv = Utils.CheckStringToInt(newVersionExploded[0], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the majorVersion : " + newVersionExploded[0], logID, Report);
                    var tv = Utils.CheckStringToInt(newVersionExploded[1], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the technicalVersion : " + newVersionExploded[1], logID, Report);
                    var ev = Utils.CheckStringToInt(newVersionExploded[2], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the editorialVersion : " + newVersionExploded[2], logID, Report);
                    var newVersionAssociated = specVersions.Where(x =>
                        x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                        && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                        && x.MajorVersion == mv
                        && x.TechnicalVersion == tv
                        && x.EditorialVersion == ev)
                        .FirstOrDefault();

                    if (newVersionAssociated != null)
                    {
                        newCR.NewVersion = newVersionAssociated;
                        newCR.Fk_NewVersion = newVersionAssociated.Pk_VersionId;
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "New version not found : " + legacyCRNewVersion + ", for CR : " + logID);
                    }
                }

                //Version TARGET (<=> CURRENT)
                var targetVersionExploded = legacyCRTargetVersion.Split('.');

                if (targetVersionExploded.Count() != 3)
                {
                    Report.LogWarning(RefImportForLog + "Target version invalid format : " + legacyCRTargetVersion + ", for CR : " + logID);
                }
                else
                {
                    var mv = Utils.CheckStringToInt(targetVersionExploded[0], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the majorVersion : " + targetVersionExploded[0], logID, Report);
                    var tv = Utils.CheckStringToInt(targetVersionExploded[1], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the technicalVersion : " + targetVersionExploded[1], logID, Report);
                    var ev = Utils.CheckStringToInt(targetVersionExploded[2], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the editorialVersion : " + targetVersionExploded[2], logID, Report);

                    var targetVersionAssociated = specVersions.Where(x =>
                        x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                        && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                        && x.MajorVersion == mv
                        && x.TechnicalVersion == tv
                        && x.EditorialVersion == ev)
                        .FirstOrDefault();

                    if (targetVersionAssociated != null)
                    {
                        newCR.CurrentVersion = targetVersionAssociated;
                        newCR.Fk_CurrentVersion = targetVersionAssociated.Pk_VersionId;
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "Target version not found : " + legacyCRTargetVersion + ", for CR : " + logID);
                    }
                }
            }
        }

        private void MeetingsCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var TSGMeeting = Utils.CheckString(legacyCR.Meeting_1st_Level, 10, RefImportForLog + "TSG meeting : " + legacyCR.Meeting_1st_Level, logID, Report);
            var WGMeeting = Utils.CheckString(legacyCR.Meeting_2nd_Level, 10, RefImportForLog + "TSG meeting : " + legacyCR.Meeting_2nd_Level, logID, Report);
            
            //TSG
            if (!string.IsNullOrEmpty(TSGMeeting) && TSGMeeting != "-")
            {
                var mtg = meetings.Where(m => m.MtgShortRef == TSGMeeting).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "TSG meeting not found: " + TSGMeeting + " for CR : " + logID );
                else
                    newCR.TSGMeeting = mtg.MTG_ID;
            }

            //WG
            if (!string.IsNullOrEmpty(WGMeeting) && WGMeeting != "-")
            {
                var mtg = meetings.Where(m => m.MtgShortRef == WGMeeting).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "WG meeting not found: " + TSGMeeting + " for CR : " + logID);
                else
                    newCR.WGMeeting = mtg.MTG_ID;
            }
        }

        private void RemarksCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var remarksField = Utils.CheckString(legacyCR.Remarks, 255, RefImportForLog + " Remarks ", logID, Report);

            if (remarksField == null || remarksField.Length == 0)
                return;

            var remark = new Domain.Remark()
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                ChangeRequest = newCR
            };
            newCR.Remarks.Add(remark);
        }

        private void WICase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            var WIField = Utils.CheckString(legacyCR.Workitem, 50, RefImportForLog + " WI ", logID, Report);

            if (WIField == null || WIField.Length == 0)
                return;
            else
            {
                var wiExploded = WIField.Split('/');

                foreach (var wilabel in wiExploded)
                {
                    var wi = wis.Where(m => m.Acronym == wilabel).FirstOrDefault();

                    if (wi == null)
                        Report.LogWarning(RefImportForLog + "WI not found: " + WIField + " for CR : " + logID);
                    else
                    {
                        var crWi = new CR_WorkItems();
                        crWi.ChangeRequest = newCR;
                        crWi.WorkItem = wi;
                        newCR.CR_WorkItems.Add(crWi);
                    }
                }
            }
        }

        private void TSGWGTargetCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR, string logID)
        {
            //WGSourceForTSG
            var WGResponsibleLegacy = legacyCR.WG_Responsible;

            if (WGResponsibleLegacy != null && !String.IsNullOrEmpty(WGResponsibleLegacy))
            {
                var community = NewContext.Communities.Where(x => x.ShortName == WGResponsibleLegacy).FirstOrDefault();
                if (community != null)
                {
                    //If the community (WG responsible) is found so we take it as the WGTarget and his parent as TSGtarget
                    newCR.WGTarget = community.TbId;
                    newCR.TSGTarget = community.ParentTbId;
                    newCR.WGSourceForTSG = community.TbId;
                }
                else
                {
                    Report.LogWarning(RefImportForLog + "Community not found: " + WGResponsibleLegacy + " for CR : " + logID + " (WG and TSG target not assigned)");
                }
            }

            //Targets (TSG/WG)
            //newCR.WGSourceForTSG = ; "Not for CRs broughts directly to TSG by an organization"
        }
        #endregion
    }
}
