using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class ReleaseHeaderControl : System.Web.UI.UserControl
    {
        #region Public Properties

        public bool IsEditMode { get; set; }
        public Release ReleaseDataSource
        {
            set;
            get;
        }
        public Specification SpecificationDataSource
        {
            set;
            get;
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            lblReleaseName.Text = ReleaseDataSource.Name;
            lblStatus.Text = string.Format("({0})", SpecificationDataSource.Status);
        }

        #endregion
    }
}