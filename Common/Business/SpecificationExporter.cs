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
using System.Text;

namespace Etsi.Ultimate.Business
{
    public class SpecificationExporter
    {
        #region Properties

        private IUltimateUnitOfWork _uoW;
        private string DOC_TITLE = "Specification_3gpp_" + DateTime.Now.ToString("yyMMdd");
        private string ZIP_NAME = "Specification_3gpp_" + DateTime.Now.ToString("yyMMdd");

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="UoW">Unit Of Work</param>
        public SpecificationExporter(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Export Specification
        /// </summary>
        /// <param name="exportPath">Export Path</param>
        public bool ExportSpecification(string exportPath)
        {
            try
            {
                var specManager = new SpecificationManager();
                specManager.UoW = _uoW;
                var communityManager = new CommunityManager();
                communityManager.UoW = _uoW;
                var personManager = new PersonManager();
                personManager.UoW = _uoW;

                var specs = specManager.GetAllSpecifications(0);
                specs.ForEach(x => x.PrimeResponsibleGroupShortName = (x.PrimeResponsibleGroup == null) ? String.Empty : communityManager.GetCommmunityshortNameById(x.PrimeResponsibleGroup.Fk_commityId));
                specs.ForEach(x => x.PrimeSpecificationRapporteurName = (x.PrimeResponsibleGroup == null) ? String.Empty : personManager.GetPersonDisplayName(x.PrimeSpecificationRapporteurIds.FirstOrDefault()));

                List<SpecificationForExport> specExportObjects = new List<SpecificationForExport>();
                specExportObjects.AddRange(specs.OrderBy(x => x.Title).ToList().Select(y => new SpecificationForExport(y)));

                ExportToExcel(specExportObjects, exportPath);
                //ExportToWord(specExportObjects, exportPath);
                if (!String.IsNullOrEmpty(exportPath) && specs.Count >= 1)
                {
                    List<string> filesToCompress = new List<string>() { exportPath + DOC_TITLE + ".xlsx" };
                    Zip.CompressSetOfFiles(ZIP_NAME, filesToCompress, exportPath);
                }
                return true;
            }
            catch (IOException ex)
            {
                LogManager.Error("Export of the specification failed", ex);
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export Specification to Excel
        /// </summary>
        /// <param name="exportWorkPlan">Work Plan</param>
        /// <param name="exportPath">Export Path</param>
        private void ExportToExcel(List<SpecificationForExport> exportSpecification, string exportPath)
        {
            if (!String.IsNullOrEmpty(exportPath) && exportSpecification.Count >= 1)
            {
                try
                {
                    //Create Empty Work Book
                    string file = exportPath + DOC_TITLE + ".xlsx";
                    if (File.Exists(file)) File.Delete(file);
                    FileInfo newFile = new FileInfo(file);

                    using (ExcelPackage pck = new ExcelPackage(newFile))
                    {
                        // get the handle to the existing worksheet
                        var wsData = pck.Workbook.Worksheets.Add("Specifications");

                        /*------------*/
                        /* Set Styles */
                        /*------------*/
                        int rowHeader = 1;
                        int rowDataEnd = exportSpecification.Count + 1;
                        int columnStart = 1;
                        int columnEnd = 10;

                        //Set Font Style
                        wsData.Cells.Style.Font.Size = 8;
                        wsData.Cells.Style.Font.Name = "Arial";
                        //Set Header Style
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Font.Bold = true;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(Drawing.Color.LightGray);

                        //Set Cell Borders
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        //Set Filters
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].AutoFilter = true;

                        //Set Column Width
                        wsData.DefaultColWidth = 10;
                        wsData.Column(2).Width = wsData.Column(4).Width = wsData.Column(6).Width = 25;
                        wsData.Column(3).Width = 40;
                        wsData.Column(8).Width = wsData.Column(10).Width = 15;

                        //Set Row Height
                        wsData.DefaultRowHeight = 12;
                        //Set Zoom to 85%
                        wsData.View.ZoomScale = 85;

                        //Upload Data to Excel
                        var dataRange = wsData.Cells["A1"].LoadFromCollection(
                                                      from s in exportSpecification
                                                      orderby s.Title
                                                      select new
                                                      {
                                                          Spec_No = s.Number,
                                                          Type = s.Type,
                                                          Title = s.Title,
                                                          Status = s.Status,
                                                          Primary_Resp_Grp = s.PrimaryRespGrp,
                                                          Primary_rapporteur = s.PrimaryRapporteur,
                                                          Innitial_planned_Release = s.InnitialPlannedRelease,
                                                          Publication = s.IsForPublication,
                                                          Common_IMS = s.CommonIMS,
                                                          Technology = s.Technologies
                                                      },
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
    /// Format Specification object which is suitable for export
    /// </summary>
    internal class SpecificationForExport
    {
        #region Properties

        private const string BLANK_CELL = "  -  ";
        public string Number { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string PrimaryRespGrp { get; set; }
        public string PrimaryRapporteur { get; set; }
        public string InnitialPlannedRelease { get; set; }
        public string IsForPublication { get; set; }
        public string CommonIMS { get; set; }
        public string Technologies { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to format & convert Specification properties
        ///   - Empty Records      :: '  -  '
        ///   - Date               :: yyyy-MM-dd
        /// </summary>
        /// <param name="Specification">Specification</param>
        public SpecificationForExport(Domain.Specification spec)
        {
            Number = String.IsNullOrEmpty(spec.Number) ? BLANK_CELL : spec.Number;
            Type = (spec.IsTS != null) ? (spec.IsTS.Value ? "Technical Specification (TS)" : "Technical Report (TR)") : BLANK_CELL;
            Title = spec.Title;
            Status = spec.Status;
            PrimaryRespGrp = String.IsNullOrEmpty(spec.PrimeResponsibleGroupShortName) ? BLANK_CELL : spec.PrimeResponsibleGroupShortName;
            PrimaryRapporteur = String.IsNullOrEmpty(spec.PrimeSpecificationRapporteurName) ? BLANK_CELL : spec.PrimeSpecificationRapporteurName;
            InnitialPlannedRelease = String.IsNullOrEmpty(spec.SpecificationInitialRelease) ? BLANK_CELL : spec.SpecificationInitialRelease;
            IsForPublication = (spec.IsForPublication != null) ? (spec.IsForPublication.Value ? "For publication" : "Internal") : BLANK_CELL;
            CommonIMS = spec.ComIMS != null ? spec.ComIMS.Value.ToString() : BLANK_CELL;
            Technologies = String.Join(",", spec.SpecificationTechnologies.ToList().Select(x => x.Enum_Technology.Code));
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

            //if (colName.Equals("Name"))
            //{
            //    switch (Level)
            //    {
            //        case 0:
            //            index += 9;
            //            break;
            //        case 1:
            //            index += 6;
            //            break;
            //        case 2:
            //            index += 3;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //if (StoppedMeeting)
            //{
            //    index += 3;
            //}
            //else
            //{
            //    if (Completion.Value == 1)
            //        index += 2;
            //    else
            //        index += 1;
            //}
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
}
