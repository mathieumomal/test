using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;
/*
namespace DatabaseImport.ModuleImport
{
    public class TDocImport : IModuleImport
    {
        public const string RefImportForLog = "[TDoc]";
        List<Enum_TDocStatus> enumTDocStatus;
        int countStatusUndefinded = 0;

        /// <summary>
        /// Old table(s) : 
        /// 2006-03-17_tdocs
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase(){}

        public void FillDatabase()
        {
            enumTDocStatus = NewContext.Enum_TDocStatus.ToList();
            CreateTable();
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
        private void CreateTable()
        {
            NewContext.SetAutoDetectChanges(false);
            var total = LegacyContext.C2006_03_17_tdocs.Count();
            var count = 0;
            foreach (var legacyTdoc in LegacyContext.C2006_03_17_tdocs)
            {
                var newTDoc = new Domain.TDoc();

                newTDoc.UID = Utils.CheckString(legacyTdoc.doc_tdoc, 255, RefImportForLog + " UID ", legacyTdoc.doc_tdoc, Report);//NOT SURE !!!!!!!!!!!!!!!!!!!!!!
                newTDoc.Title = Utils.CheckString(legacyTdoc.doc_title,255,RefImportForLog + " Title ",legacyTdoc.doc_tdoc, Report);
                //newTDoc.DecisionDate = ;
                //newTDoc.Filename = ;

                //newTDoc.SubmittedAt = ;
                //newTDoc.ReservedAt = ;
                //newTDoc.Abstract = ;
                newTDoc.Source = Utils.CheckString(legacyTdoc.doc_source, 255, RefImportForLog + " Source ", legacyTdoc.doc_tdoc, Report); ;

                //newTDoc.Fk_InputGroup = ;
                //newTDoc.Fk_MainContact = ;
                //newTDoc.Fk_MeetingAllocation = ;

                //newTDoc.Fk_AgendaAllocation = ;
                StatusCase(newTDoc, legacyTdoc, countStatusUndefinded);//Accès via remarques
                //newTDoc.Fk_TDocFor = ;
                //newTDoc.Fk_TDocType = ;
                NewContext.TDocs.Add(newTDoc);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
                if (count % 100000 == 0)
                {
                    Console.Write("\r" + RefImportForLog + " Intermediary Recording  ");
                    //NewContext.SetValidateOnSave(false);
                    NewContext.SaveChanges();
                    foreach (var elt in NewContext.TDocs)
                    {
                        NewContext.SetDetached(elt);
                    }
                    //NewContext.SetValidateOnSave(true);
                }
            }
            NewContext.SetAutoDetectChanges(true);
            Report.LogWarning(RefImportForLog + " " + countStatusUndefinded + " TDoc Status undefined.");
        }

        private void StatusCase(Domain.TDoc newTDoc, OldDomain.C2006_03_17_tdocs legacyTDoc, int countStatusUndefinded)
        {
            var legacyStatus = Utils.CheckString(legacyTDoc.doc_remarks, 0, RefImportForLog + " status ", legacyTDoc.doc_tdoc, Report).ToLower();
            var status = enumTDocStatus.Where(x => x.Status == legacyStatus).FirstOrDefault();

            if (status != null)
            {
                newTDoc.Enum_TDocStatus = status;
                newTDoc.Fk_TDocStatus = status.Pk_EnumTDocStatus;
            }
            else
            {
                countStatusUndefinded++;
            }
        }
        #endregion
         
    }
         
}* * */
