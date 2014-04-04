using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Controls
{
    public partial class ShareUrlControl : System.Web.UI.UserControl
    {
        #region parameters
        public string BaseAddress
        {
            get
            {
                return (ViewState["SURL_" + ID + "BaseAddress"] != null) ? (string)ViewState["SURL_" + ID + "BaseAddress"] : String.Empty;
            }
            set
            {
                ViewState["SURL_" + ID + "BaseAddress"] = value;
            }
        }
        public int ModuleId
        {
            get
            {
                return (ViewState["SURL_" + ID + "ModuleId"] != null) ? Convert.ToInt32(ViewState["SURL_" + ID + "ModuleId"]) : default(int);
            }
            set
            {
                ViewState["SURL_" + ID + "ModuleId"] = value;
            }
        }
        public int TabId { get; set; }

        public Dictionary<string, string> UrlParams
        {
            get
            {
                return (ViewState["SURL_" + ID + "UrlParams"] != null) ? (Dictionary<string, string>)ViewState["SURL_" + ID + "UrlParams"] : default(Dictionary<string, string>);
            }
            set
            {
                ViewState["SURL_" + ID + "UrlParams"] = value;
                UrlService urlService = new UrlService();
                txtLink.Text = urlService.GetPageIdAndFullAddressForModule(ModuleId, BaseAddress, UrlParams).Value;
            }
        }
        #endregion

        #region Control methods
        protected void Page_Load(object sender, EventArgs e)
        {
            UrlService urlService = new UrlService();
            txtLink.Text = urlService.GetPageIdAndFullAddressForModule(ModuleId, BaseAddress, UrlParams).Value;
        }

        protected void CheckBoxGetShortUrl_CheckedChanged(object sender, EventArgs e)
        {
            UrlService urlService = new UrlService();
            if (CheckBoxGetShortUrl.Checked)
            {
                txtLink.Text = urlService.CreateShortUrl(ModuleId, BaseAddress, UrlParams);
            }
            else
            {
                txtLink.Text = urlService.GetPageIdAndFullAddressForModule(ModuleId, BaseAddress, UrlParams).Value;
            }

        }
        #endregion
    }
}