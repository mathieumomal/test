using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Controls
{
    public partial class HistoryControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            historyTable.MasterTableView.EditMode = Telerik.Web.UI.GridEditMode.EditForms;
        }

        public void LoadGrid(List<History> source)
        {
            historyTable.DataSource = source;
            historyTable.DataBind();
        }
    }
}