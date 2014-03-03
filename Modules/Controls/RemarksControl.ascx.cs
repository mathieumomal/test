using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Remarks Control Component
    /// </summary>
    public partial class RemarksControl : System.Web.UI.UserControl
    {
        #region Public Properties

        public event EventHandler AddRemarkHandler;
        public bool IsEditMode { get; set; }

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
        }

        /// <summary>
        /// Grid Pre Render Event
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void releaseDetailGrid_PreRender(object sender, System.EventArgs e)
        {
            if ((!IsPostBack) && (IsEditMode))
            {
                foreach (GridItem item in releaseDetailGrid.MasterTableView.Items)
                {
                    if (item is GridEditableItem)
                    {
                        GridEditableItem editableItem = item as GridDataItem;
                        editableItem.Edit = true;
                    }
                }
                releaseDetailGrid.Rebind();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load Remarks Grid
        /// </summary>
        /// <param name="source">List of Remarks</param>
        public void LoadGrid(List<Remark> source)
        {
            releaseDetailGrid.DataSource = source;
        }

        #endregion
    }
}