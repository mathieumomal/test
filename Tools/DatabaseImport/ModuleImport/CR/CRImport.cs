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
            enumCategory = NewContext.Enum_CRCategory.ToList();
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
                newCR.Revision = Utils.CheckStringToInt(legacyCR.Rev, null, " Revision ", legacyCR.CR, Report);
                newCR.Subject = Utils.CheckString(legacyCR.Subject, 300, RefImportForLog + " Subject", legacyCR.CR, Report);
                newCR.CreationDate = legacyCR.created;
                CategoryCase(newCR, legacyCR);

                //newCR.Fk_TargetRelease = ;
                //
                //newCR.Fk_TSGMeeting = ;
                //newCR.Fk_WGMeeting = ;
                //newCR.Fk_TSGTarget = ;
                //newCR.Fk_WGTarget = ;
                //newCR.Fk_WGSourceForTSG = ;
                //newCR.Fk_TSGTDoc = ;
                //newCR.Fk_WGTDoc = ;

                //Faisable
                //newCR.TSGSourceOrganizations = ; Source 1 level
                //newCR.WGSourceOrganizations = ; Source 2 level
                //newCR.TSGStatus = ;Status 1 level
                //newCR.WGStatus = ;Status 2 level
                //newCR.Fk_Specification = ;
                //newCR.Fk_TargetVersion = ;
                //newCR.Fk_NewVersion = ;

                NewContext.ChangeRequests.Add(newCR);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }


        private void CategoryCase(Domain.ChangeRequest newCR, OldDomain.List_of_GSM___3G_CRs legacyCR)
        {
            var legacyCRCategory = Utils.CheckString(legacyCR.Cat, 5, " category ", legacyCR.CR, Report);
            var categoryAssiocated = enumCategory.Where(x => x.Category == legacyCRCategory).FirstOrDefault();

            if (categoryAssiocated != null)
            {
                newCR.Enum_CRCategory = categoryAssiocated;
                newCR.Fk_Enum_CRCategory = categoryAssiocated.Pk_EnumCRCategory;
            }
            else
            {
                Report.LogWarning(RefImportForLog + "Category not found : " + categoryAssiocated + " for CR : " + legacyCR.CR);
            }
        }
        #endregion
    }
}
