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
    public partial class ChangeRequest
    {
        public ChangeRequest()
        {
            this.Histories = new HashSet<History>();
            this.Remarks = new HashSet<Remark>();
            this.CR_WorkItems = new HashSet<CR_WorkItems>();
            this.ChangeRequestTsgDatas = new HashSet<ChangeRequestTsgData>();
        }
    
        public int Pk_ChangeRequest { get; set; }
        public string CRNumber { get; set; }
        public Nullable<int> Revision { get; set; }
        public string Subject { get; set; }
        public Nullable<int> Fk_WGStatus { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string WGSourceOrganizations { get; set; }
        public Nullable<int> WGSourceForTSG { get; set; }
        public Nullable<int> WGMeeting { get; set; }
        public Nullable<int> WGTarget { get; set; }
        public Nullable<int> Fk_Enum_CRCategory { get; set; }
        public Nullable<int> Fk_Specification { get; set; }
        public Nullable<int> Fk_Release { get; set; }
        public Nullable<int> Fk_CurrentVersion { get; set; }
        public Nullable<int> Fk_NewVersion { get; set; }
        public Nullable<int> Fk_Impact { get; set; }
        public string WGTDoc { get; set; }
        public string MOD_BY { get; set; }
        public Nullable<System.DateTime> MOD_TS { get; set; }
        public string ClausesAffected { get; set; }
        public string OtherCoreSpecifications { get; set; }
        public string TestSpecifications { get; set; }
        public string OMSpecifications { get; set; }
    
        public virtual SpecVersion CurrentVersion { get; set; }
        public virtual SpecVersion NewVersion { get; set; }
        public virtual Release Release { get; set; }
        public virtual Enum_CRCategory Enum_CRCategory { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual Enum_CRImpact Enum_CRImpact { get; set; }
        public virtual ICollection<CR_WorkItems> CR_WorkItems { get; set; }
        public virtual Enum_ChangeRequestStatus WgStatus { get; set; }
        public virtual ICollection<ChangeRequestTsgData> ChangeRequestTsgDatas { get; set; }
        public virtual Specification Specification { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
