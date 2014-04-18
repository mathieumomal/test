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
        protected HistoryControl specificationHistory;
        protected RemarksControl specificationRemarks;
        protected RapporteurControl specificationRapporteurs;
        //protected RelatedWiControl SpecificationRelatedWorkItems;
        protected SpecificationListControl parentSpecifications;
        protected SpecificationListControl childSpecifications;

        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_RESPONSIBILITY_TAB = "Responsibility";
        private static String CONST_RELATED_TAB = "Related";
        private static String CONST_RELEASES_TAB = "Releases";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";

        public static readonly string DsId_Key = "ETSI_DS_ID";
        private int UserId;
        private bool fromEdit;
        public Nullable<int> SpecificationId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();
            
                LoadReleaseDetails();                
            }
        }

        private void LoadReleaseDetails()
        {
            if (SpecificationId != null)
            {
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
                        BuildTabsDisplay();
                        SetRadioTechnologiesItems(new List<Domain.Enum_Technology>(){
                            new Domain.Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 1,
                                Code = "2G",
                                Description = "Second generation"
                            },
                            new Domain.Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 2,
                                Code = "3G",
                                Description = "third generation"
                            },
                            new Domain.Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 3,
                                Code = "LTE",
                                Description = "Long Term Evolution"
                            }
                        });
                        FillGeneralTab(userRights, specification);
                        FillResponsiblityTab(specification);
                        FillRelatedSpecificationsTab(specification);
                        FillHistoryTab(specification);
                        ManageButtonDisplay(userRights);
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
                typeVal.Text = string.IsNullOrEmpty(specification.SpecificationType) ? CONST_EMPTY_FIELD : specification.SpecificationType;
                stageVal.Text = specification.Enum_SpecificationStage == null ? CONST_EMPTY_FIELD : specification.Enum_SpecificationStage.Code;
                initialPlannedReleaseVal.Text = ""; //TODO Calculate initil release
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
        private void FillRelatedSpecificationsTab(Domain.Specification specification)
        {
            if (specification != null)
            {

                parentSpecifications.IsEditMode = false;
                parentSpecifications.IsParentList = true;

                if (specification.SpecificationChilds != null)
                    parentSpecifications.DataSource = specification.SpecificationParents.ToList();
                else
                    parentSpecifications.DataSource = null;

                childSpecifications.IsEditMode = false;
                childSpecifications.IsParentList = false;

                if (specification.SpecificationChilds != null)
                    childSpecifications.DataSource = specification.SpecificationChilds.ToList();
                else
                    childSpecifications.DataSource = null;

                /*List<Domain.WorkItem> workItemsSource = new List<Domain.WorkItem>();
                specification.Specification_WorkItem.ToList().ForEach(s => workItemsSource.Add(s.WorkItem));
                SpecificationRelatedWorkItems.IsEditMode = false;
                SpecificationRelatedWorkItems.DataSource = workItemsSource;*/
               
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
                PrimaryResponsibleGroupVal.Text = specification.PrimeResponsibleGroupShortName;
                SecondaryResponsibleGroupsVal.Text= specification.SecondaryResponsibleGroupsShortNames;
                specificationRapporteurs.ListIdPersonSelect = specification.PrimeSpecificationRapporteurs;
                specificationRapporteurs.ListIdPersonsSelected_MULTIMODE = specification.FullSpecificationRapporteurs;                
            }
            else
            {
                PrimaryResponsibleGroupVal.Text = CONST_EMPTY_FIELD;
                SecondaryResponsibleGroupsVal.Text = CONST_EMPTY_FIELD;
                specificationRapporteurs.ListIdPersonSelect = null;
                specificationRapporteurs.ListIdPersonsSelected_MULTIMODE = null;
            }
        }

        /// <summary>
        /// Build Rediotechnologies check list relying on Enum_Technology possible values
        /// </summary>
        /// <param name="technologies">List of Enum_technologie in DB</param>
        private void SetRadioTechnologiesItems(List<Domain.Enum_Technology> technologies)
        {
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
            fromEdit = (Request.QueryString["fromEdit"] != null) ? Convert.ToBoolean(Request.QueryString["fromEdit"]) : false;
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