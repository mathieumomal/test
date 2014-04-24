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

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationDetails : System.Web.UI.Page
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
        private const string SPEC_HEADER = "Specification #: ";
        private List<string> LIST_OF_TABS = new List<string>() {};
        public static readonly string DsId_Key = "ETSI_DS_ID";
        //Properties
        private int UserId;
        private string selectedTab;
        public Nullable<int> SpecificationId;

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
                        lblHeaderText.Text = SPEC_HEADER + ((String.IsNullOrEmpty(specification.Number)) ? CONST_EMPTY_FIELD : specification.Number); 
                        BuildTabsDisplay();
                        SetRadioTechnologiesItems(svc.GetAllSpecificationTechnologies());
                        FillGeneralTab(userRights, specification);
                        FillResponsiblityTab(specification);
                        FillRelatedSpecificationsTab(specification, selectedTab);
                        FillHistoryTab(specification);
                        ManageButtonDisplay(userRights);
                        // Check if selectedTab is specified then select the according Tab and View page
                        SpecificationDetailsRadTabStrip.Tabs.ToList().ForEach(t => LIST_OF_TABS.Add(t.Text));
                        if (string.IsNullOrEmpty(selectedTab) != null && LIST_OF_TABS.Contains(selectedTab))
                        {
                            SpecificationDetailsRadTabStrip.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].Selected = true;
                            SpecificationDetailsRadTabStrip.Tabs[LIST_OF_TABS.IndexOf(selectedTab)].PageView.Selected = true;
                        }
                        
                    }
                }
            }
            else{
                specificationDetailsBody.Visible = false;
                specificationMessages.Visible = true;
                specificationMessagesTxt.Text = "No avaible data for the requested query";
                specificationMessages.CssClass = "Warning";
                specificationMessagesTxt.CssClass = "WarningTxt";
            }
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
                statusVal.Text =string.IsNullOrEmpty(specification.Status) ? CONST_EMPTY_FIELD :  specification.Status;
                typeVal.Text = string.IsNullOrEmpty(specification.SpecificationTypeFullText) ? CONST_EMPTY_FIELD : specification.SpecificationTypeFullText;
                stageVal.Text = specification.Enum_SpecificationStage == null ? CONST_EMPTY_FIELD : specification.Enum_SpecificationStage.Code;
                initialPlannedReleaseVal.Text = string.IsNullOrEmpty(specification.SpecificationInitialRelease) ? CONST_EMPTY_FIELD : specification.SpecificationInitialRelease; 
                internalVal.Checked = specification.IsForPublication == null ? true : !specification.IsForPublication.Value;
                commonIMSVal.Checked = specification.ComIMS == null ? false : specification.ComIMS.Value;
                
                foreach (Domain.SpecificationTechnology technology in specification.SpecificationTechnologies)
                {
                    if (technology.Enum_Technology != null && radioTechnologyVals.Items.FindByValue(technology.Enum_Technology.Pk_Enum_TechnologyId.ToString()) != null)
                    {
                        radioTechnologyVals.Items.FindByValue(technology.Enum_Technology.Pk_Enum_TechnologyId.ToString()).Selected = true;
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

                if (specification.SpecificationChilds != null)
                    parentSpecifications.DataSource = specification.SpecificationParents.ToList();
                else
                    parentSpecifications.DataSource = null;

                childSpecifications.IsEditMode = false;
                childSpecifications.IsParentList = false;
                childSpecifications.SelectedTab = CONST_RELATED_TAB;

                if (specification.SpecificationChilds != null)
                    childSpecifications.DataSource = specification.SpecificationChilds.ToList();
                else
                    childSpecifications.DataSource = null;

                List<Domain.WorkItem> workItemsSource = new List<Domain.WorkItem>();
                if (specification.Specification_WorkItem != null && specification.Specification_WorkItem.ToList().Count>0)
                    specification.Specification_WorkItem.ToList().ForEach(s => workItemsSource.Add(s.WorkItem));
                SpecificationRelatedWorkItems.IsEditMode = false;
                SpecificationRelatedWorkItems.ScrollHeight = 140;
                SpecificationRelatedWorkItems.DataSource = workItemsSource;
               
            }
            
        }

        /// <summary>
        /// Fill the history Tab with the retrieved data 
        /// </summary>
        /// <param name="specification">The retrieved specification</param>
        private void FillHistoryTab(Domain.Specification specification)
        {
            if(specification != null){
                specificationHistory.DataSource = specification.Histories.ToList();
                specificationHistory.ScrollHeight = 590;
            }
            else{
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
            if (specification != null)
            {
                PrimaryResponsibleGroupVal.Text = (string.IsNullOrEmpty(specification.PrimeResponsibleGroupShortName)) ? CONST_EMPTY_FIELD : specification.PrimeResponsibleGroupShortName;
                SecondaryResponsibleGroupsVal.Text = (string.IsNullOrEmpty(specification.SecondaryResponsibleGroupsShortNames)) ? CONST_EMPTY_FIELD : specification.SecondaryResponsibleGroupsShortNames;

                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurs;
                specificationRapporteurs.ListIdPersonsSelected_MULTIMODE = specification.FullSpecificationRapporteurs;             
            }
            else
            {
                PrimaryResponsibleGroupVal.Text = CONST_EMPTY_FIELD;
                SecondaryResponsibleGroupsVal.Text = CONST_EMPTY_FIELD;

                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurs;
                specificationRapporteurs.ListIdPersonsSelected_MULTIMODE = specification.FullSpecificationRapporteurs;  
            }
        }

        /// <summary>
        /// Build Rediotechnologies check list relying on Enum_Technology possible values
        /// </summary>
        /// <param name="technologies">List of Enum_technologie in DB</param>
        private void SetRadioTechnologiesItems(List<Domain.Enum_Technology> technologies)
        {
            radioTechnologyVals.Style.Add("border-spacing","0");
            foreach (Domain.Enum_Technology technology in technologies)
            {
                radioTechnologyVals.Items.Add(new ListItem() { 
                    Value = technology.Pk_Enum_TechnologyId.ToString() ,
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
            selectedTab = (Request.QueryString["selectedTab"] != null) ? Request.QueryString["selectedTab"]  : string.Empty;
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
                WithdrawBtn.Visible = false;
        }

        /// <summary>
        /// Set the view boxes layouts
        /// </summary>
        private void SetViewLayout()
        {
            fixContainer.Height = new System.Web.UI.WebControls.Unit(700, UnitType.Pixel);
            SpecificationDetailsRadMultiPage.Height = new System.Web.UI.WebControls.Unit(650, UnitType.Pixel);
        }

        protected void EditSpecificationDetails_Click(object sender, EventArgs e)
        {
        }


        protected void WithdrawSpecificatione_Click(object sender, EventArgs e)
        {
        }


        protected void ExitSpecificationDetails_Click(object sender, EventArgs e)
        {
        }
    }

}