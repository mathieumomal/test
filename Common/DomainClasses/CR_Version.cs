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
    public partial class CR_Version
    {
        public CR_Version()
        {
            this.ChangeRequests = new HashSet<ChangeRequest>();
        }
    
        public int Pk_SpecVersion { get; set; }
        public int Fk_CR { get; set; }
        public int Fk_Version { get; set; }
        public bool IsNew { get; set; }
    
        public virtual ChangeRequest ChangeRequest { get; set; }
        public virtual ICollection<ChangeRequest> ChangeRequests { get; set; }
        public virtual SpecVersion Version { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
