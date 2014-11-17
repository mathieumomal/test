using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Etsi.Ngppdb.DomainClasses;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ngppdb.DataAccess;
using System.Diagnostics;

namespace DatabaseImport.ModuleImport.NGPPDB.Contribution
{
    public class ContributionImport : IModuleImport
    {
        private const string RefImportForLog = "[Contribution(NGPPDB)]";
        List<Enum_ContributionStatus> _enumContributionStatus;
        List<Enum_ContributionType> _enumContributionType;
        List<Meeting> _meetings;
        List<plenary_meetings_with_end_dates> _legacyMeetings;



        /// <summary>
        /// Old table(s) : 
        /// 2006-03-17_tdocs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }
        public HashSet<string> CrWgTdocUids { get; set; }
        private List<string> _lsTDocs;

        public void CleanDatabase()
        {
            NgppdbContext.Contribution_CleanAll();
        }

        public void FillDatabase()
        {
            //Init necessary objects
            _enumContributionStatus = NgppdbContext.Enum_ContributionStatus.ToList();
            _enumContributionType = NgppdbContext.Enum_ContributionType.ToList();
            _legacyMeetings = LegacyContext.plenary_meetings_with_end_dates.AsNoTracking().ToList();
            _lsTDocs = LegacyContext.LSs_importedSnapshot.Select(x => x.Tdoc).Distinct().ToList();

            CrWgTdocUids = new HashSet<string>(LegacyContext.List_of_GSM___3G_CRs.Where(c => c.Doc_2nd_Level != null && !string.IsNullOrEmpty(c.Doc_2nd_Level))
                .AsNoTracking().Select(c => c.Doc_2nd_Level).Distinct());



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
                Console.WriteLine("[CRITICAL ERROR] DB update exception : " + ex.InnerException);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CRITICAL ERROR] Exception : " + ex.InnerException);
                Console.ReadLine();
            }
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.C2006_03_17_tdocs.Count();
            var count = 0;

            var t = new Stopwatch();
            t.Start();

            var legacyTDocs = LegacyContext.C2006_03_17_tdocs.OrderBy(r => r.Row_id).Skip(count).Take(10000).AsNoTracking().ToList();


            while (count != total)
            {
                foreach (var legacyTdoc in legacyTDocs)
                {
                    var newTDoc = new Etsi.Ngppdb.DomainClasses.Contribution
                    {
                        uid = Utils.CheckString(legacyTdoc.doc_tdoc, 200, RefImportForLog + " UID ", legacyTdoc.doc_tdoc),
                        title = Utils.CheckString(legacyTdoc.doc_title, 1000, RefImportForLog + " Title ", legacyTdoc.doc_tdoc),
                        MainContact = "Import from MS Access",
                        fk_Owner = 0,
                        Denorm_Source = Utils.CheckString(legacyTdoc.doc_source, 500, RefImportForLog + " Source ", legacyTdoc.doc_tdoc),
                        lastModificationDate = DateTime.Now,
                        fk_Enum_For = 1,//Decision
                    };

                    StatusCase(newTDoc, legacyTdoc);

                    TypeCase(newTDoc, legacyTdoc);

                    MeetingCase(newTDoc, legacyTdoc);


                    NgppdbContext.Contribution.Add(newTDoc);
                    count++;
                    if (count % 100 == 0)
                    {
                        t.Stop();
                        Console.Write("\r" + RefImportForLog + " {0}/{1} ({2} ms)  ", count, total, t.ElapsedMilliseconds);
                        t.Restart();

                    }
                }
                //Save each 10000 CRs cause of memory reasons
                t.Restart();
                Console.WriteLine();
                Console.Write("\r" + RefImportForLog + " Intermediary Recording  ");
                NgppdbContext.SetValidateOnSave(false);
                NgppdbContext.SaveChanges();
                t.Stop();

                Console.Write("\r" + RefImportForLog + " Saved changes in " + t.ElapsedMilliseconds + " ms");
                legacyTDocs = LegacyContext.C2006_03_17_tdocs.OrderBy(leg => leg.Row_id).Skip(count).Take(10000).AsNoTracking().ToList();
                t.Restart();
                foreach (var elt in NgppdbContext.Contribution)
                {
                    NgppdbContext.SetDetached(elt);
                }
                foreach (var elt in NgppdbContext.MeetingAllocations)
                {
                    NgppdbContext.SetDetached(elt);
                }
                t.Stop();
                Console.Write("\r" + RefImportForLog + " Cleaned context in " + t.ElapsedMilliseconds + " ms");
                Console.WriteLine();
                //NgppdbContext = new NGPPDBContext();*/
                t.Restart();
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
            var defaultStatus = _enumContributionStatus.FirstOrDefault(x => x.Enum_Code.Trim() == Enum_ContributionStatus.Unknown);

            newTDoc.Enum_ContributionStatus = defaultStatus;
            newTDoc.fk_Enum_ContributionStatus = defaultStatus.pk_Enum_ContributionStatus;
        }

        /// <summary>
        /// Status case
        /// Remark : crFlag is always false for each contribution. We cannot determine easily if a contribution is a CR
        /// </summary>
        /// <param name="newTDoc"></param>
        /// <param name="legacyTDoc"></param>
        private void TypeCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc)
        {
            Enum_ContributionType type = null;

            var attachedCr = CrWgTdocUids.FirstOrDefault(t => t == newTDoc.uid);
            var lsTDoc = _lsTDocs.Find(x => x == newTDoc.uid);
            if (attachedCr != null)
            {
                type = _enumContributionType.Find(x => x.Enum_Code == Enum_ContributionType.ChangeRequest);
            }
            else if (lsTDoc != null)
            {
                type = _enumContributionType.Find(x => x.Enum_Code == Enum_ContributionType.LiaisonStatementOut);
            }
            else
            {
                type = _enumContributionType.Find(x => x.Enum_Code == Enum_ContributionType.OtherContribution);
            }

            newTDoc.Enum_ContributionType = type;
            newTDoc.fk_Enum_ContributionType = type.pk_Enum_ContributionType;
        }

        /// <summary>
        /// Assigned meeting
        /// </summary>
        private void MeetingCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc)
        {
            var meetingUid = Utils.CheckString(legacyTDoc.doc_mtg, 25, RefImportForLog + "Meeting string format : " + legacyTDoc.doc_mtg, legacyTDoc.doc_tdoc);

            if (!String.IsNullOrEmpty(meetingUid) && !meetingUid.Equals("-"))
            {
                var mtgId = MtgHelper.FindMeetingId(meetingUid);

                if (mtgId.HasValue)
                {
                    // Check in legacy meeting db.
                    newTDoc.ContribAllocation.Add(new ContribAllocation
                    {
                        fk_Meeting = mtgId.Value,
                        lastModificationAuthor = "Import from MS Access",
                        lastModificationDate = DateTime.Now,
                        ContribAllocation_Date = DateTime.Now,
                        ContribAllocation_Number = 0
                    });
                }
                else
                {
                    LogManager.LogWarning(RefImportForLog + "Meeting not found: " + meetingUid + " for contribution : " + legacyTDoc.doc_tdoc);
                }
            }
            else
            {
                LogManager.LogWarning(RefImportForLog + "Meeting not found: " + meetingUid + " for contribution : " + legacyTDoc.doc_tdoc);
            }
        }

        #endregion

    }

}
