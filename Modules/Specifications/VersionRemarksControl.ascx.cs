using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    /// <summary>
    /// User Control for Version Remarks which are included in Specification Release Tab
    /// </summary>
    public partial class VersionRemarksControl : System.Web.UI.UserControl
    {
        #region Public Properties

        /// <summary>
        /// Remarks Control
        /// </summary>
        protected RemarksControl versionRemarks;

        /// <summary>
        /// True - Edit Mode 
        /// False - View Mode
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Person ID
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// Specification Version Details
        /// </summary>
        public SpecVersion SpecVersion { set; get; }

        /// <summary>
        /// Specification Release ID
        /// </summary>
        public int SpecReleaseID { get; set; }

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
            versionRemarks.UserRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecReleaseID).FirstOrDefault().Value;

            if (!IsPostBack)
            {
                if (SpecVersion != null && SpecVersion.Remarks != null && SpecVersion.Remarks.Count > 0)
                {
                    var remarks = GetActualRemarksFromProxy(SpecVersion.Remarks.ToList());
                    versionRemarks.DataSource = remarks;
                    if (IsEditMode)
                    {
                        if (basePage.VersionRemarks.Exists(x => x.Key == SpecVersion.Pk_VersionId))
                        {
                            var specVersionRemarks = basePage.VersionRemarks.Find(x => x.Key == SpecVersion.Pk_VersionId);
                            basePage.VersionRemarks.Remove(specVersionRemarks);
                        }
                        basePage.VersionRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecVersion.Pk_VersionId, remarks));                       
                    }
                }
                imgVersionRemarks.OnClientClick = "OpenVersionRemarksWindow" + this.ClientID + "(); return false;";
            }
            else
            {
                if (IsEditMode)
                {
                    var specVersionRemarks = basePage.VersionRemarks.Find(x => x.Key == SpecVersion.Pk_VersionId);
                    if (!specVersionRemarks.Equals(default(KeyValuePair<int, List<Remark>>)))
                        versionRemarks.DataSource = specVersionRemarks.Value;
                }
            }

            versionRemarks.IsEditMode = IsEditMode;
            versionRemarks.ScrollHeight = 100;
            if (IsEditMode)
                versionRemarks.AddRemarkHandler += versionRemarks_AddRemarkHandler;
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        protected void versionRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
            var userRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecReleaseID).FirstOrDefault();

            List<Remark> datasource = versionRemarks.DataSource;
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(PersonId ?? default(int));
            datasource.Add(new Remark()
            {
                Fk_PersonId = PersonId,
                IsPublic = userRights.Value != null ? !userRights.Value.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = versionRemarks.RemarkText,
                PersonName = personDisplayName
            });
            versionRemarks.DataSource = datasource;

            if (basePage.VersionRemarks.Exists(x => x.Key == SpecVersion.Pk_VersionId))
            {
                var specVersionRemarks = basePage.VersionRemarks.Find(x => x.Key == SpecVersion.Pk_VersionId);
                basePage.VersionRemarks.Remove(specVersionRemarks);
            }
            basePage.VersionRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecVersion.Pk_VersionId, datasource));
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
            if (this.upContentRemarks != null)
                this.upContentRemarks.Unload += new EventHandler(UpdatePanel_Unload);
        }

        /// <summary>
        /// Unload event of update panel
        /// </summary>
        /// <param name="sender">Update Panel</param>
        /// <param name="e">Event Arguments</param>
        private void UpdatePanel_Unload(object sender, EventArgs e)
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
                Fk_VersionId = x.Fk_VersionId,
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