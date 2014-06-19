using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using System.Configuration;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class MassivePromote : System.Web.UI.Page
    {
        #region Properties

        //Static fields
        public static readonly string DsId_Key = "ETSI_DS_ID";

        //Properties
        private int userId;
        private static Release latestRelease;

        #endregion

        #region Events

        /// <summary>
        /// Main event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInitialReleaseDropdown();

                LoadSpecificationGrid();
            }
        }

        /// <summary>
        /// Massive promote button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPromote_Click(object sender, EventArgs e)
        {
            List<Specification> specPromoteList = new List<Specification>();
            int targetReleaseId;
            if (int.TryParse(ddlInitialRelease.Items[ddlInitialRelease.SelectedIndex - 1].Value, out targetReleaseId))
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                foreach (GridDataItem item in rgSpecificationList.MasterTableView.Items)
                {
                    int specificationId;

                    if (!((CheckBox)item.FindControl("chkPromoteInhibited")).Checked && int.TryParse(item["Pk_SpecificationId"].Text, out specificationId))
                    {
                        Specification specToPromote = specSvc.GetSpecificationDetailsById(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), specificationId).Key;
                        specToPromote.IsNewVersionCreationEnabled = ((CheckBox)item.FindControl("chkCreateNewVersion")).Checked;
                        specPromoteList.Add(specToPromote);
                    }
                }

                if (specPromoteList.Count > 0)
                {
                    if (specSvc.PerformMassivePromotion(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), specPromoteList, targetReleaseId))
                    {
                        LoadSpecificationGrid();
                        ShowAlert(String.Format("{0} specification(s) promoted successfully!", specPromoteList.Count), "Promote Status");
                    }
                    else
                        ShowAlert("Failed to promote specification(s)! Please try later.", "Promote Status");
                }
                else
                    ShowAlert("Please select specification(s) to promote.", "Promote Status");
            }
            else
                ShowAlert(String.Format("{0} is not a valid target release.", ddlInitialRelease.Items[ddlInitialRelease.SelectedIndex - 1].Text), "Promote Status");
        }

        /// <summary>
        /// Initial release dropdownload list index changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlInitialRelease_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTargetRelease.Text = (ddlInitialRelease.SelectedIndex == 0) ? latestRelease.Name : ddlInitialRelease.Items[ddlInitialRelease.SelectedIndex - 1].Text;

            LoadSpecificationGrid();
        }

        /// <summary>
        /// Speicification List ItemDataBound event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void rgSpecificationList_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                CheckBox chkPromoteInhibited = (CheckBox)dataItem.FindControl("chkPromoteInhibited");
                CheckBox chkCreateNewVersion = (CheckBox)dataItem.FindControl("chkCreateNewVersion");

                Specification currentSpecification = (Specification)e.Item.DataItem;
                chkPromoteInhibited.Attributes.Add("OnClick", "ToggleCreateNewStatus(this , '" + dataItem.ItemIndex + "');");
                chkCreateNewVersion.Attributes.Add("OnClick", "ToggleCreateNewStatus(this , '" + dataItem.ItemIndex + "');");

                //Set "Promote inhibited" &	"Create new version" checkbox values
                if (currentSpecification.promoteInhibited != null && currentSpecification.promoteInhibited.Value)
                {
                    chkPromoteInhibited.Checked = true;
                    chkPromoteInhibited.Enabled = chkCreateNewVersion.Checked = chkCreateNewVersion.Enabled = false;
                }
                else
                    chkCreateNewVersion.Checked = chkCreateNewVersion.Enabled = currentSpecification.IsNewVersionCreationEnabled;

                //Trim TITLE to fit inside grid column
                if (!String.IsNullOrEmpty(currentSpecification.Title) && currentSpecification.Title.Length > 20)
                    dataItem["Title"].Text = currentSpecification.Title.Remove(19) + "...";
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            userId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
        }

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="UserInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
            return 0;
        }

        /// <summary>
        /// Bind Specification grid based on ddlInitialRelease selected release
        /// </summary>
        private void LoadSpecificationGrid()
        {
            int releaseId;
            if (int.TryParse(ddlInitialRelease.SelectedValue, out releaseId) && releaseId > 0)
            {
                var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                rgSpecificationList.DataSource = specSvc.GetSpecificationForMassivePromotion(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), releaseId).Key;
                rgSpecificationList.DataBind();
            }
        }

        /// <summary>
        /// Load Initial Release dropdown
        /// </summary>
        private void LoadInitialReleaseDropdown()
        {
            IReleaseService relSvc = ServicesFactory.Resolve<IReleaseService>();
            var releases = relSvc.GetAllReleases(userId).Key.Where(x => x.Enum_ReleaseStatus.Code == Domain.Enum_ReleaseStatus.Open
                                                                   || x.Enum_ReleaseStatus.Code == Domain.Enum_ReleaseStatus.Frozen)
                                                                   .OrderByDescending(x => x.SortOrder).ToList();

            if (releases.Count > 0)
            {
                latestRelease = releases[0];
                releases.Remove(latestRelease);
                lblTargetRelease.Text = latestRelease.Name;
            }

            ddlInitialRelease.DataSource = releases;
            ddlInitialRelease.DataValueField = "Pk_ReleaseId";
            ddlInitialRelease.DataTextField = "Name";
            ddlInitialRelease.DataBind();
        }

        private void ShowAlert(string Message, string Title)
        {
            RadWindowManager1.RadAlert(Message, 400, 100, Title, null);
        }
        #endregion
    }
}