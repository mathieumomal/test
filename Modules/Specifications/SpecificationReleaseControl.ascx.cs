﻿using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationReleaseControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_DATASOURCE = "SpecificationReleaseControl_datasource";
        private const string CONST_IS_EDIT_MODE = "SpecificationReleaseControl_isEditMode";
        private const string CONST_PERSON_ID = "SpecificationReleaseControl_personId";

        #endregion

        #region Public Properties

        public int? PersonId
        {
            get
            {
                return (int?)ViewState[CONST_PERSON_ID];
            }
            set
            {
                ViewState[CONST_PERSON_ID] = value;
            }
        }
        public bool IsEditMode
        {
            get
            {
                return ((bool?)ViewState[CONST_IS_EDIT_MODE]).GetValueOrDefault();
            }
            set
            {
                ViewState[CONST_IS_EDIT_MODE] = value;
            }
        }

        public Specification DataSource
        {
            get
            {
                if (ViewState[CONST_DATASOURCE] == null)
                    return new Specification();
                else
                    return (Specification)ViewState[CONST_DATASOURCE];
            }
            set
            {
                ViewState[CONST_DATASOURCE] = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Specification Release Control
        /// </summary>
        /// <param name="sender">Specification Release Control</param>
        /// <param name="e">Page load event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Load this control only in EditMode & when it has atleast one spec-release mapping
            if ((Request.QueryString["action"] == null || Request.QueryString["action"] != "create") && (DataSource.SpecificationReleases != null && DataSource.SpecificationReleases.Count > 0))
            {
                if (!IsPostBack)
                {
                    foreach (var release in DataSource.SpecificationReleases.OrderByDescending(sr => sr.SortOrder))
                    {
                        RadPanelItem item = new RadPanelItem();
                        item.Value = release.Pk_ReleaseId.ToString();
                        rpbReleases.Items.Add(item);
                    }

                    // Get the rights of the user
                    ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    var userRightsPerSpecRelease = specSvc.GetRightsForSpecReleases(PersonId.GetValueOrDefault(), DataSource);

                    SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
                    basePage.SpecReleaseRights.Clear();
                    userRightsPerSpecRelease.ForEach( x => basePage.SpecReleaseRights.Add(new KeyValuePair<int,UserRightsContainer>(x.Key.Pk_Specification_ReleaseId, x.Value)));
                }

                double panelHeight = rpbReleases.Height.Value;
                double panelItemsHeaderHeight = (rpbReleases.Items.Count) * 31;
                double iconsHeight = 40;
                double gridHeaderHeight = 30;
                double padding = 15;
                double scrollHeight = panelHeight - panelItemsHeaderHeight - iconsHeight - gridHeaderHeight - padding;

                //Dynamic Header & Content controls always needs to re-create for each postback
                foreach (RadPanelItem item in rpbReleases.Items)
                {
                    var release = DataSource.SpecificationReleases.Where(x => x.Pk_ReleaseId.ToString() == item.Value).FirstOrDefault();
                    var versions = DataSource.Versions.Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == release.Pk_ReleaseId : false)
                                                               .OrderByDescending(x => x.MajorVersion)
                                                               .ThenByDescending(x => x.TechnicalVersion)
                                                               .ThenByDescending(x => x.EditorialVersion).ToList();

                    var specRelease = DataSource.Specification_Release.Where(x => x.Fk_ReleaseId.ToString() == item.Value && x.Fk_SpecificationId == DataSource.Pk_SpecificationId).FirstOrDefault();
                    CustomHeaderTemplate customHeaderTemplate = new CustomHeaderTemplate(specRelease, IsEditMode, PersonId.GetValueOrDefault(), this.Page);
                    CustomContentTemplate customContentTemplate = new CustomContentTemplate(specRelease, versions, IsEditMode, PersonId.GetValueOrDefault(), this.Page, scrollHeight);
                    item.HeaderTemplate = customHeaderTemplate;
                    item.ApplyHeaderTemplate();
                    item.ContentTemplate = customContentTemplate;
                    customContentTemplate.InstantiateIn(item);
                    item.DataBind();
                    item.ChildGroupHeight = 0;
                }

                if (Request.QueryString["Rel"] != null)
                {
                    var item = rpbReleases.Items.FindItemByValue(Request.QueryString["Rel"]);
                    if (item != null)
                    {
                        item.Expanded = true;
                    }
                }
                else
                {
                    if (rpbReleases.Items.Count > 0)
                        rpbReleases.Items[0].Expanded = true;
                }
            }
        }

        #endregion
    }
}