//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.DomainClasses
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class SyncInfo
    {
        public int Pk_SyncId { get; set; }
        public string TerminalName { get; set; }
        public int Offline_PK_Id { get; set; }
        public Nullable<int> Fk_VersionId { get; set; }
    
        public virtual SpecVersion Version { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}