﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationExporter
    {
        #region Properties

        private IUltimateUnitOfWork UoW;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SpecificationExporter(IUltimateUnitOfWork iUoW)
        {
            UoW = iUoW;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Export Specification
        /// </summary>
        public string ExportSpecification(int personId, SpecificationSearch searchObj, string baseurl)
        {
            try
            {
                var specManager = new SpecificationManager {UoW = UoW};
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = UoW;
                var personManager = ManagerFactory.Resolve<IPersonManager>();
                personManager.UoW = UoW;
                var relMgr = ManagerFactory.Resolve<IReleaseManager>();
                relMgr.UoW = UoW;

                searchObj.PageSize = 0;
                var specs = specManager.GetSpecificationBySearchCriteria(personId, searchObj, true).Key.Key;
                specs.ForEach(x => x.PrimeResponsibleGroupShortName = (x.PrimeResponsibleGroup == null) ? String.Empty : communityManager.GetCommmunityshortNameById(x.PrimeResponsibleGroup.Fk_commityId));

                // Bind the rapporteur's name
                // For this, we retrieve all the distinct person names that are responsible for a spec, we go fetch their data in DB, and we build their display name
                var personIdList = specs.Select(x => x.PrimeSpecificationRapporteurIds.FirstOrDefault()).Distinct().ToList();
                var responsiblePeople = personManager.GetByIds(personIdList);
                foreach (var spec in specs)
                {
                    spec.PrimeSpecificationRapporteurName = "";
                    if (spec.PrimeSpecificationRapporteurIds.Count > 0 )
                    {
                        var p = responsiblePeople.Find(x => x.PERSON_ID ==  spec.PrimeSpecificationRapporteurIds.FirstOrDefault());
                        if (p != null)
                        {
                            spec.PrimeSpecificationRapporteurName = p.FIRSTNAME + " " + p.LASTNAME;
                        }
                    }
                }
                
                // Bind the initial release by asking the Release manager to get the list of ordered releases
                // list of ordered releases.
                var allReleases = relMgr.GetAllReleases(personId).Key.OrderBy(r => r.SortOrder).ToList();
                foreach (var spec in specs)
                {
                    if ((spec.Specification_Release != null) && spec.Specification_Release.Count != 0)
                    {
                        var specReleaseId = spec.Specification_Release.Select(sr => sr.Fk_ReleaseId);
                        var firstRelease = allReleases.FirstOrDefault(r => specReleaseId.Contains(r.Pk_ReleaseId));
                        if (firstRelease != null)
                        {
                            spec.SpecificationInitialRelease = firstRelease.Code;
                        }
                    }
                }

                var specExportObjects = new List<SpecificationForExport>();
                specExportObjects.AddRange(specs.Select(y => new SpecificationForExport(y)));

                var filename= DateTime.Now.ToString("yyyy-MM-dd_HHmm")+"_SpecificationList_"+ Guid.NewGuid().ToString().Substring(0, 6)+".xlsx";

                var fullExcelFilePath = Utils.ConfigVariables.DefaultPublicTmpPath + filename;
                if (! Directory.Exists(Utils.ConfigVariables.DefaultPublicTmpPath))
                    Directory.CreateDirectory(Utils.ConfigVariables.DefaultPublicTmpPath);

                ExportToExcel(specExportObjects, fullExcelFilePath, baseurl);
                return Utils.ConfigVariables.DefaultPublicTmpAddress+filename;
            }
            catch (IOException ex)
            {
                LogManager.Error("Export of the specification failed", ex);
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export Specification to Excel
        /// </summary>
        /// <param name="exportSpecification"></param>
        /// <param name="exportPath">Export Path</param>
        /// <param name="baseurl"></param>
        private void ExportToExcel(List<SpecificationForExport> exportSpecification, string exportPath, string baseurl)
        {
            if (!String.IsNullOrEmpty(exportPath) && exportSpecification != null)
            {
                try
                {
                    //Create Empty Work Book
                    if (File.Exists(exportPath)) File.Delete(exportPath);
                    FileInfo newFile = new FileInfo(exportPath);

                    using (ExcelPackage pck = new ExcelPackage(newFile))
                    {
                        // get the handle to the existing worksheet
                        var wsData = pck.Workbook.Worksheets.Add("Specifications");

                        /*------------*/
                        /* Set Styles */
                        /*------------*/
                        const int rowHeader = 1;
                        const int rowDataStart = 2;
                        int rowDataEnd = exportSpecification.Count + 1;
                        const int columnStart = 1;
                        const int columnEnd = 10;

                        #region hyperlink Style
                        wsData.Cells[rowDataStart, columnStart, rowDataEnd, columnStart].Style.Font.Bold = true;
                        wsData.Cells[rowDataStart, columnStart, rowDataEnd, columnStart].Style.Font.UnderLine = true;
                        wsData.Cells[rowDataStart, columnStart, rowDataEnd, columnStart].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        #endregion

                        //Set Font Style
                        wsData.Cells.Style.Font.Size = 8;
                        wsData.Cells.Style.Font.Name = "Arial";
                        //Set Header Style
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Font.Bold = true;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

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

                        //Get datas specs
                        var datas = from s in exportSpecification
                                    orderby s.Number
                                    select new
                                    {
                                        Spec_No = s.Number, 
                                        s.Type,
                                        s.Title,
                                        s.Status,
                                        Primary_Resp_Grp = s.PrimaryRespGrp,
                                        Primary_rapporteur = s.PrimaryRapporteur,
                                        Initial_planned_Release = s.InitialPlannedRelease,
                                        Publication = s.IsForPublication,
                                        Common_IMS = s.CommonIMS,
                                        Technology = s.Technologies
                                    };

                        var specIdRelatedList = from s in exportSpecification
                                 orderby s.Number
                                 select new
                                 {
                                     s.SpecId
                                 };

                        //Upload Data to Excel
                        wsData.Cells["A1"].LoadFromCollection(
                            datas,
                            true, 
                            OfficeOpenXml.Table.TableStyles.None);

                        #region add hyperlink
                        var count = 2;
                        foreach (var specId in specIdRelatedList)
                        {
                            string hyperlink = new StringBuilder()
                                .Append(baseurl)
                                .Append(@"/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=")
                                .Append(specId.SpecId)
                                .ToString();
                            wsData.Cells[count, columnStart].Hyperlink = new Uri(hyperlink, UriKind.RelativeOrAbsolute);
                            count++;
                        }
                        #endregion

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
        /// <param name="style"></param>
        private void SetCellContent(TableRow tableRow, TableCell currentCell, string cellContent, DomainClasses.DocxStylePool.STYLES_KEY style)
        {
            DocxStyle docx = DocxStylePool.GetDocxStyle((int)style);
            Paragraph cellParagraph = SetParagraphContent(docx, cellContent, "Arial", "16");
            currentCell.Append(cellParagraph);
            var tcp = new TableCellProperties();
            var griedSpan = new GridSpan();
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
        private Paragraph SetParagraphContent(DomainClasses.DocxStyle docx, string text, string fontStyle, string fontSizeValue)
        {
            var contentParagraph = new Paragraph();
            var contentParagraph_run = new Run();
            var contentParagraph_text = new Text(text) { Space = SpaceProcessingModeValues.Preserve };
            var contentParagraph__pPr = new ParagraphProperties(new SpacingBetweenLines() { After = "0" });
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
        private RunProperties SetParagraphStyle(DomainClasses.DocxStyle docx, string fontStyle, string fontSizeValue)
        {
            var cellParagraph_runPro = new RunProperties();
            var runFont = new RunFonts() { Ascii = fontStyle, ComplexScript = fontStyle };
            var color = new Color() { Val = docx.GetFontColorHex() };
            var fontSize = new FontSize() { Val = fontSizeValue };
            cellParagraph_runPro.Append(runFont);
            if (docx.IsBold)
                cellParagraph_runPro.Append(new Bold());
            cellParagraph_runPro.Append(color);
            cellParagraph_runPro.Append(fontSize);
            return cellParagraph_runPro;
        }

        #endregion
    }

    /// <summary>
    /// Format Specification object which is suitable for export
    /// </summary>
    internal class SpecificationForExport
    {
        #region Properties
        //Display elements
        private const string BLANK_CELL = "  -  ";
        public string Number { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string PrimaryRespGrp { get; set; }
        public string PrimaryRapporteur { get; set; }
        public string InitialPlannedRelease { get; set; }
        public string IsForPublication { get; set; }
        public string CommonIMS { get; set; }
        public string Technologies { get; set; }

        //HyperLink element
        public int SpecId { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to format & convert Specification properties
        ///   - Empty Records      :: '  -  '
        ///   - Date               :: yyyy-MM-dd
        /// </summary>
        public SpecificationForExport(Specification spec)
        {
            //Display elements
            Number = String.IsNullOrEmpty(spec.Number) ? BLANK_CELL : spec.Number;
            Type = (spec.IsTS != null) ? (spec.IsTS.Value ? "Technical Specification (TS)" : "Technical Report (TR)") : BLANK_CELL;
            Title = spec.Title;
            Status = spec.Status;
            PrimaryRespGrp = String.IsNullOrEmpty(spec.PrimeResponsibleGroupShortName) ? BLANK_CELL : spec.PrimeResponsibleGroupShortName;
            PrimaryRapporteur = String.IsNullOrEmpty(spec.PrimeSpecificationRapporteurName) ? BLANK_CELL : spec.PrimeSpecificationRapporteurName;
            InitialPlannedRelease = String.IsNullOrEmpty(spec.SpecificationInitialRelease) ? BLANK_CELL : spec.SpecificationInitialRelease;
            IsForPublication = (spec.IsForPublication != null) ? (spec.IsForPublication.Value ? "For publication" : "Internal") : BLANK_CELL;
            CommonIMS = spec.ComIMS != null ? spec.ComIMS.Value.ToString() : BLANK_CELL;
            Technologies = String.Join(",", spec.SpecificationTechnologies.ToList().Select(x => x.Enum_Technology.Code).OrderBy(y => y));

            //Hyperlink element
            SpecId = spec.Pk_SpecificationId;
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
            const int index = 0;
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
            var space = new StringBuilder();
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
