using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private const string CONST_REMARKTEXT = "RemarkText";

        #endregion

        #region Public Properties

        public event EventHandler AddRemarkHandler;
        public bool IsEditMode { get; set; }
        public string RemarkText { get { return this.txtAddRemark.Text; } }
        public List<Remark> DataSource 
        {
            get {
                if (ViewState[CONST_REMARKS_GRID_DATA] == null)
                    ViewState[CONST_REMARKS_GRID_DATA] = new List<Remark>();

                return (List<Remark>)ViewState[CONST_REMARKS_GRID_DATA];
            }
            set {
                ViewState[CONST_REMARKS_GRID_DATA] = value;
                releaseDetailGrid.Rebind();
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
                    GridEditCommandColumn gridEditCommandColumn = (GridEditCommandColumn)releaseDetailGrid.MasterTableView.GetColumn("EditCommandColumn");
                    gridEditCommandColumn.Visible = false;

                    GridTemplateColumn isPublicColumn = (GridTemplateColumn)releaseDetailGrid.MasterTableView.GetColumn("IsPublic");
                    isPublicColumn.Visible = false;

                    txtAddRemark.Visible = false;
                    btnAddRemark.Visible = false;
                }
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
        protected void releaseDetailGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            releaseDetailGrid.DataSource = DataSource;
        }

        /// <summary>
        /// Update Command Event of Remarks Grid
        /// </summary>
        /// <param name="source">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void releaseDetailGrid_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;

            List<Remark> dataSource = DataSource;
            Remark changedRow = dataSource.Find(x => x.Pk_RemarkId.ToString() == editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][CONST_PK_REMARKID].ToString());
            if (changedRow == null)
            {
                e.Canceled = true;
                return;
            }

            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);
            try
            {
                changedRow.IsPublic = Convert.ToBoolean(newValues[CONST_ISPUBLIC]);
                changedRow.RemarkText = newValues[CONST_REMARKTEXT].ToString();
                DataSource = dataSource;
            }
            catch (Exception)
            {
                e.Canceled = true;
            }
        }

        #endregion
    }
}