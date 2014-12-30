using System.Data;
using System.Web.Services;
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
        private const string VS_CR_SEARCH_OBJ = "VS_CR_SEARCH_OBJ";
        private const string PK_ENUMCHANGEREQUESTSTATUS = "Pk_EnumChangeRequestStatus";
        private const string DESCRIPTION = "Description";

        #endregion

        #region properties

        protected FullView CrFullView;
        protected ShareUrlControl crShareUrl;
        protected ReleaseSearchControl releaseSearchControl;
        private bool isUrlSearch;

        /// <summary>
        /// Gets or sets the search object.
        /// </summary>
        private ChangeRequestsSearch searchObj
        {
            get
            {
                return (ChangeRequestsSearch)ViewState[VS_CR_SEARCH_OBJ];
            }
            set
            {
                ViewState[VS_CR_SEARCH_OBJ] = value;
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
                releaseSearchControl.IsLoadingNeeded = !crList.Visible;
                crList.Visible = true;
                GetRequestParameters();
                searchObj = new ChangeRequestsSearch() { ReleaseIds = new List<int>(), StatusIds = new List<int>(), MeetingIds = new List<int>(), WorkItemIds = new List<int>() };
                searchObj.PageSize = rgCrList.PageSize = ConfigVariables.CRsListRecordsMaxSize;

                //Load CR Statuses
                LoadCrStatuses();

                releaseSearchControl.Load += releaseSearchControl_Load;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            isUrlSearch = false;
            searchObj.SpecificationNumber = txtSpecificationNumber.Text;
            searchObj.MeetingIds = GetSelectedMeetingIds();
            searchObj.WorkItemIds = GetSelectedWorkItemIds();
            searchObj.ReleaseIds = releaseSearchControl.SelectedReleaseIds;
            searchObj.StatusIds = GetSelectedStatusIds();
            rgCrList.CurrentPageIndex = 0;
            rgCrList.Rebind();
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
                        specLink.NavigateUrl = "javascript:openSpecification('"+String.Format(ConfigVariables.SpecificationDetailsUrl, currentCr.SpecId)+"','"+currentCr.SpecId+"')";
                    else
                        specLink.Enabled = false;

                    var releaseLink = (HyperLink)dataItem["TargetRelease"].Controls[0];
                    if (currentCr.TargetRelease != null)
                        releaseLink.Text = currentCr.TargetRelease.ToString(CultureInfo.InvariantCulture);
                    if (currentCr.TargetReleaseId != 0)
                        releaseLink.NavigateUrl = "javascript:openRelease('" + String.Format(ConfigVariables.ReleaseDetailsUrl, currentCr.TargetReleaseId) + "','" + currentCr.TargetReleaseId+"')";
                    else
                        releaseLink.Enabled = false;

                    var wgTdocLink = (HyperLink)dataItem["WgTdocNumber"].Controls[0];
                    if (currentCr.WgTdocNumber != null)
                    {
                        wgTdocLink.Text = currentCr.WgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        wgTdocLink.NavigateUrl = "javascript:openTdoc('"+String.Format(ConfigVariables.TdocDetailsUrl, currentCr.WgTdocNumber)+"','"+currentCr.WgTdocNumber+"')";
                        if (currentCr.WgTdocNumber.Equals("-"))
                            wgTdocLink.Enabled = false;
                    }
                    
                    var tsgTdocLink = (HyperLink)dataItem["TsgTdocNumber"].Controls[0];
                    if (currentCr.TsgTdocNumber != null)
                    {
                        tsgTdocLink.Text = currentCr.TsgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        tsgTdocLink.NavigateUrl = "javascript:openTdoc('" + String.Format(ConfigVariables.TdocDetailsUrl, currentCr.TsgTdocNumber) + "','" + currentCr.TsgTdocNumber + "')";
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
            LoadData();
        }

        #endregion

        #region Load data

        /// <summary>
        /// Load data in the RadGrid each time needed
        /// </summary>
        private void LoadData()
        {
            if (isUrlSearch)
                LoadUrlData();
            else
                searchObj.SkipRecords = rgCrList.CurrentPageIndex * searchObj.PageSize;

            var crSvc = ServicesFactory.Resolve<IChangeRequestService>();
            var response = crSvc.GetChangeRequests(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);
            if (response.Report.GetNumberOfErrors() != 0)
                return;

            rgCrList.VirtualItemCount = response.Result.Value;
            rgCrList.CurrentPageIndex = searchObj.SkipRecords / searchObj.PageSize;
            rgCrList.DataSource = response.Result.Key;

            ManageShareUrl();
            ManageFullView();

            SetSearchLabel();
        }

        /// <summary>
        /// Loads the URL data.
        /// </summary>
        private void LoadUrlData()
        {
            //[1] Specification Number
            if (!String.IsNullOrEmpty(Request.QueryString["specnumber"]))
                txtSpecificationNumber.Text = searchObj.SpecificationNumber = Request.QueryString["specnumber"];

            //[2] Releases
            if (!String.IsNullOrEmpty(Request.QueryString["release"]))
            {
                var releases = Request.QueryString["release"].Split(',').ToList();
                var releaseIds = new List<int>();
                releases.ForEach(x =>
                {
                    int releaseId = 0;
                    if (int.TryParse(x, out releaseId))
                        releaseIds.Add(releaseId);
                });
                releaseSearchControl.SelectedReleaseIds = searchObj.ReleaseIds = releaseIds;
            }

            //[3] Statuses
            if (!String.IsNullOrEmpty(Request.QueryString["status"]))
            {
                var statuses = Request.QueryString["status"].Split(',').ToList();
                var statusIds = new List<int>();
                statuses.ForEach(x =>
                {
                    int statusId = 0;
                    if (int.TryParse(x, out statusId))
                        statusIds.Add(statusId);
                });
                searchObj.StatusIds = statusIds;
                rcbStatus.Items.ToList().ForEach(x => { x.Checked = statusIds.Contains(Convert.ToInt32(x.Value)); });
            }

            //[4] Meetings
            if (!String.IsNullOrEmpty(Request.QueryString["meeting"]))
            {
                var meetings = Request.QueryString["meeting"].Split(',').ToList();
                var meetingIds = new List<int>();
                meetings.ForEach(x =>
                {
                    int meetingId = 0;
                    if (int.TryParse(x, out meetingId))
                        meetingIds.Add(meetingId);
                });
                searchObj.MeetingIds = meetingIds;

                var meetingSvc = ServicesFactory.Resolve<IMeetingService>();
                var meetingsList = meetingSvc.GetMeetingsByIds(meetingIds);

                meetingIds.ForEach(x =>
                {
                    var meeting = meetingsList.Find(m => m.MTG_ID == x);
                    if (meeting != null)
                        racMeeting.Entries.Add(new AutoCompleteBoxEntry(meeting.MtgDdlText, x.ToString()));
                });
            }

            //[5] Work Items
            if (!String.IsNullOrEmpty(Request.QueryString["workitem"]))
            {
                var workitems = Request.QueryString["workitem"].Split(',').ToList();
                var workItemIds = new List<int>();
                workitems.ForEach(x =>
                {
                    int workItemId = 0;
                    if (int.TryParse(x, out workItemId))
                        workItemIds.Add(workItemId);
                });
                searchObj.WorkItemIds = workItemIds;

                var workItemSvc = ServicesFactory.Resolve<IWorkItemService>();
                var workItemsList = workItemSvc.GetWorkItemByIds(0, workItemIds).Key;

                workItemIds.ForEach(x =>
                {
                    var workItem = workItemsList.Find(wi => wi.Pk_WorkItemUid == x);
                    if(workItem != null)
                        racWorkItem.Entries.Add(new AutoCompleteBoxEntry(workItem.WorkItemDdlText, x.ToString()));
                });
            }

            //[6] Page Index
            int pageIndex = 0;
            if (!String.IsNullOrEmpty(Request.QueryString["pageindex"]) && (int.TryParse(Request.QueryString["pageindex"], out pageIndex)))
                searchObj.SkipRecords = pageIndex * searchObj.PageSize;
        }

        /// <summary>
        /// Loads the cr statuses.
        /// </summary>
        private void LoadCrStatuses()
        {
            var svcChangeRequestService = ServicesFactory.Resolve<IChangeRequestService>();
            var svcResponse = svcChangeRequestService.GetChangeRequestStatuses();

            if (!svcResponse.Key) return;
            rcbStatus.DataValueField = PK_ENUMCHANGEREQUESTSTATUS;
            rcbStatus.DataTextField = DESCRIPTION;
            rcbStatus.DataSource = svcResponse.Value.OrderBy(x => x.Description);
            rcbStatus.DataBind();
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
                int meetingId = 0;
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
                int workItemId = 0;
                if (int.TryParse(entry.Value, out workItemId))
                    selectedWorkItemIds.Add(workItemId);
            }

            return selectedWorkItemIds;
        }

        /// <summary>
        /// Gets the selected status ids.
        /// </summary>
        /// <returns>Selected status ids</returns>
        private List<int> GetSelectedStatusIds()
        {
            var selectedStatusIds = new List<int>();

            rcbStatus.CheckedItems.ToList().ForEach(x => {
                int statusId = 0;
                if (int.TryParse(x.Value, out statusId))
                    selectedStatusIds.Add(statusId);
            });

            return selectedStatusIds;
        }

        #endregion

        #region utils

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="userInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo userInfo)
        {
            if (userInfo.UserID < 0)
                return 0;
            else
            {
                int personId;
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                    return personId;
            }
            return 0;
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
            crShareUrl.IsShortUrlChecked = false;
            crShareUrl.ModuleId = ModuleId;
            crShareUrl.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            crShareUrl.BaseAddress = address;

            crShareUrl.UrlParams = ManageUrlParams();
        }

        /// <summary>
        /// Generate Url parameters for Short/FullView Url
        /// </summary>
        /// <returns>Url parameters</returns>
        private Dictionary<string, string> ManageUrlParams()
        {
            var urlParams = new Dictionary<string, string>();
            urlParams.Add("q", "1");
            urlParams.Add("specnumber", searchObj.SpecificationNumber);
            urlParams.Add("release", String.Join(",", searchObj.ReleaseIds));
            urlParams.Add("status", String.Join(",", searchObj.StatusIds));
            urlParams.Add("meeting", String.Join(",", searchObj.MeetingIds));
            urlParams.Add("workitem", String.Join(",", searchObj.WorkItemIds));
            urlParams.Add("pageindex", (searchObj.SkipRecords / searchObj.PageSize).ToString());

            return urlParams;
        }

        /// <summary>
        /// Extract query strings from Url
        /// </summary>
        private void GetRequestParameters()
        {
            isUrlSearch = (Request.QueryString["q"] != null);
        }

        /// <summary>
        /// Sets the search label.
        /// </summary>
        private void SetSearchLabel()
        {
            if (searchObj != null)
            {
                StringBuilder sb = new StringBuilder();

                if (!String.IsNullOrEmpty(searchObj.SpecificationNumber))
                    sb.Append(String.Format("{0}, ", searchObj.SpecificationNumber));

                string releaseText = releaseSearchControl.SearchString;
                sb.Append(String.Format("{0}, ", String.IsNullOrEmpty(releaseText) ? "Open Releases" : releaseText));

                if (searchObj.StatusIds != null && searchObj.StatusIds.Count > 0)
                    sb.Append(String.Format("Status({0}), ", String.Join(", ", rcbStatus.CheckedItems.Select(x => x.Text).ToList())));
                if (searchObj.MeetingIds != null && searchObj.MeetingIds.Count > 0)
                    sb.Append(String.Format("Meetings({0}), ", racMeeting.Text));
                if (searchObj.WorkItemIds != null && searchObj.WorkItemIds.Count > 0)
                    sb.Append(String.Format("WorkItems({0}), ", racWorkItem.Text));

                lblCrSearchHeader.Text = String.Format("Search form ({0})", (sb.Length > 100) ? sb.ToString().Trim().TrimEnd(',').Substring(0, 100) + "..." : sb.ToString().Trim().TrimEnd(','));
            }
        }

        #endregion
    }
}