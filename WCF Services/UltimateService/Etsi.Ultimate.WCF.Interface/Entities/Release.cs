using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Release entity
    /// </summary>
    [DataContract]
    public class Release
    {
        /// <summary>
        /// Gets or sets the PK_ release identifier.
        /// </summary>
        /// <value>
        /// The PK_ release identifier.
        /// </value>
        [DataMember]
        public int Pk_ReleaseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>
        /// The short name.
        /// </value>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the sort order
        /// </summary>
        [DataMember]
        public int? SortOrder { get; set; }
    }
}
