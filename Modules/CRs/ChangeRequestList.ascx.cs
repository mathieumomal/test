using System;
using System.Globalization;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Telerik.Web.UI;
using Etsi.Ultimate.Controls;
using System.Collections.Generic;
using System.Web;

namespace Etsi.Ultimate.Module.CRs
{
    public partial class ChangeRequestList : PortalModuleBase
    {
        #region constants
        private const string DsIdKey = "ETSI_DS_ID";
        #endregion

        #region properties

        protected FullView CrFullView;
        protected ShareUrlControl crShareUrl;
        private bool isUrlSearch;

        private const string VS_CR_SEARCH_OBJ = "VS_CR_SEARCH_OBJ";
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
            if (!IsPostBack)
            {
                rgCrList.PageSize = ConfigVariables.CRsListRecordsMaxSize;
                GetRequestParameters();
                searchObj = new ChangeRequestsSearch();
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
            LoadData();
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
                }   
            }
        }

        #endregion

        #region Load data

        /// <summary>
        /// Load data in the RadGrid each time needed
        /// </summary>
        private void LoadData()
        {
            if (isUrlSearch)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["specnumber"]))
                    txtSpecificationNumber.Text = searchObj.SpecificationNumber = Request.QueryString["specnumber"];
                int pageIndex = 0;
                if (!String.IsNullOrEmpty(Request.QueryString["pageindex"]) && (int.TryParse(Request.QueryString["pageindex"], out pageIndex)))
                    searchObj.SkipRecords = pageIndex * rgCrList.PageSize;
            }
            else
                searchObj.SkipRecords = rgCrList.CurrentPageIndex * rgCrList.PageSize;
            
            searchObj.PageSize = rgCrList.PageSize;

            var crSvc = ServicesFactory.Resolve<IChangeRequestService>();
            var response = crSvc.GetChangeRequests(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);
            if (response.Report.GetNumberOfErrors() != 0)
                return;

            rgCrList.DataSource = response.Result.Key;
            rgCrList.VirtualItemCount = response.Result.Value;

            ManageShareUrl();
            ManageFullView();

            lblCrSearchHeader.Text = String.Format("Search form ({0})", String.IsNullOrEmpty(searchObj.SpecificationNumber) ? "All" : searchObj.SpecificationNumber.Trim());
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

        #endregion
    }
}