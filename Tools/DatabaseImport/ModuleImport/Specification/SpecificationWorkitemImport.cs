using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service = Etsi.Ultimate.Services;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;

namespace DatabaseImport.ModuleImport
{
    public class SpecificationWorkitemImport : IModuleImport
    {
        #region IModuleImport Membres
        public const string RefImportForLog = "[Specification/WorkItems]";
        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            NewContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.C2008_03_08_Specs_vs_WIs.Count();
            var count = 0;
            foreach (var old_spec_work in LegacyContext.C2008_03_08_Specs_vs_WIs)
            {
                var new_spec_Work = new Domain.Specification_WorkItem();

                var spec = NewContext.Specifications.Where(x => x.Number == old_spec_work.Spec.Trim()).FirstOrDefault();
                var work = NewContext.WorkItems.Where(x => x.Pk_WorkItemUid == old_spec_work.WI_UID).FirstOrDefault();

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
                        NewContext.Specification_WorkItem.Add(new_spec_Work);
                    }
                    else
                    {
                        Report.LogWarning(RefImportForLog + " Specification not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                }
                else
                {
                    if (spec == null)
                    {
                        Report.LogWarning(RefImportForLog + " Specification not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                    if (work == null)
                    {
                        Report.LogWarning(RefImportForLog + " WorkItem not found (Spec Number : " + old_spec_work.Spec + ", for WorkItem UID : " + old_spec_work.WI_UID + ")");
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
