using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// Represents a change request status
    /// </summary>
    [DataContract]
    class Enum_ChangeRequestStatus
    {
        /// <summary>
        /// The change request identifier, for further linking with Change requests
        /// </summary>
        [DataMember]
        public int Pk_ChangeRequestStatus { get; set; }

        /// <summary>
        /// The Code identifying the Status
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// The description of the status
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
