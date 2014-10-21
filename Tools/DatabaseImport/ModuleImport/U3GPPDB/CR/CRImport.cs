using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.CR
{
    public class CrImport : IModuleImport
    {
        //Report part indicator
        private const string RefImportForLog = "[CR]";
        //Objects list initialize one shoot
        List<Enum_CRCategory> _enumChangeRequestCategory;
        List<Enum_ChangeRequestStatus> _enumChangeRequestStatus;
        List<Etsi.Ultimate.DomainClasses.Specification> _specs;
        List<Release> _releases;
        List<SpecVersion> _specVersions;
        List<Meeting> _meetings;
        List<WorkItem> _wis;

        /// <summary>
        /// Old table(s) : 
        /// List_of_GSM_&_3G_CRs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Report Report { get; set; }

        public void CleanDatabase(){
            //NewContext.CR_CleanAll();
        }

        public void FillDatabase()
        {
            const int dataSortNumber = 7;
            var count = 1;
            //Initialization
            Console.WriteLine("Loading data relative to CR... (could be a long operation...)");
            Console.WriteLine("Loading CR Category... ({0}/{1})", count, dataSortNumber);
            _enumChangeRequestCategory = UltimateContext.Enum_CRCategory.ToList(); count+=1;
            Console.WriteLine("Loading CR Status... ({0}/{1})", count, dataSortNumber);
            _enumChangeRequestStatus = UltimateContext.Enum_ChangeRequestStatus.ToList(); count += 1;
            Console.WriteLine("Loading Specs... ({0}/{1})", count, dataSortNumber);
            _specs = UltimateContext.Specifications.ToList(); count += 1;
            Console.WriteLine("Loading Releases... ({0}/{1})", count, dataSortNumber);
            _releases = UltimateContext.Releases.ToList(); count += 1;
            Console.WriteLine("Loading Specs version... ({0}/{1})", count, dataSortNumber);
            _specVersions = UltimateContext.SpecVersions.ToList(); count += 1;
            Console.WriteLine("Loading Meetings... ({0}/{1})", count, dataSortNumber);
            _meetings = UltimateContext.Meetings.ToList(); count += 1;
            Console.WriteLine("Loading WIs... ({0}/{1})", count, dataSortNumber);
            _wis = UltimateContext.WorkItems.ToList();

            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);

            try
            {
                UltimateContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                var test = ex;
                Console.WriteLine("[ERROR GRAVE] DB update exception : " + ex.InnerException);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR GRAVE] Exception : " + ex.InnerException);
                Console.ReadLine();
            }
        }

        #endregion

        #region migration methods
        /// <summary>
        /// Create CR datas
        /// </summary>
        private void CreateDatas()
        {
            var total = LegacyContext.List_of_GSM___3G_CRs.Count();
            var count = 0;
            foreach (var legacyCr in LegacyContext.List_of_GSM___3G_CRs)
            {
                //Log legacy CR indicator
                var logId = new StringBuilder()
                    .Append(legacyCr.Spec)
                    .Append("#")
                    .Append(legacyCr.CR)
                    .Append("#")
                    .Append(legacyCr.Rev)
                    .ToString();

                var newCr = new ChangeRequest
                {
                    //CR baseline attribute initialization
                    CRNumber = Utils.CheckString(legacyCr.CR, 10, RefImportForLog + " CRNumber", logId, Report),
                    Subject = Utils.CheckString(legacyCr.Subject, 300, RefImportForLog + " Subject", logId, Report),
                    CreationDate = legacyCr.created
                };

                //Others cases
                RevisionCase(newCr, legacyCr, logId);

                SpecReleaseAndVersionCase(newCr, legacyCr, logId);

                RemarksCase(newCr, legacyCr, logId);

                MeetingsCase(newCr, legacyCr, logId);

                CategoryCase(newCr, legacyCr, logId);

                StatusCase(newCr, legacyCr, logId);

                WiCase(newCr, legacyCr, logId);

                newCr.TSGTDoc = Utils.CheckString(legacyCr.Doc_1st_Level, 50, RefImportForLog + " TSG Tdoc ", logId, Report);
                newCr.WGTDoc = Utils.CheckString(legacyCr.Doc_2nd_Level, 50, RefImportForLog + " WG Tdoc ", logId, Report);

                newCr.TSGSourceOrganizations = Utils.CheckString(legacyCr.Source_1st_Level, 100, RefImportForLog + " TSGSourceOrganization ", logId, Report);
                newCr.WGSourceOrganizations = Utils.CheckString(legacyCr.Source_2nd_Level, 100, RefImportForLog + " WGSourceOrganization ", logId, Report);

                TsgwgTargetCase(newCr, legacyCr, logId);

                //not taking in consideration for the moment : Impact
                
                UltimateContext.ChangeRequests.Add(newCr);
                count++;
                if (count % 100 == 0)
                    Console.Write("\r" + RefImportForLog + " {0}/{1}  ", count, total);
                //Save each 10000 CRs cause of memory reasons
                if (count % 1000 == 0)
                {
                    Console.Write("\r" + RefImportForLog + " Intermediary Recording  ");
                    UltimateContext.SaveChanges();
                    foreach (var elt in UltimateContext.ChangeRequests)
                    {
                        UltimateContext.SetDetached(elt);
                    }
                }
            }
        }

        /// <summary>
        /// Interpret and assign a revision
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void RevisionCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var legacyVersion = Utils.CheckString(legacyCr.Rev,10,RefImportForLog + " Revision ", logId, Report);
            if (!String.IsNullOrEmpty(legacyVersion) && !legacyVersion.Equals("-"))
            {
                newCr.Revision = Utils.CheckStringToInt(legacyVersion, null, RefImportForLog + " Revision ", logId, Report);
            }
            else
            {
                newCr.Revision = 0;
            }
        }

        /// <summary>
        /// Assigned TSG/WG TDOC status
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void StatusCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var legacyWgStatus = Utils.CheckString(legacyCr.Status_2nd_Level, 20, RefImportForLog + " WG status ", logId, Report).ToLower();
            var legacyTsgStatus = Utils.CheckString(legacyCr.Status_1st_Level, 20, RefImportForLog + " TSG status ", logId, Report).ToLower();
            var wgStatus = _enumChangeRequestStatus.FirstOrDefault(x => legacyWgStatus.ToLower().Contains(x.Code.ToLower()));
            var tsgStatus = _enumChangeRequestStatus.FirstOrDefault(x => legacyTsgStatus.ToLower().Contains(x.Code.ToLower()));

            if (wgStatus != null)
            {
                newCr.WgStatus = wgStatus;
                newCr.Fk_WGStatus = wgStatus.Pk_EnumChangeRequestStatus;
            }
            else
            {
                if (!legacyWgStatus.Equals("-"))
                {
                    Report.LogWarning(RefImportForLog + "WG Status not found : " + legacyWgStatus + " for CR : " + logId);
                }
            }

            if (tsgStatus != null)
            {
                newCr.TsgStatus = tsgStatus;
                newCr.Fk_TSGStatus = tsgStatus.Pk_EnumChangeRequestStatus;
            }
            else
            {
                if (!legacyTsgStatus.Equals("-"))
                {
                    Report.LogWarning(RefImportForLog + "TSG Status not found : " + legacyTsgStatus + " for CR : " + logId);
                }
            }
        }

        /// <summary>
        /// Assigned category
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void CategoryCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var legacyCrCategory = Utils.CheckString(legacyCr.Cat, 5, RefImportForLog + " category ", logId, Report);

            if (!String.IsNullOrEmpty(legacyCrCategory))
            {
                var categoryAssiocated = _enumChangeRequestCategory.FirstOrDefault(x => x.Code == legacyCrCategory);

                if (categoryAssiocated != null)
                {
                    newCr.Enum_CRCategory = categoryAssiocated;
                    newCr.Fk_Enum_CRCategory = categoryAssiocated.Pk_EnumCRCategory;
                }
                else
                {
                    Report.LogWarning(RefImportForLog + "Category not found : " + legacyCrCategory + " for CR : " + logId);
                }
            }
        }

        /// <summary>
        /// Assigned to a spec, a release and versions (current, new)
        /// a specification 
        /// + a release 
        /// + a new/current version
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void SpecReleaseAndVersionCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var legacyCrSpecNumber = Utils.CheckString(legacyCr.Spec, 10, RefImportForLog + " Spec Number ", logId, Report);
            var legacyCrReleaseCode = Utils.CheckString(legacyCr.Phase, 10, RefImportForLog + " Release Code ", logId, Report);
            var specAssociated = _specs.FirstOrDefault(x => x.Number == legacyCrSpecNumber);
            var releaseAssociated = _releases.FirstOrDefault(x => x.Code == legacyCrReleaseCode);

            //Spec 
            if (specAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Spec not found : " + legacyCrSpecNumber + ", for CR : " + logId);
            }
            else
            {
                newCr.Specification = specAssociated;
                newCr.Fk_Specification = specAssociated.Pk_SpecificationId;
            }

            //Release
            if (releaseAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Release not found : " + legacyCrReleaseCode + ", for CR : " + logId);
            }
            else
            {
                newCr.Release = releaseAssociated;
                newCr.Fk_Release = releaseAssociated.Pk_ReleaseId;
            }

            //Version NEW
            if (releaseAssociated == null || specAssociated == null)
            {
                Report.LogWarning(RefImportForLog + "Versions (target and current) undefined because spec or release undefined, for CR : " + logId);
            }
            else
            {
                var legacyVersion = Utils.CheckString(legacyCr.Version_New, 0, RefImportForLog + " version Checkstring ", logId, Report);
                if (!String.IsNullOrEmpty(legacyVersion) && !legacyVersion.Equals("-"))
                {
                    //VERSION NEW
                    var newVersionExploded = legacyVersion.Split('.');

                    if (newVersionExploded.Count() != 3)
                    {
                        Report.LogWarning(RefImportForLog + "New version invalid format : " + legacyVersion + ", for CR : " + logId);
                    }
                    else
                    {
                        var mv = Utils.CheckStringToInt(newVersionExploded[0], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the majorVersion : " + newVersionExploded[0], logId, Report);
                        var tv = Utils.CheckStringToInt(newVersionExploded[1], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the technicalVersion : " + newVersionExploded[1], logId, Report);
                        var ev = Utils.CheckStringToInt(newVersionExploded[2], 0, RefImportForLog + "(legacyNewVersion) cannot convert string to int for the editorialVersion : " + newVersionExploded[2], logId, Report);
                        var newVersionAssociated = _specVersions.FirstOrDefault(x => x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                                                                                     && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                                                                                     && x.MajorVersion == mv
                                                                                     && x.TechnicalVersion == tv
                                                                                     && x.EditorialVersion == ev);

                        if (newVersionAssociated != null)
                        {
                            newCr.NewVersion = newVersionAssociated;
                            newCr.Fk_NewVersion = newVersionAssociated.Pk_VersionId;
                        }
                        else
                        {
                            Report.LogWarning(RefImportForLog + "New version not found : " + legacyVersion + " with releaseId = " + releaseAssociated.Pk_ReleaseId + " and specId = " + specAssociated.Pk_SpecificationId + ", for CR : " + logId);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(legacyCr.Version_Current))
                {
                    var legacyCrTargetVersion = Utils.CheckString(legacyCr.Version_Current, 10, RefImportForLog + " Version-current ", logId, Report);
                    //Version TARGET (<=> CURRENT)
                    var targetVersionExploded = legacyCrTargetVersion.Split('.');

                    if (targetVersionExploded.Count() != 3)
                    {
                        Report.LogWarning(RefImportForLog + "Target version invalid format : " + legacyCrTargetVersion + ", for CR : " + logId);
                    }
                    else
                    {
                        var mv = Utils.CheckStringToInt(targetVersionExploded[0], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the majorVersion : " + targetVersionExploded[0], logId, Report);
                        var tv = Utils.CheckStringToInt(targetVersionExploded[1], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the technicalVersion : " + targetVersionExploded[1], logId, Report);
                        var ev = Utils.CheckStringToInt(targetVersionExploded[2], 0, RefImportForLog + "(legacyTargetVersion) cannot convert string to int for the editorialVersion : " + targetVersionExploded[2], logId, Report);

                        var targetVersionAssociated = _specVersions.FirstOrDefault(x => x.Fk_ReleaseId == releaseAssociated.Pk_ReleaseId
                                                                                        && x.Fk_SpecificationId == specAssociated.Pk_SpecificationId
                                                                                        && x.MajorVersion == mv
                                                                                        && x.TechnicalVersion == tv
                                                                                        && x.EditorialVersion == ev);

                        if (targetVersionAssociated != null)
                        {
                            newCr.CurrentVersion = targetVersionAssociated;
                            newCr.Fk_CurrentVersion = targetVersionAssociated.Pk_VersionId;
                        }
                        else
                        {
                            Report.LogWarning(RefImportForLog + "Target version not found : " + legacyCrTargetVersion + ", for CR : " + logId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assigned meetings
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void MeetingsCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var tsgMeeting = Utils.CheckString(legacyCr.Meeting_1st_Level, 10, RefImportForLog + "TSG meeting string format : " + legacyCr.Meeting_1st_Level, logId, Report);
            var wgMeeting = Utils.CheckString(legacyCr.Meeting_2nd_Level, 10, RefImportForLog + "WG meeting string format : " + legacyCr.Meeting_2nd_Level, logId, Report);
            
            //TSG
            if (!String.IsNullOrEmpty(tsgMeeting) && !tsgMeeting.Equals("-"))
            {
                var mtg = _meetings.FirstOrDefault(m => m.MtgShortRef.Equals(tsgMeeting));

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "TSG meeting not found: " + tsgMeeting + " for CR : " + logId );
                else
                    newCr.TSGMeeting = mtg.MTG_ID;
            }

            //WG
            if (!String.IsNullOrEmpty(wgMeeting) && !wgMeeting.Equals("-"))
            {
                var mtg = _meetings.FirstOrDefault(m => m.MtgShortRef.Equals(wgMeeting));

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "WG meeting not found: " + wgMeeting + " for CR : " + logId);
                else
                    newCr.WGMeeting = mtg.MTG_ID;
            }
        }

        /// <summary>
        /// Insert remark associated 
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void RemarksCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var remarksField = Utils.CheckString(legacyCr.Remarks, 255, RefImportForLog + " Remarks ", logId, Report);

            if (string.IsNullOrEmpty(remarksField))
                return;

            var remark = new Remark
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                ChangeRequest = newCr
            };
            newCr.Remarks.Add(remark);
        }

        /// <summary>
        /// Assigned a list of WI (separate by '/')
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void WiCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            var wiField = Utils.CheckString(legacyCr.Workitem, 50, RefImportForLog + " WI ", logId, Report);

            if (!string.IsNullOrEmpty(wiField))
            {
                var wiExploded = wiField.Split('/');

                foreach (var wilabel in wiExploded)
                {
                    var wi = _wis.FirstOrDefault(m => m.Acronym == wilabel);

                    if (wi == null)
                        Report.LogWarning(RefImportForLog + "WI not found: " + wiField + " for CR : " + logId);
                    else
                    {
                        var crWi = new CR_WorkItems {ChangeRequest = newCr, WorkItem = wi};
                        newCr.CR_WorkItems.Add(crWi);
                    }
                }
            }
        }

        /// <summary>
        /// Assigned TSG/WG targets + WGSourceForTSG :
        /// Notice : 
        /// - Source WG -> Source-2-level
        /// - Source TSG -> Source-1-level
        /// - WGTarget -> WG_Responsible
        /// - TSGTarget -> WG_Responsible PARENT
        /// - WGSourceForTSG -> WG_Responsible
        /// </summary>
        /// <param name="newCr"></param>
        /// <param name="legacyCr"></param>
        /// <param name="logId"></param>
        private void TsgwgTargetCase(ChangeRequest newCr, Etsi.Ultimate.Tools.TmpDbDataAccess.List_of_GSM___3G_CRs legacyCr, string logId)
        {
            //WGSourceForTSG
            var wgResponsibleLegacy = legacyCr.WG_Responsible;

            if (wgResponsibleLegacy != null && !String.IsNullOrEmpty(wgResponsibleLegacy))
            {
                var community = UltimateContext.Communities.FirstOrDefault(x => x.ShortName == wgResponsibleLegacy);
                if (community != null)
                {
                    //If the community (WG responsible) is found : we take it as the WGTarget and its parent as TSGtarget
                    newCr.WGTarget = community.TbId;
                    newCr.TSGTarget = community.ParentTbId;
                    newCr.WGSourceForTSG = community.TbId;
                }
                else
                {
                    Report.LogWarning(RefImportForLog + "Community not found: " + wgResponsibleLegacy + " for CR : " + logId + " (WG and TSG target not assigned)");
                }
            }
        }
        #endregion
    }
}
