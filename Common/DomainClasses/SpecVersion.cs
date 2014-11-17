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
    public partial class SpecVersion
    {
        public SpecVersion()
        {
            this.Remarks = new HashSet<Remark>();
            this.CurrentChangeRequests = new HashSet<ChangeRequest>();
            this.FoundationsChangeRequests = new HashSet<ChangeRequest>();
            this.SyncInfoes = new HashSet<SyncInfo>();
        }
    
        public int Pk_VersionId { get; set; }
        public Nullable<int> MajorVersion { get; set; }
        public Nullable<int> TechnicalVersion { get; set; }
        public Nullable<int> EditorialVersion { get; set; }
        public Nullable<System.DateTime> AchievedDate { get; set; }
        public Nullable<System.DateTime> ExpertProvided { get; set; }
        public string Location { get; set; }
        public bool SupressFromSDO_Pub { get; set; }
        public bool ForcePublication { get; set; }
        public Nullable<System.DateTime> DocumentUploaded { get; set; }
        public Nullable<System.DateTime> DocumentPassedToPub { get; set; }
        public bool Multifile { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<int> ETSI_WKI_ID { get; set; }
        public Nullable<int> ProvidedBy { get; set; }
        public Nullable<int> Fk_SpecificationId { get; set; }
        public Nullable<int> Fk_ReleaseId { get; set; }
        public string ETSI_WKI_Ref { get; set; }
    
        public virtual Release Release { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual Specification Specification { get; set; }
        public virtual ICollection<ChangeRequest> CurrentChangeRequests { get; set; }
        public virtual ICollection<ChangeRequest> FoundationsChangeRequests { get; set; }
        public virtual ICollection<SyncInfo> SyncInfoes { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
