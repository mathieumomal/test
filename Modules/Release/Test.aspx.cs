using System;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Module.Release
{
    public partial class Test : System.Web.UI.Page
    {
        public const string DsIdKey = "ETSI_DS_ID";

        protected void Page_Load(object sender, EventArgs e)
        {
            // ------------ !!! DO NOT MODIFY !!!

            //Check user rights (only admin allowed to open this page)
            var userId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            var rightsService = ServicesFactory.Resolve<IRightsService>();
            var userRights = rightsService.GetGenericRightsForUser(userId);
            if (!userRights.HasRight(Enum_UserRights.Release_Test_Page_Access))
            {
                pnlTestPageBody.Visible = false;
                pnlTestPageMessage.Visible = true;
                pnlTestPageMessage.CssClass = "messageBox error";
                lblTestPage.Text = Localization.RightError;
                return;
            }

            // ------------ !!! TO MODIFY !!!

            TestLogs();
        }

        private void TestLogs()
        {
            //TEST DEBUG
            LogManager.Debug("Test debug");

            //TEST ERROR WITH EXCEPTION AND INNER EXCEPTIONS
            var ex = new Exception("message", 
                new Exception("inner message", 
                    new Exception("inner inner message")));
            LogManager.Error("Test error", ex);
        }

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="userInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo userInfo)
        {
            if (userInfo.UserID < 0)
                return 0;

            int personId;
            if (int.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                return personId;

            return 0;
        }
    }
}