//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Tools.TmpDbDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Release
    {
        public string Release_code { get; set; }
        public string Release_description { get; set; }
        public string Release_short_description { get; set; }
        public string version_2g { get; set; }
        public string version_3g { get; set; }
        public Nullable<short> sort_order { get; set; }
        public Nullable<bool> defunct { get; set; }
        public string remarks { get; set; }
        public string wpm_code_2g { get; set; }
        public string wpm_code_3g { get; set; }
        public string freeze_meeting { get; set; }
        public Nullable<int> PROJECT_ID { get; set; }
        public Nullable<System.DateTime> rel_proj_start { get; set; }
        public Nullable<System.DateTime> rel_proj_end { get; set; }
        public string ITUR_code { get; set; }
        public string version_2g_dec { get; set; }
        public string version_3g_dec { get; set; }
        public string previousRelease { get; set; }
        public int Row_id { get; set; }
        public string Stage1_freeze { get; set; }
        public string Stage2_freeze { get; set; }
        public string Stage3_freeze { get; set; }
        public string Protocols_freeze { get; set; }
        public string Closed { get; set; }
    }
}
