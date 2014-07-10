using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;
using System.Web.UI;
using System.Reflection;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Remarks Control Component
    /// </summary>
    public partial class RemarksControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_REMARKS_GRID_DATA = "RemarksGridData";
        private const string CONST_REMARKS_USER_RIGHTS = "RemarksUserRights";
        private const string CONST_REMARK_TEXT = "RemarkText";
        private const string CONST_CREATIONDATE = "CreationDate";
        private const string CONST_PK_REMARKID = "Pk_RemarkId";
        private const string CONST_ISPUBLIC = "IsPublic";
        private const int CONST_MIN_SCROLL_HEIGHT = 100;

        #endregion

        #region Public Properties

        public event EventHandler AddRemarkHandler;
        public bool IsEditMode { get; set; }
        public UserRightsContainer UserRights
        {
            get
            {
                if (ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] == null)
                    ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] = new UserRightsContainer();

                return (UserRightsContainer)ViewState[ClientID + CONST_REMARKS_USER_RIGHTS];
            }
            set
            {
                ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] = value;
            }
        }
        public string RemarkText { get { return this.txtAddRemark.Text; } }
        public int ScrollHeight { get; set; }
        public List<Remark> DataSource
        {
            get
            {
                if (ViewState[ClientID + CONST_REMARKS_GRID_DATA] == null)
                    ViewState[ClientID + CONST_REMARKS_GRID_DATA] = new List<Remark>();

                return (List<Remark>)ViewState[ClientID + CONST_REMARKS_GRID_DATA];
            }
            set
            {

                ViewState[ClientID + CONST_REMARKS_GRID_DATA] = GetActualRemarksFromProxy(value);
                remarksGrid.DataSource = null;
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
            btnAddRemark.Enabled = false;
            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    GridTemplateColumn isPublicColumn = (GridTemplateColumn)remarksGrid.MasterTableView.GetColumn(CONST_ISPUBLIC);
                    isPublicColumn.Visible = false;

                    txtAddRemark.Visible = false;
                    btnAddRemark.Visible = false;
                }
            }
            remarksGrid.ClientSettings.Scrolling.ScrollHeight = (ScrollHeight < CONST_MIN_SCROLL_HEIGHT) ? Unit.Pixel(CONST_MIN_SCROLL_HEIGHT) : Unit.Pixel(ScrollHeight);
            txtAddRemark.Attributes.Add("onkeyup", String.Format("SetAddRemarkState{0}(); return false;", ClientID));

            btnAddRemark.Attributes.Add("onclick", "javascript:setAddingProgress" + this.ClientID + "(true);");
            remarksGrid.ClientSettings.ClientEvents.OnDataBound = "setAddingProgress" + this.ClientID + "(false)";
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
            var remarks = (UserRights.HasRight(Enum_UserRights.Remarks_ViewPrivate)) ? DataSource : DataSource.FindAll(x => x.IsPublic == true);
            remarksGrid.DataSource = remarks.OrderByDescending(x=>x.CreationDate);
            legendLabel.Text = String.Format("Remarks ({0})", (UserRights.HasRight(Enum_UserRights.Remarks_ViewPrivate)) ? DataSource.Count : DataSource.FindAll(x => x.IsPublic == true).Count);
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
            //Existing Records
            if ((changedRow != null) && (changedRow.Pk_RemarkId != default(int)))
            {
                changedRow.IsPublic = Convert.ToBoolean(dropdownlist.SelectedValue);
                DataSource = dataSource;
            }
            else //New Records
            {
                Remark newRow = dataSource.Find(x => (x.RemarkText == editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][CONST_REMARK_TEXT].ToString())
                                                  && (x.CreationDate.ToString() == editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][CONST_CREATIONDATE].ToString()));
                if (newRow != null)
                {
                    newRow.IsPublic = Convert.ToBoolean(dropdownlist.SelectedValue);
                    DataSource = dataSource;
                }
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


        private List<Remark> GetActualRemarksFromProxy(List<Remark> proxyRemarks)
        {
            List<Remark> remarks = new List<Remark>();
            if (proxyRemarks != null)
            {
                proxyRemarks.ForEach(x => remarks.Add(new Remark()
                    {
                        Pk_RemarkId = x.Pk_RemarkId,
                        Fk_PersonId = x.Fk_PersonId,
                        Fk_WorkItemId = x.Fk_WorkItemId,
                        Fk_VersionId = x.Fk_VersionId,
                        Fk_SpecificationId = x.Fk_SpecificationId,
                        Fk_ReleaseId = x.Fk_ReleaseId,
                        Fk_SpecificationReleaseId = x.Fk_SpecificationReleaseId,
                        Fk_CRId = x.Fk_CRId,
                        IsPublic = x.IsPublic,
                        CreationDate = x.CreationDate,
                        RemarkText = x.RemarkText,
                        PersonName = x.PersonName,
                    }));
            }
            return remarks;
        }
        #endregion
    }
}