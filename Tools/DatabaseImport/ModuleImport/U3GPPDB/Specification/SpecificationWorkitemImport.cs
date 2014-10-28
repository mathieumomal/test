using System;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    public class SpecificationWorkitemImport : IModuleImport
    {
        #region IModuleImport Membres
        public const string RefImportForLog = "[Specification/WorkItems]";
        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            UltimateContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.C2008_03_08_Specs_vs_WIs.Count();
            var count = 0;
            foreach (var old_spec_work in LegacyContext.C2008_03_08_Specs_vs_WIs)
            {
                var new_spec_Work = new Specification_WorkItem();

                var spec = UltimateContext.Specifications.Where(x => x.Number == old_spec_work.Spec.Trim()).FirstOrDefault();
                var work = UltimateContext.WorkItems.Where(x => x.Pk_WorkItemUid == old_spec_work.WI_UID).FirstOrDefault();

                if (spec != null && work != null)
                {
                    new_spec_Work.Fk_SpecificationId = spec.Pk_SpecificationId;
                    new_spec_Work.Fk_WorkItemId = work.Pk_WorkItemUid;
                    new_spec_Work.IsSetByUser = true;
                    var old_spec = LegacyContext.Specs_GSM_3G.Where(x => x.Number == spec.Number).FirstOrDefault();
                    if (old_spec != null)
                    {
                        if (old_spec.WI_UID.Equals(old_spec_work.WI_UID))
                            new_spec_Work.isPrime = true;
                        else
                            new_spec_Work.isPrime = false;
                        UltimateContext.Specification_WorkItem.Add(new_spec_Work);
                    }
                    else
                    {
                        LogManager.LogWarning(RefImportForLog + " Specification not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                }
                else
                {
                    if (spec == null)
                    {
                        LogManager.LogWarning(RefImportForLog + " Specification not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                    if (work == null)
                    {
                        LogManager.LogWarning(RefImportForLog + " WorkItem not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                }
                count++;
                if(count%100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }
        #endregion
    }
}
