using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Remarks Control Component
    /// </summary>
    public partial class RemarksControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_REMARKS_GRID_DATA = "RemarksGridData";
        private const string CONST_PK_REMARKID = "Pk_RemarkId";
        private const string CONST_ISPUBLIC = "IsPublic";
        private const int CONST_MIN_SCROLL_HEIGHT = 100;

        #endregion

        #region Public Properties

        public event EventHandler AddRemarkHandler;
        public bool IsEditMode { get; set; }
        public bool HidePrivateRemarks { get; set; }
        public string RemarkText { get { return this.txtAddRemark.Text; } }
        public int ScrollHeight { get; set; }
        public List<Remark> DataSource 
        {
            get {
                if (ViewState[CONST_REMARKS_GRID_DATA] == null)
                    ViewState[CONST_REMARKS_GRID_DATA] = new List<Remark>();

                return (List<Remark>)ViewState[CONST_REMARKS_GRID_DATA];
            }
            set {
                ViewState[CONST_REMARKS_GRID_DATA] = value;
                remarksGrid.Rebind();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    GridTemplateColumn isPublicColumn = (GridTemplateColumn)remarksGrid.MasterTableView.GetColumn(CONST_ISPUBLIC);
                    isPublicColumn.Visible = false;

                    txtAddRemark.Visible = false;
                    btnAddRemark.Visible = false;
                }

                remarksGrid.ClientSettings.Scrolling.ScrollHeight = (ScrollHeight < CONST_MIN_SCROLL_HEIGHT) ? Unit.Pixel(CONST_MIN_SCROLL_HEIGHT) : Unit.Pixel(ScrollHeight);
            }
        }

        /// <summary>
        /// Click event for Add Remarks
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void btnAddRemark_Click(object sender, EventArgs e)
        {
            AddRemarkHandler(sender, e);
            this.txtAddRemark.Text = String.Empty;
        }

        /// <summary>
        /// Need Data Source Event of Remarks Grid
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void remarksGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            remarksGrid.DataSource = DataSource;
            legendLabel.Text = String.Format("Remarks ({0})", (HidePrivateRemarks) ? DataSource.FindAll(x => x.IsPublic == true).Count : DataSource.Count);
        }

        /// <summary>
        /// Item DataBound Event of Remarks Grid
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void remarksGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (HidePrivateRemarks)
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    if (!Convert.ToBoolean(item.GetDataKeyValue(CONST_ISPUBLIC)))
                    {
                        item.Display = false;
                    }
                }
            }
        }

        /// <summary>
        /// Grid Pre Render Event
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void remarksGrid_PreRender(object sender, System.EventArgs e)
        {
            ChangeGridToEditMode();
        }

        /// <summary>
        /// Remark Type drop down changed
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void rddlRemarkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadDropDownList dropdownlist = (RadDropDownList)sender;
            GridDataItem editedItem = (GridDataItem)dropdownlist.NamingContainer;

            List<Remark> dataSource = DataSource;
            Remark changedRow = dataSource.Find(x => x.Pk_RemarkId.ToString() == editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][CONST_PK_REMARKID].ToString());
            if (changedRow != null)
            {
                changedRow.IsPublic = Convert.ToBoolean(dropdownlist.SelectedValue);
                DataSource = dataSource;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Change all grid rows to Edit Mode
        /// </summary>
        private void ChangeGridToEditMode()
        {
            if (IsEditMode)
            {
                foreach (GridItem item in remarksGrid.MasterTableView.Items)
                {
                    if (item is GridEditableItem)
                    {
                        GridEditableItem editableItem = item as GridDataItem;
                        editableItem.Edit = true;
                    }
                }
                remarksGrid.Rebind();
            }
        }

        #endregion
    }
}