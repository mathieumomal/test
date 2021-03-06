//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.DomainClasses
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class Release
    {
        public Release()
        {
            this.Remarks = new HashSet<Remark>();
            this.Histories = new HashSet<History>();
            this.WorkItems = new HashSet<WorkItem>();
            this.Specification_Release = new HashSet<Specification_Release>();
            this.Versions = new HashSet<SpecVersion>();
            this.ChangeRequests = new HashSet<ChangeRequest>();
        }
    
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public Nullable<int> Fk_ReleaseStatus { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> Stage1FreezeDate { get; set; }
        public Nullable<int> Stage1FreezeMtgId { get; set; }
        public Nullable<System.DateTime> Stage2FreezeDate { get; set; }
        public Nullable<int> Stage2FreezeMtgId { get; set; }
        public Nullable<System.DateTime> Stage3FreezeDate { get; set; }
        public Nullable<int> Stage3FreezeMtgId { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> EndMtgId { get; set; }
        public Nullable<System.DateTime> ClosureDate { get; set; }
        public Nullable<int> ClosureMtgId { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> Version2g { get; set; }
        public Nullable<int> Version3g { get; set; }
        public string WpmCode2g { get; set; }
        public string WpmCode3g { get; set; }
        public Nullable<int> WpmProjectId { get; set; }
        public string IturCode { get; set; }
        public string Code { get; set; }
        public string Stage1FreezeMtgRef { get; set; }
        public string Stage2FreezeMtgRef { get; set; }
        public string Stage3FreezeMtgRef { get; set; }
        public string EndMtgRef { get; set; }
        public string ClosureMtgRef { get; set; }
        public int Pk_ReleaseId { get; set; }
        public string LAST_MOD_BY { get; set; }
        public Nullable<System.DateTime> LAST_MOD_TS { get; set; }
    
        public virtual Enum_ReleaseStatus Enum_ReleaseStatus { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<WorkItem> WorkItems { get; set; }
        public virtual ICollection<Specification_Release> Specification_Release { get; set; }
        public virtual ICollection<SpecVersion> Versions { get; set; }
        public virtual ICollection<ChangeRequest> ChangeRequests { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
