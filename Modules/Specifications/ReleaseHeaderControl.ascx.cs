using Etsi.Ultimate.Controls;
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

        protected RemarksControl releaseRemarks;

        /// <summary>
        /// Specification Release
        /// </summary>
        public Specification_Release SpecRelease { get; set; }

        /// <summary>
        /// Edit Mode - True
        /// View Mode - False
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Person ID
        /// </summary>
        public int? PersonId { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Release Header Control
        /// </summary>
        /// <param name="sender">Release Header Control</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
            if(SpecRelease != null)
            releaseRemarks.UserRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecRelease.Pk_Specification_ReleaseId).FirstOrDefault().Value;

            if (!IsPostBack)
            {
                lblReleaseName.Text = SpecRelease.Release.Name;
                string status = string.Empty;
                if (SpecRelease.Specification.IsActive)
                {
                    bool? isWithdrawn = SpecRelease.isWithdrawn;
                    if (isWithdrawn != null && !isWithdrawn.Value)
                    {
                        if (SpecRelease.Specification.IsUnderChangeControl != null && SpecRelease.Specification.IsUnderChangeControl.Value)
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

                if (SpecRelease != null && SpecRelease.Remarks != null && SpecRelease.Remarks.Count > 0)
                {
                    var remarks = GetActualRemarksFromProxy(SpecRelease.Remarks.ToList());
                    releaseRemarks.DataSource = remarks;

                    if (IsEditMode)
                    {
                        if (basePage.SpecReleaseRemarks.Exists(x => x.Key == SpecRelease.Pk_Specification_ReleaseId))
                        {
                            var specReleaseRemarks = basePage.SpecReleaseRemarks.Find(x => x.Key == SpecRelease.Pk_Specification_ReleaseId);
                            basePage.SpecReleaseRemarks.Remove(specReleaseRemarks);
                        }
                        basePage.SpecReleaseRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecRelease.Pk_Specification_ReleaseId, remarks));
                    }

                    Remark latestRemark;
                    if (releaseRemarks.UserRights.HasRight(Enum_UserRights.Remarks_ViewPrivate))
                        latestRemark = SpecRelease.Remarks.OrderByDescending(x => x.CreationDate ?? DateTime.MinValue).FirstOrDefault();
                    else
                        latestRemark = SpecRelease.Remarks.Where(r => r.IsPublic.GetValueOrDefault()).OrderByDescending(x => x.CreationDate ?? DateTime.MinValue).FirstOrDefault();

                    lblLatestRemark.Text = ((latestRemark.CreationDate != null) ? string.Format("({0})", latestRemark.CreationDate.Value.ToString("yyyy-MM-dd")) : String.Empty) + latestRemark.RemarkText;
                }
                imgRemarks.OnClientClick = "OpenReleaseHeaderRemarksWindow" + this.ClientID + "(); return false;";
            }
            else
            {
                if(IsEditMode)
                {
                    var specReleaseRemarks = basePage.SpecReleaseRemarks.Find(x => x.Key == SpecRelease.Pk_Specification_ReleaseId);
                    if (!specReleaseRemarks.Equals(default(KeyValuePair<int, List<Remark>>)))
                        releaseRemarks.DataSource = specReleaseRemarks.Value;
                }
            }

            releaseRemarks.IsEditMode = IsEditMode;
            releaseRemarks.ScrollHeight = 100;
            if (IsEditMode)
                releaseRemarks.AddRemarkHandler += releaseRemarks_AddRemarkHandler;
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        protected void releaseRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
            var userRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecRelease.Pk_Specification_ReleaseId).FirstOrDefault();

            List<Remark> datasource = releaseRemarks.DataSource;
            //Get display name
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(PersonId ?? default(int));
            datasource.Add(new Remark()
            {
                Fk_PersonId = PersonId,
                IsPublic = userRights.Value != null ? !userRights.Value.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = releaseRemarks.RemarkText,
                PersonName = personDisplayName
            });
            releaseRemarks.DataSource = datasource;

            if (basePage.SpecReleaseRemarks.Exists(x => x.Key == SpecRelease.Pk_Specification_ReleaseId))
            {
                var specReleaseRemarks = basePage.SpecReleaseRemarks.Find(x => x.Key == SpecRelease.Pk_Specification_ReleaseId);
                basePage.SpecReleaseRemarks.Remove(specReleaseRemarks);
            }
            basePage.SpecReleaseRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecRelease.Pk_Specification_ReleaseId, datasource));
        }

        #endregion

        #region FIX : Cannot unregister UpdatePanel...

        /// <summary>
        /// Override OnInit event to register missing update panel for ajax
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.upHeaderRemarks != null)
                this.upHeaderRemarks.Unload += new EventHandler(UpdatePanel_Unload);
        }

        /// <summary>
        /// Unload event of update panel
        /// </summary>
        /// <param name="sender">Update Panel</param>
        /// <param name="e">Event Arguments</param>
        void UpdatePanel_Unload(object sender, EventArgs e)
        {
            this.RegisterUpdatePanel(sender as UpdatePanel);
        }

        /// <summary>
        /// Register Update Panel to page
        /// </summary>
        /// <param name="panel">Update Panel</param>
        public void RegisterUpdatePanel(UpdatePanel panel)
        {
            foreach (MethodInfo methodInfo in typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (methodInfo.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"))
                {
                    methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Actual POCO entities for Remark objects
        /// </summary>
        /// <param name="proxyRemarks">Proxy Remarks</param>
        /// <returns>List of Remarks</returns>
        private List<Remark> GetActualRemarksFromProxy(List<Remark> proxyRemarks)
        {
            List<Remark> remarks = new List<Remark>();
            proxyRemarks.ForEach(x => remarks.Add(new Remark()
            {
                Pk_RemarkId = x.Pk_RemarkId,
                Fk_PersonId = x.Fk_PersonId,
                Fk_SpecificationReleaseId = x.Fk_SpecificationReleaseId,
                IsPublic = x.IsPublic,
                CreationDate = x.CreationDate,
                RemarkText = x.RemarkText,
                PersonName = x.PersonName,
            }));
            return remarks;
        }

        #endregion
    }
}