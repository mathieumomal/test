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
            this.CR_Version = new HashSet<CR_Version>();
            this.Histories = new HashSet<History>();
            this.Remarks = new HashSet<Remark>();
        }
    
        public int Pk_ChangeRequest { get; set; }
        public string CRNumber { get; set; }
        public Nullable<int> Revision { get; set; }
        public string Subject { get; set; }
        public Nullable<int> Fk_TSGStatus { get; set; }
        public Nullable<int> Fk_WGStatus { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string TSGSourceOrganizations { get; set; }
        public string WGSourceOrganizations { get; set; }
        public Nullable<int> TSGMeeting { get; set; }
        public Nullable<int> TSGTarget { get; set; }
        public Nullable<int> WGSourceForTSG { get; set; }
        public Nullable<int> Fk_TSGTDoc { get; set; }
        public Nullable<int> WGMeeting { get; set; }
        public Nullable<int> WGTarget { get; set; }
        public Nullable<int> Fk_WGTDoc { get; set; }
        public Nullable<int> Fk_Enum_CRCategory { get; set; }
        public Nullable<int> Fk_SpecRelease { get; set; }
    
        public virtual ICollection<CR_Version> CR_Version { get; set; }
        public virtual Enum_CRCategory Enum_CRCategory { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual Specification_Release Specification_Release { get; set; }
        public virtual Enum_TDocStatus Enum_TDocStatusTSG { get; set; }
        public virtual TDoc TDocTSG { get; set; }
        public virtual Enum_TDocStatus Enum_TDocStatusWG { get; set; }
        public virtual TDoc TDocWG { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
