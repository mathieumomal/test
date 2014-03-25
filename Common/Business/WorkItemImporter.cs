using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using OfficeOpenXml.Style;

namespace Etsi.Ultimate.Business
{
    public class WorkItemImporter
    {
        private static readonly string CACHE_KEY = "WI_IMPORT_";

        public IUltimateUnitOfWork UoW { get; set; }

        public KeyValuePair<string, ImportReport> TryImportCsv(string filePath)
        {

            var path = filePath;
            // Treat the file
            if (path.EndsWith("zip"))
            {
                path = ExtractZipGetCsv(filePath);
                if (String.IsNullOrEmpty(path))
                {
                    var report = new ImportReport();
                    report.LogError(Utils.Localization.WorkItem_Import_Bad_Zip_File);
                    return new KeyValuePair<string, ImportReport>("", report);
                }
                
            }

            string token = "";

            var csvParser = ManagerFactory.Resolve<IWorkItemCsvParser>();
            csvParser.UoW = UoW ;
            var result = csvParser.ParseCsv(path);
            if (result.Value.GetNumberOfErrors() == 0)
            {
                token = Guid.NewGuid().ToString();
                CacheManager.InsertForLimitedTime(CACHE_KEY+token, result.Key, 10);
            }
            return new KeyValuePair<string, ImportReport>(token, result.Value);

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

        public bool ImportWorkPlan(string token, string exportPath)
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

            foreach (var wi in workPlan)
            {
                wiRepo.InsertOrUpdate(wi);
            }

            ExportToExcel(workPlan, exportPath);
            ExportToWord(workPlan, exportPath);

            return true;
        }

        /// <summary>
        /// Export Work Plan to Excel
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToExcel(List<WorkItem> workPlan, string exportPath)
        {
            if (!String.IsNullOrEmpty(exportPath))
            {
                //Create Empty Work Book
                string file = exportPath + @"WorkItemExport.xlsx";
                if (File.Exists(file)) File.Delete(file);
                FileInfo newFile = new FileInfo(file);

                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    List<WorkItemForExport> exportWorkPlan = new List<WorkItemForExport>();
                    workPlan.ForEach(x => exportWorkPlan.Add(new WorkItemForExport(x)));

                    // get the handle to the existing worksheet
                    var wsData = pck.Workbook.Worksheets.Add("Work Items");

                    /*------------*/
                    /* Set Styles */
                    /*------------*/
                    //Set Font Style
                    wsData.Cells.Style.Font.Size = 8;
                    wsData.Cells.Style.Font.Name = "Arial";
                    //Set Header Style
                    wsData.Cells[1, 1, 1, 16].Style.Font.Bold = true;
                    wsData.Cells[1, 1, 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsData.Cells[1, 1, 1, 16].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    //Set Name Column as Bold
                    wsData.Cells[1, 3, exportWorkPlan.Count + 1, 3].Style.Font.Bold = true;
                    //Set Complete column with Percentage Format
                    wsData.Cells[1, 10, exportWorkPlan.Count + 1, 10].Style.Numberformat.Format = "0%";
                    //Set Cell Borders
                    wsData.Cells[1, 1, exportWorkPlan.Count + 1, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[1, 1, exportWorkPlan.Count + 1, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[1, 1, exportWorkPlan.Count + 1, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[1, 1, exportWorkPlan.Count + 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //Set Filters
                    wsData.Cells[1, 1, 1, 16].AutoFilter = true;

                    /*--------------*/
                    /* Add Formulas */
                    /*--------------*/
                    ExcelAddress nameAddress = new ExcelAddress(2, 3, exportWorkPlan.Count + 1, 3);
                    //Name should be Red if Unique ID is 0
                    var ruleNoUniqueKey = wsData.ConditionalFormatting.AddExpression(nameAddress);
                    ruleNoUniqueKey.Style.Font.Color.Color = Color.Red;
                    ruleNoUniqueKey.Formula = "B2=0";
                    ruleNoUniqueKey.Priority = 1;

                    //Level 1 Name should be in Blue font
                    var ruleLevel1 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                    ruleLevel1.Style.Font.Color.Color = Color.Blue;
                    ruleLevel1.Formula = "E2=1";
                    ruleLevel1.Priority = 2;

                    //Level 2 Name should be in Black font
                    var ruleLevel2 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                    ruleLevel2.Style.Font.Color.Color = Color.Black;
                    ruleLevel2.Formula = "E2=2";
                    ruleLevel2.Priority = 3;

                    //Level 3 Name should be in Black font without Bold
                    var ruleLevel3 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                    ruleLevel3.Style.Font.Color.Color = Color.Black;
                    ruleLevel3.Style.Font.Bold = false;
                    ruleLevel3.Formula = "E2=3";
                    ruleLevel3.Priority = 4;

                    //Level 4 Name should be in Black font without Bold
                    var ruleLevel4 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                    ruleLevel4.Style.Font.Color.Color = Color.Black;
                    ruleLevel4.Style.Font.Bold = false;
                    ruleLevel4.Formula = "E2=4";
                    ruleLevel4.Priority = 5;

                    ExcelAddress completeTableAddress = new ExcelAddress(2, 1, exportWorkPlan.Count + 1, 16);
                    //Stopped WorkItems should have light brown background
                    var ruleDeleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                    ruleDeleted.Style.Fill.BackgroundColor.Color = Color.FromArgb(227, 227, 227);
                    ruleDeleted.Formula = "LEFT(TRIM($C2), LEN(\"Deleted -\")) = \"Deleted -\"";
                    ruleDeleted.Priority = 6;

                    //100% completed workitems should have light green background
                    var ruleCompleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                    ruleCompleted.Style.Fill.BackgroundColor.Color = Color.FromArgb(204, 255, 204);
                    ruleCompleted.Formula = "$J2=100%";
                    ruleCompleted.Priority = 7;

                    //Upload Data to Excel
                    var dataRange = wsData.Cells["A1"].LoadFromCollection(
                                                  from s in exportWorkPlan
                                                  orderby s.UID
                                                  select s, true, OfficeOpenXml.Table.TableStyles.None);
                    dataRange.AutoFitColumns();

                    pck.Save();
                }
            }
        }

        private void ExportToWord(List<WorkItem> workPlan, string exportPath)
        { 
        
        }
    }
}
