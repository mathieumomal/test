//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a �t� g�n�r� � partir d'un mod�le.
//
//    Des modifications manuelles apport�es � ce fichier peuvent conduire � un comportement inattendu de votre application.
//    Les modifications manuelles apport�es � ce fichier sont remplac�es si le code est r�g�n�r�.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.DomainClasses
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class Remark
    {
        public Remark()
        {
            this.SyncInfoes = new HashSet<SyncInfo>();
        }
    
        public int Pk_RemarkId { get; set; }
        public Nullable<int> Fk_ReleaseId { get; set; }
        public Nullable<int> Fk_PersonId { get; set; }
        public Nullable<bool> IsPublic { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string RemarkText { get; set; }
        public Nullable<int> Fk_WorkItemId { get; set; }
        public string PersonName { get; set; }
        public Nullable<int> Fk_SpecificationId { get; set; }
        public Nullable<int> Fk_SpecificationReleaseId { get; set; }
        public Nullable<int> Fk_VersionId { get; set; }
        public Nullable<int> Fk_CRId { get; set; }
    
        public virtual Release Release { get; set; }
        public virtual WorkItem WorkItem { get; set; }
        public virtual Specification Specification { get; set; }
        public virtual Specification_Release Specification_Release { get; set; }
        public virtual SpecVersion Version { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }
        public virtual ICollection<SyncInfo> SyncInfoes { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
