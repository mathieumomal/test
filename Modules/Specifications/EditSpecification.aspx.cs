﻿using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class EditSpecification : System.Web.UI.Page
    {
        #region Fields

        // Custom controls
        protected HistoryControl specificationHistory;
        protected RemarksControl specificationRemarks;
        protected RapporteurControl specificationRapporteurs;
        protected RelatedWiControl SpecificationRelatedWorkItems;
        protected CommunityControl PrimaryResGrpCtrl;
        protected CommunityControl SecondaryResGrpCtrl;
        protected SpecificationListControl parentSpecifications;
        protected SpecificationListControl childSpecifications;

        //Static fields
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_RESPONSIBILITY_TAB = "Responsibility";
        private static String CONST_RELATED_TAB = "Related";
        private static String CONST_RELEASES_TAB = "Releases";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";
        private const string SPEC_HEADER = "Specification #: ";
        private const String CREATION_MODE = "create";
        private const String EDIT_MODE = "edit";
        private List<string> LIST_OF_TABS = new List<string>() { };
        public static readonly string DsId_Key = "ETSI_DS_ID";
        

        //Properties
        private int userId;
        private string selectedTab;
        public static Nullable<int> SpecificationId;
        private static String action;
        

        #endregion

        #region Events

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
                LoadSpecificationDetails();
            }
            specificationRemarks.AddRemarkHandler += specificationRemarks_AddRemarkHandler;
        }

        /// <summary>
        /// Create/Save Specification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveSpec_Click(object sender, EventArgs e)
        {
            ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
            Domain.Specification spec;
            Domain.ImportReport result;

            userId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            if (action.Equals(EDIT_MODE))
            {
                spec = svc.GetSpecificationDetailsById(userId, SpecificationId.Value).Key;
                FillSpecificationObject(spec);
                result = svc.EditSpecification(userId, spec).Value;
            }
            else
            {
                spec = new Domain.Specification();
                FillSpecificationObject(spec);
                result = svc.CreateSpecification(userId, spec).Value;
            }

            if (result.ErrorList.Count > 0)
            {
                specMsg.Visible = true;
                specMsg.CssClass = "Spec_Edit_Error";
                specMsgTxt.CssClass = "ErrorTxt";

                foreach (string errorMessage in result.ErrorList)
                    specMsgTxt.Text = errorMessage + "<br/>";

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "setTimeout(function(){ $('#" + specMsg.ClientID + "').hide('slow');} , 3000);", true);
            }

        }

        /// <summary>
        /// Close Create/Edit spec popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExitSpecEdit_Click(object sender, EventArgs e)
        {
            if (action.Equals(EDIT_MODE))
                Response.Redirect("SpecificationDetails.aspx?specificationId=" + SpecificationId, true);
            else
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
        }

        /// <summary>
        /// Add event handler for specificationRemarks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void specificationRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            List<Domain.Remark> datasource = specificationRemarks.DataSource;
            //Get display name
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
            datasource.Add(new Domain.Remark()
            {
                Fk_PersonId = userId,
                Fk_SpecificationId = SpecificationId,
                IsPublic = specificationRemarks.UserRights != null ? specificationRemarks.UserRights.HasRight(Domain.Enum_UserRights.Remarks_AddPrivateByDefault) : false,
                CreationDate = DateTime.UtcNow,
                RemarkText = specificationRemarks.RemarkText,
                PersonName = personDisplayName
            });
            specificationRemarks.DataSource = datasource;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Loads the content of the page 
        /// </summary>
        private void LoadSpecificationDetails()
        {

            ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
            IReleaseService relSvc = ServicesFactory.Resolve<IReleaseService>();
            var releases = relSvc.GetAllReleases(userId).Key.Where(x=>x.Enum_ReleaseStatus.Code == Domain.Enum_ReleaseStatus.Open 
                                                                   || x.Enum_ReleaseStatus.Code == Domain.Enum_ReleaseStatus.Frozen).OrderByDescending(x=>x.StartDate).ToList();
            if (action.Equals(EDIT_MODE))
            {
                // Retrieve data
                KeyValuePair<DomainClasses.Specification, DomainClasses.UserRightsContainer> specificationRightsObject = svc.GetSpecificationDetailsById(userId, SpecificationId.Value);
                Domain.Specification specification = specificationRightsObject.Key;
                DomainClasses.UserRightsContainer userRights = specificationRightsObject.Value;

                if (specification == null)
                {
                    specBody.Visible = false;
                    specMsg.Visible = true;
                    specMsgTxt.Text = "No avaible data for the requested query";
                    specMsg.CssClass = "Warning";
                    specMsgTxt.CssClass = "WarningTxt";
                }
                else
                {
                    if (!userRights.HasRight(Domain.Enum_UserRights.Specification_ViewDetails))
                    {
                        specBody.Visible = false;
                        specMsg.Visible = true;
                        specMsgTxt.Text = "You dont have the right to visualize this content";
                        specMsg.CssClass = "Error";
                        specMsgTxt.CssClass = "ErrorTxt";
                    }
                    else
                    {
                        lblHeaderText.Text = SPEC_HEADER + ((String.IsNullOrEmpty(specification.Number)) ? CONST_EMPTY_FIELD : specification.Number);

                        BuildTabsDisplay();
                        SetRadioTechnologiesItems(svc.GetAllSpecificationTechnologies());
                        FillGeneralTab(userRights, specification, releases);
                        FillResponsiblityTab(specification);
                        FillRelatedSpecificationsTab(specification, selectedTab);
                        FillHistoryTab(specification);

                        // Check if selectedTab is specified then select the according Tab and View page
                        rtsSpecEdit.Tabs.ToList().ForEach(t => LIST_OF_TABS.Add(t.Text));
                        if (!string.IsNullOrEmpty(selectedTab) && LIST_OF_TABS.Contains(selectedTab))
                        {
                            rtsSpecEdit.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].Selected = true;
                            rtsSpecEdit.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].PageView.Selected = true;
                        }

                        btnSaveDisabled.Style.Add("display", "none");
                    }
                }
            }

            if (action.Equals(CREATION_MODE))
            {
                BuildTabsDisplay();
                SetRadioTechnologiesItems(svc.GetAllSpecificationTechnologies());
                FillGeneralTab(null, null, releases);
                FillResponsiblityTab(null);
                FillRelatedSpecificationsTab(null, null);
                FillHistoryTab(null);

                btnSave.Style.Add("display", "none");
            }
        }

        /// <summary>
        /// Set the tabs display
        /// </summary>
        /// <param name="userRights"></param>
        private void BuildTabsDisplay()
        {

            rtsSpecEdit.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected = true
                });

            rtsSpecEdit.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RESPONSIBILITY_TAB,
                    Text = CONST_RESPONSIBILITY_TAB,
                    Selected = false
                });

            rtsSpecEdit.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELATED_TAB,
                    Text = CONST_RELATED_TAB,
                    Selected = false
                });

            if (action.Equals(EDIT_MODE))
            {
                rtsSpecEdit.Tabs.Add(
                    new RadTab()
                    {
                        PageViewID = "RadPage" + CONST_RELEASES_TAB,
                        Text = CONST_RELEASES_TAB,
                        Selected = false
                    });

                rtsSpecEdit.Tabs.Add(
                    new RadTab()
                    {
                        PageViewID = "RadPage" + CONST_HISTORY_TAB,
                        Text = CONST_HISTORY_TAB,
                        Selected = false
                    });
            }
        }

        /// <summary>
        /// Build Rediotechnologies check list relying on Enum_Technology possible values
        /// </summary>
        /// <param name="technologies">List of Enum_technologie in DB</param>
        private void SetRadioTechnologiesItems(List<Domain.Enum_Technology> technologies)
        {
            cblRadioTechnology.Style.Add("border-spacing", "0");
            foreach (Domain.Enum_Technology technology in technologies.OrderBy(t => t.Code))
            {
                cblRadioTechnology.Items.Add(new ListItem()
                {
                    Value = technology.Pk_Enum_TechnologyId.ToString(),
                    Text = technology.Code
                });
            }
        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights">Current user rights</param>
        /// <param name="specification">The retrieved specification</param>
        private void FillGeneralTab(Domain.UserRightsContainer userRights, Domain.Specification specification, List<Domain.Release> releases)
        {
            specificationRemarks.IsEditMode = true;

            //update actual datasource
            if (releases != null)
            {
                ddlPlannedRelease.DataSource = releases.Where(x => x.Enum_ReleaseStatus.Description == Domain.Enum_ReleaseStatus.Open
                                                                || x.Enum_ReleaseStatus.Description == Domain.Enum_ReleaseStatus.Frozen).ToList();
                ddlPlannedRelease.DataValueField = "Pk_ReleaseId";
                ddlPlannedRelease.DataTextField = "Name";
                ddlPlannedRelease.DataBind();
            }

            if (specification != null && userRights != null)
            {
                txtReference.Text = string.IsNullOrEmpty(specification.Number) ? CONST_EMPTY_FIELD : specification.Number;
                txtTitle.Text = string.IsNullOrEmpty(specification.Title) ? CONST_EMPTY_FIELD : specification.Title;
                lblStatus.Text = string.IsNullOrEmpty(specification.Status) ? CONST_EMPTY_FIELD : specification.Status;
                if (specification.IsTS != null)
                    ddlType.SelectedIndex = specification.IsTS.Value ? 0 : 1;

                foreach (ListItem item in ddlPlannedRelease.Items)
                    if (item.Text == specification.SpecificationInitialRelease)
                    {
                        item.Selected = true;
                        break;
                    }

                chkInternal.Checked = specification.IsForPublication == null ? true : !specification.IsForPublication.Value;
                chkCommonIMSSpec.Checked = specification.ComIMS == null ? false : specification.ComIMS.Value;
                if (specification.SpecificationTechnologiesList != null && specification.SpecificationTechnologiesList.Count > 0)
                {
                    foreach (Domain.Enum_Technology technology in specification.SpecificationTechnologiesList)
                    {
                        if (technology != null && cblRadioTechnology.Items.FindByValue(technology.Pk_Enum_TechnologyId.ToString()) != null)
                        {
                            cblRadioTechnology.Items.FindByValue(technology.Pk_Enum_TechnologyId.ToString()).Selected = true;
                        }

                    }
                }

                specificationRemarks.UserRights = userRights;
                specificationRemarks.DataSource = specification.Remarks.ToList();
            }
        }

        /// <summary>
        /// Fill the Responsibility Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillResponsiblityTab(Domain.Specification specification)
        {
            specificationRapporteurs.IsEditMode = true;
            specificationRapporteurs.IsSinglePersonMode = false;
            specificationRapporteurs.PersonLinkBaseAddress = ConfigurationManager.AppSettings["RapporteurDetailsAddress"];
            
            if (specification != null)
            {
                PrimaryResGrpCtrl.SelectedCommunityID = specification.PrimeResponsibleGroup.Fk_commityId;
                SecondaryResGrpCtrl.SelectedCommunityIds = specification.SpecificationResponsibleGroups.Select(x => x.Fk_commityId).ToList();

                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurs;
                specificationRapporteurs.ListIdPersonsSelected_multimode = specification.FullSpecificationRapporteurs;
            }
        }

        /// <summary>
        /// Fill the related Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillRelatedSpecificationsTab(Domain.Specification specification, string selectedTab)
        {
            parentSpecifications.IsEditMode = true;
            parentSpecifications.IsParentList = true;
            parentSpecifications.SelectedTab = CONST_RELATED_TAB;
            parentSpecifications.ScrollHeight = 70;

            childSpecifications.IsEditMode = true;
            childSpecifications.IsParentList = false;
            childSpecifications.SelectedTab = CONST_RELATED_TAB;
            childSpecifications.ScrollHeight = 70;

            SpecificationRelatedWorkItems.IsEditMode = true;
            SpecificationRelatedWorkItems.ScrollHeight = 110;

            if (specification != null)
            {
                if (specification.SpecificationChilds != null)
                    parentSpecifications.DataSource = specification.SpecificationParents.ToList();
                else
                    parentSpecifications.DataSource = null;

                if (specification.SpecificationChilds != null)
                    childSpecifications.DataSource = specification.SpecificationChilds.ToList();
                else
                    childSpecifications.DataSource = null;

                if (specification.SpecificationWIsList != null)
                    SpecificationRelatedWorkItems.DataSource = specification.SpecificationWIsList;
                else
                    SpecificationRelatedWorkItems.DataSource = null;
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
                specificationHistory.ScrollHeight = (int)rmpSpecEdit.Height.Value - 50;
            }
            else
            {
                specificationHistory.DataSource = new List<Domain.History>();
            }
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            userId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            SpecificationId = (Request.QueryString["specificationId"] != null) ? (int.TryParse(Request.QueryString["specificationId"], out output) ? new Nullable<int>(output) : null) : null;
            selectedTab = (Request.QueryString["selectedTab"] != null) ? Request.QueryString["selectedTab"] : string.Empty;
            action = (Request.QueryString["action"] != null) ? Request.QueryString["action"] : string.Empty;
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
        /// Fill spec object from form elements
        /// </summary>
        /// <param name="spec">new/existing spec object</param>
        /// <param name="isNewSpec">flag to determine new/existing spec</param>
        private void FillSpecificationObject(Domain.Specification spec)
        {
            //General tab
            spec.Number = txtReference.Text;

            spec.Title = txtTitle.Text;
            spec.IsTS = Convert.ToBoolean(ddlType.SelectedValue);

            int releaseId;
            if (int.TryParse(ddlPlannedRelease.SelectedItem.Value, out releaseId))
                spec.Specification_Release.Add(new Domain.Specification_Release() { Fk_ReleaseId = releaseId });

            spec.IsForPublication = !chkInternal.Checked;
            spec.ComIMS = chkCommonIMSSpec.Checked;

            foreach (ListItem item in cblRadioTechnology.Items)
            {
                int Pk_Enum_TechnologyId;
                if (item.Selected && int.TryParse(item.Value, out Pk_Enum_TechnologyId))
                    spec.SpecificationTechnologies.Add(new Domain.SpecificationTechnology() { Fk_Enum_Technology = Pk_Enum_TechnologyId });
            }
            spec.Remarks = specificationRemarks.DataSource;

            //Responsibility
            spec.SpecificationResponsibleGroups.Add(new Domain.SpecificationResponsibleGroup() { Fk_commityId = PrimaryResGrpCtrl.SelectedCommunityID, IsPrime = true });

            foreach (var communityId in SecondaryResGrpCtrl.SelectedCommunityIds)
                spec.SpecificationResponsibleGroups.Add(new Domain.SpecificationResponsibleGroup() { Fk_commityId = communityId, IsPrime = false });

            foreach (var rapporteurId in specificationRapporteurs.ListIdPersonSelect)
                spec.SpecificationRapporteurs.Add(new Domain.SpecificationRapporteur() { Fk_RapporteurId = rapporteurId });

            //Related
            foreach (Domain.WorkItem wi in SpecificationRelatedWorkItems.DataSource)
                spec.Specification_WorkItem.Add(new Domain.Specification_WorkItem() { Fk_WorkItemId = wi.Pk_WorkItemUid, isPrime = wi.IsPrimary, IsSetByUser = wi.IsUserAddedWi });
        }

        #endregion
    }
}