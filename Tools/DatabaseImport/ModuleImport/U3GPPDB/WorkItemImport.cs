using System;
using System.IO;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace DatabaseImport.ModuleImport.U3GPPDB
{
    public class WorkItemImport : IModuleImport
    {
        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext
        {
            get;
            set;
        }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext
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
            UltimateContext.WorkItems_CleanAll();
        }

        public void FillDatabase()
        {
            Console.WriteLine("Saving previous transactions .....");
            UltimateContext.SaveChanges();

            if (File.Exists("./referenceWorkPlan.csv"))
            {
                Console.WriteLine("Importing workplan from reference file");
                var csvParser = new WorkItemCsvParser();
                csvParser.UoW = new UltimateUnitOfWork();
                var result = csvParser.ParseCsv("./referenceWorkPlan.csv");
                var workItemList = result.Key;

                UltimateContext.SetAutoDetectChanges(false);
                Console.WriteLine("Found " + workItemList.Count + " work items");
                foreach (var wi in workItemList)
                {
                    UltimateContext.SetAdded(wi);
                }
                UltimateContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("Please perform the WI import using WorkItem Module and press Enter to continue");
                Console.Read();
            }
        }
    }
}
