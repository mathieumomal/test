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
        private const string CONST_REMARKS_GRID_DATA = "RemarksGridData";
        private const string CONST_REMARKS_USER_RIGHTS = "RemarksUserRights";

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

        public UserRightsContainer UserRights
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

        public List<Remark> SpecReleaseRemarks
        {
            get
            {
                if (ViewState[ClientID + CONST_REMARKS_GRID_DATA] == null)
                    ViewState[ClientID + CONST_REMARKS_GRID_DATA] = new List<Remark>();

                return (List<Remark>)ViewState[ClientID + CONST_REMARKS_GRID_DATA];
            }
            set
            {
                ViewState[ClientID + CONST_REMARKS_GRID_DATA] = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Release Header Control
        /// </summary>
        /// <param name="sender">Release Header Control</param>
        /// <param name="e">Event arguments</param>
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

                //Remarks
                var specRelease = SpecificationDataSource.Specification_Release.FirstOrDefault(x => x.Fk_ReleaseId == ReleaseDataSource.Pk_ReleaseId);
                if (specRelease != null && specRelease.Remarks != null && specRelease.Remarks.Count > 0)
                {
                    releaseRemarks.UserRights = UserRights = UserReleaseRights;
                    releaseRemarks.DataSource = SpecReleaseRemarks = specRelease.Remarks.ToList();
                    var latestRemark = specRelease.Remarks.OrderByDescending(x => x.CreationDate ?? DateTime.MinValue).FirstOrDefault();
                    lblLatestRemark.Text = ((latestRemark.CreationDate != null) ? string.Format("({0})", latestRemark.CreationDate.Value.ToString("yyyy-MM-dd")) : String.Empty) + latestRemark.RemarkText;
                }
                imgRemarks.OnClientClick = "OpenReleaseHeaderRemarksWindow" + this.ClientID + "(); return false;";
            }
            else
            {
                releaseRemarks.UserRights = UserRights;
                releaseRemarks.DataSource = SpecReleaseRemarks;
            }

            releaseRemarks.IsEditMode = IsEditMode;
            releaseRemarks.ScrollHeight = 100;
            releaseRemarks.AddRemarkHandler += releaseRemarks_AddRemarkHandler;
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        protected void releaseRemarks_AddRemarkHandler(object sender, EventArgs e)
        {
            List<Remark> datasource = releaseRemarks.DataSource;
            //Get display name
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(PersonId ?? default(int));
            datasource.Add(new Remark()
            {
                Fk_PersonId = PersonId,
                IsPublic = UserRights != null ? !UserRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = releaseRemarks.RemarkText,
                PersonName = personDisplayName
            });
            releaseRemarks.DataSource = SpecReleaseRemarks = datasource;
        }

        #endregion

        #region FIX : Cannot unregister UpdatePanel...
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.upHeaderRemarks != null)
                this.upHeaderRemarks.Unload += new EventHandler(UpdatePanel_Unload);
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
                    methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
                }
            }
        }
        #endregion
    }
}