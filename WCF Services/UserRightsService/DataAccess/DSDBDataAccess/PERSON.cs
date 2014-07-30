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
    
    public partial class PERSON
    {
        public PERSON()
        {
            this.PERSON_IN_LIST = new HashSet<PERSON_IN_LIST>();
            this.PERSON_LIST = new HashSet<PERSON_LIST>();
        }
    
        public int PERSON_ID { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string PERS_TITL { get; set; }
        public string ADDRESS { get; set; }
        public string EMAILTYP { get; set; }
        public string PHONE { get; set; }
        public string FAX { get; set; }
        public string EXPERT_FLG { get; set; }
        public string HOMEADDR { get; set; }
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        public string BIRTH_PLAC { get; set; }
        public string MAR_STATUS { get; set; }
        public string PP_NUMBER { get; set; }
        public string PP_ISS_AT { get; set; }
        public Nullable<System.DateTime> PP_ISSUED { get; set; }
        public Nullable<System.DateTime> PP_EXPIR { get; set; }
        public Nullable<System.DateTime> WP_START { get; set; }
        public Nullable<decimal> WP_MONTHS { get; set; }
        public string PERSINIT { get; set; }
        public string CTY_CODE { get; set; }
        public string ESTAFF_FLG { get; set; }
        public string VMS_USER { get; set; }
        public string DEPT_CODE { get; set; }
        public string JOB_CODE { get; set; }
        public string ETSI_INIT { get; set; }
        public string ROOM { get; set; }
        public string TEL_EXT { get; set; }
        public Nullable<int> ORGA_ID { get; set; }
        public string RAPTR_FLG { get; set; }
        public string ESMS_CODE { get; set; }
        public string CTY_POST { get; set; }
        public string ZIP { get; set; }
        public string CITY { get; set; }
        public string ADD_CTY_CODE { get; set; }
        public string OFFICIAL_FLG { get; set; }
        public string CONTACT_FLG { get; set; }
        public string ACTIVE_EXPERT_FLG { get; set; }
        public string OFFICIAL_CONTACT_FLG { get; set; }
        public string PT_CONTRACT_FLG { get; set; }
        public Nullable<int> SORT_KEY { get; set; }
        public string SECRETARY_PHONE { get; set; }
        public string STANDARD_PHONE { get; set; }
        public string MOBILE_PHONE { get; set; }
        public string OTHER_PHONE { get; set; }
        public string OTHER_PHONE_QUAL { get; set; }
        public string PAGER_NUMB { get; set; }
        public string FAX2 { get; set; }
        public string CTY_POST_EXPRS { get; set; }
        public string ZIP_EXPRS { get; set; }
        public string CITY_EXPRS { get; set; }
        public string CTY_CODE_EXPRS { get; set; }
        public string ADDRESS_EXPRS { get; set; }
        public string DELETED_FLG { get; set; }
        public string BUSINESS_TITLE { get; set; }
        public string PICTURE_PATH { get; set; }
        public string VIDEO_CONF_NUMB { get; set; }
        public string ETSI_ADD { get; set; }
        public string INTERNET_ADD { get; set; }
        public string X400_ADD { get; set; }
        public string OTHER_ADD { get; set; }
        public System.DateTime MOD_TS { get; set; }
        public string MOD_BY { get; set; }
        public string TEL_DIRECTORY_FLG { get; set; }
        public string WEB_UPDATE_STATE { get; set; }
        public Nullable<System.DateTime> WEB_MOD_TS { get; set; }
        public string WEB_MOD_BY { get; set; }
        public string SIMPLE_NAME { get; set; }
        public Nullable<int> LAST_REP_ORGA_ID { get; set; }
        public string ACCEPT_EMAIL_FLG { get; set; }
    
        public virtual ICollection<PERSON_IN_LIST> PERSON_IN_LIST { get; set; }
        public virtual ICollection<PERSON_LIST> PERSON_LIST { get; set; }
    }
}
