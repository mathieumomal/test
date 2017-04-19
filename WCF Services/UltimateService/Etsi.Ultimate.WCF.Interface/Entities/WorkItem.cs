using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Etsi.Ultimate.WCF.Interface.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class WorkItem
    {
        /// <summary>
        /// Gets or sets the PK_ work item uid.
        /// </summary>
        /// <value>
        /// The PK_ work item uid.
        /// </value>
        [DataMember]
        public int Pk_WorkItemUid { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        [DataMember]
        public string UID { get; set; }

        /// <summary>
        /// Gets or sets the acronym.
        /// </summary>
        /// <value>
        /// The acronym.
        /// </value>
        [DataMember]
        public string Acronym { get; set; }

        /// <summary>
        /// Gets or sets the acronym.
        /// </summary>
        /// <value>
        /// The acronym.
        /// </value>
        [DataMember]
        public string EffectiveAcronym { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the responsible groups.
        /// </summary>
        /// <value>
        /// The responsible groups.
        /// </value>
        [DataMember]
        public string ResponsibleGroups { get; set; }

        [DataMember]
        public List<int> ResponsibleGroupIds { get; set; } 
    }
}
