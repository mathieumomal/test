//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SyncInterface.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    
    [DataContract]
    public class Specification_WorkItem
    {
        [DataMember]
        public int Pk_Specification_WorkItemId { get; set; }
        [DataMember]
        public int Fk_SpecificationId { get; set; }
        [DataMember]
        public int Fk_WorkItemId { get; set; }
        [DataMember]
        public Nullable<bool> isPrime { get; set; }
        [DataMember]
        public Nullable<bool> IsSetByUser { get; set; }
    }
}
