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
    public class Users_Groups
    {
        [DataMember]
        public int PERSON_ID { get; set; }
        [DataMember]
        public int PLIST_ID { get; set; }
        [DataMember]
        public Nullable<int> TB_ID { get; set; }
        [DataMember]
        public string PERS_ROLE_CODE { get; set; }
    }
}
