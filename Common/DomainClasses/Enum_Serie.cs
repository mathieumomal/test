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
    public partial class Enum_Serie
    {
        public Enum_Serie()
        {
            this.Specifications = new HashSet<Specification>();
        }
    
        public int Pk_Enum_SerieId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Specification> Specifications { get; set; }
    }
}
