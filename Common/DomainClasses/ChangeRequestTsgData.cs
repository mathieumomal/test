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
    public partial class ChangeRequestTsgData
    {
        public int Pk_ChangeRequestTsgData { get; set; }
        public string TSGTdoc { get; set; }
        public Nullable<int> TSGTarget { get; set; }
        public Nullable<int> TSGMeeting { get; set; }
        public string TSGSourceOrganizations { get; set; }
        public int Fk_ChangeRequest { get; set; }
        public Nullable<int> Fk_TsgStatus { get; set; }
    
        public virtual ChangeRequest ChangeRequest { get; set; }
        public virtual Enum_ChangeRequestStatus TsgStatus { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}