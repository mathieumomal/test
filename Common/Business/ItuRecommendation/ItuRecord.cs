using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Record that will be exported to Excel by ItuRecommendationExporter.
    /// </summary>
    public class ItuRecord
    {
        /// <summary>
        /// Column: Paragraph
        /// </summary>
        public string ClauseNumber { get; set; }

        /// <summary>
        /// Column: Specification
        /// </summary>
        public string SpecificationNumber { get; set; }
        
        /// <summary>
        /// Column: Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Column: SDO
        /// </summary>
        public string Sdo { get; set; }

        /// <summary>
        /// Column: SDOversion
        /// </summary>
        public string SdoVersionReleaase { get; set; }

        /// <summary>
        /// Column: SDOref
        /// </summary>
        public string SdoReference { get; set; }

        /// <summary>
        /// Column: Rev
        /// </summary>
        public string SpecVersionNumber { get; set; }

        /// <summary>
        /// Column: Status
        /// </summary>
        public string VersionPublicationStatus { get; set; }

        /// <summary>
        /// Column: App date
        /// </summary>
        public DateTime PublicationDate { get; set; }

        /// <summary>
        /// Column: hyperlink
        /// </summary>
        public string Hyperlink { get; set; }
    }
}
