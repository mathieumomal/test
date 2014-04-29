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
using System.IO;

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
            NewContext.WorkItems_CleanAll();
        }

        public void FillDatabase()
        {
            Console.WriteLine("Saving previous transactions .....");
            NewContext.SaveChanges();

            if (File.Exists("../../referenceWorkPlan.csv"))
            {
                Console.WriteLine("Importing workplan from reference file");
                var csvParser = new WorkItemCsvParser();
                csvParser.UoW = new UltimateUnitOfWork();
                var result = csvParser.ParseCsv("../../referenceWorkPlan.csv");
                var workItemList = result.Key;

                NewContext.SetAutoDetectChanges(false);
                Console.WriteLine("Found " + workItemList.Count + " work items");
                foreach (var wi in workItemList)
                {
                    NewContext.SetAdded(wi);
                }
                NewContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("Please perform the WI import using WorkItem Module and press Enter to continue");
                Console.Read();
            }
        }
    }
}
