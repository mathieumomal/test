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
        public string BaseAddress { get; set; }
        public int ModuleId { get; set; }
        public int TabId { get; set; }
        public Dictionary<string, string> UrlParams { get; set; }
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