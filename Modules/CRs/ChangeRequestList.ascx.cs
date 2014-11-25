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

namespace Etsi.Ultimate.Module.CRs
{
    public partial class ChangeRequestList : PortalModuleBase
    {
        #region constants
        private const string DsIdKey = "ETSI_DS_ID";
        #endregion

        #region properties
        protected Controls.FullView CrFullView;
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
            }
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
                        specLink.NavigateUrl = String.Format(ConfigVariables.SpecificationDetailsUrl, currentCr.SpecId);
                    else
                        specLink.Enabled = false;

                    var releaseLink = (HyperLink)dataItem["TargetRelease"].Controls[0];
                    if (currentCr.TargetRelease != null)
                        releaseLink.Text = currentCr.TargetRelease.ToString(CultureInfo.InvariantCulture);
                    if (currentCr.TargetReleaseId != 0)
                        releaseLink.NavigateUrl = String.Format(ConfigVariables.ReleaseDetailsUrl, currentCr.TargetReleaseId);
                    else
                        releaseLink.Enabled = false;

                    var wgTdocLink = (HyperLink)dataItem["WgTdocNumber"].Controls[0];
                    if (currentCr.WgTdocNumber != null)
                    {
                        wgTdocLink.Text = currentCr.WgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        wgTdocLink.NavigateUrl = String.Format(ConfigVariables.TdocDetailsUrl, currentCr.WgTdocNumber);
                    }
                    
                    var tsgTdocLink = (HyperLink)dataItem["TsgTdocNumber"].Controls[0];
                    if (currentCr.TsgTdocNumber != null)
                    {
                        tsgTdocLink.Text = currentCr.TsgTdocNumber.ToString(CultureInfo.InvariantCulture);
                        tsgTdocLink.NavigateUrl = String.Format(ConfigVariables.TdocDetailsUrl, currentCr.TsgTdocNumber);
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
            var searchObj = new ChangeRequestsSearch
            {
                SkipRecords = rgCrList.CurrentPageIndex*rgCrList.PageSize,
                PageSize = rgCrList.PageSize
            };

            var crSvc = ServicesFactory.Resolve<IChangeRequestService>();
            var response = crSvc.GetChangeRequests(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);
            if (response.Report.GetNumberOfErrors() != 0)
                return;

            rgCrList.DataSource = response.Result.Key;
            rgCrList.VirtualItemCount = response.Result.Value;

            //Init full view according to the filters
            InitFullView();
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
        /// Init full view
        /// </summary>
        private void InitFullView()
        {
            CrFullView.ModuleId = ModuleId;
            CrFullView.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            CrFullView.BaseAddress = address;

            //TODO : CrFullView.UrlParams (for filters)
            CrFullView.Display();
        }
        #endregion
    }
}