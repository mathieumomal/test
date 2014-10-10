using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// ChangeRequestPackInfo
    /// </summary>
    [DataContract]
    public class ChangeRequestPackInfo
    {
        /// <summary>
        /// The change requests uid decision list
        /// </summary>
        [DataMember]
        public List<KeyValuePair<string, string>> ChangeRequestsUidDecisionList;

    }
}
