using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Specification entity
    /// </summary>
    [DataContract]
    public class Specification
    {
        /// <summary>
        /// Gets or sets the PK_ specification identifier.
        /// </summary>
        /// <value>
        /// The PK_ specification identifier.
        /// </value>
        [DataMember]
        public int Pk_SpecificationId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the spec number and title.
        /// </summary>
        /// <value>
        /// The spec number and title.
        /// </value>
        [DataMember]
        public string SpecNumberAndTitle { get; set; }

        /// <summary>
        /// Gets or sets the community identifier of primary responsible group.
        /// </summary>
        /// <value>
        /// The community identifier of primary responsible group.
        /// </value>
        [DataMember]
        public int PrimaryResponsibleGroup_CommunityId { get; set; }
    }
}
