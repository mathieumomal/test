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
    public partial class ETSI_WorkItem
    {
        public int WKI_ID { get; set; }
        public string WKI_REFERENCE { get; set; }
        public int published { get; set; }
        public Nullable<System.DateTime> PublicationDate { get; set; }
        public string StandardType { get; set; }
        public string Number { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
    
        public Enum_EntityStatus EntityStatus { get; set; }
    }
}
