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
    public partial class ResponsibleGroup_Secretary
    {
        public Nullable<int> TbId { get; set; }
        public string Email { get; set; }
        public int PersonId { get; set; }
        public Nullable<System.DateTime> roleExpirationDate { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
