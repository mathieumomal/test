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
    public class Release
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ShortName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Nullable<int> Fk_ReleaseStatus { get; set; }
        [DataMember]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> Stage1FreezeDate { get; set; }
        [DataMember]
        public Nullable<int> Stage1FreezeMtgId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> Stage2FreezeDate { get; set; }
        [DataMember]
        public Nullable<int> Stage2FreezeMtgId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> Stage3FreezeDate { get; set; }
        [DataMember]
        public Nullable<int> Stage3FreezeMtgId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EndDate { get; set; }
        [DataMember]
        public Nullable<int> EndMtgId { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ClosureDate { get; set; }
        [DataMember]
        public Nullable<int> ClosureMtgId { get; set; }
        [DataMember]
        public Nullable<int> SortOrder { get; set; }
        [DataMember]
        public Nullable<int> Version2g { get; set; }
        [DataMember]
        public Nullable<int> Version3g { get; set; }
        [DataMember]
        public string WpmCode2g { get; set; }
        [DataMember]
        public string WpmCode3g { get; set; }
        [DataMember]
        public Nullable<int> WpmProjectId { get; set; }
        [DataMember]
        public string IturCode { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Stage1FreezeMtgRef { get; set; }
        [DataMember]
        public string Stage2FreezeMtgRef { get; set; }
        [DataMember]
        public string Stage3FreezeMtgRef { get; set; }
        [DataMember]
        public string EndMtgRef { get; set; }
        [DataMember]
        public string ClosureMtgRef { get; set; }
        [DataMember]
        public int Pk_ReleaseId { get; set; }
        [DataMember]
        public string LAST_MOD_BY { get; set; }
        [DataMember]
        public Nullable<System.DateTime> LAST_MOD_TS { get; set; }
    }
}
