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
    public class History
    {
        [DataMember]
        public int Pk_HistoryId { get; set; }
        [DataMember]
        public Nullable<int> Fk_ReleaseId { get; set; }
        [DataMember]
        public Nullable<int> Fk_PersonId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreationDate { get; set; }
        [DataMember]
        public string HistoryText { get; set; }
        [DataMember]
        public string PersonName { get; set; }
        [DataMember]
        public Nullable<int> Fk_SpecificationId { get; set; }
        [DataMember]
        public Nullable<int> Fk_CRId { get; set; }
    }
}