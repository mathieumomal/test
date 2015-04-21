using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Change Request TSG data
    /// </summary>
    [DataContract]
    public class ChangeRequestTsgData
    {
        /// <summary>
        /// Id of ChangeRequestTsgData
        /// </summary>
        [DataMember]
        public int PkChangeRequestTsgData { get; set; }

        /// <summary>
        /// TsgTdoc number
        /// </summary>
        [DataMember]
        public string TsgTdoc { get; set; }

        /// <summary>
        /// Gets or sets the TSG target.
        /// </summary>
        [DataMember]
        public int? TsgTarget { get; set; }

        /// <summary>
        /// Gets or sets the TSG meeting
        /// </summary>
        [DataMember]
        public int? TsgMeeting { get; set; }

        /// <summary>
        /// Gets or sets TSG source organization
        /// </summary>
        [DataMember]
        public string TsgSourceOrganizations { get; set; }

        /// <summary>
        /// TSG status
        /// </summary>
        [DataMember]
        public string TsgStatus { get; set; }

        /// <summary>
        /// Id of TSG status
        /// </summary>
        [DataMember]
        public int? FkTsgStatus { get; set; }

        /// <summary>
        /// Id of related CR
        /// </summary>
        [DataMember]
        public int FkChangeRequest { get; set; }
    }
}
