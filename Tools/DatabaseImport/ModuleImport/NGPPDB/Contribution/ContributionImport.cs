using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Etsi.Ngppdb.DomainClasses;

namespace DatabaseImport.ModuleImport.NGPPDB.Contribution
{
    public class ContributionImport : IModuleImport
    {
        private const string RefImportForLog = "[Contribution(NGPPDB)]";
        List<Enum_ContributionStatus> _enumContributionStatus;

        /// <summary>
        /// Old table(s) : 
        /// 2006-03-17_tdocs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase(){}

        public void FillDatabase()
        {
            //Init necessary objects
            _enumContributionStatus = NgppdbContext.Enum_ContributionStatus.ToList();

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
                var newTDoc = new Etsi.Ngppdb.DomainClasses.Contribution();

                newTDoc.uid = Utils.CheckString(legacyTdoc.doc_tdoc, 255, RefImportForLog + " UID ", legacyTdoc.doc_tdoc, Report);//NOT SURE !!!!!!!!!!!!!!!!!!!!!!
                newTDoc.title = Utils.CheckString(legacyTdoc.doc_title,255,RefImportForLog + " Title ",legacyTdoc.doc_tdoc, Report);
                //newTDoc.DecisionDate = ;
                //newTDoc.Filename = ;

                //newTDoc.SubmittedAt = ;
                //newTDoc.ReservedAt = ;
                //newTDoc.Abstract = ;
                //newTDoc.Source = Utils.CheckString(legacyTdoc.doc_source, 255, RefImportForLog + " Source ", legacyTdoc.doc_tdoc, Report); ;

                //newTDoc.Fk_InputGroup = ;
                //newTDoc.Fk_MainContact = ;
                //newTDoc.Fk_MeetingAllocation = ;

                //newTDoc.Fk_AgendaAllocation = ;
                //StatusCase(newTDoc, legacyTdoc, CountStatusUndefinded);//Accès via remarques
                //newTDoc.Fk_TDocFor = ;
                //newTDoc.Fk_TDocType = ;
                NgppdbContext.Contribution.Add(newTDoc);
                count++;
                if (count % 100 == 0)
                    Console.Write("\r" + RefImportForLog + " {0}/{1}  ", count, total);
                //Save each 10000 CRs cause of memory reasons
                if (count % 1000 == 0)
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

        /*private void StatusCase(Etsi.Ngppdb.DomainClasses.Contribution newTDoc, Etsi.Ultimate.Tools.TmpDbDataAccess.C2006_03_17_tdocs legacyTDoc, int countStatusUndefinded)
        {
            var legacyStatus = Utils.CheckString(legacyTDoc.doc_remarks, 0, RefImportForLog + " status ", legacyTDoc.doc_tdoc, Report).ToLower();
            var status = _enumContributionStatus.Where(x => x.Enum_Code == legacyStatus).FirstOrDefault();

            if (status != null)
            {
                newTDoc.Enum_ContributionStatus = status;
                newTDoc.fk_Enum_ContributionStatus = status.pk_Enum_ContributionStatus;
            }
            else
            {
                countStatusUndefinded++;
            }
        }*/
        #endregion
         
    }
         
}
