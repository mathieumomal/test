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
    public partial class Community
    {
        public int TbId { get; set; }
        public string TbName { get; set; }
        public string TbType { get; set; }
        public string TbTitle { get; set; }
        public Nullable<int> ParentTbId { get; set; }
        public string ActiveCode { get; set; }
        public string ShortName { get; set; }
        public string DetailsURL { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
