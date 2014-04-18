using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SPECIFICATION_GRID_DATA = "SpecificationsGridData";
        private const int CONST_MIN_SCROLL_HEIGHT = 100;

        #endregion

        #region Public Properties

        public event EventHandler AddSpecificationHandler;
        public bool IsEditMode { get; set; }
        public bool IsParentList {get; set;}

        public int ScrollHeight { get; set; }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                if (!IsEditMode)
                {
                    AddSpecificationLbl.Visible = false;                
                    txtAddSpecification.Visible = false;
                    btnAddSpecification.Visible = false;
                }
                else{
                    if(IsParentList){
                        AddSpecificationLbl.Text = "Add parent specification";
                    }
                    else{
                        AddSpecificationLbl.Text = "Add child specification"; 
                    }
                }
                specificationsGrid.ClientSettings.Scrolling.ScrollHeight = (ScrollHeight < CONST_MIN_SCROLL_HEIGHT) ? Unit.Pixel(CONST_MIN_SCROLL_HEIGHT) : Unit.Pixel(ScrollHeight);
            }
        }

        protected void specificationsGrid_PreRender(object sender, System.EventArgs e)
        {
            ChangeGridToEditMode();
        }

        protected void specificationsGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var specifications = DataSource ;
            specificationsGrid.DataSource = specifications.OrderByDescending(s=>s.Number);            
        }

        #region Private Methods

        /// <summary>
        /// Change all grid rows to Edit Mode
        /// </summary>
        private void ChangeGridToEditMode()
        {
            if (IsEditMode)
            {
                foreach (GridItem item in specificationsGrid.MasterTableView.Items)
                {
                    if (item is GridEditableItem)
                    {
                        GridEditableItem editableItem = item as GridDataItem;
                        editableItem.Edit = true;
                    }
                }
                specificationsGrid.Rebind();
            }
        }

        #endregion
    }
}