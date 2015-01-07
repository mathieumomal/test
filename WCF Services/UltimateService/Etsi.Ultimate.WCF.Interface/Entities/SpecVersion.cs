using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Version entity
    /// </summary>
    [DataContract]
    public class SpecVersion
    {
        /// <summary>
        /// Gets or sets the Primary key of version identifier.
        /// </summary>
        [DataMember]
        public int Pk_VersionId { get; set; }

        /// <summary>
        /// Gets or sets the Major version
        /// </summary>
        [DataMember]
        public int? MajorVersion { get; set; }

        /// <summary>
        /// Gets or sets the Technical version
        /// </summary>
        [DataMember]
        public int? TechnicalVersion { get; set; }

        /// <summary>
        /// Gets or sets the Editorial version
        /// </summary>
        [DataMember]
        public int? EditorialVersion { get; set; }

        /// <summary>
        /// Gets or sets the Related Tdoc
        /// </summary>
        [DataMember]
        public string RelatedTDoc { get; set; }
    }
}
