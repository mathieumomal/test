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
            foreach (GridDataItem item in rgSpecificationList.MasterTableView.Items)
            {
                CheckBox chkPromoteInhibited = (CheckBox)item.FindControl("chkPromoteInhibited");
                //CheckBox chkCreateNewVersion = (CheckBox)item.FindControl("chkCreateNewVersion");

                if (!chkPromoteInhibited.Checked)
                {
                    specPromoteList.Add((Specification)item.DataItem);
                }
            }
            RadWindowManager1.RadAlert(specPromoteList.Count + " specification(s) promoted successfully!", 400, 100, "Promote Status", null);
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

                //Set "Promote inhibited" &	"Create new version" checkbox values
                if (currentSpecification.promoteInhibited != null && currentSpecification.promoteInhibited.Value)
                {
                    chkPromoteInhibited.Checked = true;
                    chkPromoteInhibited.Enabled = chkCreateNewVersion.Checked = chkCreateNewVersion.Enabled = false;
                }
                else
                    chkCreateNewVersion.Checked = currentSpecification.IsNewVersionCreationEnabled;

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
                var result = specSvc.GetSpecificationBySearchCriteria(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), new SpecificationSearch { SelectedReleaseIds = new List<int> { releaseId } });
                rgSpecificationList.DataSource = result.Key.Key;
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

        #endregion
    }
}