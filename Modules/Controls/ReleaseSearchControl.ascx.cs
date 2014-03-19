using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Controls
{
    public partial class ReleaseSearchControl : System.Web.UI.UserControl
    {
        public int Width { get; set; }
        public int DropDownWidth { get; set; }
        public List<int> SelectedReleaseIds { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rcbReleases.Width = Width;
                rcbReleases.DropDownWidth = DropDownWidth;
            }
        }
    }
}