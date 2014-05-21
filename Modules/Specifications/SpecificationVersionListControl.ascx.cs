﻿using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationVersionListControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_SPECIFICATION_GRID_DATA = "SpecificationListControlData_{0}";
        private const string CONST_SELECTED_TAB = "SPEC_SELECTED_TAB";
        private const string CONST_SPEC_ID = "SPECIFICATION_ID";
        private const string CONST_REL_ID = "RELEASE_ID";
        private const string CONST_PERSON_ID = "PERSON_ID";

        #endregion

        #region Public Properties
        public bool IsEditMode { get; set; }
        public bool IsParentList { get; set; }
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
        /// <summary>
        /// The list of rights of the user regarding the release.
        /// </summary>
        public UserRightsContainer UserReleaseRights
        {
            get;
            set;
        }
        public List<SpecVersion> DataSource
        {
            set;
            get;
        }
        public int? SpecId {
            get; set;
        
        }
        public int? ReleaseId {
            get; set;
        
        }
        public int? PersonId
        {
            get; set;
          
        }
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            specificationsVersionGrid.DataSource = DataSource;
            specificationsVersionGrid.DataBind();

            if (!IsPostBack)
            {
                imgForceTransposition.Visible = !IsEditMode && UserReleaseRights.HasRight(Enum_UserRights.Specification_ForceTransposition);
                imgUnforceTransposition.Visible = !IsEditMode && UserReleaseRights.HasRight(Enum_UserRights.Specification_UnforceTransposition);

                if (!IsEditMode)
                {
                }
                else
                {
                }

            }
        }

        protected void specificationsVersionGrid_PreRender(object sender, System.EventArgs e)
        {
            //ChangeGridToEditMode();
        }

        protected void specificationsVersionGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            specificationsVersionGrid.DataSource = DataSource;
        }

        #endregion

        protected void specificationsVersionGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                if (IsEditMode && item["SpecificationActions"].FindControl("btnRemoveSpec") != null)
                    ((ImageButton)item["SpecificationActions"].FindControl("btnRemoveSpec")).Visible = true;
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
                specSvc.ForceTranspositionForRelease(PersonId.Value, ReleaseId.Value, SpecId.Value);
                Redirect();
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

        /// <summary>
        /// Redirect user. Removes previous "selectedTab" and "Rel" flags.
        /// </summary>
        private void Redirect()
        {
            var address = HttpContext.Current.Request.Url.AbsoluteUri.Split('&').ToList();
            address.RemoveAll(s => s.Contains("selectedTab"));
            address.RemoveAll(s => s.Contains("Rel"));
            Response.Redirect(string.Join("&",address)+"&selectedTab=Releases&Rel="+ReleaseId.Value);

        }

    }
}