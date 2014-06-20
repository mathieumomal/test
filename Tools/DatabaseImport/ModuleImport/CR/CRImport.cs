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

            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            NewContext.SaveChanges();
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

                newCR.CRNumber = Utils.CheckString(legacyCR.CR, 10, RefImportForLog + " CRNumber", legacyCR.CR, Report);
                newCR.Revision = Utils.CheckStringToInt(legacyCR.Rev, null, RefImportForLog + " Revision ", legacyCR.CR, Report);
                newCR.Subject = Utils.CheckString(legacyCR.Subject, 300, RefImportForLog + " Subject", legacyCR.CR, Report);
                newCR.CreationDate = legacyCR.created;
                CategoryCase(newCR, legacyCR); // Cas 1 2 3 4 ?????
                StatusCase(newCR, legacyCR);//WG = LEVEL 2, TSG = LEVEL 1
                //Text ???? sure ?
                newCR.TSGSourceOrganizations = Utils.CheckString(legacyCR.Source_1st_Level, 100, RefImportForLog + " TSGSourceorganization ", legacyCR.CR, Report);
                //Text ???? sure ?
                newCR.WGSourceOrganizations = Utils.CheckString(legacyCR.Source_2nd_Level, 100, RefImportForLog + " WGSourceorganization ", legacyCR.CR, Report);
                SpecReleaseCase(newCR, legacyCR);

                TargetVersionCase(newCR, legacyCR);
                NewVersionCase(newCR, legacyCR);

               
                //newCR.TSGMeeting = ;
                //newCR.WGMeeting = ;
                //newCR.WGSourceForTSG = ;
                //newCR.Fk_TSGTDoc = ;
                //newCR.Fk_WGTDoc = ;

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

        private void SpecReleaseCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRSpecNumber = Utils.CheckString(legacyCR.Spec, 10, RefImportForLog + " Spec Number ", legacyCR.CR, Report);
            var legacyCRReleaseCode = Utils.CheckString(legacyCR.Phase, 10, RefImportForLog + " Release Code ", legacyCR.CR, Report);
            var specAssociated = NewContext.Specifications.Where(x => x.Number == legacyCRSpecNumber).FirstOrDefault();
            var releaseAssociated = NewContext.Releases.Where(x => x.Code == legacyCRReleaseCode).FirstOrDefault();

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
                var specReleaseAssociated = NewContext.Specification_Release.Where(x => x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId).FirstOrDefault();

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
            
        }

        private void NewVersionCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRSpecNumber = Utils.CheckString(legacyCR.Spec, 10, RefImportForLog + " Spec Number ", legacyCR.CR, Report);
            var legacyCRReleaseCode = Utils.CheckString(legacyCR.Phase, 10, RefImportForLog + " Release Code ", legacyCR.CR, Report);
            var specAssociated = NewContext.Specifications.Where(x => x.Number == legacyCRSpecNumber).FirstOrDefault();
            var releaseAssociated = NewContext.Releases.Where(x => x.Code == legacyCRReleaseCode).FirstOrDefault();

            var legacyCRNewVersion = Utils.CheckString(legacyCR.Version_New, 10, RefImportForLog + " Version-new ", legacyCR.CR, Report);

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
                    var newVersionAssociated = NewContext.SpecVersions.Where(x =>
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
                        NewContext.CR_Versions.Add(newCRNewVersion);

                        newCR.CR_Versions.Add(newCRNewVersion);
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "New legacy version not found : " + legacyCRNewVersion + ", for CR : " + legacyCR.CR);
                    }
                }
            }
        }

        private void TargetVersionCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRSpecNumber = Utils.CheckString(legacyCR.Spec, 10, RefImportForLog + " Spec Number ", legacyCR.CR, Report);
            var legacyCRReleaseCode = Utils.CheckString(legacyCR.Phase, 10, RefImportForLog + " Release Code ", legacyCR.CR, Report);
            var specAssociated = NewContext.Specifications.Where(x => x.Number == legacyCRSpecNumber).FirstOrDefault();
            var releaseAssociated = NewContext.Releases.Where(x => x.Code == legacyCRReleaseCode).FirstOrDefault();

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
                    var targetVersionAssociated = NewContext.SpecVersions.Where(x =>
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
                        NewContext.CR_Versions.Add(newCRTargetVersion);

                        newCR.CR_Versions.Add(newCRTargetVersion);
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + "Target legacy version not found : " + legacyCRTargetVersion + ", for CR : " + legacyCR.CR);
                    }
                }
            }
        }
        #endregion
    }
}
