using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Etsi.Ultimate.Module.Specifications.App_LocalResources;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class ITURecommendation : System.Web.UI.Page
    {
        #region constants
        private const string ConstKey = "Key";
        private const string ConstValue = "Value";
        private const string DsIdKey = "ETSI_DS_ID";

        private readonly Dictionary<string, string> _ituRecommendations = new Dictionary<string, string>
        {
            {"ITU-T Q.1741", "ITU-T Q.1741"},
            {"ITU-R M.1457", "ITU-R M.1457"},
            {"ITU-R M.2012", "ITU-R M.2012"}
        };
        #endregion

        #region properties
        protected Controls.MeetingControl RcbSaMeeting;
        private int PersonId { get; set; }

        #endregion

        #region events

        /// <summary>
        /// Page load method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ResetPanelVisibility();
            PersonId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());

            RcbSaMeeting.NewestFirst = true;
            RcbSaMeeting.TbRestriction = "SP";

            if (!IsPostBack)
            {
                LoadComponentsData();
            }
        }

        /// <summary>
        /// Export action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            var ituSvc = ServicesFactory.Resolve<IItuRecommendationService>();
            var startReleaseId = ConvertStringToInt(rcbStartRelease.SelectedValue);
            var endReleaseId = ConvertStringToInt(rcbEndRelease.SelectedValue);
            var ituRecommendationName = rcbItuRec.SelectedValue;
            var saPlenaryMeetingId = RcbSaMeeting.SelectedMeetingId;
            var seedFilePath = UploadSeedFile();
            if (String.IsNullOrEmpty(seedFilePath))
                return;
            
            var result = ituSvc.ExportItuRecommendation(PersonId, ituRecommendationName, startReleaseId, endReleaseId, saPlenaryMeetingId, seedFilePath);
            if (result.Report.GetNumberOfErrors() == 0)
            {
                Response.Redirect(result.Result);
            }
            else
            {
                var errorMessage = new StringBuilder();
                result.Report.ErrorList.ForEach(x => errorMessage.Append(x).Append("\n\r"));
                ThrowAnError(errorMessage.ToString(), false);
            }
        }
        #endregion

        #region private
        private void LoadComponentsData()
        {
            LoadItuContributions();
            LoadReleases();
        }
        #endregion

        #region DropDown Collection Loading
        private void LoadItuContributions()
        {
            rcbItuRec.DataValueField = ConstKey;
            rcbItuRec.DataTextField = ConstValue;
            rcbItuRec.DataSource = _ituRecommendations;
            rcbItuRec.DataBind();
            //Set default value
            rcbItuRec.SelectedIndex = 0;
        }

        private void LoadReleases()
        {
            var releaseSvc = ServicesFactory.Resolve<IReleaseService>();
            var releases = releaseSvc.GetAllReleases(PersonId);
            if (releases.Key == null || releases.Key.Count == 0) return;
            var releasesDictionnary = releases.Key.OrderByDescending( r => r.SortOrder).ToDictionary(x => x.Pk_ReleaseId, x => x.ShortName);

            //Start release
                rcbStartRelease.DataValueField = ConstKey;
                rcbStartRelease.DataTextField = ConstValue;
                rcbStartRelease.DataSource = releasesDictionnary;
                rcbStartRelease.DataBind();

                //Set default value
                rcbStartRelease.Items.Insert(0, new RadComboBoxItem("-", "0"));
                rcbStartRelease.SelectedIndex = 0;
                
            //End release
                rcbEndRelease.DataValueField = ConstKey;
                rcbEndRelease.DataTextField = ConstValue;
                rcbEndRelease.DataSource = releasesDictionnary;
                rcbEndRelease.DataBind();

                //Set default value
                rcbEndRelease.Items.Insert(0, new RadComboBoxItem("-", "0"));
                rcbEndRelease.SelectedIndex = 0;
        }
        #endregion

        #region utils
        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="userInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo userInfo)
        {
            if (userInfo.UserID < 0)
                return 0;
            else
            {
                int personId;
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                    return personId;
            }
            return 0;
        }

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int ConvertStringToInt(string str)
        {
            int result;
            return int.TryParse(str, out result) ? result : 0;
        }

        /// <summary>
        /// Display an error
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <param name="allowToProceed">Allow/stop the user to proceed further actions</param>
        private void ThrowAnError(string errorMessage, bool allowToProceed)
        {
            pnlItuRecommendations.Visible = allowToProceed;
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "messageBox error";
            lblMsg.Text = errorMessage;
        }

        private void ResetPanelVisibility()
        {
            pnlItuRecommendations.Visible = true;
            pnlMsg.Visible = false;
        }

        /// <summary>
        /// Upload seed file
        /// </summary>
        /// <returns></returns>
        private string UploadSeedFile()
        {
            //local variables
            var temporaryPath = ConfigVariables.DefaultPublicTmpPath;
            var temporaryPathList = new List<string>();

            //Create temp directory
            if (!Directory.Exists(temporaryPath))
                Directory.CreateDirectory(temporaryPath);
            if (rdauUploadSeedFile.UploadedFiles.Count == 0)
            {
                ThrowAnError(SpecificationDetails_aspx.ITU_Error_NoFileToUpload, true);
                return string.Empty;
            }
            foreach (UploadedFile f in rdauUploadSeedFile.UploadedFiles)
            {
                //Upload files in the temp directory
                var path = new StringBuilder()
                    .Append(temporaryPath)
                    .Append(@"\")
                    .Append(f.FileName)
                    .ToString();
                try
                {
                    f.SaveAs(path);
                    temporaryPathList.Add(path);
                    return path;
                }
                catch (Exception exc)
                {
                    ThrowAnError(new StringBuilder().Append(exc.Message).Append("(File : ").Append(f.FileName).Append(")").ToString(), false);
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        #endregion
    }
}