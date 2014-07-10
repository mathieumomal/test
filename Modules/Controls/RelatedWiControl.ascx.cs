using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Related Work Items Control
    /// </summary>
    public partial class RelatedWiControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_MOD_WI_DATASOURCE = "Modified_RelatedWiGrid_DataSource";
        private const string CONST_RELATED_WI_GRID_DATA = "relatedWiGridData";
        private const string CONST_CREATIONDATE = "CreationDate";
        private const string CONST_DSID_KEY = "ETSI_DS_ID";

        private const int CONST_MIN_SCROLL_HEIGHT = 30;

        #endregion

        #region Properties

        public bool IsEditMode { get; set; }
        public int ScrollHeight { get; set; }
        public List<WorkItem> DataSource
        {
            get
            {
                if (ViewState[CONST_RELATED_WI_GRID_DATA] == null)
                    ViewState[CONST_RELATED_WI_GRID_DATA] = new List<WorkItem>();

                return ((List<WorkItem>)ViewState[CONST_RELATED_WI_GRID_DATA]).OrderByDescending(x => x.IsPrimary)
                                                                              .ThenBy(x => x.WorkplanId).ToList();
            }
            set
            {
                ViewState[CONST_RELATED_WI_GRID_DATA] = GetActualWorkItemsFromProxy(value);
                relatedWiGrid.Rebind();
            }
        }

        private List<WorkItem> modifiedDataSource
        {
            get
            {
                if (ViewState[CONST_MOD_WI_DATASOURCE] == null)
                    ViewState[CONST_MOD_WI_DATASOURCE] = new List<WorkItem>();

                return ((List<WorkItem>)ViewState[CONST_MOD_WI_DATASOURCE]).OrderByDescending(x => x.IsPrimary)
                                                                           .ThenBy(x => x.WorkplanId).ToList();
            }
            set
            {
                ViewState[CONST_MOD_WI_DATASOURCE] = GetActualWorkItemsFromProxy(value);
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
                    btnShowWiEditWindow.Visible = false;
                    ScrollHeight += 25;
                }
                else
                {
                    //set datasource only if the user has edit rights
                    relatedWiGrid_Edit.DataSource = DataSource;
                    relatedWiGrid_Search.DataSource = String.Empty;
                }

                //Set values of hiddn fields (IsPrimary & Associated Wis) from DataSoruce
                SetHiddenWisValue(DataSource);
                //SetHidPrimaryWi(DataSource);

                relatedWiGrid.DataSource = DataSource;
                relatedWiGrid.ClientSettings.Scrolling.ScrollHeight = (ScrollHeight < CONST_MIN_SCROLL_HEIGHT) ? Unit.Pixel(CONST_MIN_SCROLL_HEIGHT) : Unit.Pixel(ScrollHeight);
            }
        }

        /// <summary>
        /// Need Data Source Event of WI Grid
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void relatedWiGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            relatedWiGrid.DataSource = DataSource;
            legendLabel.Text = "Related Work Items";
        }

        /// <summary>
        /// Search WIs & bind to relatedWiGrid_Search Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchWi_Click(object sender, EventArgs e)
        {
            //Get the list of matching WIs
            IWorkItemService svc = ServicesFactory.Resolve<IWorkItemService>();
            var matchedWis = svc.GetWorkItemsBySearchCriteria(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), txtSearchText.Text);

            //modifiedDataSource is a list of WIs that are added/removed/modified in EDIT mode
            // - Remove those WIs from search result
            // - Else remove WIs that are alrey in assigned DataSource
            if (modifiedDataSource.Count > 0)
                matchedWis.Key.RemoveAll(x => modifiedDataSource.Any(y => y.Pk_WorkItemUid == x.Pk_WorkItemUid));
            else
                matchedWis.Key.RemoveAll(x => DataSource.Any(y => y.Pk_WorkItemUid == x.Pk_WorkItemUid));

            BindGrid(relatedWiGrid_Search, matchedWis.Key);
        }

        /// <summary>
        /// Add searched WIs to relatedWiGrid_Edit grid (temporary)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddWisToGrid_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hidSelectedWis.Value.Trim(',')))
            {
                int value;
                IWorkItemService svc = ServicesFactory.Resolve<IWorkItemService>();
                var matchedWis = svc.GetWorkItemsBySearchCriteria(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), txtSearchText.Text);

                //Get list of WIs added in the UI
                var selectedWiUid = hidSelectedWis.Value.Trim(',').Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                var systemWis = hidSystemWis.Value.Trim(',').Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();

                //Add WIs from DataSource/modifiedDataSource if missing in searched Wis
                if (modifiedDataSource.Count > 0)
                    matchedWis.Key.AddRange(modifiedDataSource.Where(x => !matchedWis.Key.Any(y => x.Pk_WorkItemUid == y.Pk_WorkItemUid)));
                else
                    matchedWis.Key.AddRange(DataSource.Where(x => !matchedWis.Key.Any(y => x.Pk_WorkItemUid == y.Pk_WorkItemUid)));

                var WiList = matchedWis.Key.Where(x => selectedWiUid.Any(y => x.Pk_WorkItemUid == y)).ToList();
                WiList.Where(x => systemWis.All(y => y != x.Pk_WorkItemUid)).ToList().ForEach(x => x.IsUserAddedWi = true);
                BindGrid(relatedWiGrid_Edit, WiList);

                hidSelectedWis.Value = hidSelectedWis.Value.Trim(',') + ",";
                modifiedDataSource = WiList;

                //Remove added WIs from the search Grid & bind
                matchedWis.Key.RemoveAll(x => selectedWiUid.Any(y => y == x.Pk_WorkItemUid) || DataSource.Any(z => z.Pk_WorkItemUid == x.Pk_WorkItemUid));
                BindGrid(relatedWiGrid_Search, matchedWis.Key);
            }
        }

        /// <summary>
        /// Remove WIs from relatedWiGrid_Edit Grid (temporary)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemoveWis_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int WiId;
            var WiIdStr = ((ImageButton)sender).CommandArgument;
            if (!string.IsNullOrEmpty(WiIdStr) && int.TryParse(((ImageButton)sender).CommandArgument, out WiId))
            {
                //If modifiedDataSource is set; then remove from it else take data from DataSource to modifiedDataSource & remove
                if (modifiedDataSource.Count > 0)
                {
                    var tempList = modifiedDataSource.ToList();
                    tempList.RemoveAll(x => x.Pk_WorkItemUid == WiId);
                    modifiedDataSource = tempList;

                    SetHiddenWisValue(modifiedDataSource);
                    BindGrid(relatedWiGrid_Edit, modifiedDataSource);
                }
                else
                {
                    var tempList = DataSource.ToList();
                    tempList.RemoveAll(x => x.Pk_WorkItemUid == WiId);
                    modifiedDataSource = tempList;

                    SetHiddenWisValue(modifiedDataSource);
                    BindGrid(relatedWiGrid_Edit, modifiedDataSource);
                }
            }

        }

        /// <summary>
        /// Save EDIT mode changes to DataSource(RelatedWiGrid)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddToRelatedWiGrid_Click(object sender, EventArgs e)
        {
            List<string> modWiIds = modifiedDataSource.Select(x => x.Pk_WorkItemUid.ToString()).ToList();
            List<string> hidWiIds = (string.IsNullOrEmpty(hidSelectedWis.Value)) ? new List<string>() : hidSelectedWis.Value.Trim(',').Split(',').ToList();

            if (modWiIds.OrderBy(x => x).SequenceEqual(hidWiIds.OrderBy(x => x)))
            {
                this.DataSource = modifiedDataSource;
                modifiedDataSource = null;
            }

            //Set Primary WI values
            if (String.IsNullOrEmpty(hidPrimaryWi.Value) || hidPrimaryWi.Value == "-1")
                this.DataSource.ForEach(x => x.IsPrimary = false);
            else
            {
                int WiId;
                if (int.TryParse(hidPrimaryWi.Value, out WiId))
                {
                    if (this.DataSource.Where(x => x.Pk_WorkItemUid == WiId).Count() > 0)
                    {
                        this.DataSource.ForEach(x => x.IsPrimary = false);
                        this.DataSource.First(x => x.Pk_WorkItemUid == WiId).IsPrimary = true;
                    }
                }
            }

            BindGrid(relatedWiGrid, this.DataSource);

            //Reset WorkItem search panel
            txtSearchText.Text = "";
            BindGrid(relatedWiGrid_Search, String.Empty);
        }

        /// <summary>
        /// Revert changes made in EDTI mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRevertChanges_Click(object sender, EventArgs e)
        {
            //Reset all to the original state
            modifiedDataSource = null;
            SetHiddenWisValue(DataSource);
            SetHidPrimaryWi(DataSource);

            BindGrid(relatedWiGrid_Edit, DataSource);
            BindGrid(relatedWiGrid_Search, String.Empty);
        }

        /// <summary>
        /// Set the IsPrimary row to selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void relatedWiGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //Select the row if the WI is a primary WorkItem
                GridDataItem item = (GridDataItem)e.Item;
                if (item["IsPrimary"].Text != null && item["IsPrimary"].Text.ToLower() == "true")
                {
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// Set the IsPrimary row to selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void relatedWiGrid_Edit_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //Select the row if the WI is a primary WorkItem
                GridDataItem item = (GridDataItem)e.Item;
                if (item["IsPrimary"].Text != null && item["IsPrimary"].Text.ToLower() == "true")
                {
                    item.Selected = true;
                }
                else if (hidPrimaryWi.Value != null && hidPrimaryWi.Value == item["UID"].Text)
                {
                    item.Selected = true;
                }


                //Hide the REMOVE button if the WI is a system assigned WI
                if (item["IsUserAddedWi"].Text != null && item["IsUserAddedWi"].Text.ToLower() == "false")
                {
                    ImageButton btn = (ImageButton)item["Delete"].FindControl("btnRemoveWis");
                    btn.Visible = false;
                }
            }
        }

        #endregion

        #region Helper methods

        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(CONST_DSID_KEY), out personID))
                    return personID;
            }
            return 0;
        }

        private void SetHiddenWisValue(List<WorkItem> WiList)
        {
            hidSelectedWis.Value = (WiList.Count > 0) ? (String.Join(",", WiList.Select(x => x.Pk_WorkItemUid).ToList()) + ",") : String.Empty;
            hidSystemWis.Value = (WiList.Count > 0) ? (String.Join(",", WiList.Where(x => x.IsUserAddedWi == false).Select(x => x.Pk_WorkItemUid).ToList()) + ",") : String.Empty;
        }

        private void SetHidPrimaryWi(List<WorkItem> list)
        {
            hidPrimaryWi.Value = (list.FirstOrDefault(x => x.IsPrimary == true) != null) ? list.FirstOrDefault(x => x.IsPrimary == true).Pk_WorkItemUid.ToString() : "-1";
        }

        private void BindGrid(RadGrid radGrid, Object obj)
        {
            radGrid.DataSource = obj;
            radGrid.DataBind();
        }

        /// <summary>
        /// Provide simplified workitem objects with required properties
        /// </summary>
        /// <param name="proxyWorkItems">List of workitems</param>
        /// <returns>List of workitems with simplified properties</returns>
        private List<WorkItem> GetActualWorkItemsFromProxy(List<WorkItem> proxyWorkItems)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            if ((proxyWorkItems != null) && (proxyWorkItems.Count > 0))
                proxyWorkItems.ForEach(x => workItems.Add(GetActualWorkItemFromProxy(x)));

            return workItems;
        }

        /// <summary>
        /// Provide simplified workitem object with required fields
        /// </summary>
        /// <param name="proxyWorkItem">Workitem</param>
        /// <returns>Workitem with required properties</returns>
        private WorkItem GetActualWorkItemFromProxy(WorkItem proxyWorkItem)
        {
            WorkItem workItem = new WorkItem()
            {
                Pk_WorkItemUid = proxyWorkItem.Pk_WorkItemUid,
                WiLevel = proxyWorkItem.WiLevel,
                Acronym = proxyWorkItem.Acronym,
                Name = proxyWorkItem.Name,
                IsPrimary = proxyWorkItem.IsPrimary
            };

            proxyWorkItem.WorkItems_ResponsibleGroups.ToList().ForEach(x => workItem.WorkItems_ResponsibleGroups.Add(new WorkItems_ResponsibleGroups() { Pk_WorkItemResponsibleGroups = x.Pk_WorkItemResponsibleGroups, ResponsibleGroup = x.ResponsibleGroup } ));

            return workItem;
        }

        #endregion
    }
}