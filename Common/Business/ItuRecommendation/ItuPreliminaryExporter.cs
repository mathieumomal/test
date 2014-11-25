using System.Drawing;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml.Style;
using Path = System.IO.Path;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Business class to export itu preliminary data
    /// </summary>
    public class ItuPreliminaryExporter : IItuPreliminaryExporter
    {
        #region variables
        private int _rowDataEnd;
        #endregion

        #region exporter
        /// <summary>
        /// Generates the .xlsx export of the records.
        /// </summary>
        /// <param name="filePath">Path where the file should be stored</param>
        /// <param name="records">Records to export</param>
        /// <returns>True if export was successful.</returns>
        public bool CreateItuPreliminaryFile(string filePath, List<ItuPreliminaryRecord> records)
        {
            if (String.IsNullOrEmpty(filePath))
                return false;
            if (records == null || records.Count == 0)
                return false;
            try
            {
                //Create Empty Work Book
                if (File.Exists(filePath)) File.Delete(filePath);
                var newFile = new FileInfo(filePath);

                using (var pck = new ExcelPackage(newFile))
                {
                    //Create excel file
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    var wsData = pck.Workbook.Worksheets.Add(fileNameWithoutExtension);

                    //Preparation of the excel file (Style, columns, ...)
                    InitExcelFile(wsData, records);

                    //Filled excel file
                    ExportItuPreliminaryRecords(wsData, records);

                    //Set hyperlinks
                    SetHyperLinks(wsData, records);

                    //Save excel file
                    pck.Save();
                }
            }
            catch (IOException ex)
            {
                LogManager.Error(ex.Message, ex);
                return false;
            }

            return true;
        }
        #endregion

        #region export private methods
        /// <summary>
        /// Filled excel file
        /// </summary>
        /// <param name="wsData"></param>
        /// <param name="records"></param>
        private void ExportItuPreliminaryRecords(ExcelWorksheet wsData, List<ItuPreliminaryRecord> records)
        {

            //Format data
            var formatedData = from s in records
                orderby s.SpecificationNumber
                select new
                {
                    Type = s.Type,
                    SpecNumber = s.SpecificationNumber,
                    Title = s.Title
                };

            //Filled excel
            wsData.Cells["A2"].LoadFromCollection(formatedData, false, OfficeOpenXml.Table.TableStyles.None);
        }

        /// <summary>
        /// Init excel file in term of style
        /// </summary>
        /// <param name="wsData"></param>
        /// <param name="records"></param>
        private void InitExcelFile(ExcelWorksheet wsData, List<ItuPreliminaryRecord> records)
        {
            //Set Zoom to 100%
            wsData.View.ZoomScale = ItuPreliminaryRecordConfiguration.Scale;

            //Set rowDataEnd
            _rowDataEnd = records.Count + 1;

            //Set Font Style
            wsData.Cells.Style.Font.Size = ItuPreliminaryRecordConfiguration.FontSize;
            wsData.Cells.Style.Font.Name = ItuPreliminaryRecordConfiguration.Font;

            //Set Header Style
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            //Set Cell Borders
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, _rowDataEnd, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, _rowDataEnd, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, _rowDataEnd, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.ColumnStart, _rowDataEnd, ItuPreliminaryRecordConfiguration.ColumnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            //Set Column Width
            wsData.Column(ItuPreliminaryRecordConfiguration.Type.Index).Width = ItuPreliminaryRecordConfiguration.Type.Width;
            wsData.Column(ItuPreliminaryRecordConfiguration.SpecNumber.Index).Width = ItuPreliminaryRecordConfiguration.SpecNumber.Width;
            wsData.Column(ItuPreliminaryRecordConfiguration.Title.Index).Width = ItuPreliminaryRecordConfiguration.Title.Width;

            //Set Row Height
            wsData.DefaultRowHeight = ItuPreliminaryRecordConfiguration.DefaultColHeight;

            //Set header column titles
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.Type.Index].Value = ItuPreliminaryRecordConfiguration.Type.Name;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.SpecNumber.Index].Value = ItuPreliminaryRecordConfiguration.SpecNumber.Name;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader, ItuPreliminaryRecordConfiguration.Title.Index].Value = ItuPreliminaryRecordConfiguration.Title.Name;

            //Set hyperlink style
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader + 1, ItuPreliminaryRecordConfiguration.SpecNumber.Index, _rowDataEnd, ItuPreliminaryRecordConfiguration.SpecNumber.Index].Style.Font.Bold = true;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader + 1, ItuPreliminaryRecordConfiguration.SpecNumber.Index, _rowDataEnd, ItuPreliminaryRecordConfiguration.SpecNumber.Index].Style.Font.UnderLine = true;
            wsData.Cells[ItuPreliminaryRecordConfiguration.RowHeader + 1, ItuPreliminaryRecordConfiguration.SpecNumber.Index, _rowDataEnd, ItuPreliminaryRecordConfiguration.SpecNumber.Index].Style.Font.Color.SetColor(Color.Blue);
        }

        /// <summary>
        /// Manage hyperlinks
        /// </summary>
        /// <param name="wsData"></param>
        /// <param name="records"></param>
        private void SetHyperLinks(ExcelWorksheet wsData, List<ItuPreliminaryRecord> records)
        {
            var excelRow = ItuPreliminaryRecordConfiguration.RowHeader + 1;

            foreach (var record in records)
            {
                if (record.SpecificationId != 0)
                {
                    var specificationHyperlink = String.Format(ConfigVariables.SpecificationDetailsUrl, record.SpecificationId);
                    wsData.Cells[excelRow, ItuPreliminaryRecordConfiguration.SpecNumber.Index].Hyperlink = new Uri(specificationHyperlink, UriKind.RelativeOrAbsolute);
                }

                excelRow++;
            }
        }
        #endregion
    }

    /// <summary>
    /// Exporter that can generate excel export of the ITU recommendation.
    /// </summary>
    public interface IItuPreliminaryExporter
    {
        /// <summary>
        /// Generates the .xlsx export of the records.
        /// </summary>
        /// <param name="filePath">Path where the file should be stored</param>
        /// <param name="records">Records to export</param>
        /// <returns>True if export was successful.</returns>
        bool CreateItuPreliminaryFile(string filePath, List<ItuPreliminaryRecord> records);
    }
}
