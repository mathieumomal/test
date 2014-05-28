using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class VersionRemarksPopUp : System.Web.UI.Page
    {
        /// <summary>
        /// Remarks control
        /// </summary>
        protected Etsi.Ultimate.Controls.RemarksControl rcVersion;

        private int specVerId;
        private bool isEditMode;

        public static readonly string DsId_Key = "ETSI_DS_ID";

        protected void Page_Load(object sender, EventArgs e)
        {
            GetRequestParameters();

            var verSvc = ServicesFactory.Resolve<ISpecVersionService>();
            var version = verSvc.GetVersionsById(specVerId, GetPersonId());

            if (version.Key != null && version.Key.Remarks != null)
                rcVersion.DataSource = version.Key.Remarks.ToList();

            if (isEditMode)
            {
                rcVersion.IsEditMode = true;
                rcVersion.AddRemarkHandler += rcVersion_AddRemarkHandler;
            }
        }

        void rcVersion_AddRemarkHandler(object sender, EventArgs e)
        {
            var remarks = rcVersion.DataSource;
        }

        private void GetRequestParameters()
        {
            int tmpSpecId;
            if (Int32.TryParse(Request.QueryString["specVerId"], out tmpSpecId))
            {
                specVerId = tmpSpecId;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["IsEditMode"]))
                isEditMode = Convert.ToBoolean(Request.QueryString["IsEditMode"]);

        }

        private int GetPersonId()
        {
            var userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            if (userInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
            return 0;
        }
    }
}