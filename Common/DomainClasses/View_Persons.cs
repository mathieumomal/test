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
    }
}
