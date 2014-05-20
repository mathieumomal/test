using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationVersionListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SPECIFICATION_GRID_DATA = "SpecificationListControlData_{0}";
        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";

        #endregion

        #region Public Properties
        public bool IsEditMode { get; set; }
        public bool IsParentList { get; set; }
        public string SelectedTab
        {
            get
            {
                if (ViewState[CONST_SELECTED_TAB] == null)
                    return string.Empty;
                else
                    return ViewState[CONST_SELECTED_TAB].ToString();

            }
            set
            {
                ViewState[CONST_SELECTED_TAB] = value;
            }
        }
        public List<SpecVersion> DataSource
        {
            set;
            get;
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            specificationsVersionGrid.DataSource = DataSource;
            specificationsVersionGrid.DataBind();

            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                }
                else
                {
                }

            }
        }

        protected void specificationsVersionGrid_PreRender(object sender, System.EventArgs e)
        {
            //ChangeGridToEditMode();
        }

        protected void specificationsVersionGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            specificationsVersionGrid.DataSource = DataSource;
        }

        #endregion

        protected void specificationsVersionGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                if (IsEditMode && item["SpecificationActions"].FindControl("btnRemoveSpec") != null)
                    ((ImageButton)item["SpecificationActions"].FindControl("btnRemoveSpec")).Visible = true;
            }
        }

    }
}