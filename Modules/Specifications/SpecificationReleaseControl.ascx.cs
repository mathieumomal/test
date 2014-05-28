using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["action"] == null || Request.QueryString["action"] != "create")
            {
                // Get the rights of the user
                ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                var userRightsPerSpecRelease = specSvc.GetRightsForSpecReleases(PersonId.GetValueOrDefault(), DataSource);

                rpbReleases.EnableViewState = false;

                bool removeInhibitPromoteRight = false;

                foreach (var release in DataSource.SpecificationReleases.OrderByDescending(sr => sr.SortOrder))
                {

                    RadPanelItem item = new RadPanelItem();
                    var rights = userRightsPerSpecRelease.Where(r => r.Key.Fk_ReleaseId == release.Pk_ReleaseId).FirstOrDefault().Value;

                    if (removeInhibitPromoteRight)
                    {
                        rights.RemoveRight(Enum_UserRights.Specification_InhibitPromote, null);
                        rights.RemoveRight(Enum_UserRights.Specification_RemoveInhibitPromote, null);
                        removeInhibitPromoteRight = true;
                    }

                    item.HeaderTemplate = new CustomHeaderTemplate(release, DataSource, IsEditMode, rights, PersonId.GetValueOrDefault(), this.Page);
                    item.Value = release.Pk_ReleaseId.ToString();
                    rpbReleases.Items.Add(item);
                }




                ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                var versionsList = svc.GetVersionsBySpecId(DataSource.Pk_SpecificationId);

                CustomContentTemplate template;

                double panelHeight = rpbReleases.Height.Value;
                double panelItemsHeaderHeight = (rpbReleases.Items.Count) * 31;
                double iconsHeight = 30;
                double gridHeaderHeight = 30;
                double padding = 15;
                double scrollHeight = panelHeight - panelItemsHeaderHeight - iconsHeight - gridHeaderHeight - padding;

                foreach (RadPanelItem item in rpbReleases.Items)
                {
                    int releaseId = Convert.ToInt32(item.Value);

                    var datasource = versionsList.Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == releaseId : false)
                                                               .OrderByDescending(x => x.MajorVersion)
                                                               .ThenByDescending(x => x.TechnicalVersion)
                                                               .ThenByDescending(x => x.EditorialVersion).ToList();
                    var rights = userRightsPerSpecRelease.Where(r => r.Key.Fk_ReleaseId == releaseId).FirstOrDefault().Value;
                    template = new CustomContentTemplate(datasource, rights, PersonId.GetValueOrDefault(), DataSource.Pk_SpecificationId, releaseId, IsEditMode, this.Page, scrollHeight);
                    item.ContentTemplate = template;
                    template.InstantiateIn(item);
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