using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// CR key
    /// TsgTdocNumber, SpecId, CrNumber, RevisionNumber, Status, Pk_EnumStatus
    /// </summary>
    [DataContract]
    public class CrOfCrPackFacade
    {
        /// <summary>
        /// Gets or sets the TSG tdoc number.
        /// </summary>
        [DataMember]
        public string TsgTdocNumber { get; set; }


        /// <summary>
        /// Gets or sets the spec identifier.
        /// </summary>
        [DataMember]
        public int SpecId { get; set; }

        /// <summary>
        /// Gets or sets the cr number.
        /// </summary>
        [DataMember]
        public string CrNumber { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [DataMember]
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the Status (string).
        /// </summary>
        [DataMember]
        public string Status { get; set; }
    }
}
