﻿using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationVersionListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";

        #endregion

        #region Public Properties

        public bool IsEditMode { get; set; }
        public string SelectedTab
        {
            get
            {
                if (ViewState[CONST_SELECTED_TAB] == null)
                    return string.Empty;
                else
                    return ViewState[CONST_SELECTED_TAB].ToString();

            }
            set
            {
                ViewState[CONST_SELECTED_TAB] = value;
            }
        }
        public int? SpecId
        {
            get;
            set;

        }
        public int? ReleaseId
        {
            get;
            set;
        }
        public int? PersonId
        {
            get;
            set;

        }
        public double ScrollHeight { get; set; }
        public List<SpecVersion> Versions { get; set; }
        public int SpecReleaseID { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
                    var userRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecReleaseID).FirstOrDefault().Value;

                    imgForceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_ForceTransposition);
                    imgUnforceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_UnforceTransposition);

                    imgInhibitPromote.Visible = imgPromoteSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_InhibitPromote);
                    imgRemoveInhibitPromote.Visible = userRights.HasRight(Enum_UserRights.Specification_RemoveInhibitPromote);


                    imgWithdrawSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_WithdrawFromRelease);
                    imgWithdrawSpec.OnClientClick = "openRadWin(" + SpecId.GetValueOrDefault() + "," + ReleaseId.GetValueOrDefault() + "); return false;";

                    imgAllocateVersion.Visible = userRights.HasRight(Enum_UserRights.Versions_Allocate);
                    imgAllocateVersion.OnClientClick = "openRadWinVersion('" + ReleaseId.GetValueOrDefault() + "','" + SpecId.GetValueOrDefault() + "','allocate'); return false;";

                    imgUploadVersion.Visible = userRights.HasRight(Enum_UserRights.Versions_Upload);
                    imgUploadVersion.OnClientClick = "openRadWinVersion('" + ReleaseId.GetValueOrDefault() + "','" + SpecId.GetValueOrDefault() + "', 'upload'); return false;";
                }
                else
                {
                    pnlIconStrip.Visible = false;
                }
                specificationsVersionGrid.ClientSettings.Scrolling.ScrollHeight = Unit.Parse(ScrollHeight.ToString());
            }

            //Assign Missing Meeting Reference
            var svc = ServicesFactory.Resolve<IMeetingService>();
            foreach (SpecVersion specVersion in Versions)
            {
                if (specVersion.Source.HasValue)
                {
                    var mtg = svc.GetMeetingById(specVersion.Source.Value);
                    if (mtg != null)
                        specVersion.MtgShortRef = mtg.MtgShortRef;
                }
            }

            specificationsVersionGrid.DataSource = Versions;
            specificationsVersionGrid.DataBind();
        }

        protected void specificationsVersionGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            specificationsVersionGrid.DataSource = Versions;
        }

        protected void specificationsVersionGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string remarkText = ((Label)item["LatestRemark"].FindControl("lblRemarkText")).Text;

                if (remarkText.Length > 30)
                    ((Label)item["LatestRemark"].FindControl("lblRemarkText")).Text = remarkText.Substring(0, 29) + "...";

                SpecVersion specVersion = (SpecVersion)item.DataItem;
                if (specVersion != null)
                {
                    VersionRemarksControl versionRemarks = (VersionRemarksControl)item["LatestRemark"].FindControl("versionRemarksControl");
                    versionRemarks.IsEditMode = IsEditMode;
                    versionRemarks.PersonId = PersonId;
                    versionRemarks.SpecVersion = specVersion;
                    versionRemarks.SpecReleaseID = SpecReleaseID;
                }

                if (!String.IsNullOrEmpty(item["Source"].Text))
                {
                    HyperLink link = (HyperLink)item["Meetings"].FindControl("lnkMeetings");
                    link.Text = item["MtgShortRef"].Text;
                    link.NavigateUrl = ConfigVariables.MeetingDetailsAddress + item["Source"].Text;
                }
            }
        }

        /// <summary>
        /// Upon click on the Force Transposition icon. Calls the service, then redirects user independently of result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgForceTransposition_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && ReleaseId.HasValue && PersonId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.ForceTranspositionForRelease(PersonId.Value, ReleaseId.Value, SpecId.Value);
                Redirect();
            }
        }

        /// <summary>
        /// Upon click on the Unforce Transposition icon. Calls the service, then redirects user independently of result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgUnforceTransposition_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && ReleaseId.HasValue && PersonId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.UnforceTranspositionForRelease(PersonId.Value, ReleaseId.Value, SpecId.Value);

                Redirect();
            }
        }

        protected void imgInhibitPromote_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && PersonId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.SpecificationInhibitPromote(PersonId.Value, SpecId.Value);
                Redirect();
            }
        }

        protected void imgRemoveInhibitPromote_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && PersonId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.SpecificationRemoveInhibitPromote(PersonId.Value, SpecId.Value);
                Redirect();
            }
        }

        /// <summary>
        /// Click event for Promote Specification Image
        /// </summary>
        /// <param name="sender">Promote Specification Image</param>
        /// <param name="e">Event Args</param>
        protected void imgPromoteSpec_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && PersonId.HasValue && ReleaseId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.PromoteSpecification(PersonId.Value, SpecId.Value, ReleaseId.Value);
                Refresh();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Redirect user. Removes previous "selectedTab" and "Rel" flags.
        /// </summary>
        private void Redirect()
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&", address) + "&selectedTab=Releases&Rel=" + ReleaseId.Value);

        }

        /// <summary>
        /// Refresh the grid & stay on Releases tab
        /// </summary>
        private void Refresh()
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&", address) + "&selectedTab=Releases");
        }

        #endregion
    }
}