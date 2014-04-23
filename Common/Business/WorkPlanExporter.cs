using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.IO;
using System.Linq;
using Domain = Etsi.Ultimate.DomainClasses;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
{    
    public class WorkPlanExporter
    {
        #region Properties

        private IUltimateUnitOfWork _uoW;
        private string DOC_TITLE = "Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd");
        private string ZIP_NAME = "Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd"); 

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
        public bool ExportWorkPlan(string exportPath)
        {
            try
            {
                var workItemManager = new WorkItemManager(_uoW);
                var workItems = workItemManager.GetAllWorkItems(0);
                ExportToExcel(workItems.Key, exportPath);
                ExportToWord(workItems.Key, exportPath);
                if (!String.IsNullOrEmpty(exportPath) && workItems.Key.Count >= 1)
                {
                    List<string> filesToCompress = new List<string>() { exportPath + DOC_TITLE + ".xlsx", exportPath + DOC_TITLE + ".docx" };
                    Zip.CompressSetOfFiles(ZIP_NAME, filesToCompress, exportPath);
                }
                return true;
            }
            catch (IOException ex)
            {
                LogManager.Error("Export of the work plan failed", ex);
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export Work Plan to Excel
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToExcel(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!String.IsNullOrEmpty(exportPath) && workPlan.Count >= 1)
            {
                try
                {
                    //Create Empty Work Book
                    string file = exportPath + DOC_TITLE+".xlsx";
                    if (File.Exists(file)) File.Delete(file);
                    FileInfo newFile = new FileInfo(file);

                    using (ExcelPackage pck = new ExcelPackage(newFile))
                    {
                        List<Domain.WorkItemForExport> exportWorkPlan = WorkItemForExport.GetWorkItemsListForExport(workPlan);

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
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(Drawing.Color.LightGray);
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
                        ruleNoUniqueKey.Style.Font.Color.Color = Drawing.Color.Red;
                        ruleNoUniqueKey.Formula = "B2=0";
                        ruleNoUniqueKey.Priority = 1;

                        //Level 1 Name should be in Blue font
                        var ruleLevel1 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel1.Style.Font.Color.Color = Drawing.Color.Blue;
                        ruleLevel1.Formula = "E2=1";
                        ruleLevel1.Priority = 2;

                        //Level 2 Name should be in Black font
                        var ruleLevel2 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel2.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel2.Formula = "E2=2";
                        ruleLevel2.Priority = 3;

                        //Level 3 Name should be in Black font without Bold
                        var ruleLevel3 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel3.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel3.Style.Font.Bold = false;
                        ruleLevel3.Formula = "E2=3";
                        ruleLevel3.Priority = 4;

                        //Level 4 Name should be in Black font without Bold
                        var ruleLevel4 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel4.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel4.Style.Font.Bold = false;
                        ruleLevel4.Formula = "E2=4";
                        ruleLevel4.Priority = 5;

                        ExcelAddress completeTableAddress = new ExcelAddress(rowDataStart, columnStart, rowDataEnd, columnEnd);
                        var stoppedMeetingIds = exportWorkPlan.Where(x => x.StoppedMeeting == true && x.Wpid != null).Select(y => y.Wpid).ToList();

                        //Stopped WorkItems should have light brown background
                        var ruleDeleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                        ruleDeleted.Style.Fill.BackgroundColor.Color = Drawing.Color.FromArgb(227, 227, 227);
                        ruleDeleted.Formula = "SEARCH(CONCATENATE(\"[\",$A2,\"]\"), \"[" + String.Join("]\"&\"[", stoppedMeetingIds) + "]\")>0";
                        ruleDeleted.Priority = 6;

                        //100% completed workitems should have light green background
                        var ruleCompleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                        ruleCompleted.Style.Fill.BackgroundColor.Color = Drawing.Color.FromArgb(204, 255, 204);
                        ruleCompleted.Formula = "$J2=100%";
                        ruleCompleted.Priority = 7;

                        //Upload Data to Excel
                        var dataRange = wsData.Cells["A1"].LoadFromCollection(
                                                      from s in exportWorkPlan
                                                      orderby s.Wpid
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
                    throw new IOException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Export Work Plan to Word
        /// </summary>
        /// <param name="workPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToWord(List<Domain.WorkItem> workPlan, string exportPath)
        {
            if (!string.IsNullOrEmpty(exportPath))
            {

                try
                {
                    string file = exportPath + DOC_TITLE + ".docx";
                    if (File.Exists(file)) File.Delete(file);
                    List<Domain.WorkItemForExport> exportWorkPlan = new List<Domain.WorkItemForExport>();
                    exportWorkPlan = Domain.WorkItemForExport.GetWorkItemsListForExport(workPlan);
                    int rowsNumber = exportWorkPlan.Count;
                    using (WordprocessingDocument theDoc = WordprocessingDocument.Create(file, WordprocessingDocumentType.Document))
                    {                        
                        MainDocumentPart mainPart = theDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = new Body();
                        mainPart.Document.Append(body);
                        var doc = theDoc.MainDocumentPart.Document;

                        //Title 
                        string doc_tile = DOC_TITLE;
                        Domain.DocxStyle docx = Domain.DocxStylePool.GetDocxStyle((int)Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_WHITE);
                        Paragraph titleParagraph = SetParagraphContent(docx, doc_tile, "Arial", "32");
                        doc.Body.Append(titleParagraph);
                        doc.Body.Append(new Paragraph());

                        Table legTable = new Table();
                        SetTableStyle(legTable);

                        for (var i = 0; i < 5; i++)
                        {
                            var tr = new TableRow();
                            legTable.Append(tr);
                            switch (i)
                            {
                                case 0: SetCellContent(tr, new TableCell(), " LEGEND    ", Domain.DocxStylePool.STYLES_KEY.BLUE_WHITE); break;
                                case 1: SetCellContent(tr, new TableCell(), " ONGOING    ", Domain.DocxStylePool.STYLES_KEY.BLACK_WHITE); break;
                                case 2: SetCellContent(tr, new TableCell(), " COMPLETED    ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GREEN); break;
                                case 3: SetCellContent(tr, new TableCell(), " STOPPED    ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY); break;
                                case 4: SetCellContent(tr, new TableCell(), " -    ", Domain.DocxStylePool.STYLES_KEY.BLACK_WHITE); break;
                                default: break;
                            }
                        }
                        doc.Body.Append(legTable);
                        doc.Body.Append(new Paragraph());

                        Table table = new Table();                        
                        SetTableStyle(table);

                        // tableHeader
                        {
                            var tr = new TableRow();
                            table.Append(tr);

                            SetCellContent(tr, new TableCell(), " ID ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Unique ID ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Name ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Acronym ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Level ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Release ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Resource Names ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Start Date ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Finish Date ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Percent Complete ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Hyperlink ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Status Report ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " WI rapporteur name ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " WI rapporteur e-mail ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Notes ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
                            SetCellContent(tr, new TableCell(), " Impacted TSs and TRs ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);                                                      
                        }

                        for (var i = 0; i < rowsNumber; i++)
                        {
                            var tr = new TableRow();
                            table.Append(tr);

                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Wpid.ToString().Trim(), "Wpid", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].UID.ToString(), "UID", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Name, "Name", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Acronym, "Acronym", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Level.ToString(), "Level", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Release, "Release", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].ResponsibleGroups, "ResponsibleGroups", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].StartDate, "StartDate", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].EndDate, "EndDate", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), (exportWorkPlan[i].Completion.Value * 100).ToString().Trim() + "%", "Completion", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].HyperLink, "HyperLink", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].StatusReport, "StatusReport", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].WIRaporteur, "WIRaporteur", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].WIRaporteurEmail, "WIRaporteurEmail", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Notes, "Notes", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].RelatedTSs_TRs, "RelatedTSs_TRs", exportWorkPlan[i]);
                        }

                        doc.Body.Append(table);
                        doc.Save();
                    }
                }
                catch (IOException ex)
                {
                    LogManager.Error(ex.Message, ex);
                    throw new IOException(ex.Message);
                }

            }
        }

        private void SetCellContent(TableRow tableRow, TableCell currentCell, string cellContent, string colName, Domain.WorkItemForExport row)
        {
            Domain.DocxStyle docx = Domain.DocxStylePool.GetDocxStyle(row.GetCellStyle(colName));
            Paragraph cellParagraph = SetParagraphContent(docx, cellContent, "Arial", "16");
            currentCell.Append(cellParagraph);
            TableCellProperties tcp = new TableCellProperties();
            GridSpan griedSpan = new GridSpan();
            griedSpan.Val = 4;
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = docx.GetFontColorHex(), Fill = docx.GetBgColorHex() });
            tcp.Append(griedSpan);
            currentCell.Append(tcp);
            tableRow.Append(currentCell);

        }

        private void SetCellContent(TableRow tableRow, TableCell currentCell, string cellContent, Domain.DocxStylePool.STYLES_KEY style)
        {
            Domain.DocxStyle docx = Domain.DocxStylePool.GetDocxStyle((int)style);
            Paragraph cellParagraph = SetParagraphContent(docx, cellContent, "Arial", "16");
            currentCell.Append(cellParagraph);            
            TableCellProperties tcp = new TableCellProperties();
            GridSpan griedSpan = new GridSpan();
            griedSpan.Val = 4;
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = docx.GetFontColorHex(), Fill = docx.GetBgColorHex() });
            tcp.Append(griedSpan);            
            currentCell.Append(tcp);
            tableRow.Append(currentCell);
        }

        private Paragraph SetParagraphContent(Domain.DocxStyle docx, string text, string fontStyle, string fontSizeValue)
        {
            Paragraph contentParagraph = new Paragraph();
            Run contentParagraph_run = new Run();
            Text contentParagraph_text = new Text(text) { Space = SpaceProcessingModeValues.Preserve };
            ParagraphProperties contentParagraph__pPr = new ParagraphProperties(new SpacingBetweenLines() { After = "0" });            
            RunProperties contentParagraph__runPro = SetParagraphStyle(docx, fontStyle, fontSizeValue);
            contentParagraph_run.Append(contentParagraph__runPro);
            contentParagraph.Append(contentParagraph__pPr);
            contentParagraph_run.Append(contentParagraph_text);
            contentParagraph.Append(contentParagraph_run);
            return contentParagraph;
        }

        private RunProperties SetParagraphStyle(Domain.DocxStyle docx, string fontStyle, string fontSizeValue)
        {
            RunProperties cellParagraph_runPro = new RunProperties();
            RunFonts runFont = new RunFonts() { Ascii = fontStyle, ComplexScript = fontStyle };
            DocumentFormat.OpenXml.Wordprocessing.Color color = new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = docx.GetFontColorHex() };
            FontSize fontSize = new FontSize() { Val = fontSizeValue };
            cellParagraph_runPro.Append(runFont);
            if (docx.IsBold)
                cellParagraph_runPro.Append(new Bold());
            cellParagraph_runPro.Append(color);
            cellParagraph_runPro.Append(fontSize);
            return cellParagraph_runPro;
        }

        private void SetTableStyle(Table table)
        {
            TableProperties props = new TableProperties(
                      new TableBorders(
                        new TopBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 8
                        },
                        new BottomBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 8
                        },
                        new LeftBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),                            
                            Size = 8
                        },
                        new RightBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 8
                        },
                        new InsideHorizontalBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 8
                        },
                        new InsideVerticalBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 8
                        },
                        new TableLayout 
                        { 
                            Type = TableLayoutValues.Autofit 
                        }
                        ));
            table.AppendChild<TableProperties>(props);            
        }

        #endregion
    }
}
