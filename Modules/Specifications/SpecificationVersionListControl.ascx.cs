using Etsi.Ultimate.DomainClasses;
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
using Etsi.Ultimate.Module.Specifications.App_LocalResources;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationVersionListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";
        private const string CONST_WEBCONFIG_WI_REPORT_PATH = "WIReportPath";

        private const string LinkDisplayCssClasses = "linkStyle";
        private const string LinkDisabledCssClasses = "linkStyle disabled notAvailable";

        private const string VersionPopupPath =
            "openVersionDetailsPopup({0},'{1}','Version details'); return false;";

        //url, windowId, width, height, title, shouldCallMethodWhenClose
        private const string OpenRadWin = "openRadWin('{0}', '{1}', {2}, {3}, '{4}', {5}); return false;";

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
                    var userRights = basePage.SpecReleaseRights.First(x => x.Key == SpecReleaseID).Value;

                    imgForceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_ForceTransposition);
                    imgUnforceTransposition.Visible = userRights.HasRight(Enum_UserRights.Specification_UnforceTransposition);

                    imgInhibitPromote.Visible = userRights.HasRight(Enum_UserRights.Specification_InhibitPromote);
                    imgPromoteSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_Promote);
                    imgRemoveInhibitPromote.Visible = userRights.HasRight(Enum_UserRights.Specification_RemoveInhibitPromote);
                    imgDemoteSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_Demote);

                    imgWithdrawSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_WithdrawFromRelease);
                    imgWithdrawSpec.OnClientClick = string.Format(OpenRadWin, string.Format(ConfigVariables.SpecificationWithdrawMeetingSelectPopUpRelativeLink, SpecId.GetValueOrDefault(), ReleaseId.GetValueOrDefault()), "Withdraw", 450, 220, "Withdraw specification", "false");

                    imgUnWithdrawnSpec.Visible = userRights.HasRight(Enum_UserRights.Specification_UnWithdrawFromRelease);

                    imgAllocateVersion.Visible = (userRights.HasRight(Enum_UserRights.Versions_Allocate) && IsSpecNumberAssigned);
                    imgAllocateVersion.OnClientClick = string.Format(OpenRadWin, string.Format(ConfigVariables.SpecificationUploadVersionRelativeLink, ReleaseId.GetValueOrDefault(), SpecId.GetValueOrDefault(), "allocate"), "Version allocate", 440, 320, "Allocate version", "true");

                    imgUploadVersion.Visible = (userRights.HasRight(Enum_UserRights.Versions_Upload) && IsSpecNumberAssigned);
                    imgUploadVersion.OnClientClick = string.Format(OpenRadWin, string.Format(ConfigVariables.SpecificationUploadVersionRelativeLink, ReleaseId.GetValueOrDefault(), SpecId.GetValueOrDefault(), "upload"), "Version upload", 440, 320, "Upload version", "true");

                    imgDeleteSpecRelease.Visible = (userRights.HasRight(Enum_UserRights.SpecificationRelease_Remove) || userRights.HasRight(Enum_UserRights.SpecificationRelease_Remove_Disabled));
                    imgDeleteSpecRelease.Enabled = userRights.HasRight(Enum_UserRights.SpecificationRelease_Remove);
                    imgDeleteSpecRelease.ToolTip = userRights.HasRight(Enum_UserRights.SpecificationRelease_Remove)
                        ? SpecificationDetails_aspx.Button_Delete_SpecRelease_Tooltip_Allowed
                        : SpecificationDetails_aspx.Button_Delete_SpecRelease_Tooltip_NotAllowed;
                    imgDeleteSpecRelease.OnClientClick = string.Format(OpenRadWin, string.Format(ConfigVariables.SpecificationRemoveSpecReleasePopUpRelativeLink, SpecId.GetValueOrDefault(), ReleaseId.GetValueOrDefault()), "Remove Spec-Release", 450, 150, "Remove Specification-Release", "true");
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

                var specVersion = (SpecVersion)item.DataItem;
                if (specVersion != null)
                {
                    var versionDetails = (ImageButton)item["LatestRemark"].FindControl("imgVersionDetails");
                    versionDetails.OnClientClick = string.Format(VersionPopupPath, specVersion.Pk_VersionId, IsEditMode);

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

        /// <summary>
        /// Click event for Demote Specification Image
        /// </summary>
        /// <param name="sender">Demote Specification Image</param>
        /// <param name="e">Event Args</param>
        protected void imgDemoteSpec_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (SpecId.HasValue && PersonId.HasValue && ReleaseId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.DemoteSpecification(PersonId.Value, SpecId.Value, ReleaseId.Value);
                Refresh();
            }
        }

        protected void imgUnWithdrawnSpec_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if(SpecId.HasValue && PersonId.HasValue && ReleaseId.HasValue)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                specSvc.UnWithdrawnForRelease(PersonId.Value, ReleaseId.Value, SpecId.Value);
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

                var currentFoundationCr = string.Empty;
                var foundationCrs = new List<string>();
                foreach (var cr in currentVersion.FoundationCrs)
                {
                    if (!string.IsNullOrEmpty(cr.CrNumber))
                        currentFoundationCr += "CR: " + cr.CrNumber;
                    if (cr.Revision != 0)
                        currentFoundationCr += " - Rev: " + cr.Revision;
                    if (currentFoundationCr.Length > 0)
                        foundationCrs.Add(currentFoundationCr);
                    currentFoundationCr = string.Empty;
                }
                foundationCrs = foundationCrs.OrderBy(x => x).ToList();

                relatedCrs.ToolTip = string.Join("\n", foundationCrs);
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
            Response.Redirect(string.Join("&", address) + "&selectedTab=Versions&Rel=" + ReleaseId.Value);
        }

        private void RedirectWithErrorMsg(int errorIndex)
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&", address) + "&selectedTab=Versions&Rel=" + ReleaseId.Value + "&FailedOperationIndex=" + errorIndex);
        }

        /// <summary>
        /// Refresh the grid & stay on Releases tab
        /// </summary>
        private void Refresh()
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&", address) + "&selectedTab=Versions");
        }
        

        #endregion


    }
}