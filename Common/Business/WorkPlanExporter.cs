using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
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
        #region Properties

        private IUltimateUnitOfWork _uoW;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="UoW">Unit Of Work</param>
        public WorkPlanExporter(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Export Work Plan
        /// </summary>
        /// <param name="exportPath">Export Path</param>
        public void ExportWorkPlan(string exportPath)
        {
            var workItemManager = new WorkItemManager(_uoW);
            var workItems = workItemManager.GetAllWorkItems(0);
            exportToExcel(workItems.Key, exportPath);
            exportToWord(workItems.Key, exportPath);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export Work Plan to Excel
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void exportToExcel(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!String.IsNullOrEmpty(exportPath) && workPlan.Count >= 1)
            {
                try
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
                        int columnEnd = 16;
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
                        ruleDeleted.Formula = "SEARCH(CONCATENATE(\"[\",$A2,\"]\"), \"[" + String.Join("]\"&\"[", stoppedMeetingIds) + "]\")>0";
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
                                                          Impacted_TSs_and_TRs = s.RelatedTSs_TRs                                                 
                                                  } ,
                                                      true, OfficeOpenXml.Table.TableStyles.None);
                        pck.Save();
                    }
                }
                catch (IOException ex)
                {
                    LogManager.Error(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Export Work Plan to Word
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void exportToWord(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!string.IsNullOrEmpty(exportPath))
            {

                string file = exportPath + "WorkItemExport.docx";
                if (File.Exists(file)) File.Delete(file);
                List<Domain.WorkItemForExport> exportWorkPlan = new List<Domain.WorkItemForExport>();
                exportWorkPlan = Domain.WorkItemForExport.GetWorkItemsListForExport(workPlan);
                
                using (DocX document = DocX.Create(file))
                {
                     double FONT_SIZE_HEADER = 16D;

                    document.MarginLeft = 100F;
                    document.MarginRight = 0F;                    

                    var headLineFormat = new Formatting();
                    headLineFormat.FontFamily = new FontFamily("Arial");
                    headLineFormat.Size = FONT_SIZE_HEADER;
                    headLineFormat.Bold = true;

                    document.InsertParagraph("Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd"), false, headLineFormat);
                    document.InsertParagraph("");

                    Table legendTable = document.AddTable(5, 1);                    

                    setCellContent(legendTable.Rows[0].Cells[0], "LEGEND", Domain.DocxStylePool.STYLES_KEY.BLUE_WHITE);
                    setCellContent(legendTable.Rows[1].Cells[0], "ONGOING", Domain.DocxStylePool.STYLES_KEY.BLACK_WHITE);
                    setCellContent(legendTable.Rows[2].Cells[0], "COMPLETED", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GREEN);
                    setCellContent(legendTable.Rows[3].Cells[0], "STOPPED", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                    setCellContent(legendTable.Rows[4].Cells[0], "-", Domain.DocxStylePool.STYLES_KEY.BLACK_WHITE);
                    

                    document.InsertTable(legendTable);

                    document.InsertParagraph("");

                    Table t = document.AddTable(exportWorkPlan.Count, 16);                     
                                       

                    for (int i = 0; i < exportWorkPlan.Count; i++)
                    {
                        
                        setCellContent(t.Rows[i].Cells[0],exportWorkPlan[i].Wpid.ToString().Trim(), "Wpid", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[1], exportWorkPlan[i].UID.ToString(), "UID", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[2], exportWorkPlan[i].Name, "Name", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[3], exportWorkPlan[i].Acronym, "Acronym", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[4], exportWorkPlan[i].Level.ToString(), "Level", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[5], exportWorkPlan[i].Release, "Release", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[6], exportWorkPlan[i].ResponsibleGroups, "ResponsibleGroups", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[7], exportWorkPlan[i].StartDate, "StartDate", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[8], exportWorkPlan[i].EndDate, "EndDate", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[9], (exportWorkPlan[i].Completion.Value * 100).ToString().Trim() + "%", "Completion", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[10], exportWorkPlan[i].HyperLink, "HyperLink", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[11], exportWorkPlan[i].StatusReport, "StatusReport", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[12], exportWorkPlan[i].WIRaporteur, "WIRaporteur", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[13], exportWorkPlan[i].WIRaporteurEmail, "WIRaporteurEmail", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[14], exportWorkPlan[i].Notes, "Notes", exportWorkPlan[i]);
                        setCellContent(t.Rows[i].Cells[15], exportWorkPlan[i].RelatedTSs_TRs, "RelatedTSs_TRs", exportWorkPlan[i]);                        
                    }

                    t.AutoFit = AutoFit.Window;
                    t.Alignment = Alignment.left;    
                    // Insert the Table into the document.
                    document.InsertTable(t);
                    document.Save();
                }
            }
        }

        /// <summary>
        /// Set Cell Content for Word Table
        /// </summary>
        /// <param name="currentCell">Current Cell</param>
        /// <param name="cellContent">Cell Content</param>
        /// <param name="colName">Column Name</param>
        /// <param name="row">Row</param>
        private void setCellContent(Cell currentCell,string cellContent, string colName, Domain.WorkItemForExport row)
        {
            FontFamily FONT_ARIAL = new FontFamily("Arial");
            double FONT_SIZE_PARAGRAPH = 8D;
            currentCell.MarginLeft = 0D;
            currentCell.MarginRight = 0D;
            currentCell.MarginTop = 0D;            
            currentCell.Paragraphs.First().Append(cellContent);            
            currentCell.Paragraphs.First().Color(Domain.DocxStylePool.GetDocxStyle(row.GetCellStyle(colName)).FontColor);
            if (cellContent != null && !cellContent.Equals(string.Empty))
            {
                currentCell.Paragraphs.First().Font(FONT_ARIAL);
                currentCell.Paragraphs.First().FontSize(FONT_SIZE_PARAGRAPH);
                if (!colName.Equals("Notes") || !colName.Equals("RelatedTSs_TRs"))
                    currentCell.Width = (double)((cellContent.Length) * FONT_SIZE_PARAGRAPH);
                else
                {
                    currentCell.Width = (double)((50) * FONT_SIZE_PARAGRAPH);
                }
            }
            if (Domain.DocxStylePool.GetDocxStyle(row.GetCellStyle(colName)).IsBold) currentCell.Paragraphs.First().Bold();
            currentCell.FillColor = Domain.DocxStylePool.GetDocxStyle(row.GetCellStyle(colName)).BgColor;
        }

        /// <summary>
        /// Set Cell Content for Word Table
        /// </summary>
        /// <param name="currentCell">Current Cell</param>
        /// <param name="cellContent">Cell Content</param>
        /// <param name="style">Style</param>
        private void setCellContent(Cell currentCell, string cellContent, Domain.DocxStylePool.STYLES_KEY style)
        {
            FontFamily FONT_ARIAL = new FontFamily("Arial");
            double FONT_SIZE_PARAGRAPH = 8D;
            /*currentCell.MarginLeft = 0D;
            currentCell.MarginRight = 0D;
            currentCell.MarginTop = 0D;*/
            currentCell.Width = (double)(cellContent.Length*FONT_SIZE_PARAGRAPH);
            currentCell.Paragraphs.First().Append(cellContent);
            currentCell.Paragraphs.First().Color(Domain.DocxStylePool.GetDocxStyle((int)style).FontColor);            
            if (cellContent != null && !cellContent.Equals(string.Empty))
            {
                currentCell.Paragraphs.First().Font(FONT_ARIAL);
                currentCell.Paragraphs.First().FontSize(FONT_SIZE_PARAGRAPH);
            }
            if (Domain.DocxStylePool.GetDocxStyle((int)style).IsBold) currentCell.Paragraphs.First().Bold();
            currentCell.FillColor = Domain.DocxStylePool.GetDocxStyle((int)style).BgColor;
        }

        #endregion
    }
}
