using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using System.Configuration;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Module.Specifications.App_LocalResources;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationDetails : SpecificationBasePage
    {
        // Custom controls
        protected HistoryControl specificationHistory;
        protected RemarksControl specificationRemarks;
        protected RapporteurControl specificationRapporteurs;
        protected RelatedWiControl SpecificationRelatedWorkItems;
        protected SpecificationListControl parentSpecifications;
        protected SpecificationListControl childSpecifications;

        //Static fields
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_RESPONSIBILITY_TAB = "Responsibility";
        private static String CONST_RELATED_TAB = "Related";
        private static String CONST_RELEASES_TAB = "Releases";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";
        private const int ErrorFadeTimeout = 10000;
        
        // Errors and warnings
        public const string CONST_ERROR_SENDMAIL_SPEC_MGR = "sendMailSpecMgr";
        public const string CONST_ERROR_SENDMAIL_MCC = "sendMailMcc";
        public const string CONST_WARNING_SENDMAIL_MCC = "mailSentToSpecMgr";
        
        
        
        private const string SPEC_HEADER = "Specification #: ";
        private List<string> LIST_OF_TABS = new List<string>() { };
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private const String CONST_ERRORPANEL_CSS = "Spec_Edit_Error";
        private const String CONST_ERRORTEXT_CSS = "ErrorTxt";

        private const String CONST_INFOPANEL_CSS = "Spec_Edit_Info";
        private const String CONST_INFOTEXT_CSS = "InfoTxt";
        //Properties
        private int UserId;
        private string selectedTab;
        private bool fromEdit;
        public static Nullable<int> SpecificationId;
        private string CreateError;
        public static Nullable<int> FailedOperationIndex;
        private static Dictionary<int, string> OperationFailureMsgs = new Dictionary<int, string>() { { 1, "forced transposition failed" } };

        /// <summary>
        /// Main event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();

                SetViewLayout();

                LoadReleaseDetails();

                //Load parent page to reflect changes
                if (fromEdit)
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Refresh", "window.opener.location.reload(true);", true);

                if (!String.IsNullOrEmpty(CreateError))
                {
                    specMsg.Visible = true;
                    specMsg.CssClass = CONST_ERRORPANEL_CSS;
                    specMsgTxt.CssClass = CONST_ERRORTEXT_CSS;

                    if (CreateError.Equals(CONST_WARNING_SENDMAIL_MCC))
                    {
                        specMsgTxt.Text = SpecificationDetails_aspx.Warning_NumberNeeded_NotifySpec_Mgr;
                        specMsg.CssClass = CONST_INFOPANEL_CSS;
                        specMsgTxt.CssClass = CONST_INFOTEXT_CSS;
                    }
                    else if (CreateError.Equals(CONST_ERROR_SENDMAIL_SPEC_MGR))
                    {
                        specMsgTxt.Text = SpecificationDetails_aspx.Error_NumberNeeded_NotifySpecMgr_NoEmail;
                    }
                    if (CreateError.Equals(CONST_ERROR_SENDMAIL_MCC))
                    {
                        specMsgTxt.Text = SpecificationDetails_aspx.Error_NumberAssigned_NotifyMCC_NoEmail;
                    }

                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "setTimeout(function(){ $('#" + specMsg.ClientID + "').hide('slow');} , "+ErrorFadeTimeout+");", true);
                }

            }
        }

        /// <summary>
        /// Loads the content of the page 
        /// </summary>
        private void LoadReleaseDetails()
        {

            if (SpecificationId != null)
            {
                // Retrieve data
                ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
                KeyValuePair<DomainClasses.Specification, DomainClasses.UserRightsContainer> specificationRightsObject = svc.GetSpecificationDetailsById(UserId, SpecificationId.Value);
                Domain.Specification specification = specificationRightsObject.Key;
                DomainClasses.UserRightsContainer userRights = specificationRightsObject.Value;

                if (specification == null)
                {
                    specificationDetailsBody.Visible = false;
                    specificationMessages.Visible = true;
                    specificationMessagesTxt.Text = "No avaible data for the requested query";
                    specificationMessages.CssClass = "Warning";
                    specificationMessagesTxt.CssClass = "WarningTxt";
                }
                else
                {
                    if (!userRights.HasRight(Domain.Enum_UserRights.Specification_ViewDetails))
                    {
                        specificationDetailsBody.Visible = false;
                        specificationMessages.Visible = true;
                        specificationMessagesTxt.Text = "You dont have the right to visualize this content";
                        specificationMessages.CssClass = "Error";
                        specificationMessagesTxt.CssClass = "ErrorTxt";
                    }
                    else
                    {
                        if (FailedOperationIndex != null)
                        {
                            notifMsg.Visible = true; 
                            notifMsgTxt.Text = OperationFailureMsgs[FailedOperationIndex.GetValueOrDefault()];
                            notifMsg.CssClass = "ErrorForControl";
                            notifMsgTxt.CssClass = "ErrorTxt";
                        }
                        lblHeaderText.Text = SPEC_HEADER + ((String.IsNullOrEmpty(specification.Number)) ? CONST_EMPTY_FIELD : specification.Number);
                        BuildTabsDisplay();
                        SetRadioTechnologiesItems(svc.GetAllSpecificationTechnologies());
                        FillGeneralTab(userRights, specification);
                        FillResponsiblityTab(specification);
                        FillRelatedSpecificationsTab(specification, selectedTab);
                        FillReleasesTab(specification);
                        FillHistoryTab(specification);
                        ManageButtonDisplay(userRights);
                        // Check if selectedTab is specified then select the according Tab and View page
                        SpecificationDetailsRadTabStrip.Tabs.ToList().ForEach(t => LIST_OF_TABS.Add(t.Text));
                        if (!string.IsNullOrEmpty(selectedTab) && LIST_OF_TABS.Contains(selectedTab))
                        {
                            SpecificationDetailsRadTabStrip.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].Selected = true;
                            SpecificationDetailsRadTabStrip.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].PageView.Selected = true;
                        }

                    }
                }
            }
            else
            {
                specificationDetailsBody.Visible = false;
                specificationMessages.Visible = true;
                specificationMessagesTxt.Text = "No avaible data for the requested query";
                specificationMessages.CssClass = "Warning";
                specificationMessagesTxt.CssClass = "WarningTxt";
            }
        }

        /// <summary>
        /// Fill the Release Tab with the retrieved data 
        /// </summary>
        private void FillReleasesTab(Domain.Specification specification)
        {
            SpecificationReleaseControl1.DataSource = specification.GetSpecificationForUI();
            SpecificationReleaseControl1.PersonId = UserId;
        }

        /// <summary>
        /// Set the tabs display
        /// </summary>
        /// <param name="userRights"></param>
        private void BuildTabsDisplay()
        {

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected = true
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RESPONSIBILITY_TAB,
                    Text = CONST_RESPONSIBILITY_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELATED_TAB,
                    Text = CONST_RELATED_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELEASES_TAB,
                    Text = CONST_RELEASES_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_HISTORY_TAB,
                    Text = CONST_HISTORY_TAB,
                    Selected = false
                });
        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights">Current user rights</param>
        /// <param name="specification">The retrieved specification</param>
        private void FillGeneralTab(Domain.UserRightsContainer userRights, Domain.Specification specification)
        {
            if (specification != null && userRights != null)
            {
                referenceVal.Text = string.IsNullOrEmpty(specification.Number) ? CONST_EMPTY_FIELD : specification.Number;
                titleVal.Text = string.IsNullOrEmpty(specification.Title) ? CONST_EMPTY_FIELD : specification.Title;
                statusVal.Text = string.IsNullOrEmpty(specification.Status) ? CONST_EMPTY_FIELD : specification.Status;
                if (specification.IsUnderChangeControl ?? false)
                {
                    lnkChangeRequest.Visible = true;
                    lnkChangeRequest.NavigateUrl = "#";
                }
                typeVal.Text = string.IsNullOrEmpty(specification.SpecificationTypeFullText) ? CONST_EMPTY_FIELD : specification.SpecificationTypeFullText;
                initialPlannedReleaseVal.Text = string.IsNullOrEmpty(specification.SpecificationInitialRelease) ? CONST_EMPTY_FIELD : specification.SpecificationInitialRelease;
                internalVal.Checked = specification.IsForPublication == null ? true : !specification.IsForPublication.Value;
                commonIMSVal.Checked = specification.ComIMS == null ? false : specification.ComIMS.Value;
                if (specification.SpecificationTechnologiesList != null && specification.SpecificationTechnologiesList.Count > 0)
                {
                    foreach (Domain.Enum_Technology technology in specification.SpecificationTechnologiesList)
                    {
                        if (technology != null && radioTechnologyVals.Items.FindByValue(technology.Pk_Enum_TechnologyId.ToString()) != null)
                        {
                            radioTechnologyVals.Items.FindByValue(technology.Pk_Enum_TechnologyId.ToString()).Selected = true;
                        }

                    }
                }
                disableAllCheckBoxes();

                specificationRemarks.IsEditMode = false;
                specificationRemarks.UserRights = userRights;
                specificationRemarks.DataSource = specification.Remarks.ToList();
            }

        }

        /// <summary>
        /// Fill the related Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillRelatedSpecificationsTab(Domain.Specification specification, string selectedTab)
        {
            if (specification != null)
            {

                parentSpecifications.IsEditMode = false;
                parentSpecifications.IsParentList = true;
                parentSpecifications.SelectedTab = CONST_RELATED_TAB;
                parentSpecifications.ScrollHeight = 70;

                if (specification.SpecificationChilds != null)
                    parentSpecifications.DataSource = specification.SpecificationParents.ToList();
                else
                    parentSpecifications.DataSource = null;

                childSpecifications.IsEditMode = false;
                childSpecifications.IsParentList = false;
                childSpecifications.SelectedTab = CONST_RELATED_TAB;
                childSpecifications.ScrollHeight = 70;

                if (specification.SpecificationChilds != null)
                    childSpecifications.DataSource = specification.SpecificationChilds.ToList();
                else
                    childSpecifications.DataSource = null;

                /*List<Domain.WorkItem> workItemsSource = new List<Domain.WorkItem>();
                if (specification.SpecificationWIsList != null && specification.SpecificationWIsList.Count > 0)
                    specification.Specification_WorkItem.ToList().ForEach(s => workItemsSource.Add(s.WorkItem));*/
                SpecificationRelatedWorkItems.IsEditMode = false;
                SpecificationRelatedWorkItems.ScrollHeight = 110;
                SpecificationRelatedWorkItems.DataSource = specification.SpecificationWIsList;

            }

        }

        /// <summary>
        /// Fill the history Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillHistoryTab(Domain.Specification specification)
        {
            if (specification != null)
            {
                specificationHistory.DataSource = specification.Histories.ToList();
                specificationHistory.ScrollHeight = (int)SpecificationDetailsRadMultiPage.Height.Value - 50;
                //specificationHistory.ScrollHeight = 640;
            }
            else
            {
                specificationHistory.DataSource = null;
            }
        }

        /// <summary>
        /// Fill the Responsibility Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillResponsiblityTab(Domain.Specification specification)
        {
            specificationRapporteurs.IsEditMode = false;
            specificationRapporteurs.IsSinglePersonMode = false;
            specificationRapporteurs.PersonLinkBaseAddress = ConfigurationManager.AppSettings["RapporteurDetailsAddress"];
            if (specification != null)
            {
                PrimaryResponsibleGroupVal.Text = (string.IsNullOrEmpty(specification.PrimeResponsibleGroupFullName)) ? CONST_EMPTY_FIELD : specification.PrimeResponsibleGroupFullName;
                SecondaryResponsibleGroupsVal.Text = (string.IsNullOrEmpty(specification.SecondaryResponsibleGroupsFullNames)) ? CONST_EMPTY_FIELD : specification.SecondaryResponsibleGroupsFullNames;

                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurIds;
                specificationRapporteurs.ListIdPersonsSelected_multimode = specification.FullSpecificationRapporteurs;
            }
            else
            {
                PrimaryResponsibleGroupVal.Text = CONST_EMPTY_FIELD;
                SecondaryResponsibleGroupsVal.Text = CONST_EMPTY_FIELD;

                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurIds;
                specificationRapporteurs.ListIdPersonsSelected_multimode = specification.FullSpecificationRapporteurs;
            }
        }

        /// <summary>
        /// Build Rediotechnologies check list relying on Enum_Technology possible values
        /// </summary>
        /// <param name="technologies">List of Enum_technologie in DB</param>
        private void SetRadioTechnologiesItems(List<Domain.Enum_Technology> technologies)
        {
            radioTechnologyVals.Style.Add("border-spacing", "0");
            foreach (Domain.Enum_Technology technology in technologies.OrderBy(t => t.Code))
            {
                radioTechnologyVals.Items.Add(new ListItem()
                {
                    Value = technology.Pk_Enum_TechnologyId.ToString(),
                    Text = technology.Code
                });
            }
        }

        /// <summary>
        /// Disable all check boxes in the view mode
        /// </summary>
        private void disableAllCheckBoxes()
        {
            commonIMSVal.Enabled = false;
            internalVal.Enabled = false;
            radioTechnologyVals.Enabled = false;
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            SpecificationId = (Request.QueryString["specificationId"] != null) ? (int.TryParse(Request.QueryString["specificationId"], out output) ? new Nullable<int>(output) : null) : null;
            selectedTab = (Request.QueryString["selectedTab"] != null) ? Request.QueryString["selectedTab"] : string.Empty;
            fromEdit = (Request.QueryString["fromEdit"] != null);
            CreateError = (Request.QueryString["error"] != null) ? Request.QueryString["error"] : string.Empty;
            int index;
            FailedOperationIndex = (Request.QueryString["FailedOperationIndex"] != null) ? (int.TryParse(Request.QueryString["FailedOperationIndex"], out index) ? new Nullable<int>(index) : null) : null;            
        }

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="UserInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
            return 0;
        }

        /// <summary>
        /// Manage buttons' display relying on user rights
        /// </summary>
        /// <param name="userRights">User Rights</param>
        private void ManageButtonDisplay(Domain.UserRightsContainer userRights)
        {
            if ((!userRights.HasRight(Domain.Enum_UserRights.Specification_EditFull)) && (!userRights.HasRight(Domain.Enum_UserRights.Specification_EditLimitted)))
                EditBtn.Visible = false;

            if (!userRights.HasRight(Domain.Enum_UserRights.Specification_Withdraw))
            {
                WithdrawBtn.Visible = false;
            }
            else
            {
                //Set button events 
                WithdrawBtn.OnClientClick = "openDefinitiveWithdrawlRadWin(); return false;";
                WithdrawRadWindow.NavigateUrl += "?SpecId=" + SpecificationId.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Set the view boxes layouts
        /// </summary>
        private void SetViewLayout()
        {
            fixContainer.Height = new System.Web.UI.WebControls.Unit(600, UnitType.Pixel);
            SpecificationDetailsRadMultiPage.Height = new System.Web.UI.WebControls.Unit(560, UnitType.Pixel);
        }

        protected void EditSpecificationDetails_Click(object sender, EventArgs e)
        {
            if (SpecificationId != null)
                Response.Redirect("EditSpecification.aspx?specificationId=" + SpecificationId + "&action=edit", true);
        }


        /// <summary>
        /// Get Actual POCO entities for Remark objects
        /// </summary>
        /// <param name="proxyRemarks">Proxy Remarks</param>
        /// <returns>List of Remarks</returns>
        private List<Remark> GetActualRemarksFromProxy(List<Remark> proxyRemarks)
        {
            List<Remark> remarks = new List<Remark>();
            proxyRemarks.ForEach(x => remarks.Add(new Remark()
            {
                Pk_RemarkId = x.Pk_RemarkId,
                Fk_PersonId = x.Fk_PersonId,
                Fk_WorkItemId = x.Fk_WorkItemId,
                Fk_VersionId = x.Fk_VersionId,
                Fk_SpecificationId = x.Fk_SpecificationId,
                Fk_ReleaseId = x.Fk_ReleaseId,
                Fk_SpecificationReleaseId = x.Fk_SpecificationReleaseId,
                IsPublic = x.IsPublic,
                CreationDate = x.CreationDate,
                RemarkText = x.RemarkText,
                PersonName = x.PersonName,
            }));
            return remarks;
        }
    }
}