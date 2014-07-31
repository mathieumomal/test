using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class DefinitiveWithdrawlMeetingSelectPopUp : System.Web.UI.Page
    {
        /// <summary>
        /// Meeting control
        /// </summary>
        protected Etsi.Ultimate.Controls.MeetingControl withrawalMeetinfVal;

        private int SpecId;

        public const string DsId_Key = "ETSI_DS_ID";

        protected void Page_Load(object sender, EventArgs e)
        {
            GetRequestParameters();
        }

        private void GetRequestParameters()
        {
            int tmpSpecId;
            if (Int32.TryParse(Request.QueryString["SpecId"], out tmpSpecId))
            {
                SpecId = tmpSpecId;
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
            GetRequestParameters();
            var mtgId = withrawalMeetinfVal.SelectedMeetingId;
            bool operationResult = false; 
            if (mtgId == default(int))
            {
                lblError.Visible = true;
            }
            else
            {                
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                operationResult = specSvc.DefinitivelyWithdrawSpecification(GetPersonId(), SpecId, mtgId);               
                this.ClientScript.RegisterStartupScript(GetType(), "ConfirmScript", "<script language='javascript'> returnToParent('" + operationResult.ToString() + "')</script>");                         
            }
        }

    }
}