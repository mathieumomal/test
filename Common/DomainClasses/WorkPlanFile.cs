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
    public partial class WorkPlanFile
    {
        public int Pk_WorkPlanFileId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string WorkPlanFilePath { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
