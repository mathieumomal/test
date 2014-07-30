using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Etsi.UserRights.Interface
{
    /// <summary>
    /// Class to provide User Rights to requested client
    /// </summary>
    [DataContract]
    public class PersonRights
    {
        /// <summary>
        /// Application Rights for User
        /// </summary>
        [DataMember]
        public List<string> ApplicationRights { get; set; }

        /// <summary>
        /// Committee Rights for User (specific to Committees)
        /// </summary>
        [DataMember]
        public Dictionary<int, List<string>> CommitteeRights { get; set; }
    }
}
