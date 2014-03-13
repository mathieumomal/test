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
    public partial class Meeting
    {
        public int MTG_ID { get; set; }
        public string MTG_CATEG_CODE { get; set; }
        public string MTG_TYPE_CODE { get; set; }
        public string MTGS_CODE { get; set; }
        public string MTG_REF { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public Nullable<int> MTG_YEAR { get; set; }
        public Nullable<float> MTG_SEQNO { get; set; }
        public string MTG_TITLE { get; set; }
        public string OWNERSHIP_LEVEL { get; set; }
        public Nullable<System.DateTime> INVITATION_DATE { get; set; }
        public Nullable<System.DateTime> DEADLINE_DATE { get; set; }
        public Nullable<int> DURATION { get; set; }
        public Nullable<int> INVITED_NUMB { get; set; }
        public Nullable<int> ATTENDANT_NUMB { get; set; }
        public string LOCAL_FLG { get; set; }
        public Nullable<int> ORGA_ID { get; set; }
        public string LOC_ADDRESS { get; set; }
        public string LOC_ZIP { get; set; }
        public string LOC_CITY { get; set; }
        public string LOC_CTY_CODE { get; set; }
        public string MTGREF_MASK { get; set; }
        public Nullable<System.DateTime> START_REGISTRATION_DATE { get; set; }
        public string LOCATION_DETAILS_URL { get; set; }
        public Nullable<int> TB_ID { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public string ShortName { get; set; }
        public string MtgShortRef { get; set; }
    }
}
