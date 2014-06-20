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
            NewContext.SetAutoDetectChanges(false);

            //Initialization
            enumCategory = NewContext.Enum_CRCategory.ToList();
            enumTDocStatus = NewContext.Enum_TDocStatus.ToList();
            specs = NewContext.Specifications.ToList();
            releases = NewContext.Releases.ToList();
            specRelease = NewContext.Specification_Release.ToList();
            specVersions = NewContext.SpecVersions.ToList();
            meetings = NewContext.Meetings.ToList();

            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
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
            var total = LegacyContext.List_of_GSM___3G_CRs.Count();
            var count = 0;
            foreach (var legacyCR in LegacyContext.List_of_GSM___3G_CRs)
            {
                var newCR = new Domain.ChangeRequest();

                //Baseline attributes
                newCR.CRNumber = Utils.CheckString(legacyCR.CR, 10, RefImportForLog + " CRNumber", legacyCR.CR, Report);
                newCR.Revision = Utils.CheckStringToInt(legacyCR.Rev, null, RefImportForLog + " Revision ", legacyCR.CR, Report);
                newCR.Subject = Utils.CheckString(legacyCR.Subject, 300, RefImportForLog + " Subject", legacyCR.CR, Report);
                newCR.CreationDate = legacyCR.created;

                //Special cases
                    //Spec & release by Specrelease table AND Versions (new & target)
                SpecReleaseAndVersionCase(newCR, legacyCR);
                    //Remarks
                RemarksCase(newCR, legacyCR);
                    //History : nothing to add


                    //Meetins (TSG/WG)
                MeetingsCase(newCR, legacyCR); // What's "#2/93" ???? for example ????????????????
                    //Category
                CategoryCase(newCR, legacyCR); // Cas 1 2 3 4 ???????????????????? rowId ??
                    //Status (TSG/WG)
                StatusCase(newCR, legacyCR);//WG = LEVEL 2, TSG = LEVEL 1 ?????????????????????
                //Text ???? sure ?
                newCR.TSGSourceOrganizations = Utils.CheckString(legacyCR.Source_1st_Level, 100, RefImportForLog + " TSGSourceorganization ", legacyCR.CR, Report);
                //Text ???? sure ?
                newCR.WGSourceOrganizations = Utils.CheckString(legacyCR.Source_2nd_Level, 100, RefImportForLog + " WGSourceorganization ", legacyCR.CR, Report);
                //Targets (TSG/WG)
                //TDoc TSG/WG
                //newCR.WGSourceForTSG = ; "Not for CRs broughts directly to TSG by an organization"
                //Impact
                //WorkItems
                NewContext.ChangeRequests.Add(newCR);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }


        private void StatusCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyWGStatus = Utils.CheckString(legacyCR.Status_2nd_Level, 20, RefImportForLog + " WG status ", legacyCR.CR, Report).ToLower();
            var legacyTSGStatus = Utils.CheckString(legacyCR.Status_1st_Level, 20, RefImportForLog + " TSG status ", legacyCR.CR, Report).ToLower();
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
                    Report.LogWarning(RefImportForLog + "WG Status not found : " + legacyWGStatus + " for CR : " + legacyCR.CR);
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
                    Report.LogWarning(RefImportForLog + "TSG Status not found : " + legacyTSGStatus + " for CR : " + legacyCR.CR);
                }
            }
        }

        private void CategoryCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRCategory = Utils.CheckString(legacyCR.Cat, 5, RefImportForLog + " category ", legacyCR.CR, Report);
            var categoryAssiocated = enumCategory.Where(x => x.Category == legacyCRCategory).FirstOrDefault();

            if (categoryAssiocated != null)
            {
                newCR.Enum_CRCategory = categoryAssiocated;
                newCR.Fk_Enum_CRCategory = categoryAssiocated.Pk_EnumCRCategory;
            }
            else
            {
                Report.LogWarning(RefImportForLog + "Category not found : " + legacyCRCategory + " for CR : " + legacyCR.CR);
            }
        }

        private void SpecReleaseAndVersionCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRSpecNumber = Utils.CheckString(legacyCR.Spec, 10, RefImportForLog + " Spec Number ", legacyCR.CR, Report);
            var legacyCRReleaseCode = Utils.CheckString(legacyCR.Phase, 10, RefImportForLog + " Release Code ", legacyCR.CR, Report);
            var specAssociated = specs.Where(x => x.Number == legacyCRSpecNumber).FirstOrDefault();
            var releaseAssociated = releases.Where(x => x.Code == legacyCRReleaseCode).FirstOrDefault();

            var legacyCRNewVersion = Utils.CheckString(legacyCR.Version_New, 10, RefImportForLog + " Version-new ", legacyCR.CR, Report);
            var legacyCRTargetVersion = Utils.CheckString(legacyCR.Version_Current, 10, RefImportForLog + " Version-current ", legacyCR.CR, Report);

            if (specAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Spec not found : " + legacyCRSpecNumber + ", for CR : " + legacyCR.CR);
            }
            else if (releaseAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Release not found : " + legacyCRReleaseCode + ", for CR : " + legacyCR.CR);
            }
            else
            {
                //SpecRelease 
                if (specAssociated == null)
                {
                    Report.LogWarning(RefImportForLog + "Spec not found : " + legacyCRSpecNumber + ", for CR : " + legacyCR.CR);
                }
                else if (releaseAssociated == null)
                {
                    Report.LogWarning(RefImportForLog + "Release not found : " + legacyCRReleaseCode + ", for CR : " + legacyCR.CR);
                }
                else
                {
                    var specReleaseAssociated = specRelease.Where(x => x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId).FirstOrDefault();

                    if (specReleaseAssociated != null)
                    {
                        newCR.Specification_Release = specReleaseAssociated;
                        newCR.Fk_SpecRelease = specReleaseAssociated.Pk_Specification_ReleaseId;
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "Spec and release couple not found : spec number -> " + legacyCRSpecNumber + ", release code -> " + legacyCRReleaseCode + ", for CR : " + legacyCR.CR);
                    }
                }

                //New
                var newVersionExploded = legacyCRNewVersion.Split('.');

                if (newVersionExploded.Count() != 3)
                {
                    Report.LogWarning(RefImportForLog + "Target legacy version invalid format : " + legacyCRNewVersion + ", for CR : " + legacyCR.CR);
                }
                else
                {
                    var mv = Utils.CheckStringToInt(newVersionExploded[0], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the majorVersion : " + newVersionExploded[0], legacyCR.CR, Report);
                    var tv = Utils.CheckStringToInt(newVersionExploded[1], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the technicalVersion : " + newVersionExploded[1], legacyCR.CR, Report);
                    var ev = Utils.CheckStringToInt(newVersionExploded[2], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the editorialVersion : " + newVersionExploded[2], legacyCR.CR, Report);
                    var newVersionAssociated = specVersions.Where(x =>
                        x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                        && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                        && x.MajorVersion == mv
                        && x.TechnicalVersion == tv
                        && x.EditorialVersion == ev)
                        .FirstOrDefault();

                    if (newVersionAssociated != null)
                    {
                        var newCRNewVersion = new Domain.CR_Version();
                        newCRNewVersion.Fk_Version = newVersionAssociated.Pk_VersionId;
                        newCRNewVersion.Version = newVersionAssociated;
                        newCRNewVersion.IsNew = true;
                        newCR.CR_Version.Add(newCRNewVersion);
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "New legacy version not found : " + legacyCRNewVersion + ", for CR : " + legacyCR.CR);
                    }
                }

                //Target
                var targetVersionExploded = legacyCRTargetVersion.Split('.');

                if (targetVersionExploded.Count() != 3)
                {
                    Report.LogWarning(RefImportForLog + "Target legacy version invalid format : " + legacyCRTargetVersion + ", for CR : " + legacyCR.CR);
                }
                else
                {
                    var mv = Utils.CheckStringToInt(targetVersionExploded[0], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the majorVersion : " + targetVersionExploded[0], legacyCR.CR, Report);
                    var tv = Utils.CheckStringToInt(targetVersionExploded[1], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the technicalVersion : " + targetVersionExploded[1], legacyCR.CR, Report);
                    var ev = Utils.CheckStringToInt(targetVersionExploded[2], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the editorialVersion : " + targetVersionExploded[2], legacyCR.CR, Report);
                    var targetVersionAssociated = specVersions.Where(x =>
                        x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                        && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                        && x.MajorVersion == mv
                        && x.TechnicalVersion == tv
                        && x.EditorialVersion == ev)
                        .FirstOrDefault();

                    if (targetVersionAssociated != null)
                    {
                        var newCRTargetVersion = new Domain.CR_Version();
                        newCRTargetVersion.Fk_Version = targetVersionAssociated.Pk_VersionId;
                        newCRTargetVersion.Version = targetVersionAssociated;
                        newCRTargetVersion.IsNew = false;
                        newCR.CR_Version.Add(newCRTargetVersion);
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "Target legacy version not found : " + legacyCRTargetVersion + ", for CR : " + legacyCR.CR);
                    }
                }
            }
        }

        private void MeetingsCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var TSGMeeting = Utils.CheckString(legacyCR.Meeting_1st_Level, 10, RefImportForLog + "TSG meeting : " + legacyCR.Meeting_1st_Level, legacyCR.CR, Report);
            var WGMeeting = Utils.CheckString(legacyCR.Meeting_2nd_Level, 10, RefImportForLog + "TSG meeting : " + legacyCR.Meeting_2nd_Level, legacyCR.CR, Report);
            
            //TSG
            if (!string.IsNullOrEmpty(TSGMeeting) && TSGMeeting != "-")
            {
                var mtg = meetings.Where(m => m.MtgShortRef == TSGMeeting).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "TSG meeting not found: " + TSGMeeting);
                else
                {
                    newCR.TSGMeeting = mtg.MTG_ID;
                }
            }

            //WG
            if (!string.IsNullOrEmpty(WGMeeting) && WGMeeting != "-")
            {
                var mtg = meetings.Where(m => m.MtgShortRef == WGMeeting).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "WG meeting not found: " + TSGMeeting);
                else
                {
                    newCR.WGMeeting = mtg.MTG_ID;
                }
            }
        }

        private void RemarksCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var remarksField = Utils.CheckString(legacyCR.Remarks,255,RefImportForLog + " Remarks ", legacyCR.CR, Report);

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
        #endregion
    }
}
