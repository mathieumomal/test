using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
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

        #region events
        /// <summary>
        /// Page load method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RgCrList_NeedDataSource(object o, GridNeedDataSourceEventArgs e)
        {
            var searchObj = new ChangeRequestsSearch();
            //TODO : searchObj

            var crSvc = new ChangeRequestService();
            //rgCrList.DataSource = crSvc.GetChangeRequests(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);

            //TEST
            var crList = new List<ChangeRequestListFacade>
            {
                new ChangeRequestListFacade
                {
                    ChangeRequestId = 1,
                    SpecNumber = "22.222",
                    ChangeRequestNumber = "0003",
                    Revision = 1,
                    ImpactedVersion = "12.0.1",
                    TargetRelease = "Rel-12",
                    Title = "Mon titre",
                    WgTdocNumber = "SP-1", 
                    WgStatus = "Approved",
                    TsgTdocNumber = "SA61", 
                    TsgStatus = "Approved", 
                    NewVersion  = "12.3.0"
                },
                new ChangeRequestListFacade
                {
                    ChangeRequestId = 1,
                    SpecNumber = "22.222",
                    ChangeRequestNumber = "0003",
                    Revision = 1,
                    ImpactedVersion = "12.0.1",
                    TargetRelease = "Rel-12",
                    Title = "Mon titre",
                    WgTdocNumber = "SP-1", 
                    WgStatus = "Approved",
                    TsgTdocNumber = "SA61", 
                    TsgStatus = "Approved", 
                    NewVersion  = "12.3.0",
                    SpecId = 2
                }
            };
            rgCrList.DataSource = crList;
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

                //Set hyperlink text value
                var specLink = (HyperLink)dataItem["SpecNumber"].Controls[0];
                specLink.Text = currentCr.SpecId.ToString(CultureInfo.InvariantCulture);
                specLink.NavigateUrl = String.Format(ConfigVariables.SpecificationDetailsUrl, currentCr.SpecId);

                var releaseLink = (HyperLink)dataItem["TargetRelease"].Controls[0];
                releaseLink.Text = currentCr.TargetRelease.ToString(CultureInfo.InvariantCulture);
                releaseLink.NavigateUrl = String.Format(ConfigVariables.ReleaseDetailsUrl, currentCr.TargetReleaseId);

                var wgTdocLink = (HyperLink)dataItem["WgTdocNumber"].Controls[0];
                wgTdocLink.Text = currentCr.WgTdocNumber.ToString(CultureInfo.InvariantCulture);
                wgTdocLink.NavigateUrl = String.Format(ConfigVariables.TdocDetailsUrl, currentCr.WgTdocNumber);

                var tsgTdocLink = (HyperLink)dataItem["TsgTdocNumber"].Controls[0];
                tsgTdocLink.Text = currentCr.TsgTdocNumber.ToString(CultureInfo.InvariantCulture);
                tsgTdocLink.NavigateUrl = String.Format(ConfigVariables.TdocDetailsUrl, currentCr.TsgTdocNumber);

                var newVersionLink = (HyperLink)dataItem["NewVersion"].Controls[0];
                newVersionLink.Text = currentCr.NewVersion.ToString(CultureInfo.InvariantCulture);
                newVersionLink.NavigateUrl = currentCr.NewVersionPath;

                //Disabled necessary links
                if (currentCr.SpecId == 0)
                    specLink.Enabled = false;
                if (currentCr.TargetReleaseId == 0)
                    releaseLink.Enabled = false;
                if (string.IsNullOrEmpty(currentCr.WgTdocNumber))
                    wgTdocLink.Enabled = false;
                if (string.IsNullOrEmpty(currentCr.TsgTdocNumber))
                    tsgTdocLink.Enabled = false;
                if (string.IsNullOrEmpty(currentCr.NewVersionPath))
                    newVersionLink.Enabled = false;
            }
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
        #endregion
    }
}