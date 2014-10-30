using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tools.TmpDbDataAccess
{
    public interface ITmpDb
    {
        IDbSet<C2001_04_25_schedule> C2001_04_25_schedule { get; set; }
        IDbSet<C2001_11_06_filius_patris> C2001_11_06_filius_patris { get; set; }
        IDbSet<C2002_04_25_WPM_title_lines> C2002_04_25_WPM_title_lines { get; set; }
        IDbSet<C2002_08_06_CR_implementation_errors> C2002_08_06_CR_implementation_errors { get; set; }
        IDbSet<C2003_03_04_work_plan> C2003_03_04_work_plan { get; set; }
        IDbSet<C2003_06_25_schedule_essentials_table> C2003_06_25_schedule_essentials_table { get; set; }
        IDbSet<C2004_01_15_WID_history> C2004_01_15_WID_history { get; set; }
        IDbSet<C2005_06_15_TSGs> C2005_06_15_TSGs { get; set; }
        IDbSet<C2006_01_08_WI_rapporteurs> C2006_01_08_WI_rapporteurs { get; set; }
        IDbSet<C2006_03_17_tdocs> C2006_03_17_tdocs { get; set; }
        IDbSet<C2008_03_08_Specs_vs_WIs> C2008_03_08_Specs_vs_WIs { get; set; }
        IDbSet<C2009_06_11_CRs_to_WIs> C2009_06_11_CRs_to_WIs { get; set; }
        IDbSet<C2010_02_08_SpecXRef> C2010_02_08_SpecXRef { get; set; }
        IDbSet<cmtee_officers> cmtee_officers { get; set; }
        IDbSet<committee> committees { get; set; }
        IDbSet<CR_categories> CR_categories { get; set; }
        IDbSet<CR_status_values> CR_status_values { get; set; }
        IDbSet<jmm_spec_series> jmm_spec_series { get; set; }
        IDbSet<List_of_GSM___3G_CRs> List_of_GSM___3G_CRs { get; set; }
        IDbSet<Release> Releases { get; set; }
        IDbSet<Specs_GSM_3G> Specs_GSM_3G { get; set; }
        IDbSet<Specs_GSM_3G_release_info> Specs_GSM_3G_release_info { get; set; }
        IDbSet<wpm_spec_release_mapping> wpm_spec_release_mapping { get; set; }
        IDbSet<plenary_meetings_with_end_dates> plenary_meetings_with_end_dates { get; set; }
        IDbSet<LSs_importedSnapshot> LSs_importedSnapshot { get; set; }

        void SetDetached(object elt);
    }
}
