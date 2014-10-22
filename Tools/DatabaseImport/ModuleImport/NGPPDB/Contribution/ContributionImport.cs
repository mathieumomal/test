using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Etsi.Ngppdb.DomainClasses;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.NGPPDB.Contribution
{
    public class ContributionImport : IModuleImport
    {
        private const string RefImportForLog = "[Contribution(NGPPDB)]";
        List<Enum_ContributionStatus> _enumContributionStatus;
        List<Enum_ContributionType> _enumContributionType;
        List<Meeting> _meetings;

        /// <summary>
        /// Old table(s) : 
        /// 2006-03-17_tdocs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Report Report { get; set; }

        public void CleanDatabase()
        {
            NgppdbContext.Contribution_CleanAll();
        }

        public void FillDatabase()
        {
            //Init necessary objects
            _enumContributionStatus = NgppdbContext.Enum_ContributionStatus.ToList();
            _enumContributionType = NgppdbContext.Enum_ContributionType.ToList();
            _meetings = UltimateContext.Meetings.ToList();

            NgppdbContext.SetAutoDetectChanges(false);
            CreateDatas();
            NgppdbContext.SetAutoDetectChanges(true);

            try
            {
                NgppdbContext.SaveChanges();
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
        private void CreateDatas()
        {
            var total = LegacyContext.C2006_03_17_tdocs.Count();
            var count = 0;
            foreach (var legacyTdoc in LegacyContext.C2006_03_17_tdocs)
            {
                var newTDoc = new Etsi.Ngppdb.DomainClasses.Contribution
                {
                    uid = Utils.CheckString(legacyTdoc.doc_tdoc, 200, RefImportForLog + " UID ", legacyTdoc.doc_tdoc, Report),
                    title = Utils.CheckString(legacyTdoc.doc_title, 200, RefImportForLog + " Title ", legacyTdoc.doc_tdoc, Report),
                    MainContact = "Import from MS Access",
                    fk_Owner = 0,
                    Denorm_Source = Utils.CheckString(legacyTdoc.doc_source, 500, RefImportForLog + " Source ", legacyTdoc.doc_tdoc, Report),
                    lastModificationDate = DateTime.Now,
                    fk_Enum_For = 1,//Decision
                };

                StatusCase(newTDoc, legacyTdoc);

                TypeCase(newTDoc, legacyTdoc);

                MeetingCase(newTDoc, legacyTdoc);

                
                NgppdbContext.Contribution.Add(newTDoc);
                count++;
                if (count % 100 == 0)
                    Console.Write("\r" + RefImportForLog + " {0}/{1}  ", count, total);
                //Save each 10000 CRs cause of memory reasons
                if (count % 10000 == 0)
                {
                    Console.Write("\r" + RefImportForLog + " Intermediary Recording  ");
                    NgppdbContext.SetValidateOnSave(false);
                    NgppdbContext.SaveChanges();
                    foreach (var elt in NgppdbContext.Contribution)
                    {
                        NgppdbContext.SetDetached(elt);
                    }
                    NgppdbContext.SetValidateOnSave(true);
                }
            }
        }

        /// <summary>
        /// Status case
        /// Default value : treated
        /// </summary>
        /// <param name="newTDoc"></param>
        /// <param name="legacyTDoc"></param>
        private void StatusCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc)
        {
            var legacyStatus = Utils.CheckString(legacyTDoc.doc_remarks, 0, RefImportForLog + " status ", legacyTDoc.doc_tdoc, Report).ToLower();
            var status = _enumContributionStatus.FirstOrDefault(x => legacyStatus.ToLower().Contains(x.Enum_Value.ToLower()));
            var defaultStatus = _enumContributionStatus.FirstOrDefault(x => x.Enum_Code.Trim() == Enum_ContributionStatus.Treated);

            if (status != null)
            {
                newTDoc.Enum_ContributionStatus = status;
                newTDoc.fk_Enum_ContributionStatus = status.pk_Enum_ContributionStatus;
            }
            else
            {
                newTDoc.Enum_ContributionStatus = defaultStatus;
                newTDoc.fk_Enum_ContributionStatus = defaultStatus.pk_Enum_ContributionStatus;
                Report.LogWarning(RefImportForLog + "Status not found : " + legacyStatus + " for contribution : " + legacyTDoc.doc_tdoc + "default value setted : " + defaultStatus.Enum_Value);
            }
        }

        /// <summary>
        /// Status case
        /// Remark : crFlag is always false for each contribution. We cannot determine easily if a contribution is a CR
        /// </summary>
        /// <param name="newTDoc"></param>
        /// <param name="legacyTDoc"></param>
        private void TypeCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc)
        {
            var type = _enumContributionType.Find(x => x.Enum_Code == Enum_ContributionType.OtherContribution);
            newTDoc.Enum_ContributionType = type;
            newTDoc.fk_Enum_ContributionType = type.pk_Enum_ContributionType;
        }

        /// <summary>
        /// Assigned meeting
        /// </summary>
        private void MeetingCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc)
        {
            var meetingUid = Utils.CheckString(legacyTDoc.doc_mtg, 25, RefImportForLog + "Meeting string format : " + legacyTDoc.doc_mtg, legacyTDoc.doc_tdoc, Report);

            if (!String.IsNullOrEmpty(meetingUid) && !meetingUid.Equals("-"))
            {
                var mtg = _meetings.FirstOrDefault(m => m.MtgShortRef.Equals(meetingUid));

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "Meeting not found: " + meetingUid + " for contribution : " + legacyTDoc.doc_tdoc);
                else
                    newTDoc.ContribAllocation.Add(new ContribAllocation
                    {
                        fk_Meeting = mtg.MTG_ID,
                        lastModificationAuthor = "Import from MS Access",
                        lastModificationDate = DateTime.Now,
                        ContribAllocation_Date = DateTime.Now,
                        ContribAllocation_Number = 0
                    });
            }
            else
            {
                Report.LogWarning(RefImportForLog + "Meeting not found: " + meetingUid + " for contribution : " + legacyTDoc.doc_tdoc);
            }
        }

        #endregion
         
    }
         
}
