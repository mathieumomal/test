﻿using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
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
using System.Text;

namespace Etsi.Ultimate.Business
{
    public class WorkPlanExporter : IWorkPlanExporter
    {
        public IUltimateUnitOfWork UoW { get; set; }

        #region Properties

        private string DOC_TITLE = "Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd");
        private string ZIP_NAME = "Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd"); 

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
                LogManager.Info("WORKPLAN EXPORT: System is generating WorkPlan... Final path:" + exportPath);
                var workItemManager = new WorkItemManager(UoW);
                LogManager.Info("WORKPLAN EXPORT:   System is gathering all workitems...");
                var workItems = workItemManager.GetAllWorkItems(0);
                List<WorkItemForExport> workItemExportObjects = new List<WorkItemForExport>();
                workItemExportObjects.AddRange(workItems.Key.Where(x => !x.DeletedFlag).OrderBy(x => x.WorkplanId).ToList().Select(y => new WorkItemForExport(y)));

                ExportToExcel(workItemExportObjects, exportPath);
                ExportToWord(workItemExportObjects, exportPath);
                if (!String.IsNullOrEmpty(exportPath) && workItems.Key.Count >= 1)
                {
                    List<string> filesToCompress = new List<string> { exportPath + DOC_TITLE + ".xlsx", exportPath + DOC_TITLE + ".docx" };
                    Zip.CompressSetOfFiles(ZIP_NAME, filesToCompress, exportPath);
                    LogManager.Info("WORKPLAN EXPORT:   WorkPlan compressed.");
                }
                LogManager.Info("WORKPLAN EXPORT: WorkPlan successfully generated. END.");
                return true;
            }
            catch (IOException ex)
            {
                ExtensionLogger.Exception(ex, new List<object> { exportPath }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "IOException");
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export Work Plan to Excel. (This excel is fully created thanks to the above code and is not based on any template !!!)
        /// </summary>
        /// <param name="exportWorkPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToExcel(List<WorkItemForExport> exportWorkPlan, string exportPath)
        {
            LogManager.Info("WORKPLAN EXPORT:   Generation of WorkPlan Excel...");
            if (!string.IsNullOrEmpty(exportPath) && exportWorkPlan.Count >= 1)
            {
                try
                {
                    //Create Empty Work Book
                    var file = exportPath + DOC_TITLE+".xlsx";
                    if (File.Exists(file)) File.Delete(file);
                    var newFile = new FileInfo(file);

                    using (ExcelPackage pck = new ExcelPackage(newFile))
                    {
                        // get the handle to the existing worksheet
                        var wsData = pck.Workbook.Worksheets.Add("Work Items");

                        /*-----------*/
                        /* Variables */
                        /*----------*/
                        var rowHeader = 1;
                        var rowDataStart = 2;
                        var rowDataEnd = exportWorkPlan.Count + 1;
                        var columnStart = 1;
                        var columnEnd = 17;

                        /*----------------*/
                        /* Column letters and numbers */
                        /*----------------*/
                        var idColumnPos = new KeyValuePair<int, string>(1, "A");
                        var uniqueIdColumnPos = new KeyValuePair<int, string>(2, "B");
                        var nameColumnPos = new KeyValuePair<int, string>(3, "C");
                        var acronymColumnPos = new KeyValuePair<int, string>(4, "D");
                        var effectiveAcronymColumnPos = new KeyValuePair<int, string>(5, "E");
                        var outlineLevelColumnPos = new KeyValuePair<int, string>(6, "F");
                        var releaseColumnPos = new KeyValuePair<int, string>(7, "G");
                        var rsrcNamesColumnPos = new KeyValuePair<int, string>(8, "H");
                        var startDateColumnPos = new KeyValuePair<int, string>(9, "I");
                        var finishDateColumnPos = new KeyValuePair<int, string>(10, "J");
                        var completionColumnPos = new KeyValuePair<int, string>(11, "K");
                        var hyperlinkColumnPos = new KeyValuePair<int, string>(12, "L");
                        var statusColumnPos = new KeyValuePair<int, string>(13, "M");
                        var wiRapporteurNameColumnPos = new KeyValuePair<int, string>(14, "N");
                        var wiRapporteurEmailColumnPos = new KeyValuePair<int, string>(15, "O");
                        var notesColumnPos = new KeyValuePair<int, string>(16, "P");
                        var impactedColumnPos = new KeyValuePair<int, string>(17, "Q");

                        /*------------*/
                        /* Set Styles */
                        /*------------*/
                        //Set Font Style
                        wsData.Cells.Style.Font.Size = 8;
                        wsData.Cells.Style.Font.Name = "Arial";
                        //Set Header Style
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Font.Bold = true;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(Drawing.Color.LightGray);
                        //Set Name Column as Bold
                        wsData.Cells[rowDataStart, nameColumnPos.Key, rowDataEnd, nameColumnPos.Key].Style.Font.Bold = true;
                        //Set Complete column with Percentage Format
                        wsData.Cells[rowDataStart, completionColumnPos.Key, rowDataEnd, completionColumnPos.Key].Style.Numberformat.Format = "0%";
                        //Set Cell Borders
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        //Set Filters
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].AutoFilter = true;

                        //Set Column Width
                        wsData.DefaultColWidth = 10;
                        wsData.Column(effectiveAcronymColumnPos.Key).Width = 15;
                        wsData.Column(idColumnPos.Key).Width = wsData.Column(outlineLevelColumnPos.Key).Width = wsData.Column(completionColumnPos.Key).Width = 5;
                        wsData.Column(nameColumnPos.Key).Width = wsData.Column(impactedColumnPos.Key).Width = 35;

                        //Set Row Height
                        wsData.DefaultRowHeight = 12;
                        //Set Zoom to 85%
                        wsData.View.ZoomScale = 85;

                        /*--------------*/
                        /* Add Formulas */
                        /*--------------*/
                        ExcelAddress nameAddress = new ExcelAddress(rowDataStart, nameColumnPos.Key, rowDataEnd, nameColumnPos.Key);
                        //Name should be Red if Unique ID is 0
                        var ruleNoUniqueKey = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleNoUniqueKey.Style.Font.Color.Color = Drawing.Color.Red;
                        ruleNoUniqueKey.Formula = uniqueIdColumnPos.Value + "2=0";
                        ruleNoUniqueKey.Priority = 1;

                        //Level 1 Name should be in Blue font
                        var ruleLevel1 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel1.Style.Font.Color.Color = Drawing.Color.Blue;
                        ruleLevel1.Formula = outlineLevelColumnPos.Value + "2=1";
                        ruleLevel1.Priority = 2;

                        //Level 2 Name should be in Black font
                        var ruleLevel2 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel2.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel2.Formula = outlineLevelColumnPos.Value + "2=2";
                        ruleLevel2.Priority = 3;

                        //Level 3 Name should be in Black font without Bold
                        var ruleLevel3 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel3.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel3.Style.Font.Bold = false;
                        ruleLevel3.Formula = outlineLevelColumnPos.Value + "2=3";
                        ruleLevel3.Priority = 4;

                        //Level 4 Name should be in Black font without Bold
                        var ruleLevel4 = wsData.ConditionalFormatting.AddExpression(nameAddress);
                        ruleLevel4.Style.Font.Color.Color = Drawing.Color.Black;
                        ruleLevel4.Style.Font.Bold = false;
                        ruleLevel4.Formula = outlineLevelColumnPos.Value + "2=4";
                        ruleLevel4.Priority = 5;

                        ExcelAddress completeTableAddress = new ExcelAddress(rowDataStart, columnStart, rowDataEnd, columnEnd);
                        var stoppedMeetingIds = exportWorkPlan.Where(x => x.StoppedMeeting == true && x.Wpid != null).Select(y => y.Wpid).ToList();

                        //Stopped WorkItems should have light brown background
                        var ruleDeleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                        ruleDeleted.Style.Fill.BackgroundColor.Color = Drawing.Color.FromArgb(227, 227, 227);
                        ruleDeleted.Formula = "SEARCH(CONCATENATE(\"[\",$" + idColumnPos.Value + "2,\"]\"), \"[" + String.Join("]\"&\"[", stoppedMeetingIds) + "]\")>0";
                        ruleDeleted.Priority = 6;

                        //100% completed workitems should have light green background
                        var ruleCompleted = wsData.ConditionalFormatting.AddExpression(completeTableAddress);
                        ruleCompleted.Style.Fill.BackgroundColor.Color = Drawing.Color.FromArgb(204, 255, 204);
                        ruleCompleted.Formula = "$" + completionColumnPos.Value + "2=100%";
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
                                                          Effective_Acronym = s.EffectiveAcronym,
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
                    LogManager.Error("WORKPLAN EXPORT:   ", ex);
                    throw new IOException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Export Work Plan to Word
        /// </summary>
        /// <param name="exportWorkPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToWord(List<WorkItemForExport> exportWorkPlan, string exportPath)
        {
            LogManager.Info("WORKPLAN EXPORT:   Generation of WorkPlan Word (docx)...");
            if (!string.IsNullOrEmpty(exportPath))
            {
                try
                {
                    // Get file's full path and delete it if already exists
                    string file = exportPath + DOC_TITLE + ".docx";
                    if (File.Exists(file)) File.Delete(file);
                    //Get number of rows that should be appended to the WIs table
                    int rowsNumber = exportWorkPlan.Count;
                    //Generation start
                    using (WordprocessingDocument theDoc = WordprocessingDocument.Create(file, WordprocessingDocumentType.Document))
                    {        
                        //Build of structure of the document and get the main part of the document (That would be populated with data)
                        MainDocumentPart mainPart = theDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = new Body();
                        mainPart.Document.Append(body);
                        var doc = theDoc.MainDocumentPart.Document;

                        //Title as a pragraph
                        string doc_tile = DOC_TITLE;
                        Domain.DocxStyle docx = Domain.DocxStylePool.GetDocxStyle((int)Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_WHITE);
                        Paragraph titleParagraph = SetParagraphContent(docx, doc_tile, "Arial", "32");
                        doc.Body.Append(titleParagraph);
                        doc.Body.Append(new Paragraph());

                        //Initialize legends table then add it to document body
                        Table legTable = new Table();
                        //Set table style
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

                        //Initialize content table then add it to document body
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
                            SetCellContent(tr, new TableCell(), " Effective Acronym ", Domain.DocxStylePool.STYLES_KEY.BOLD_BLACK_GRAY);
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

                        //Table body
                        for (var i = 0; i < rowsNumber; i++)
                        {
                            var tr = new TableRow();
                            table.Append(tr);

                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Wpid.ToString().Trim(), "Wpid", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].UID.ToString(), "UID", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Name, "Name", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Acronym, "Acronym", exportWorkPlan[i]);
                            SetCellContent(tr, new TableCell(), exportWorkPlan[i].Acronym, "Effective Acronym", exportWorkPlan[i]);
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
                    LogManager.Error("WORKPLAN EXPORT:   ", ex);
                    throw new IOException(ex.Message);
                }

            }
        }

        /// <summary>
        /// Set the content of a cell in content table
        /// </summary>
        /// <param name="tableRow">Table row </param>
        /// <param name="currentCell">Row's cell (Property of WorkItemForExport)</param>
        /// <param name="cellContent">Value of a property of WorkItemForExport</param>
        /// <param name="colName">Name of a property of WorkItemForExport</param>
        /// <param name="row">WorkItemForExport object</param>
        private void SetCellContent(TableRow tableRow, TableCell currentCell, string cellContent, string colName, WorkItemForExport row)
        {
            //Get cell style relying on colName
            Domain.DocxStyle docx = Domain.DocxStylePool.GetDocxStyle(row.GetCellStyle(colName));
            //Set the cell's content and content's style
            Paragraph cellParagraph = SetParagraphContent(docx, cellContent, "Arial", "16");
            currentCell.Append(cellParagraph);
            //Set the cell's style
            TableCellProperties tcp = new TableCellProperties();
            GridSpan griedSpan = new GridSpan();
            griedSpan.Val = 4;
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = docx.GetFontColorHex(), Fill = docx.GetBgColorHex() });
            tcp.Append(griedSpan);
            currentCell.Append(tcp);
            //Add cell to the row
            tableRow.Append(currentCell);

        }

        /// <summary>
        /// Set the content of a cell in legends table
        /// </summary>
        /// <param name="tableRow">Table row </param>
        /// <param name="currentCell">Row's cell</param>
        /// <param name="cellContent">Cell's content</param>
        /// <param name="colName">Content style</param>
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

        /// <summary>
        /// Set a paragraph content and style
        /// </summary>
        /// <param name="docx">Style</param>
        /// <param name="text">Content</param>
        /// <param name="fontStyle">Content font</param>
        /// <param name="fontSizeValue">Content font size</param>
        /// <returns>A paragraph object</returns>
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

        /// <summary>
        /// Used to set the style of a paragraph
        /// </summary>
        /// <param name="docx">Style</param>
        /// <param name="fontStyle">Content font</param>
        /// <param name="fontSizeValue">Content font size</param>
        /// <returns>RunProperties object that contains all paragraph styling</returns>
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

        /// <summary>
        /// Used to set general style of a table
        /// </summary>
        /// <param name="table">Edited table</param>
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

    /// <summary>
    /// Format WorkItem object which is suitable for export
    /// </summary>
    internal class WorkItemForExport
    {
        #region Properties

        private const string BLANK_CELL = "  -  ";
        public Nullable<int> Wpid { get; set; }
        public int UID { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string EffectiveAcronym { get; set; }
        public int Level { get; set; }
        public string Release { get; set; }
        public string ResponsibleGroups { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Nullable<int> Completion { get; set; }
        public string HyperLink { get; set; }
        public string StatusReport { get; set; }
        public string WIRaporteur { get; set; }
        public string WIRaporteurEmail { get; set; }
        public string Notes { get; set; }
        public string RelatedTSs_TRs { get; set; }
        public bool StoppedMeeting { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to format & convert WorkItem properties
        ///   - Empty Records      :: '  -  '
        ///   - UID >= 100000000   :: 0
        ///   - Name               :: Prefix with empty string based on level
        ///   - Responsible Groups :: Comma separated
        ///   - Date               :: yyyy-MM-dd
        /// </summary>
        /// <param name="workItem">Work Item</param>
        public WorkItemForExport(Domain.WorkItem workItem)
        {
            Wpid = workItem.WorkplanId;
            UID = (workItem.Pk_WorkItemUid >= Math.Pow(10, 8)) ? 0 : workItem.Pk_WorkItemUid;
            Name = GetEmptyString(workItem.WiLevel ?? 0) + workItem.Name;
            Acronym = string.IsNullOrEmpty(workItem.Acronym) ? BLANK_CELL : workItem.Acronym;
            EffectiveAcronym = string.IsNullOrEmpty(workItem.Effective_Acronym) ? BLANK_CELL : workItem.Effective_Acronym;
            Level = workItem.WiLevel ?? 0;
            Release = (workItem.Release != null) && !string.IsNullOrEmpty(workItem.Release.Code) ? workItem.Release.Code : BLANK_CELL;
            ResponsibleGroups = (workItem.WorkItems_ResponsibleGroups.Count > 0) ? string.Join(",", workItem.WorkItems_ResponsibleGroups.Select(r => r.ResponsibleGroup).ToArray()).Trim() : BLANK_CELL;
            StartDate = (workItem.StartDate != null) ? workItem.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd") : BLANK_CELL;
            EndDate = (workItem.EndDate != null) ? workItem.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd") : BLANK_CELL;
            Completion = ((workItem.Completion != null) ? workItem.Completion : 0) / 100;
            HyperLink = !string.IsNullOrEmpty(workItem.Wid) ? workItem.Wid : BLANK_CELL;
            StatusReport = !string.IsNullOrEmpty(workItem.StatusReport) ? workItem.StatusReport : BLANK_CELL;
            WIRaporteur = !string.IsNullOrEmpty(workItem.RapporteurCompany) ? workItem.RapporteurCompany : BLANK_CELL;
            WIRaporteurEmail = !string.IsNullOrEmpty(workItem.RapporteurStr) ? workItem.RapporteurStr : BLANK_CELL;
            Notes = ((workItem.Remarks != null) && (workItem.Remarks.Count > 0)) ? string.Join(" ", workItem.Remarks.Select(r => r.RemarkText).ToArray()).Trim() : BLANK_CELL;
            RelatedTSs_TRs = string.IsNullOrEmpty(workItem.TssAndTrs) ? workItem.TssAndTrs : BLANK_CELL;
            StoppedMeeting = (workItem.TsgStoppedMtgId != null || !String.IsNullOrEmpty(workItem.TsgStoppedMtgRef)
                                                               || workItem.PcgStoppedMtgId != null
                                                               || !String.IsNullOrEmpty(workItem.PcgStoppedMtgRef));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provide the index to format cell style for word export
        /// Index Range: 1 to 12
        ///     1: STYLES_KEY.BLACK_WHITE
        ///     2: STYLES_KEY.BLACK_GREEN
        ///     3: STYLES_KEY.BLACK_GRAY
        ///     4: STYLES_KEY.BOLD_BLACK_WHITE
        ///     5: STYLES_KEY.BOLD_BLACK_GREEN
        ///     6: STYLES_KEY.BOLD_BLACK_GRAY
        ///     7: STYLES_KEY.BLUE_WHITE
        ///     8: STYLES_KEY.BLUE_GREEN
        ///     9: STYLES_KEY.BLUE_GRAY
        ///    10: STYLES_KEY.RED_WHITE
        ///    11: STYLES_KEY.RED_GREEN
        ///    12: STYLES_KEY.RED_GRAY
        /// </summary>
        /// <param name="colName">Column Name</param>
        /// <returns>Index</returns>
        public int GetCellStyle(string colName)
        {
            int index = 0;

            if (colName.Equals("Name"))
            {
                switch (Level)
                {
                    case 0:
                        index += 9;
                        break;
                    case 1:
                        index += 6;
                        break;
                    case 2:
                        index += 3;
                        break;
                    default:
                        break;
                }
            }

            if (StoppedMeeting)
            {
                index += 3;
            }
            else
            {
                if (Completion.Value == 1)
                    index += 2;
                else
                    index += 1;
            }
            return index;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Provide empty string to format (based on level)
        /// Level1
        ///    Level2
        ///       Level3
        ///          Level4
        /// </summary>
        /// <param name="level">Level</param>
        /// <returns>Empty string</returns>
        private string GetEmptyString(int level)
        {
            StringBuilder space = new StringBuilder();
            if (level > 1)
            {
                for (int i = 1; i <= (level * 3) - 3; i++)
                    space.Append(" ");
            }
            return space.ToString();
        }

        #endregion
    }

    public interface IWorkPlanExporter
    {
        IUltimateUnitOfWork UoW { get; set; }

        bool ExportWorkPlan(string exportPath);
    }
}
