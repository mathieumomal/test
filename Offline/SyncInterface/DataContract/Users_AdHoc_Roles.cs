//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SyncInterface.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    
    [DataContract]
    public class Users_AdHoc_Roles
    {
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string PERSON_ID { get; set; }
        [DataMember]
        public string RoleName { get; set; }
    }
}