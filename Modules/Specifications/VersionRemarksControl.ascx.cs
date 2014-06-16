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
    public partial class VersionRemarksControl : System.Web.UI.UserControl
    {
        #region Constants

        private const string CONST_VERSION_REMARKS = "VersionRemarks";
        private const string CONST_REMARKS_USER_RIGHTS = "RemarksUserRights";

        #endregion

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
        /// User Permissions
        /// </summary>
        public UserRightsContainer UserReleaseRights
        {
            get
            {
                if (ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] == null)
                    ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] = new UserRightsContainer();

                return (UserRightsContainer)ViewState[ClientID + CONST_REMARKS_USER_RIGHTS];
            }
            set
            {
                ViewState[ClientID + CONST_REMARKS_USER_RIGHTS] = value;
            }
        }

        /// <summary>
        /// Person ID
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// Specification Version Details
        /// </summary>
        public SpecVersion SpecVersionDataSource { set; get; }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Release Header Control
        /// </summary>
        /// <param name="sender">Release Header Control</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            versionRemarks.UserRights = UserReleaseRights;
            versionRemarks.IsEditMode = IsEditMode;
            versionRemarks.ScrollHeight = 100;
            versionRemarks.AddRemarkHandler += versionRemarks_AddRemarkHandler;
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;

            if (!IsPostBack)
            {
                if (SpecVersionDataSource != null && SpecVersionDataSource.Remarks != null && SpecVersionDataSource.Remarks.Count > 0)
                {
                    versionRemarks.DataSource = SpecVersionDataSource.Remarks.ToList();
                    if (!basePage.VersionRemarks.Exists(x => x.Key == SpecVersionDataSource.Pk_VersionId))
                        basePage.VersionRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecVersionDataSource.Pk_VersionId, SpecVersionDataSource.Remarks.ToList()));
                }
                imgVersionRemarks.OnClientClick = "OpenVersionRemarksWindow" + this.ClientID + "(); return false;";
            }
            else
            {
                if (basePage.VersionRemarks.Exists(x => x.Key == SpecVersionDataSource.Pk_VersionId))
                    versionRemarks.DataSource = basePage.VersionRemarks.Find(x => x.Key == SpecVersionDataSource.Pk_VersionId).Value;
            }
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        protected void versionRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            List<Remark> datasource = versionRemarks.DataSource;
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(PersonId ?? default(int));
            datasource.Add(new Remark()
            {
                Fk_PersonId = PersonId,
                IsPublic = UserReleaseRights != null ? !UserReleaseRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = versionRemarks.RemarkText,
                PersonName = personDisplayName
            });
            versionRemarks.DataSource = datasource;
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
            if (basePage.VersionRemarks.Exists(x => x.Key == SpecVersionDataSource.Pk_VersionId))
            {
                var specVersionRemarks = basePage.VersionRemarks.Find(x => x.Key == SpecVersionDataSource.Pk_VersionId);
                basePage.VersionRemarks.Remove(specVersionRemarks);
            }
            basePage.VersionRemarks.Add(new KeyValuePair<int, List<Remark>>(SpecVersionDataSource.Pk_VersionId, datasource));
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
    }
}