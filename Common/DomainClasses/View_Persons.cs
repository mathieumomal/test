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
    public partial class View_Persons
    {
        public int PERSON_ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public int ORGA_ID { get; set; }
        public string ORGA_NAME { get; set; }
        public string DELETED_FLG { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
