using System;
using System.Collections.Generic;
using System.Web.UI;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Module.Specifications.PopUps
{
    public partial class RemoveSpecReleasePopUp : Page
    {
        #region events
        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {}

        /// <summary>
        /// Handles the Click event of the save button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var getParams = GetRequestParameters();

            var specService = ServicesFactory.Resolve<ISpecificationService>();
            var response = specService.RemoveSpecRelease(getParams.Value, getParams.Key, GetPersonId());
            if (!response.Result || response.Report.GetNumberOfErrors() > 0)
            {
                ThrowAnError(string.Join(",",response.Report.ErrorList.ToArray()));
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "Close", "setTimeout(function(){ $('#" + btnCancel.ClientID + "').trigger('click');}, " + 100 + ");", true);
            }
        }
        #endregion

        #region helper
        /// <summary>
        /// Retreive the URL parameters (Release Id as key, and spec id as value)
        /// </summary>
        private KeyValuePair<int, int> GetRequestParameters()
        {
            var relId = (Request.QueryString["RelId"] != null) ? Convert.ToInt32(Request.QueryString["RelId"]) : 0;
            var specId = (Request.QueryString["SpecId"] != null) ? Convert.ToInt32(Request.QueryString["SpecId"]) : 0;
            if (relId == 0 || specId == 0)
                ThrowAnError(Localization.GenericError);

            return new KeyValuePair<int, int>(relId, specId);
        }

        /// <summary>
        /// Gets the person identifier.
        /// </summary>
        /// <returns></returns>
        private int GetPersonId()
        {
            var userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            if (userInfo.UserID < 0)
                return 0;

            int personId;
            return int.TryParse(userInfo.Profile.GetPropertyValue(Localization.DsIdKey), out personId) ? personId : 0;
        } 

        /// <summary>
        /// Display an error
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ThrowAnError(string errorMessage)
        {
            formContent.Visible = false;
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "messageBox error";
            lblMessage.Text = errorMessage;
        }
        #endregion
    }
}