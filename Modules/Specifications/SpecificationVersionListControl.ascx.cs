﻿using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Configuration;
using System.Text;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationVersionListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";
        private const string CONST_WEBCONFIG_WI_REPORT_PATH = "WIReportPath";

        private const string LinkDisplayCssClasses = "linkStyle";
        private const string LinkDisabledCssClasses = "linkStyle disabled notAvailable";

        #endregion

        #region Public Properties

        public bool IsEditMode { get; set; }
        public string SelectedTab
        {
            get
            {
                if (ViewState[CONST_SELECTED_TAB] == null)
                    return string.Empty;
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
        public AdditionalVersionInfo AdditionalVersionInfo { get; set; } 
        public int SpecReleaseID { get; set; }
        public bool IsSpecNumberAssigned { get; set; }
        private static Dictionary<int, string> OperationFailureMsgs = new Dictionary<int, string>() { {1, "forced transposition failed"}};

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    var basePage = (SpecificationBasePage)this.Page;
                    var userRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecReleaseID).FirstOrDefault().Value;

                    imgForceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_ForceTransposition);
                    imgUnforceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_UnforceTransposition);

                    imgInhibitPromote.Visible = userRights.HasRight(Enum_UserRights.Specification_InhibitPromote);
                    imgPromoteSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_Promote);
                    imgRemoveInhibitPromote.Visible = userRights.HasRight(Enum_UserRights.Specification_RemoveInhibitPromote);


                    imgWithdrawSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_WithdrawFromRelease);
                    imgWithdrawSpec.OnClientClick = "openRadWin(" + SpecId.GetValueOrDefault() + "," + ReleaseId.GetValueOrDefault() + "); return false;";

                    imgAllocateVersion.Visible = (userRights.HasRight(Enum_UserRights.Versions_Allocate) && IsSpecNumberAssigned);
                    imgAllocateVersion.OnClientClick = "openRadWinVersion('" + ReleaseId.GetValueOrDefault() + "','" + SpecId.GetValueOrDefault() + "','allocate', 'Allocate version'); return false;";

                    imgUploadVersion.Visible = (userRights.HasRight(Enum_UserRights.Versions_Upload) && IsSpecNumberAssigned);
                    imgUploadVersion.OnClientClick = "openRadWinVersion('" + ReleaseId.GetValueOrDefault() + "','" + SpecId.GetValueOrDefault() + "', 'upload', 'Upload version'); return false;";
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

                    //We add the WIReport link to the SDO btn
                    if (ConfigurationManager.AppSettings[CONST_WEBCONFIG_WI_REPORT_PATH] != null)
                        specVersion.WIReportPath = new StringBuilder()
                        .Append(ConfigurationManager.AppSettings[CONST_WEBCONFIG_WI_REPORT_PATH])
                        .Append(specVersion.ETSI_WKI_ID)
                        .ToString();
                    else
                        specVersion.WIReportPath = "Web.Config:WIReportPathNotConfigured";
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
                var item = (GridDataItem)e.Item;

                var remarkText = ((Label)item["LatestRemark"].FindControl("lblRemarkText")).Text;

                if (remarkText.Length > 30)
                    ((Label)item["LatestRemark"].FindControl("lblRemarkText")).Text = remarkText.Substring(0, 29) + "...";

                if (!String.IsNullOrEmpty(item["Source"].Text))
                {
                    var link = (HyperLink)item["Meetings"].FindControl("lnkMeetings");
                    link.Text = item["MtgShortRef"].Text;
                    link.NavigateUrl = ConfigVariables.MeetingDetailsAddress + item["Source"].Text;
                }

                var specVersion = (SpecVersion)item.DataItem;
                if (specVersion != null)
                {
                    var versionRemarks = (ImageButton)item["LatestRemark"].FindControl("imgVersionRemarks");
                    versionRemarks.OnClientClick = "openRemarksPopup('version','" + specVersion.Pk_VersionId + "','" + IsEditMode + "', 'Version Remarks'); return false;";

                    //Display informations contains in the SpecDecorator
                    ApplyChangeRequestInformation(item, specVersion);

                    ApplyTdocInformation(item, specVersion);
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
                if(specSvc.ForceTranspositionForRelease(PersonId.Value, ReleaseId.Value, SpecId.Value))
                    Redirect();                
                else
                    RedirectWithErrorMsg(1);
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
        /// Applied SpecDecorator informations
        /// </summary>
        /// <param name="item">Grid data item</param>
        /// <param name="specVersion">Version entity</param>
        private void ApplyChangeRequestInformation(GridDataItem item, SpecVersion specVersion)
        {
            //Get current version
            var id = specVersion.Pk_VersionId;

            //Foundamental CRs tooltip associated to a version
            var relatedCrs = (HyperLink)item["link"].FindControl("imgRelatedCRs");
            if (AdditionalVersionInfo.SpecVersionFoundationCrs == null)
            {
                relatedCrs.CssClass = LinkDisabledCssClasses;
                return;
            }
            var currentVersion = AdditionalVersionInfo.SpecVersionFoundationCrs.FirstOrDefault(x => x.VersionId == id);
            if (currentVersion != null)
            {
                if (currentVersion.FoundationCrs.Count == 0)
                {
                    relatedCrs.CssClass = LinkDisabledCssClasses;
                    return;
                }
                var tooltip = new StringBuilder();
                foreach (var cr in currentVersion.FoundationCrs)
                {
                    if (!string.IsNullOrEmpty(cr.CrNumber))
                        tooltip.Append("CR: ").Append(cr.CrNumber);
                    if (cr.Revision != 0)
                        tooltip.Append(" - Rev: ").Append(cr.Revision);
                    if (tooltip.Length > 0)
                        tooltip.Append("\n");
                }
                relatedCrs.ToolTip = tooltip.ToString();
                relatedCrs.CssClass = LinkDisplayCssClasses;
                relatedCrs.NavigateUrl = String.Format(ConfigVariables.RelativeUrlVersionRelatedCrs, specVersion.Pk_VersionId, specVersion.Fk_ReleaseId);
            }
        }

        /// <summary>
        /// Apply Tdoc information (Tooltip, Css style, Navigation url etc.,)
        /// </summary>
        /// <param name="item">Grid data item</param>
        /// <param name="specVersion">Version entity</param>
        private void ApplyTdocInformation(GridDataItem item, SpecVersion specVersion)
        {
            var relatedTDocs = (HyperLink)item["link"].FindControl("imgRelatedTDocs");
            if (relatedTDocs == null)
                return;

            if (String.IsNullOrEmpty(specVersion.RelatedTDoc))
            {
                relatedTDocs.CssClass = LinkDisabledCssClasses;
                return;
            }

            relatedTDocs.CssClass = LinkDisplayCssClasses;
            relatedTDocs.ToolTip = specVersion.RelatedTDoc;
            relatedTDocs.NavigateUrl = String.Format(ConfigVariables.TdocDetailsUrl, specVersion.RelatedTDoc);
        }

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

        private void RedirectWithErrorMsg(int errorIndex)
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&", address) + "&selectedTab=Releases&Rel=" + ReleaseId.Value + "&FailedOperationIndex=" + errorIndex);
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