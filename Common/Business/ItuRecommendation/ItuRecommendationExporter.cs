using Etsi.Ultimate.Utils.Core;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    public class ItuRecommendationExporter : IItuRecommendationExporter
    {
        /// <summary>
        /// Generates the .xlsx export of the records.
        /// </summary>
        /// <param name="filePath">Path where the file should be stored</param>
        /// <param name="records">Records to export</param>
        /// <returns>
        /// True if export was successful.
        /// </returns>
        public bool CreateItuFile(string filePath, List<ItuRecord> records)
        {
            if (String.IsNullOrEmpty(filePath))
                return false;
            if (records == null)
                return false;
            try
            {
                //Create Empty Work Book
                if (File.Exists(filePath)) File.Delete(filePath);
                var newFile = new FileInfo(filePath);

                using (var pck = new ExcelPackage(newFile))
                {
                    // get the handle to the existing worksheet
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    var wsData = pck.Workbook.Worksheets.Add(fileNameWithoutExtension);

                    /*------------*/
                    /* Set Styles */
                    /*------------*/
                    const int rowHeader = 1;
                    int rowDataEnd = records.Count + 1;
                    const int columnStart = 1;
                    const int columnEnd = 10;

                    //Set Font Style
                    wsData.Cells.Style.Font.Size = 8;
                    wsData.Cells.Style.Font.Name = "Arial";

                    //Set Header Style
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsData.Cells[rowHeader, columnStart, rowHeader, columnEnd].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    //Set Cell Borders
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    wsData.Cells[rowHeader, columnStart, rowDataEnd, columnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //Set Column Width
                    wsData.DefaultColWidth = 5;
                    wsData.Column(1).Width = wsData.Column(2).Width = wsData.Column(5).Width = wsData.Column(6).Width = wsData.Column(8).Width = wsData.Column(9).Width = 12;
                    wsData.Column(3).Width = 40;
                    wsData.Column(10).Width = 60;

                    //Set Row Height
                    wsData.DefaultRowHeight = 12;

                    //Set Zoom to 100%
                    wsData.View.ZoomScale = 100;

                    //Get datas specs
                    var datas = from s in records
                                select new
                                {
                                    Paragraph = s.ClauseNumber,
                                    Specification = s.SpecificationNumber,
                                    s.Title,
                                    s.Sdo,
                                    Sdoversion = s.SdoVersionReleaase,
                                    Sdoref = s.SdoReference,
                                    Rev = s.SpecVersionNumber,
                                    Status = s.VersionPublicationStatus,
                                    AppDate = s.PublicationDate,
                                    s.Hyperlink
                                };

                    //Upload Data to Excel
                    wsData.Cells["A1"].LoadFromCollection(
                                                  datas,
                                                  true,
                                                  OfficeOpenXml.Table.TableStyles.None);

                    //Modify Headers for space between words
                    var tmpParagraphName = fileNameWithoutExtension.Split('_')[0];
                    var paragraphName = tmpParagraphName.Substring(Math.Max(0, tmpParagraphName.Length - 4));
                    wsData.Cells[rowHeader, 1].Value = paragraphName+ " Paragraph";
                    wsData.Cells[rowHeader, 4].Value = "SDO";
                    wsData.Cells[rowHeader, 5].Value = "SDOversion";
                    wsData.Cells[rowHeader, 6].Value = "SDOref";
                    wsData.Cells[rowHeader, 9].Value = "App date";
                    wsData.Cells[rowHeader, 10].Value = "hyperlink";

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
    }

    /// <summary>
    /// Exporter that can generate excel export of the ITU recommendation.
    /// </summary>
    public interface IItuRecommendationExporter
    {
        /// <summary>
        /// Generates the .xlsx export of the records.
        /// </summary>
        /// <param name="filePath">Path where the file should be stored</param>
        /// <param name="records">Records to export</param>
        /// <returns>True if export was successful.</returns>
        bool CreateItuFile(string filePath, List<ItuRecord> records);
    }
}
