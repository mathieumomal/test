using Novacode;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Domain = Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
{
    public class WorkPlanExporter
    {

        /// <summary>
        /// Export Work Plan to Excel
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        public static void ExportToExcel(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!String.IsNullOrEmpty(exportPath) && workPlan.Count >= 1)
            {
                //Create Empty Work Book
                string file = exportPath + @"WorkItemExport.xlsx";
                if (File.Exists(file)) File.Delete(file);
                FileInfo newFile = new FileInfo(file);

                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    List<Domain.WorkItemForExport> exportWorkPlan = new List<Domain.WorkItemForExport>();
                    workPlan.ForEach(x => exportWorkPlan.Add(new Domain.WorkItemForExport(x)));

                    // get the handle to the existing worksheet
                    var wsData = pck.Workbook.Worksheets.Add("Work Items");

                    /*------------*/
                    /* Set Styles */
                    /*------------*/
                    int rowHeader = 1;
                    int rowDataStart = 2;
                    int rowDataEnd = exportWorkPlan.Count + 1;
                    int columnStart = 1;
                    int columnEnd = 17;
                    int columnName = 3;
                    int columnCompletion = 10;

                    //Set Font Style
                    wsData.Cells.Style.Font.Size = 8;
                    wsData.Cells.Style.Font.Name = "Arial";
                    //Set Header Style
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Font.Bold = true;
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    //Set Name Column as Bold
                    wsData.Cells[rowDataStart, columnName, rowDataEnd, columnName].Style.Font.Bold = true;
                    //Set Complete column with Percentage Format
                    wsData.Cells[rowDataStart, columnCompletion, rowDataEnd, columnCompletion].Style.Numberformat.Format = "0%";
                    //Set Cell Borders
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //Set Filters
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].AutoFilter = true;

                    //Set Column Width
                    wsData.DefaultColWidth = 10;
                    wsData.Column(1).Width = wsData.Column(5).Width = wsData.Column(10).Width = 5;
                    wsData.Column(3).Width = wsData.Column(16).Width = 35;

                    //Set Row Height
                    wsData.DefaultRowHeight = 12;
                    //Set Zoom to 85%
                    wsData.View.ZoomScale = 85;

                    /*--------------*/
                    /* Add Formulas */
                    /*--------------*/
                    ExcelAddress nameAddress = new ExcelAddress(rowDataStart, columnName, rowDataEnd, columnName);
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

                    ExcelAddress completeTableAddress = new ExcelAddress(rowDataStart, columnStart, rowDataEnd, columnEnd);
                    var stoppedMeetingIds = exportWorkPlan.Where(x => x.StoppedMeeting == true && x.Wpid != null).Select(y => y.Wpid).ToList();

                    //Stopped WorkItems should have light brown background
                    var ruleDeleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                    ruleDeleted.Style.Fill.BackgroundColor.Color = Color.FromArgb(227, 227, 227);
                    ruleDeleted.Formula = "SEARCH(CONCATENATE(\"[\",$A2,\"]\"), CONCATENATE(\"[" + String.Join("]\",\"[", stoppedMeetingIds) + "]\"))>0";
                    ruleDeleted.Priority = 6;

                    //100% completed workitems should have light green background
                    var ruleCompleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                    ruleCompleted.Style.Fill.BackgroundColor.Color = Color.FromArgb(204, 255, 204);
                    ruleCompleted.Formula = "$J2=100%";
                    ruleCompleted.Priority = 7;

                    //Upload Data to Excel
                    var dataRange = wsData.Cells["A1"].LoadFromCollection(
                                                  from s in exportWorkPlan
                                                  select new { 
                                                      ID = s.Wpid,
                                                      Unique_ID = s.UID,
                                                      Name = s.Name,
                                                      Acronym = s.Acronym,
                                                      Outline_Level = s.Level,
                                                      Release = s.Release,
                                                      Resource_Names = s.ResponsibleGroups,
                                                      Start_Date = s.StartDate,
                                                      Finish_Date = s.EndDate,
                                                      Percent_Complete = s.Completion,
                                                      Hyperlink = s.HyperLink,
                                                      Status_Report = s.StatusReport,
                                                      WI_rapporteur_name = s.WIRaporteur,
                                                      WI_rapporteur_e_mail = s.WIRaporteurEmail,
                                                      Notes = s.Notes,
                                                      Impacted_TSs_and_TRs = s.RelatedTSs_TRs,
                                                      Special_Focus_Doc = String.Empty                                                  
                                                  } ,
                                                  true, OfficeOpenXml.Table.TableStyles.None);
                    pck.Save();
                }
            }
        }

        public static void ExportToWord(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!string.IsNullOrEmpty(exportPath))
            {

                string file = exportPath + "WorkItemExport.docx";
                if (File.Exists(file)) File.Delete(file);
                List<Domain.WorkItemForExport> exportWorkPlan = new List<Domain.WorkItemForExport>();
                workPlan.ForEach(x => exportWorkPlan.Add(new Domain.WorkItemForExport(x)));
                using (DocX document = DocX.Create(file))
                {
                    document.MarginLeft = 0F;
                    document.MarginRight = 0F;
                    document.InsertParagraph("Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd"));
                    document.InsertParagraph("");

                    Table legendTable = document.AddTable(5, 1);
                    legendTable.Rows[0].Cells[0].Paragraphs.First().Append("LEGEND");
                    legendTable.Rows[1].Cells[0].Paragraphs.First().Append("ONGOING");
                    legendTable.Rows[2].Cells[0].Paragraphs.First().Append("COMPLETED");
                    legendTable.Rows[3].Cells[0].Paragraphs.First().Append("STOPPED");
                    legendTable.Rows[4].Cells[0].Paragraphs.First().Append("-");
                    document.InsertTable(legendTable);

                    document.InsertParagraph("");

                    Table t = document.AddTable(exportWorkPlan.Count, 12);
                    t.AutoFit = AutoFit.Contents;
                    for (int i = 0; i < exportWorkPlan.Count; i++)
                    {
                        t.Rows[i].Cells[0].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[0].Paragraphs.First().Append(exportWorkPlan[i].Wpid.Value.ToString());

                        t.Rows[i].Cells[1].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[1].Paragraphs.First().Append(exportWorkPlan[i].UID.ToString());

                        t.Rows[i].Cells[2].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[2].Paragraphs.First().Append(exportWorkPlan[i].Name);

                        t.Rows[i].Cells[3].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[3].Paragraphs.First().Append(exportWorkPlan[i].Acronym);

                        t.Rows[i].Cells[4].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[4].Paragraphs.First().Append(exportWorkPlan[i].Level.ToString());

                        t.Rows[i].Cells[5].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[5].Paragraphs.First().Append(exportWorkPlan[i].Release);

                        t.Rows[i].Cells[6].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[6].Paragraphs.First().Append(exportWorkPlan[i].ResponsibleGroups);

                        t.Rows[i].Cells[7].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[7].Paragraphs.First().Append(exportWorkPlan[i].StartDate);

                        t.Rows[i].Cells[8].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[8].Paragraphs.First().Append(exportWorkPlan[i].EndDate);

                        t.Rows[i].Cells[9].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[9].Paragraphs.First().Append(exportWorkPlan[i].Completion.Value.ToString());

                        t.Rows[i].Cells[10].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[10].Paragraphs.First().Append(exportWorkPlan[i].HyperLink);

                        t.Rows[i].Cells[11].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[11].Paragraphs.First().Append(exportWorkPlan[i].StatusReport);

                        /*t.Rows[i].Cells[12].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[12].Paragraphs.First().Append(exportWorkPlan[i].WIRaporteur);

                        t.Rows[i].Cells[13].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[13].Paragraphs.First().Append(exportWorkPlan[i].WIRaporteurEmail);

                        t.Rows[i].Cells[14].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[14].Paragraphs.First().Append(exportWorkPlan[i].Notes);

                        t.Rows[i].Cells[15].FillColor = exportWorkPlan[i].GetCellBgColor();
                        t.Rows[i].Cells[15].Paragraphs.First().Append(exportWorkPlan[i].RelatedTSs_TRs);*/

                    }
                    // Insert the Table into the document.
                    document.InsertTable(t);
                    document.Save();
                }
            }
        }


    }

}
