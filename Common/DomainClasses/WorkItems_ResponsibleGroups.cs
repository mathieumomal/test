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
    public partial class WorkItems_ResponsibleGroups
    {
        public int Pk_WorkItemResponsibleGroups { get; set; }
        public Nullable<int> Fk_WorkItemId { get; set; }
        public Nullable<int> Fk_TbId { get; set; }
        public string ResponsibleGroup { get; set; }
        public Nullable<bool> IsPrimeResponsible { get; set; }
    
        public virtual WorkItem WorkItem { get; set; }
    }
}
