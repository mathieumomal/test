﻿using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Controls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SPECIFICATION_GRID_DATA = "SpecificationListControlData_{0}";
        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";
        private const string CONST_SELECTED_MODE = "SPEC_SELECTED_MODE";
        private const int CONST_MIN_SCROLL_HEIGHT = 50;
        private const string CONST_EXCLUDE_SPEC = "SPEC_TO_EXCLUDE";
        private const string CONST_SPECIFICATION_EDIT = "SpecificationListControl_EDIT";

        #endregion

        #region Public Properties

        public bool IsEditMode
        {
            get
            {
                if (ViewState[CONST_SPECIFICATION_EDIT] == null)
                    return false;
                else
                    return (bool)ViewState[CONST_SPECIFICATION_EDIT];

            }
            set
            {
                ViewState[CONST_SPECIFICATION_EDIT] = value;
            }
        }
        public bool IsParentList { get; set; }
        public int ScrollHeight { get; set; }
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


        public List<Specification> DataSource
        {
            get
            {
                if (ViewState[String.Format(CONST_SPECIFICATION_GRID_DATA, this.ClientID)] == null)
                    ViewState[String.Format(CONST_SPECIFICATION_GRID_DATA, this.ClientID)] = new List<Specification>();

                return (List<Specification>)ViewState[String.Format(CONST_SPECIFICATION_GRID_DATA, this.ClientID)];
            }
            set
            {
                ViewState[String.Format(CONST_SPECIFICATION_GRID_DATA, this.ClientID)] = GetActualSpecificationsFromProxy(value);
                specificationsGrid.Rebind();
            }
        }

        public List<string> ToExcludeFromDataSrouce
        {
            get
            {
                if (ViewState[String.Format(CONST_EXCLUDE_SPEC, this.ClientID)] == null)
                    ViewState[String.Format(CONST_EXCLUDE_SPEC, this.ClientID)] = new List<string>();

                return (List<string>)ViewState[String.Format(CONST_EXCLUDE_SPEC, this.ClientID)];
            }
            set
            {
                ViewState[String.Format(CONST_EXCLUDE_SPEC, this.ClientID)] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!IsEditMode)
                {
                    lblAddSpecification.Visible = false;
                    rcbAddSpecification.Visible = false;
                    btnAddSpecification.Visible = false;
                }
                else
                {
                    if (IsParentList)
                        lblAddSpecification.Text = "Add parent specification";
                    else
                        lblAddSpecification.Text = "Add child specification";
                }
                specificationsGrid.ClientSettings.Scrolling.ScrollHeight = (ScrollHeight < CONST_MIN_SCROLL_HEIGHT) ? Unit.Pixel(CONST_MIN_SCROLL_HEIGHT) : Unit.Pixel(ScrollHeight);
            }

            btnAddSpecification.OnClientClick = "showProgress" + this.ClientID + "();";
        }

        protected void specificationsGrid_PreRender(object sender, System.EventArgs e)
        {
            //ChangeGridToEditMode();
        }

        protected void specificationsGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var specifications = DataSource;
            specificationsGrid.DataSource = specifications.OrderByDescending(s => s.Number);
        }

        protected void btnRemoveSpec_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int specId;
            if (Int32.TryParse(((ImageButton)sender).CommandArgument, out specId))
            {
                var tmpSrc = DataSource;
                tmpSrc.Remove(tmpSrc.Where(s => s.Pk_SpecificationId == specId).FirstOrDefault());
                DataSource = tmpSrc;
            }
            specificationsGrid.DataBind();

        }

        #endregion

        protected void specificationsGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var currentSpecification = (Specification)item.DataItem;

                if (IsEditMode && item["SpecificationActions"].FindControl("btnRemoveSpec") != null)
                    ((ImageButton)item["SpecificationActions"].FindControl("btnRemoveSpec")).Visible = true;

                //Set prime community hyperlink control
                CommunityHyperlinkControl myControl = (CommunityHyperlinkControl)item.FindControl("primaryResponsibleGroup");
                myControl.RefreshControl(currentSpecification.PrimeResponsibleGroupId, EnumCommunityNameType.SHORT.ToString());
            }
        }

        protected void btnAddSpecification_Click(object sender, EventArgs e)
        {
            int specId;
            if (int.TryParse(rcbAddSpecification.SelectedValue, out specId))
            {
                ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
                DataSource.Add(GetActualSpecificationFromProxy(svc.GetSpecificationDetailsById(0, specId).Key));

                rcbAddSpecification.Text = "";
                rcbAddSpecification.ClearSelection();

                specificationsGrid.DataSource = DataSource;
                specificationsGrid.Rebind();
            }
            else
            {
                LaunchAlert("Select a specification to add to the list.");
            }
        }

        protected void rcbAddSpecification_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var specList = svc.GetSpecificationBySearchCriteriaWithExclusion(0, e.Text, ToExcludeFromDataSrouce);
                BindDropDownData(specList);
            }

        }



        #region Private Methods

        private void BindDropDownData(List<Specification> specList)
        {
            rcbAddSpecification.DataSource = specList;
            rcbAddSpecification.DataTextField = "SpecNumberAndTitle";
            rcbAddSpecification.DataValueField = "Pk_SpecificationId";
            rcbAddSpecification.DataBind();
        }

        /// <summary>
        /// Launch an alert popup
        /// </summary>
        /// <param name="errorText"></param>
        private void LaunchAlert(string errorText)
        {
            RadWindowAlert.RadAlert(errorText, 400, 150, "Error", "", "images/error.png");
        }

        /// <summary>
        /// Provide the simplified specification objects with required properties
        /// </summary>
        /// <param name="proxySpecifications">List of specifications</param>
        /// <returns>List of simplified specifications with required properties</returns>
        private List<Specification> GetActualSpecificationsFromProxy(List<Specification> proxySpecifications)
        {
            List<Specification> specifications = new List<Specification>();
            if ((proxySpecifications != null) && (proxySpecifications.Count > 0))
                proxySpecifications.ForEach(x => specifications.Add(GetActualSpecificationFromProxy(x)));

            return specifications;
        }

        /// <summary>
        /// Provide the simplified specification object with required properties
        /// </summary>
        /// <param name="proxySpecification">Specification</param>
        /// <returns>Specification with simplified properties</returns>
        private Specification GetActualSpecificationFromProxy(Specification proxySpecification)
        {
            Specification specification = new Specification()
            {
                Pk_SpecificationId = proxySpecification.Pk_SpecificationId,
                Number = proxySpecification.Number,
                Title = proxySpecification.Title,
                PrimeResponsibleGroupShortName = proxySpecification.PrimeResponsibleGroupShortName,
                SpecificationResponsibleGroups = proxySpecification.SpecificationResponsibleGroups,
                IsTS = proxySpecification.IsTS,
                IsActive = proxySpecification.IsActive,
                IsUnderChangeControl = proxySpecification.IsUnderChangeControl
            };

            return specification;
        }

        #endregion
    }
}