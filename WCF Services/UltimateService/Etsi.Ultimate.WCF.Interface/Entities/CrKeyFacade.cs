using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// CR key
    /// SpecId, CrNumber, Revision, TsgTdocNumber
    /// </summary>
    [DataContract]
    public class CrKeyFacade
    {
        /// <summary>
        /// Gets or sets the spec identifier.
        /// </summary>
        [DataMember]
        public int SpecId { get; set; }

        /// <summary>
        /// Gets or sets the spec number.
        /// </summary>
        [DataMember]
        public string SpecNumber { get; set; }

        /// <summary>
        /// Gets or sets the cr number.
        /// </summary>
        [DataMember]
        public string CrNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [DataMember]
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the TSG tdoc number.
        /// </summary>
        [DataMember]
        public string TsgTdocNumber { get; set; }

        /// <summary>
        /// Gets or sets the TSG source organization.
        /// </summary>
        [DataMember]
        public string TsgSourceOrganization { get; set; }

        /// <summary>
        /// Gets or sets the TSG meeting identifier.
        /// </summary>
        [DataMember]
        public int TsgMeetingId { get; set; }
    }
}
