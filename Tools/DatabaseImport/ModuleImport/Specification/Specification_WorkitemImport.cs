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

namespace DatabaseImport.ModuleImport.Specification
{
    public class Specification_WorkitemImport : IModuleImport
    {
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            foreach (var spec_work in NewContext.Specification_WorkItem.ToList())
            {
                NewContext.Specification_WorkItem.Remove(spec_work);
            }
        }

        public void FillDatabase()
        {
            foreach (var old_spec_work in LegacyContext.C2008_03_08_Specs_vs_WIs)
            {
                var new_spec_Work = new Domain.Specification_WorkItem();

                var spec = NewContext.Specifications.Where(x=>x.Number == old_spec_work.Spec).FirstOrDefault();
                //Pk_WorkItemUid <=> WI_UID ?????
                var work = NewContext.WorkItems.Where(x => x.Pk_WorkItemUid == old_spec_work.WI_UID).FirstOrDefault();

                if (spec != null && work != null)
                {
                    new_spec_Work.Fk_SpecificationId = spec.Pk_SpecificationId;
                    // ""->
                    new_spec_Work.Fk_WorkItemId = work.Pk_WorkItemUid;
                    NewContext.Specification_WorkItem.Add(new_spec_Work);
                }
                else
                {
                    if (spec == null && work == null)
                    {
                        Report.LogError("Specification and WorkItem not found (Spec Number : " + old_spec_work.Spec + ", WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                    else if (spec == null)
                    {
                        Report.LogError("Specification not found (Spec Number : " + old_spec_work.Spec + ", WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                    else if (work == null)
                    {
                        Report.LogError("WorkItem not found (Spec Number : " + old_spec_work.Spec + ", WorkItem UID : " + old_spec_work.WI_UID + ")");
                    }
                }
                


                
            }
        }

        #endregion
    }
}
