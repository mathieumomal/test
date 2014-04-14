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
    public partial class Specification_Release
    {
        public Specification_Release()
        {
            this.Remarks = new HashSet<Remark>();
        }
    
        public int Pk_Specification_ReleaseId { get; set; }
        public Nullable<bool> isWithdrawn { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public int Fk_SpecificationId { get; set; }
        public int Fk_ReleaseId { get; set; }
        public Nullable<int> FreezeMeetingId { get; set; }
        public Nullable<int> WithdrawMeetingId { get; set; }
    
        public virtual Release Release { get; set; }
        public virtual ICollection<Remark> Remarks { get; set; }
        public virtual Specification Specification { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
