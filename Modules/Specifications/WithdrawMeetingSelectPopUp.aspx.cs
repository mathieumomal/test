using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class WithdrawMeetingSelectPopUp : System.Web.UI.Page
    {
        /// <summary>
        /// Meeting control
        /// </summary>
        protected Etsi.Ultimate.Controls.MeetingControl mcWithdrawal;

        public string SelectedRelease { get; set; }

        private int SpecId;
        private int? RelId;

        public static readonly string DsId_Key = "ETSI_DS_ID";

        protected void Page_Load(object sender, EventArgs e)
        {
            GetRequestParameters();
            if (RelId.HasValue)
            {
                var relSvc = ServicesFactory.Resolve<IReleaseService>();
                var rel = relSvc.GetReleaseById(GetPersonId(), RelId.Value);
                SelectedRelease = rel.Key.Name;
            }
            
        }

        private void GetRequestParameters()
        {
            int tmpSpecId;
            if (Int32.TryParse(Request.QueryString["SpecId"], out tmpSpecId))
            {
                SpecId = tmpSpecId;
            }
            if (Request.QueryString["RelId"] != null)
            {
                int tmpRelId;
                if (Int32.TryParse(Request.QueryString["RelId"], out tmpRelId))
                {
                    RelId = tmpRelId;
                }
            }
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

        protected void btnConfirmWithdraw_Click(object sender, EventArgs e)
        {
            var mtgId = mcWithdrawal.SelectedMeetingId;
            if (mtgId == default(int))
            {
                lblError.Visible = true;
            }
            else
            {
                // Close for existing release
                if (RelId.HasValue)
                {
                    var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    specSvc.WithdrawForRelease(GetPersonId(), RelId.Value, SpecId, mtgId);
                    this.ClientScript.RegisterClientScriptBlock(GetType(), "CloseScript", "refreshParentPage('Releases','"+RelId.Value+"')", true);
                }
                
            }
        }
    }
}