﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.Tools.TmpDbDataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TmpDB : DbContext, ITmpDb
    {
        public TmpDB()
            : base("name=TmpDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public IDbSet<C2001_04_25_schedule> C2001_04_25_schedule { get; set; }
        public IDbSet<C2001_11_06_filius_patris> C2001_11_06_filius_patris { get; set; }
        public IDbSet<C2002_04_25_WPM_title_lines> C2002_04_25_WPM_title_lines { get; set; }
        public IDbSet<C2002_08_06_CR_implementation_errors> C2002_08_06_CR_implementation_errors { get; set; }
        public IDbSet<C2003_03_04_work_plan> C2003_03_04_work_plan { get; set; }
        public IDbSet<C2003_06_25_schedule_essentials_table> C2003_06_25_schedule_essentials_table { get; set; }
        public IDbSet<C2004_01_15_WID_history> C2004_01_15_WID_history { get; set; }
        public IDbSet<C2005_06_15_TSGs> C2005_06_15_TSGs { get; set; }
        public IDbSet<C2006_01_08_WI_rapporteurs> C2006_01_08_WI_rapporteurs { get; set; }
        public IDbSet<C2006_03_17_tdocs> C2006_03_17_tdocs { get; set; }
        public IDbSet<C2008_03_08_Specs_vs_WIs> C2008_03_08_Specs_vs_WIs { get; set; }
        public IDbSet<C2009_06_11_CRs_to_WIs> C2009_06_11_CRs_to_WIs { get; set; }
        public IDbSet<C2010_02_08_SpecXRef> C2010_02_08_SpecXRef { get; set; }
        public IDbSet<cmtee_officers> cmtee_officers { get; set; }
        public IDbSet<committee> committees { get; set; }
        public IDbSet<CR_categories> CR_categories { get; set; }
        public IDbSet<CR_status_values> CR_status_values { get; set; }
        public IDbSet<jmm_spec_series> jmm_spec_series { get; set; }
        public IDbSet<List_of_GSM___3G_CRs> List_of_GSM___3G_CRs { get; set; }
        public IDbSet<Release> Releases { get; set; }
        public IDbSet<Specs_GSM_3G> Specs_GSM_3G { get; set; }
        public IDbSet<Specs_GSM_3G_release_info> Specs_GSM_3G_release_info { get; set; }
        public IDbSet<wpm_spec_release_mapping> wpm_spec_release_mapping { get; set; }
    }
}
