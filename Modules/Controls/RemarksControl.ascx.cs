using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    public partial class RemarksControl : System.Web.UI.UserControl
    {
        public event EventHandler AddRemarkHandler;
        public bool IsEditMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsEditMode = false;
            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    GridEditCommandColumn editButtonColumn = (GridEditCommandColumn)releaseDetailTable.MasterTableView.GetColumn("EditCommandColumn");
                    editButtonColumn.Visible = false;

                    GridTemplateColumn isPublicColumn = (GridTemplateColumn)releaseDetailTable.MasterTableView.GetColumn("IsPublic");
                    isPublicColumn.Visible = false;

                    txtAddRemark.Visible = false;
                    btnAddRemark.Visible = false;
                }
            }
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