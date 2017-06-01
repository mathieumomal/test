using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class WorkItemImporter : IWorkItemImporter
    {
        private readonly string CACHE_KEY = "WI_IMPORT_";

        public IUltimateUnitOfWork UoW { get; set; }

        public KeyValuePair<string, Report> TryImportCsv(string filePath)
        {
            LogManager.Info("WORKITEM IMPORT: System is importing WorkItems thanks to the file: " + filePath + "...");
            try
            {
                var path = filePath;
                // Treat the file
                if (path.EndsWith("zip"))
                {
                    LogManager.Info("WORKITEM IMPORT:   Extraction of the content of the ZIP...");
                    path = ExtractZipGetCsv(filePath);
                    if (String.IsNullOrEmpty(path))
                    {
                        var report = new Report();
                        report.LogError(Utils.Localization.WorkItem_Import_Bad_Zip_File);
                        LogManager.Error("WORKITEM IMPORT:  ERROR -> No CSV file found inside.");
                        return new KeyValuePair<string, Report>("", report);
                    }

                }

                string token = "";

                var csvParser = ManagerFactory.Resolve<IWorkItemCsvParser>();
                csvParser.UoW = UoW;
                var result = csvParser.ParseCsv(path);
                if (result.Value.GetNumberOfErrors() == 0)
                {
                    token = Guid.NewGuid().ToString();
                    CacheManager.InsertForLimitedTime(CACHE_KEY + token, result.Key, 10);
                }
                return new KeyValuePair<string, Report>(token, result.Value);
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { filePath }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                var report = new Report();
                report.LogError(Utils.Localization.WorkItem_Import_Error_Analysis);
                return new KeyValuePair<string, Report>("", report);
            }

        }

        private string ExtractZipGetCsv(string filePath)
        {
            var files = Zip.Extract(filePath, false);
            if (files.Count != 1 || !files.First().EndsWith("csv"))
            {
                return "";
            }
            else
            {
                return files.First();
            }
        }

        public bool ImportWorkPlan(string token)
        {
            // Fetch the data in cache. If there is no data, then it's outdated.
            var workPlan = (List<WorkItem>)CacheManager.Get(CACHE_KEY+ token);
            if (workPlan == null)
            {
                return false;
            }

            // Else, call the repository
            var wiRepo = RepositoryFactory.Resolve<IWorkItemRepository>();
            wiRepo.UoW = UoW;
            try
            {
                UoW.SetAutoDetectChanges(false);
                foreach (var wi in workPlan)
                {
                    wiRepo.InsertOrUpdate(wi);
                }
            }
            finally
            {
                UoW.SetAutoDetectChanges(true);
            }
            
            return true;
        }
    }

    public interface IWorkItemImporter
    {
        IUltimateUnitOfWork UoW { get; set; }

        KeyValuePair<string, Report> TryImportCsv(string filePath);

        bool ImportWorkPlan(string token);
    }
}
