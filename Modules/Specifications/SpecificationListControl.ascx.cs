using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SPECIFICATION_GRID_DATA = "SpecificationsGridData";
        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";
        private const int CONST_MIN_SCROLL_HEIGHT = 50;

        #endregion

        #region Public Properties

        public bool IsEditMode { get; set; }
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
                if (ViewState[CONST_SPECIFICATION_GRID_DATA] == null)
                    ViewState[CONST_SPECIFICATION_GRID_DATA] = new List<Specification>();

                return (List<Specification>)ViewState[CONST_SPECIFICATION_GRID_DATA];
            }
            set
            {
                ViewState[CONST_SPECIFICATION_GRID_DATA] = value;
                specificationsGrid.Rebind();
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
            //int WiId;
            //var WiIdStr = ((ImageButton)sender).CommandArgument;
            //if (!string.IsNullOrEmpty(WiIdStr) && int.TryParse(((ImageButton)sender).CommandArgument, out WiId))
            //{
            //    //If modifiedDataSource is set; then remove from it else take data from DataSource to modifiedDataSource & remove
            //    if (modifiedDataSource.Count > 0)
            //    {
            //        var removedWi = modifiedDataSource.FirstOrDefault(x => x.Pk_WorkItemUid == WiId);
            //        modifiedDataSource.RemoveAll(x => x.Pk_WorkItemUid == WiId);

            //        SetHiddenWisValue(modifiedDataSource);
            //        BindGrid(relatedWiGrid_Edit, modifiedDataSource);
            //    }
            //    else
            //    {
            //        modifiedDataSource = DataSource.ToList();
            //        modifiedDataSource.RemoveAll(x => x.Pk_WorkItemUid == WiId);

            //        SetHiddenWisValue(modifiedDataSource);
            //        BindGrid(relatedWiGrid_Edit, modifiedDataSource);
            //    }
            //}

        }

        #endregion

        protected void specificationsGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                if (IsEditMode && item["SpecificationActions"].FindControl("btnRemoveSpec") != null)
                    ((ImageButton)item["SpecificationActions"].FindControl("btnRemoveSpec")).Visible = true;
            }
        }

        protected void btnAddSpecification_Click(object sender, EventArgs e)
        {
            int specId;
            if (int.TryParse(rcbAddSpecification.SelectedValue, out specId))
            {
                ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
                DataSource.Add(svc.GetSpecificationDetailsById(0, specId).Key);

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
                var specList = svc.GetSpecificationBySearchCriteria(0, e.Text );
                BindDropDownData(specList);
            }

        }



        #region Private Methods

        private void BindDropDownData(List<Specification> specList)
        {
            rcbAddSpecification.DataSource = specList;
            rcbAddSpecification.DataTextField = "Title";
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

        ///// <summary>
        ///// Change all grid rows to Edit Mode
        ///// </summary>
        //private void ChangeGridToEditMode()
        //{
        //    if (IsEditMode)
        //    {
        //        foreach (GridItem item in specificationsGrid.MasterTableView.Items)
        //        {
        //            if (item is GridEditableItem)
        //            {
        //                GridEditableItem editableItem = item as GridDataItem;
        //                editableItem.Edit = true;
        //            }
        //        }
        //        specificationsGrid.Rebind();
        //    }
        //}

        #endregion
    }
}