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
    public partial class Enum_CommunitiesShortName
    {
        public int Pk_EnumCommunitiesShortNames { get; set; }
        public Nullable<int> Fk_TbId { get; set; }
        public string ShortName { get; set; }
        public Nullable<int> WpmProjectId { get; set; }
        public Nullable<int> MapId_3SS { get; set; }
        public string DetailsURL { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
