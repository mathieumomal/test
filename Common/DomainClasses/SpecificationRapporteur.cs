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
    public partial class SpecificationRapporteur
    {
        public int Pk_SpecificationRapporteurId { get; set; }
        public int Fk_SpecificationId { get; set; }
        public int Fk_RapporteurId { get; set; }
        public bool IsPrime { get; set; }
    
        public virtual Specification Specification { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
