using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class RemarksPopup : System.Web.UI.Page
    {
        #region Variables & Constants

        private string remarksModule;
        private int remarksModulePrimaryKey;
        private bool isEditMode;
        private const string DsId_Key = "ETSI_DS_ID";
        private const int ErrorFadeTimeout = 10000;
        protected RemarksControl remarksControl;

        #endregion

        #region Events

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            GetRequestParameters();
            UpdateSettings();

            if (!IsPostBack)
            {
                var remarkService = ServicesFactory.Resolve<IRemarkService>();
                var remarkServiceResult = remarkService.GetRemarks(remarksModule, remarksModulePrimaryKey, GetPersonId());
                remarksControl.UserRights = remarkServiceResult.Rights;
                remarksControl.DataSource = remarkServiceResult.Result;
            }
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        protected void remarksControl_AddRemarkHandler(object sender, EventArgs e)
        {
            List<Remark> datasource = remarksControl.DataSource;

            var remark = GetNewRemark();
            if (remark != null)
                datasource.Add(remark);

            remarksControl.DataSource = datasource;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var remarks = remarksControl.DataSource;
            if (remarks != null && remarks.Count > 0)
            {
                var remarkService = ServicesFactory.Resolve<IRemarkService>();
                var remarkServiceResult = remarkService.UpdateRemarks(remarks, GetPersonId());
                if ((!remarkServiceResult.Result) && (remarkServiceResult.Report.ErrorList.Count > 0))
                {
                    pnlErrorMessage.Visible = true;
                    remarkServiceResult.Report.ErrorList.ForEach(x => lblErrorMessage.Text = lblErrorMessage.Text + x + "<br/>");
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "setTimeout(function(){ $('#" + pnlErrorMessage.ClientID + "').hide('slow');} , " + ErrorFadeTimeout + ");", true);
                }
                else
                {
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "setTimeout(function(){ $('#" + btnClose.ClientID + "').trigger('click');}, " + 100 + ");", true);
                }
            }
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        private void GetRequestParameters()
        {
            if (Request.QueryString["remarksModule"] != null)
                remarksModule = Request.QueryString["remarksModule"].ToString();

            if (Request.QueryString["remarksModulePrimaryKey"] != null)
            {
                int tempPrimaryKey;
                if (int.TryParse(Request.QueryString["remarksModulePrimaryKey"].ToString(), out tempPrimaryKey))
                    remarksModulePrimaryKey = tempPrimaryKey;
            }

            if (Request.QueryString["isEditMode"] != null)
            {
                bool tempIsEditMode;
                if (bool.TryParse(Request.QueryString["isEditMode"].ToString(), out tempIsEditMode))
                {
                    isEditMode = tempIsEditMode;
                }
            }
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private void UpdateSettings()
        {
            remarksControl.IsEditMode = isEditMode;
            remarksControl.ScrollHeight = 100;

            if (isEditMode)
                remarksControl.AddRemarkHandler += remarksControl_AddRemarkHandler;
            else
                remarksFooter.Style.Add("display", "none");
        }

        /// <summary>
        /// Gets the new remark.
        /// </summary>
        /// <returns>Remark entity</returns>
        private Remark GetNewRemark()
        {
            //Get person name
            int personID = GetPersonId();
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(personID);

            //User Rights
            var userRights = remarksControl.UserRights;

            //New Remark entry
            var remark = new Remark();
            remark.Fk_PersonId = personID;
            remark.IsPublic = userRights != null ? !userRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true;
            remark.CreationDate = DateTime.UtcNow;
            remark.RemarkText = remarksControl.RemarkText;
            remark.PersonName = personDisplayName;

            //Provide necessary foreign key based on the module
            if (!String.IsNullOrEmpty(remarksModule))
            {
                switch (remarksModule.ToLower())
                {
                    case "version":
                        remark.Fk_VersionId = remarksModulePrimaryKey;
                        break;
                    case "specrelease":
                        remark.Fk_SpecificationReleaseId = remarksModulePrimaryKey;
                        break;
                }
            }

            return remark;
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
            else
            {
                int personID;
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
            return 0;
        } 

        #endregion
    }
}