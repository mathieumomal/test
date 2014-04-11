using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Controls
{
    public partial class FullView : System.Web.UI.UserControl
    {
        public int ModuleId { get; set; }
        public int TabId { get; set; }
        public string BaseAddress { get; set; }
        public Dictionary<string, string> UrlParams { get; set; }

        /// <summary>
        /// On page Load, display the link if:
        /// - module Id is not null
        /// - TabId is not null
        /// - the TabId differs from the one stored in database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Display();
        }

        public void Display()
        {
            lnkFullView.Visible = false;
            if (ModuleId != default(int) && TabId != default(int))
            {
                // Create a new service
                var urlService = ServicesFactory.Resolve<IUrlService>();
                var result = urlService.GetPageIdAndFullAddressForModule(ModuleId, BaseAddress, UrlParams);

                if (result.Key != 0 && result.Key != TabId)
                {
                    lnkFullView.NavigateUrl = result.Value;
                    lnkFullView.Visible = true;
                }
            }
        }
    }
}