//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
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
        }
    
        public int Pk_VersionId { get; set; }
        public string MajorVersion { get; set; }
        public string TechnicalVersion { get; set; }
        public string EditorialVersion { get; set; }
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
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
