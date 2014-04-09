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
    public class WorkItemImport : IModuleImport
    {
        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext
        {
            get;
            set;
        }

        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext
        {
            get;
            set;
        }

        public ImportReport Report
        {
            get;
            set;
        }

        /// <summary>
        /// Removes all the releases from the database.
        /// 
        /// Also removes all the remarks that are linked to releases.
        /// </summary>
        public void CleanDatabase()
        {
            foreach (var aWorkItem in NewContext.WorkItems.ToList())
            {
                // Remove associated remarks
                var remarks = NewContext.Remarks.Where(r => r.Fk_WorkItemId == aWorkItem.Pk_WorkItemUid).ToList();
                for (int i = 0; i < remarks.Count; ++i)
                    NewContext.Remarks.Remove(remarks[i]);

                var workItemResponsibles = NewContext.WorkItems_ResponsibleGroups.Where(g => g.Fk_WorkItemId == aWorkItem.Pk_WorkItemUid).ToList();
                for (int i = 0; i < workItemResponsibles.Count; ++i)
                    NewContext.WorkItems_ResponsibleGroups.Remove(workItemResponsibles[i]);

                NewContext.WorkItems.Remove(aWorkItem);
            }
        }

        public void FillDatabase()
        {
            Console.WriteLine("Saving previous transactions .....");
            NewContext.SaveChanges();
            Console.WriteLine("Please perform the WI import using WorkItem Module and press Enter to continue");
            Console.Read();            
        }
    }
}
