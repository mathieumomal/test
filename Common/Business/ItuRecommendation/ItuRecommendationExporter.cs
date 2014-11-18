using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    public class ItuRecommendationExporter: IItuRecommendationExporter
    {
        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public bool CreateItuFile(string filePath, List<ItuRecord> records)
        {
            throw new NotImplementedException();
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
