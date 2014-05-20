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

        #endregion

        #region Public Properties

        public bool IsEditMode { get; set; }
        public Specification DataSource
        {
            set;
            get;
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var release in DataSource.SpecificationReleases)
            {
                RadPanelItem item = new RadPanelItem();
                item.HeaderTemplate = new CustomHeaderTemplate(release, DataSource, this.Page);
                item.Value = release.Pk_ReleaseId.ToString();
                rpbReleases.Items.Add(item);
            }



            ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
            var versionsList = svc.GetVersionsBySpecId(DataSource.Pk_SpecificationId);

            CustomContentTemplate template;
            foreach (RadPanelItem item in rpbReleases.Items)
            {
                int releaseId = Convert.ToInt32(item.Value);

                var datasource = versionsList.Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == releaseId : false)
                                                           .OrderByDescending(x => x.MajorVersion)
                                                           .ThenByDescending(x => x.TechnicalVersion)
                                                           .ThenByDescending(x => x.EditorialVersion).ToList();

                template = new CustomContentTemplate(datasource, this.Page);
                item.ContentTemplate = template;
                template.InstantiateIn(item);
                item.DataBind();
                item.ChildGroupHeight = 0;
            }
        }

        #endregion
    }
}