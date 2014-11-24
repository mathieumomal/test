using Etsi.Ultimate.Utils.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Business class to export itu preliminary data
    /// </summary>
    public class ItuPreliminaryExporter : IItuPreliminaryExporter
    {
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

                    //TODO::

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
