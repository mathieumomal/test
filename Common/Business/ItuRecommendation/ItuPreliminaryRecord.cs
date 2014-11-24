
namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Record that will be exported to Excel by ItuPreliminaryExporter.
    /// </summary>
    public class ItuPreliminaryRecord
    {
        /// <summary>
        /// Column: Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Column: Specification
        /// </summary>
        public string SpecificationNumber { get; set; }
        
        /// <summary>
        /// Column: Title
        /// </summary>
        public string Title { get; set; }
    }
}
