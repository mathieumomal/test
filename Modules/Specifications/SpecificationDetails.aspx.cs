using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Utils;
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

        //const 
        private const string CONST_RELATED_TAB = "Related";
        private const string CONST_EMPTY_FIELD = " - ";
        private const int ErrorFadeTimeout = 10000;
        
        // Errors and warnings
        public const string CONST_ERROR_SENDMAIL_SPEC_MGR = "sendMailSpecMgr";
        public const string CONST_ERROR_SENDMAIL_MCC = "sendMailMcc";
        public const string CONST_WARNING_SENDMAIL_MCC = "mailSentToSpecMgr";
        
        
        
        private const string SPEC_HEADER = "Specification #: ";
        private List<string> LIST_OF_TABS = new List<string>() { };
        public const string DsIdKey = "ETSI_DS_ID";
        private const string CONST_ERRORPANEL_CSS = "messageBox error";
        private const string CONST_INFOPANEL_CSS = "messageBox info";
        private const string BtnDefaultClass = "btn3GPP-default";

        //Properties
        private int UserId;
        private string selectedTab;
        private bool fromEdit;

        private const string VIEWSTATE_SPECIFICATION_ID = "VS_SPECID";
        public Nullable<int> SpecificationId
        {
            get
            {
                return (Nullable<int>)ViewState[VIEWSTATE_SPECIFICATION_ID];
            }
            set
            {
                ViewState[VIEWSTATE_SPECIFICATION_ID] = value;
            }
        }

        private string CreateError;

        private const string VIEWSTATE_FAILED_OP = "VS_FAILED_OPERATION";
        public Nullable<int> FailedOperationIndex
        {
            get
            {
                return (Nullable<int>)ViewState[VIEWSTATE_FAILED_OP];
            }
            set
            {
                ViewState[VIEWSTATE_FAILED_OP] = value;
            }
        }
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

                LoadReleaseDetails();

                //Load parent page to reflect changes
                if (fromEdit)
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Refresh", "window.opener.refreshSpecList();", true);

                if (!String.IsNullOrEmpty(CreateError))
                {
                    specMsg.Visible = true;
                    specMsg.CssClass = CONST_ERRORPANEL_CSS;

                    if (CreateError.Equals(CONST_WARNING_SENDMAIL_MCC))
                    {
                        specMsgTxt.Text = SpecificationDetails_aspx.Warning_NumberNeeded_NotifySpec_Mgr;
                        specMsg.CssClass = CONST_INFOPANEL_CSS;
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
            SpecificationReleaseControl1.DataSource = specification;
            SpecificationReleaseControl1.PersonId = UserId;
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
                referenceVal.Text = string.IsNullOrEmpty(specification.Number) ? CONST_EMPTY_FIELD.Trim() : specification.Number;
                titleVal.Text = string.IsNullOrEmpty(specification.Title) ? CONST_EMPTY_FIELD : specification.Title;
                statusVal.Text = string.IsNullOrEmpty(specification.Status) ? CONST_EMPTY_FIELD : specification.Status;
                if (specification.IsUnderChangeControl ?? false)
                {
                    lnkChangeRequest.Visible = true;
                    lnkChangeRequest.NavigateUrl = string.Format(ConfigVariables.RelativeUrlWiRelatedCrs, string.Empty, string.Empty, specification.Number);
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
            foreach (Domain.Enum_Technology technology in technologies.OrderBy(t => t.SortOrder))
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
            UserId = GetUserPersonId();
            SpecificationId = (Request.QueryString["specificationId"] != null) ? (int.TryParse(Request.QueryString["specificationId"], out output) ? new Nullable<int>(output) : null) : null;
            selectedTab = (Request.QueryString["selectedTab"] != null) ? Request.QueryString["selectedTab"] : string.Empty;
            fromEdit = (Request.QueryString["fromEdit"] != null);
            CreateError = (Request.QueryString["error"] != null) ? Request.QueryString["error"] : string.Empty;
            int index;
            FailedOperationIndex = (Request.QueryString["FailedOperationIndex"] != null) ? (int.TryParse(Request.QueryString["FailedOperationIndex"], out index) ? new Nullable<int>(index) : null) : null;            
        }

        /// <summary>
        /// Retrieve person id
        /// </summary>
        /// <returns>Person id</returns>
        private int GetUserPersonId()
        {
            var userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            if (userInfo.UserID < 0)
                return 0;

            int personId;
            if (int.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                return personId;
            return 0;
        }

        /// <summary>
        /// Manage buttons' display relying on user rights
        /// </summary>
        /// <param name="userRights">User Rights</param>
        private void ManageButtonDisplay(UserRightsContainer userRights)
        {
            //EDIT
            if ((!userRights.HasRight(Enum_UserRights.Specification_EditFull)) && (!userRights.HasRight(Domain.Enum_UserRights.Specification_EditLimitted)))
                EditBtn.Visible = false;

            //DELETE
            var specService = ServicesFactory.Resolve<ISpecificationService>();
            var response = specService.CheckDeleteSpecificationAllowed(SpecificationId ?? 0, GetUserPersonId());
            //Spec deletion allowed
            if (response.Result && response.Report.GetNumberOfErrors() <= 0 && response.Report.GetNumberOfWarnings() <= 0)
            {
                DeleteBtn.Visible = true;
            }
            //Spec deletion not allowed cause by error(s) (right or existence issues): button not visible
            else if (response.Report.GetNumberOfErrors() > 0)
            {
                DeleteBtn.Visible = false;
            }
            //Spec deletion not allowed cause by warning(s) : button visible but not enabled + explanation tooltip 
            else if (response.Report.GetNumberOfWarnings() > 0)
            {
                DeleteBtn.Visible = true;
                DeleteBtn.Enabled = false;
                DeleteBtn.OnClientClick = null;
                DeleteBtn.Attributes.Add("disabled", "disabled");
                DeleteBtn.CssClass = BtnDefaultClass;
                DeleteBtn.ToolTip = string.Join("\n", response.Report.WarningList);
            }

            //WITHDRAW
            if (!userRights.HasRight(Enum_UserRights.Specification_Withdraw))
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
        /// When click on edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditSpecificationDetails_Click(object sender, EventArgs e)
        {
            if (SpecificationId != null)
            {
                var selectedTabTitle = SpecificationDetailsRadTabStrip.SelectedTab != null
                    ? SpecificationDetailsRadTabStrip.SelectedTab.Text
                    : null;
                Response.Redirect(string.Format("EditSpecification.aspx?specificationId={0}&action={1}&selectedTab={2}", SpecificationId, "edit", selectedTabTitle), true);
            }
        }

        /// <summary>
        // When confirm for spec deletion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            var svc = ServicesFactory.Resolve<ISpecificationService>();
            var response = svc.DeleteSpecification(SpecificationId ?? 0, GetUserPersonId());
            if (!response.Result || response.Report.GetNumberOfErrors() > 0)
            {
                //Raise error
                specMsg.Visible = true;
                specMsg.CssClass = CONST_ERRORPANEL_CSS;
                specMsgTxt.Text = string.Join(", ", response.Report.ErrorList);
                ClientScript.RegisterClientScriptBlock(GetType(), "Close", "setTimeout(function(){ $('#" + specMsg.ClientID + "').hide('slow');} , " + ErrorFadeTimeout + ");", true);
                return;
            }
            //Close popup
            ClientScript.RegisterClientScriptBlock(GetType(), "CloseSpecPopup", "window.opener.refreshSpecList();close();", true);
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