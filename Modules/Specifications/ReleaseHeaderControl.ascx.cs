using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class ReleaseHeaderControl : System.Web.UI.UserControl
    {
        #region Public Properties

        public bool IsEditMode { get; set; }
        public Release ReleaseDataSource
        {
            set;
            get;
        }
        public Specification SpecificationDataSource
        {
            set;
            get;
        }
        public UserRightsContainer UserReleaseRights
        {
            get;
            set;
        }
        public int? PersonId
        {
            get;
            set;

        }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblReleaseName.Text = ReleaseDataSource.Name;
                string status = string.Empty;
                if (SpecificationDataSource.IsActive)
                {
                    bool? isWithdrawn = SpecificationDataSource.Specification_Release.FirstOrDefault(x => x.Fk_ReleaseId == ReleaseDataSource.Pk_ReleaseId).isWithdrawn;
                    if (isWithdrawn != null && !isWithdrawn.Value)
                    {
                        if (SpecificationDataSource.IsUnderChangeControl != null && SpecificationDataSource.IsUnderChangeControl.Value)
                            status = "Spec is UCC for this Release";
                        else
                            status = "Spec is in Draft status";
                    }
                    else
                        status = "Spec is Withdrawn from this Release";
                }
                else
                    status = "Spec is Withdrawn from this Release";

                lblStatus.Text = string.Format("({0})", status);
            }
            var specRelease = SpecificationDataSource.Specification_Release.FirstOrDefault(x => x.Fk_ReleaseId == ReleaseDataSource.Pk_ReleaseId);

            var versionsSvc = ServicesFactory.Resolve<ISpecVersionService>();
            SpecVersion version = versionsSvc.GetVersionsById(specRelease.Pk_Specification_ReleaseId, (PersonId != null ? PersonId.Value : default(int))).Key;

            if (version != null && version.Remarks.Count > 0)
            {
                var remark = version.Remarks.OrderBy(x => x.CreationDate).FirstOrDefault();
                lblLatestRemark.Text = ((remark.CreationDate != null) ? string.Format("({0})",
                                        remark.CreationDate.Value.ToString("yyyy-MM-dd")) : "") +
                                        remark.RemarkText;
                releaseRemarks.DataSource = version.Remarks.ToList();
            }
            releaseRemarks.IsEditMode = IsEditMode;
            releaseRemarks.ScrollHeight = 100;
            releaseRemarks.AddRemarkHandler += releaseRemarks_AddRemarkHandler;


        }

        void releaseRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            var specRelease = SpecificationDataSource.Specification_Release.FirstOrDefault(x => x.Fk_ReleaseId == ReleaseDataSource.Pk_ReleaseId);
            var versionsSvc = ServicesFactory.Resolve<ISpecVersionService>();
            SpecVersion version = versionsSvc.GetVersionsById(specRelease.Pk_Specification_ReleaseId, (PersonId != null ? PersonId.Value : default(int))).Key;

            List<Remark> datasource = releaseRemarks.DataSource;
            //Get display name
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(PersonId ?? default(int));
            datasource.Add(new Remark()
            {
                Fk_PersonId = PersonId,
                Fk_SpecificationId = SpecificationDataSource.Pk_SpecificationId,
                IsPublic = UserReleaseRights != null ? UserReleaseRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : false,
                CreationDate = DateTime.UtcNow,
                RemarkText = releaseRemarks.RemarkText,
                PersonName = personDisplayName
            });
            releaseRemarks.DataSource = datasource;
        }

        #endregion

        #region FIX : Cannot unregister UpdatePanel...
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.upRemarks != null)
                this.upRemarks.Unload += new EventHandler(UpdatePanel_Unload);
        }
        void UpdatePanel_Unload(object sender, EventArgs e)
        {
            this.RegisterUpdatePanel(sender as UpdatePanel);
        }
        public void RegisterUpdatePanel(UpdatePanel panel)
        {
            foreach (MethodInfo methodInfo in typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (methodInfo.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"))
                {
                    methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { upRemarks });
                }
            }
        }
        #endregion
    }
}