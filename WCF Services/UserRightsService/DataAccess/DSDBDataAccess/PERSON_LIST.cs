//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.UserRights.DSDBDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERSON_LIST
    {
        public PERSON_LIST()
        {
            this.PERSON_IN_LIST = new HashSet<PERSON_IN_LIST>();
        }
    
        public int PLIST_ID { get; set; }
        public string PLIST_TYPE { get; set; }
        public string PLISTS_CODE { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public Nullable<int> REF_YEAR { get; set; }
        public Nullable<int> REF_SEQNO { get; set; }
        public string PLIST_TITLE { get; set; }
        public Nullable<int> MTG_ID { get; set; }
        public Nullable<int> OWNER_ID { get; set; }
        public string OWNER_DEPT { get; set; }
        public string OWNERSHIP_LEVEL { get; set; }
        public Nullable<int> TB_ID { get; set; }
        public Nullable<int> OWNER_LIST_ID { get; set; }
        public string PT_TYPE { get; set; }
        public string PT_CODE { get; set; }
        public System.DateTime MOD_TS { get; set; }
        public string MOD_BY { get; set; }
        public string WEB_MOD_BY { get; set; }
        public Nullable<System.DateTime> WEB_MOD_TS { get; set; }
    
        public virtual PERSON PERSON { get; set; }
        public virtual ICollection<PERSON_IN_LIST> PERSON_IN_LIST { get; set; }
    }
}