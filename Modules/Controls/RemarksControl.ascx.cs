using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Controls
{
    public partial class RemarksControl : System.Web.UI.UserControl
    {
        public event EventHandler AddRemarkHandler;

        protected void Page_Load(object sender, EventArgs e)
        {
            releaseDetailTable.MasterTableView.EditMode = Telerik.Web.UI.GridEditMode.EditForms;
        }

        protected void btnAddRemark_Click(object sender, EventArgs e)
        {
            AddRemarkHandler(sender, e);
        }

        public void LoadGrid(List<Remark> source)
        {
            releaseDetailTable.DataSource = source;
            releaseDetailTable.DataBind();
        }
    }
}