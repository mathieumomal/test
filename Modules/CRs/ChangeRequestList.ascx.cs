using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;
using System.Text;

namespace Etsi.Ultimate.Module.CRs
{
    public partial class ChangeRequestList : PortalModuleBase
    {
        #region constants

        private const string DsIdKey = "ETSI_DS_ID";
        private const string VsCrSearchObj = "VS_CR_SEARCH_OBJ";
        private const string PkEnumchangerequeststatus = "Pk_EnumChangeRequestStatus";
        private const string Description = "Description";

        //Send CRs to CR-Pack const
        private const string CrPackId = "pk_Contribution";
        private const string CrPackValue = "uid";
        private const string ErrorPicture = "/images/red-error.gif";
        private const string SuccessPicture = "/images/green-ok.gif";
        private KeyValuePair<int, int> _sendCrsToCrPackAlertDim = new KeyValuePair<int, int>(400, 100);

        //Pagination variables
        private const int DefaultItemsPerPage = 200;
        private const int DefaultItemsPerPageWithSpecNumberAsFilter = 3000;
        private readonly List<KeyValuePair<string, int>> _possibleNumberOfItemsPerPage = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("200", 200),
            new KeyValuePair<string, int>("1000", 1000),
            new KeyValuePair<string, int>("3000", 3000)
        };

        #endregion

        #region properties

        protected FullView CrFullView;
        protected ShareUrlControl CrShareUrl;
        protected ReleaseSearchControl ReleaseSearchControl;
        private bool _isUrlSearch;
        private VersionForCrListFacade _versionForCrListFacade;

        /// <summary>
        /// Gets or sets the search object.
        /// </summary>
        private ChangeRequestsSearch SearchObj
        {
            get
            {
                return (ChangeRequestsSearch)ViewState[VsCrSearchObj];
            }
            set
            {
                ViewState[VsCrSearchObj] = value;
            }
        }

        private const string CrTbId = "CR_TB_ID";

        /// <summary>
        /// Store tbId from URL
        /// </summary>
        private string TbId
        {
            get
            {
                if (ViewState[CrTbId] == null)
                    ViewState[CrTbId] = "";
                return (string)ViewState[CrTbId];
            }
            set
            {
                ViewState[CrTbId] = value;
            }
        }

        /// <summary>
        /// First page load flag
        /// </summary>
        private bool FirstLoad { get; set; }

        private int SelectedItemsPerPage
        {
            get
            {
                return Convert.ToInt32(SelectPageSize.SelectedValue);
            }
            set
            {
                var item = SelectPageSize.Items.FindItemByValue(value.ToString());
                if (item != null)
                    item.Selected = item.Checked = true;
            }
        }

        #endregion

        #region events

        /// <summary>
        /// Page load method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || !crList.Visible)
            {
                FirstLoad = true;
                ReleaseSearchControl.IsLoadingNeeded = !crList.Visible;
                crList.Visible = true;

                //Init page size component
                InitPageSizeComponent();

                GetRequestParameters();
                SearchObj = new ChangeRequestsSearch();
                SearchObj.PageSize = rgCrList.PageSize = DefaultItemsPerPage;

                //Load CR Statuses
                LoadCrStatuses();

                //Check rights
                GetUserRightsAndAdaptUi();

                ReleaseSearchControl.Load += releaseSearchControl_Load;

                var searchObjFromCookie = CookiesHelper.GetCookie<ChangeRequestsSearch>(Page.Request, ConfigVariables.CookieNameCrsList);
                if (!_isUrlSearch && searchObjFromCookie != null && searchObjFromCookie.GetType() == typeof(ChangeRequestsSearch))
                {
                    SearchObj = searchObjFromCookie;
                    LoadControlsFromSearchObj();
                }
            }
            else
            {
                FirstLoad = false; 
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            _isUrlSearch = false;
            SearchObj.SpecificationNumber = txtSpecificationNumber.Text;
            SearchObj.MeetingIds = GetSelectedMeetingIds();
            SearchObj.WorkItemIds = GetSelectedWorkItemIds();
            SearchObj.ReleaseIds = ReleaseSearchControl.SelectedReleasesIds;
            SearchObj.WgStatusIds = GetSelectedWgStatusIds();
            SearchObj.TsgStatusIds = GetSelectedTsgStatusIds();
            SearchObj.VersionId = 0;//By default because we don't have any UI filters for the version for the moment

            //If specNumber exists, system should automaticaly change item per page to a default value (max)
            if (!string.IsNullOrEmpty(SearchObj.SpecificationNumber))
                SelectedItemsPerPage = DefaultItemsPerPageWithSpecNumberAsFilter;
            SearchObj.PageSize = SelectedItemsPerPage;

            rgCrList.CurrentPageIndex = 0;
            rgCrList.Rebind();

            // Save SearchObj into cookie
            CookiesHelper.SetCookie(Page.Response, ConfigVariables.CookieNameCrsList, SearchObj);
        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RgCrList_NeedDataSource(object o, GridNeedDataSourceEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// On items loaded
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RgCrList_ItemDataBound(object o, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                var currentCr = (ChangeRequestListFacade)e.Item.DataItem;

                if (currentCr != null)
                {
                    var specLink = (HyperLink)dataItem["SpecNumber"].Controls[0];
                    if (currentCr.SpecNumber != null)
                        specLink.Text = currentCr.SpecNumber.ToString(CultureInfo.InvariantCulture);
                    if (currentCr.SpecId != 0)
                        specLink.NavigateUrl = "javascript:OpenSpecDetailsPage('" + String.Format(ConfigVariables.SpecificationDetailsUrl, currentCr.SpecId) + "','Specification-" + currentCr.SpecId + "')";
                    else
                        specLink.Enabled = false;

                    var releaseLink = (HyperLink)dataItem["TargetRelease"].Controls[0];
                    if (currentCr.TargetRelease != null)
                        releaseLink.Text = currentCr.TargetRelease.ToString(CultureInfo.InvariantCulture);
                    if (currentCr.TargetReleaseId != 0)
                        releaseLink.NavigateUrl = "javascript:openRelease('" + String.Format(ConfigVariables.ReleaseDetailsUrl, currentCr.TargetReleaseId) + "','" + currentCr.TargetReleaseId+"')";
                    else
                        releaseLink.Enabled = false;

                    if (currentCr.WgTdocNumber != null)
                    {
                        var wgTdocLink = (HyperLink)dataItem["WgTdocNumber"].Controls[0];
                        wgTdocLink.Text = currentCr.WgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        wgTdocLink.CssClass = "hyperlinkStyle";
                        wgTdocLink.Attributes.Add("onclick", "javascript:openTdoc('"+String.Format(ConfigVariables.TdocDetailsUrl, currentCr.WgTdocNumber)+"','"+currentCr.WgTdocNumber+"')");
                        if (currentCr.WgTdocNumber.Equals("-"))
                            wgTdocLink.Enabled = false;
                    }
                    
                    if (currentCr.TsgTdocNumber != null)
                    {
                        var tsgTdocLink = (HyperLink)dataItem["TsgTdocNumber"].Controls[0];
                        tsgTdocLink.Text = currentCr.TsgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        tsgTdocLink.CssClass = "hyperlinkStyle";
                        tsgTdocLink.Attributes.Add("onclick","javascript:openTdoc('" + String.Format(ConfigVariables.TdocDetailsUrl, currentCr.TsgTdocNumber) + "','" + currentCr.TsgTdocNumber + "')");
                        if (currentCr.TsgTdocNumber.Equals("-"))
                            tsgTdocLink.Enabled = false;
                    }
                    
                    var newVersionLink = (HyperLink)dataItem["NewVersion"].Controls[0];
                    if(currentCr.NewVersion != null)
                        newVersionLink.Text = currentCr.NewVersion.ToString(CultureInfo.InvariantCulture);
                    if(currentCr.NewVersionPath != null)
                        newVersionLink.NavigateUrl = currentCr.NewVersionPath;
                    else
                        newVersionLink.Enabled = false;

                    var impactedVersionLink = (HyperLink)dataItem["ImpactedVersion"].Controls[0];
                    if (currentCr.ImpactedVersion != null)
                        impactedVersionLink.Text = currentCr.ImpactedVersion.ToString(CultureInfo.InvariantCulture);
                    if (currentCr.ImpactedVersionPath != null)
                        impactedVersionLink.NavigateUrl = currentCr.ImpactedVersionPath;
                    else
                        impactedVersionLink.Enabled = false;

                    var crSelection = dataItem["CrSelection"].Controls[0];
                    var checkbox = crSelection.FindControl("CrSelectionCheckBox") as CheckBox;
                    if (checkbox != null)
                    {
                        if (currentCr.ShouldBeLinkToACrPack)
                        {
                            checkbox.Enabled = true;
                        }
                        else
                        {
                            checkbox.Enabled = false;
                            checkbox.CssClass = "disabledTransparent";
                            checkbox.ToolTip = Localization.CR_Cannot_Be_Send_To_CRPack;
                        }
                    }
                }   
            }
        }

        /// <summary>
        /// Handles the Load event of the releaseSearchControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void releaseSearchControl_Load(object sender, EventArgs e)
        {
            if (FirstLoad && SearchObj != null && SearchObj.ReleaseIds != null &&
                    SearchObj.ReleaseIds.Count > 0)
            {
                ReleaseSearchControl.SelectedReleasesIds = SearchObj.ReleaseIds;
            }
            else if (SearchObj != null && SearchObj.ReleaseIds != null && SearchObj.ReleaseIds.Count == 0)
            {
                SearchObj.ReleaseIds = ReleaseSearchControl.SelectedReleasesIds;
            }

            LoadData();
        }

        /// <summary>
        /// Page size change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectPageSize_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SearchObj.PageSize = Convert.ToInt32(e.Value);
            SearchObj.SkipRecords = 0;
            rgCrList.CurrentPageIndex = 0;
            rgCrList.Rebind();
        }
        #endregion

        #region Send CRs to CR-Pack events and utils
        /// <summary>
        /// Send CRs to CR-Pack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendToCrPackBtn_OnClick(object sender, EventArgs e)
        {
            if (!CheckIfSendCrsToCrPackIsPossible())
            {
                RadWindowMgr.RadAlert(Localization.CR_Crs_Or_CrPack_NotSelected, _sendCrsToCrPackAlertDim.Key, _sendCrsToCrPackAlertDim.Value, Localization.CR_Send_Crs_To_CrPack, "", ErrorPicture);
                return;
            }
            //Get the list of selected CRs' ids
            var crsIds = new List<int>();
            foreach (GridDataItem item in rgCrList.MasterTableView.Items)
            {
                var checkbox = (CheckBox)item.FindControl("CrSelectionCheckBox");
                if (checkbox.Checked)
                {
                    int id;
                    int.TryParse(item.GetDataKeyValue("ChangeRequestId").ToString(), out id);
                    crsIds.Add(id);
                }
            }

            //Save Crs inside CR-pack
            var crService = ServicesFactory.Resolve<IChangeRequestService>();
            var response = crService.SendCrsToCrPack(
                GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()),
                crsIds,
                GetSelectedCrPack().Key);

            if (response.Result)
            {
                RadWindowMgr.RadAlert(String.Format(Localization.CR_Crs_Successfully_Moved_Inside_CrPack, GetSelectedCrPack().Value), _sendCrsToCrPackAlertDim.Key, _sendCrsToCrPackAlertDim.Value, Localization.CR_Send_Crs_To_CrPack, "", SuccessPicture);
            }
            else
            {
                RadWindowMgr.RadAlert(response.Report.ErrorList.FirstOrDefault(), _sendCrsToCrPackAlertDim.Key, _sendCrsToCrPackAlertDim.Value, Localization.CR_Send_Crs_To_CrPack, "", ErrorPicture);
            }

            //Reset selection and CR-pack field
            rdcbCrPack.Text = String.Empty;
            rdcbCrPack.ClearSelection();
            foreach (GridDataItem item in rgCrList.MasterTableView.Items)
            {
                var checkbox = (CheckBox)item.FindControl("CrSelectionCheckBox");
                if (checkbox.Checked)
                    checkbox.Checked = false;
            }

            //Refresh list of CRs
            LoadData();
        }

        /// <summary>
        /// When searching for a CR-Pack
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RdcbCrPack_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                int tbIdOutput = 0;
                if (!String.IsNullOrEmpty(TbId))
                    int.TryParse(TbId, out tbIdOutput);

                var svc = ServicesFactory.Resolve<IContributionService>();
                var crPacksFound = svc.GetCrPacksByTbIdAndKeywords(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), tbIdOutput, e.Text);

                rdcbCrPack.DataSource = crPacksFound.Result;
                rdcbCrPack.DataTextField = CrPackValue;
                rdcbCrPack.DataValueField = CrPackId;
                rdcbCrPack.DataBind();
            }
        }

        /// <summary>
        /// Check if Send Crs To CR-pack is possible cause by the fact that some CRs are selected and a CR-Pack as well
        /// </summary>
        /// <returns></returns>
        private bool CheckIfSendCrsToCrPackIsPossible()
        {
            //Test if CRs selected
            var atLeastOneCrSelected = false;
            foreach (GridDataItem item in rgCrList.MasterTableView.Items)
            {
                var checkbox = (CheckBox)item.FindControl("CrSelectionCheckBox");
                if (checkbox.Checked)
                {
                    atLeastOneCrSelected = true;
                }
            }

            //Enabled or disabled Send to CR Pack button
            return GetSelectedCrPack().Key != 0 && atLeastOneCrSelected;
        }

        /// <summary>
        /// Get selected CR-pack (Id and UID)
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<int, string> GetSelectedCrPack()
        {
            if (!String.IsNullOrEmpty(rdcbCrPack.SelectedValue))
            {
                return new KeyValuePair<int, string>(Convert.ToInt32(rdcbCrPack.SelectedValue), rdcbCrPack.Text);
            }
            return new KeyValuePair<int, string>(0, String.Empty);
        } 

        #endregion

        #region Load data

        /// <summary>
        /// Load data in the RadGrid each time needed
        /// </summary>
        private void LoadData()
        {
            if (FirstLoad && !_isUrlSearch)
            {
                rgCrList.DataSource = new List<ChangeRequestListFacade>();
            }
            else
            {
                if (_isUrlSearch)
                    LoadUrlData();
                else
                    SearchObj.SkipRecords = rgCrList.CurrentPageIndex*SearchObj.PageSize;

                var crSvc = ServicesFactory.Resolve<IChangeRequestService>();
                var response =
                    crSvc.GetChangeRequests(
                        GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), SearchObj);

                if (response.Report.GetNumberOfErrors() != 0)
                    return;

                rgCrList.VirtualItemCount = response.Result.Value;

                if (SearchObj.PageSize > rgCrList.VirtualItemCount && rgCrList.VirtualItemCount > 0)
                {
                    rgCrList.PageSize = rgCrList.VirtualItemCount;
                }
                else
                {
                    rgCrList.PageSize = SearchObj.PageSize;
                }
                rgCrList.PageSize = SearchObj.PageSize;
                rgCrList.CurrentPageIndex = SearchObj.SkipRecords / SearchObj.PageSize;
                rgCrList.DataSource = response.Result.Key;

                ManageShareUrl();
                ManageFullView();

                SetSearchLabel();
            }
        }

        /// <summary>
        /// Loads the URL data.
        /// </summary>
        private void LoadUrlData()
        {
            //[1] Specification Number
            if (!String.IsNullOrEmpty(Request.QueryString["specnumber"]))
                txtSpecificationNumber.Text = SearchObj.SpecificationNumber = Request.QueryString["specnumber"];

            //[1 bis] Spec version id
            if (!String.IsNullOrEmpty(Request.QueryString["versionId"]))
            {
                int versionId;
                if (int.TryParse(Request.QueryString["versionId"], out versionId))
                {
                    SearchObj.VersionId = versionId;
                    var specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                    _versionForCrListFacade = specVersionSvc.GetVersionNumberWithSpecNumberByVersionId(
                        GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()),
                        versionId).Result;
                    txtSpecificationNumber.Text = _versionForCrListFacade.SpecNumber;
                }
                    
            }

            //[2] Releases (if release contains 0 then all releases will be selected)
            if (!String.IsNullOrEmpty(Request.QueryString["release"]))
            {
                ReleaseSearchControl.SelectedReleasesWithKeywords = Request.QueryString["release"];
                SearchObj.ReleaseIds = ReleaseSearchControl.SelectedReleasesIds;
            }

            //[3] WG Statuses
            if (!String.IsNullOrEmpty(Request.QueryString["wgstatus"]))
            {
                var wgStatuses = Request.QueryString["wgstatus"].Split(',').ToList();
                var wgStatusIds = new List<int>();
                wgStatuses.ForEach(x =>
                {
                    int wgStatusId;
                    if (int.TryParse(x, out wgStatusId))
                        wgStatusIds.Add(wgStatusId);
                });
                SearchObj.WgStatusIds = wgStatusIds;
                rcbWgStatus.Items.ToList().ForEach(x => { x.Checked = wgStatusIds.Contains(Convert.ToInt32(x.Value)); });
            }

            //[4] TSG Statuses
            if (!String.IsNullOrEmpty(Request.QueryString["tsgstatus"]))
            {
                var tsgStatuses = Request.QueryString["tsgstatus"].Split(',').ToList();
                var tsgStatusIds = new List<int>();
                tsgStatuses.ForEach(x =>
                {
                    int tsgStatusId;
                    if (int.TryParse(x, out tsgStatusId))
                        tsgStatusIds.Add(tsgStatusId);
                });
                SearchObj.TsgStatusIds = tsgStatusIds;
                rcbTsgStatus.Items.ToList().ForEach(x => { x.Checked = tsgStatusIds.Contains(Convert.ToInt32(x.Value)); });
            }

            //[5] Meetings
            if (!String.IsNullOrEmpty(Request.QueryString["meeting"]))
            {
                var meetings = Request.QueryString["meeting"].Split(',').ToList();
                var meetingIds = new List<int>();
                meetings.ForEach(x =>
                {
                    int meetingId;
                    if (int.TryParse(x, out meetingId))
                        meetingIds.Add(meetingId);
                });
                SearchObj.MeetingIds = meetingIds;

                var meetingSvc = ServicesFactory.Resolve<IMeetingService>();
                var meetingsList = meetingSvc.GetMeetingsByIds(meetingIds);

                meetingIds.ForEach(x =>
                {
                    var meeting = meetingsList.Find(m => m.MTG_ID == x);
                    if (meeting != null)
                        racMeeting.Entries.Add(new AutoCompleteBoxEntry(meeting.MtgDdlText, x.ToString(CultureInfo.InvariantCulture)));
                });
            }

            //[6] Work Items
            if (!String.IsNullOrEmpty(Request.QueryString["workitem"]))
            {
                var workitems = Request.QueryString["workitem"].Split(',').ToList();
                var workItemIds = new List<int>();
                workitems.ForEach(x =>
                {
                    int workItemId;
                    if (int.TryParse(x, out workItemId))
                        workItemIds.Add(workItemId);
                });
                SearchObj.WorkItemIds = workItemIds;

                var workItemSvc = ServicesFactory.Resolve<IWorkItemService>();
                var workItemsList = workItemSvc.GetWorkItemByIds(0, workItemIds).Key;

                workItemIds.ForEach(x =>
                {
                    var workItem = workItemsList.Find(wi => wi.Pk_WorkItemUid == x);
                    if(workItem != null)
                        racWorkItem.Entries.Add(new AutoCompleteBoxEntry(workItem.WorkItemDdlText, x.ToString(CultureInfo.InvariantCulture)));
                });
            }

            //[7] Page Index
            int pageIndex;
            if (!String.IsNullOrEmpty(Request.QueryString["pageindex"]) && (int.TryParse(Request.QueryString["pageindex"], out pageIndex)))
                SearchObj.SkipRecords = pageIndex * SearchObj.PageSize;
        }

        /// <summary>
        /// Loads the cr statuses.
        /// </summary>
        private void LoadCrStatuses()
        {
            var svcChangeRequestService = ServicesFactory.Resolve<IChangeRequestService>();
            var svcResponse = svcChangeRequestService.GetChangeRequestStatuses();

            if (!svcResponse.Key) return;

            var enumNoneStatus = new Enum_ChangeRequestStatus { Pk_EnumChangeRequestStatus = 0, Description = "None" };
            var dataSource = new List<Enum_ChangeRequestStatus>();
            dataSource.Add(enumNoneStatus);
            dataSource.AddRange(svcResponse.Value.OrderBy(x => x.Description));

            rcbWgStatus.DataValueField = PkEnumchangerequeststatus;
            rcbWgStatus.DataTextField = Description;
            rcbWgStatus.DataSource = dataSource;
            rcbWgStatus.DataBind();

            rcbTsgStatus.DataValueField = PkEnumchangerequeststatus;
            rcbTsgStatus.DataTextField = Description;
            rcbTsgStatus.DataSource = dataSource;
            rcbTsgStatus.DataBind();
        }

        /// <summary>
        /// Gets the selected meeting ids.
        /// </summary>
        /// <returns>List of meeting ids</returns>
        private List<int> GetSelectedMeetingIds()
        {
            var selectedMeetingIds = new List<int>();
            foreach (AutoCompleteBoxEntry entry in racMeeting.Entries)
            {
                int meetingId;
                if (int.TryParse(entry.Value, out meetingId))
                    selectedMeetingIds.Add(meetingId);
            }

            return selectedMeetingIds;
        }

        /// <summary>
        /// Gets the selected work item ids.
        /// </summary>
        /// <returns>List of work item ids</returns>
        private List<int> GetSelectedWorkItemIds()
        {
            var selectedWorkItemIds = new List<int>();
            foreach (AutoCompleteBoxEntry entry in racWorkItem.Entries)
            {
                int workItemId;
                if (int.TryParse(entry.Value, out workItemId))
                    selectedWorkItemIds.Add(workItemId);
            }

            return selectedWorkItemIds;
        }

        /// <summary>
        /// Gets the selected wg status ids.
        /// </summary>
        /// <returns>Selected wg status ids</returns>
        private List<int> GetSelectedWgStatusIds()
        {
            var selectedStatusIds = new List<int>();

            rcbWgStatus.CheckedItems.ToList().ForEach(x => {
                int statusId;
                if (int.TryParse(x.Value, out statusId))
                    selectedStatusIds.Add(statusId);
            });

            return selectedStatusIds;
        }

        /// <summary>
        /// Gets the selected tsg status ids.
        /// </summary>
        /// <returns>Selected tsg status ids</returns>
        private List<int> GetSelectedTsgStatusIds()
        {
            var selectedStatusIds = new List<int>();

            rcbTsgStatus.CheckedItems.ToList().ForEach(x =>
            {
                int statusId;
                if (int.TryParse(x.Value, out statusId))
                    selectedStatusIds.Add(statusId);
            });

            return selectedStatusIds;
        }

        /// <summary>
        /// Init page size component:
        /// - load default value 
        /// - add default value from the web.config
        /// - display data
        /// </summary>
        private void InitPageSizeComponent()
        {
            SelectPageSize.Items.AddRange(_possibleNumberOfItemsPerPage.Select(
                x => new RadComboBoxItem(x.Key, x.Value.ToString())));

            SelectedItemsPerPage = DefaultItemsPerPage;
        }

        /// <summary>
        /// Fills controls from the searchObj
        /// </summary>
        /// <returns></returns>
        private void LoadControlsFromSearchObj()
        {
            txtSpecificationNumber.Text = SearchObj.SpecificationNumber;
            SetSelectedMeetings(SearchObj.MeetingIds);
            SetSelectedWorkItems(SearchObj.WorkItemIds);
            SetSelectedValue(rcbWgStatus, SearchObj.WgStatusIds);
            SetSelectedValue(rcbTsgStatus, SearchObj.TsgStatusIds);
            SelectedItemsPerPage = SearchObj.PageSize;
        }

        private void SetSelectedValue(RadComboBox rcb, List<int> values)
        {
            if (values != null)
            {
                foreach (int val in values)
                {
                    RadComboBoxItem item = rcb.FindItemByValue(val.ToString());
                    if (item != null)
                        item.Checked = true;
                }
            }
        }

        private void SetSelectedMeetings(List<int> meetingIds)
        {
            if (meetingIds != null && meetingIds.Count > 0)
            {
                var meetingSvc = ServicesFactory.Resolve<IMeetingService>();
                var meetingsList = meetingSvc.GetMeetingsByIds(meetingIds);

                meetingIds.ForEach(x =>
                {
                    var meeting = meetingsList.Find(m => m.MTG_ID == x);
                    if (meeting != null)
                        racMeeting.Entries.Add(new AutoCompleteBoxEntry(meeting.MtgDdlText,
                            x.ToString(CultureInfo.InvariantCulture)));
                });
            }
        }

        private void SetSelectedWorkItems(List<int> workItemIds)
        {
            if (workItemIds != null && workItemIds.Count > 0)
            {
                var workItemSvc = ServicesFactory.Resolve<IWorkItemService>();
                var workItemsList = workItemSvc.GetWorkItemByIds(0, workItemIds).Key;

                workItemIds.ForEach(x =>
                {
                    var workItem = workItemsList.Find(wi => wi.Pk_WorkItemUid == x);
                    if (workItem != null)
                        racWorkItem.Entries.Add(new AutoCompleteBoxEntry(workItem.WorkItemDdlText,
                            x.ToString(CultureInfo.InvariantCulture)));
                });
            }
        }

        #endregion

        #region utils

        /// <summary>
        /// Get User rights and adapt UI accordingly
        /// </summary>
        private void GetUserRightsAndAdaptUi()
        {
            var personService = ServicesFactory.Resolve<IPersonService>();
            var userRights = personService.GetRights(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));

            //Check for Multiple TB selection
            var isMultipleTbsSelected = true;
            if (!String.IsNullOrEmpty(TbId))
            {
                int tbIdOutput;
                int.TryParse(TbId, out tbIdOutput);
                if (tbIdOutput != 0)
                    isMultipleTbsSelected = false;
            }

            //If user haven't 'Cr_Add_Crs_To_CrPack' right
            //- Not Display checkboxes to be able to select CRs
            //- Not Display "Send to CR-Pack" button
            //- Not Display Autocomplete CRPack textbox
            if (!userRights.HasRight(Enum_UserRights.Cr_Add_Crs_To_CrPack) || isMultipleTbsSelected)
            {
                rgCrList.MasterTableView.GetColumn("CrSelection").Display = false;
                SendToCrPackBtn.Visible = false;
                rdcbCrPack.Visible = false;
            }
        }

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="userInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo userInfo)
        {
            if (userInfo.UserID < 0)
                return 0;

            int personId;
            return Int32.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId) ? personId : 0;
        }

        /// <summary>
        /// Construct FullUrl
        /// </summary>
        private void ManageFullView()
        {
            CrFullView.ModuleId = ModuleId;
            CrFullView.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            CrFullView.BaseAddress = address;

            CrFullView.UrlParams = ManageUrlParams();
            CrFullView.Display();
        }

        /// <summary>
        /// Construct ShortUrl
        /// </summary>
        private void ManageShareUrl()
        {
            CrShareUrl.IsShortUrlChecked = false;
            CrShareUrl.ModuleId = ModuleId;
            CrShareUrl.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            CrShareUrl.BaseAddress = address;

            CrShareUrl.UrlParams = ManageUrlParams();
        }

        /// <summary>
        /// Generate Url parameters for Short/FullView Url
        /// </summary>
        /// <returns>Url parameters</returns>
        private Dictionary<string, string> ManageUrlParams()
        {
            var urlParams = new Dictionary<string, string>
            {
                {"q", "1"},
                {"specnumber", SearchObj.SpecificationNumber},
                {"release", ReleaseSearchControl.SelectedReleasesWithKeywords},
                {"wgstatus", String.Join(",", SearchObj.WgStatusIds)},
                {"tsgstatus", String.Join(",", SearchObj.TsgStatusIds)},
                {"meeting", String.Join(",", SearchObj.MeetingIds)},
                {"workitem", String.Join(",", SearchObj.WorkItemIds)},
                {"pageindex", (SearchObj.SkipRecords/SearchObj.PageSize).ToString(CultureInfo.InvariantCulture)}
            };

            return urlParams;
        }

        /// <summary>
        /// Extract query strings from Url
        /// </summary>
        private void GetRequestParameters()
        {
            _isUrlSearch = (Request.QueryString["q"] != null);
            TbId = Request.QueryString["tbid"];
        }

        /// <summary>
        /// Sets the search label.
        /// </summary>
        private void SetSearchLabel()
        {
            if (SearchObj != null)
            {
                var sb = new StringBuilder();

                if (SearchObj.VersionId != 0)
                    sb.Append(String.Format("{0}, Version({1}), ", _versionForCrListFacade.SpecNumber, _versionForCrListFacade.Version));
                else
                {
                    if (!String.IsNullOrEmpty(SearchObj.SpecificationNumber))
                        sb.Append(String.Format("{0}, ", SearchObj.SpecificationNumber));
                }

                var releaseText = ReleaseSearchControl.SearchString;
                sb.Append(String.Format("{0}, ", String.IsNullOrEmpty(releaseText) ? "Open Releases" : releaseText));

                if (SearchObj.WgStatusIds != null && SearchObj.WgStatusIds.Count > 0)
                    sb.Append(String.Format("WG Status({0}), ", String.Join(", ", rcbWgStatus.CheckedItems.Select(x => x.Text).ToList())));
                if (SearchObj.TsgStatusIds != null && SearchObj.TsgStatusIds.Count > 0)
                    sb.Append(String.Format("TSG Status({0}), ", String.Join(", ", rcbTsgStatus.CheckedItems.Select(x => x.Text).ToList())));
                if (SearchObj.MeetingIds != null && SearchObj.MeetingIds.Count > 0)
                    sb.Append(String.Format("Meetings({0}), ", racMeeting.Text));
                if (SearchObj.WorkItemIds != null && SearchObj.WorkItemIds.Count > 0)
                    sb.Append(String.Format("WorkItems({0}), ", racWorkItem.Text));

                lblCrSearchHeader.Text = String.Format("Search form ({0})", (sb.Length > 100) ? sb.ToString().Trim().TrimEnd(',').Substring(0, 100) + "..." : sb.ToString().Trim().TrimEnd(','));
            }
        }

        #endregion
    }
}