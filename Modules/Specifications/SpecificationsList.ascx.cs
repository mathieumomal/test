/*
' Copyright (c) 2014  Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Reflection;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using Telerik.Web.UI;


namespace Etsi.Ultimate.Module.Specifications
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from SpecificationsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class SpecificationsList : PortalModuleBase
    {
        #region Fields

        private const string CONST_DSID_KEY = "ETSI_DS_ID";
        private const string CONST_SERIES_DATASOURCE = "SeriesDataSource";
        private const string CONST_TECH_DATASOURCE = "TechDataSource";

        private bool isUrlSearch;
        private bool fromSearch;
        private static SpecificationSearch searchObj;

        protected Etsi.Ultimate.Controls.FullView ultFullView;
        protected Etsi.Ultimate.Controls.ShareUrlControl ultShareUrl;
        protected Etsi.Ultimate.Controls.CommunityControl CommunityCtrl;
        protected Etsi.Ultimate.Controls.ReleaseSearchControl ReleaseCtrl;

        #endregion

        #region Properties

        /// <summary>
        /// DataSource of rcbSeries
        /// </summary>
        private List<Enum_Serie> Series
        {
            get
            {
                if (ViewState[CONST_SERIES_DATASOURCE] == null)
                    ViewState[CONST_SERIES_DATASOURCE] = new List<Enum_Serie>();

                return (List<Enum_Serie>)ViewState[CONST_SERIES_DATASOURCE];
            }
            set
            {
                ViewState[CONST_SERIES_DATASOURCE] = value;
            }
        }

        /// <summary>
        /// DataSource for cblTechnology
        /// </summary>
        private List<Enum_Technology> Technologies
        {
            get
            {
                if (ViewState[CONST_TECH_DATASOURCE] == null)
                    ViewState[CONST_TECH_DATASOURCE] = new List<Enum_Technology>();

                return (List<Enum_Technology>)ViewState[CONST_TECH_DATASOURCE];
            }
            set
            {
                ViewState[CONST_TECH_DATASOURCE] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetRequestParameters();
                if (!IsPostBack)
                {
                    // Display or not NumberNotYetAllocated
                    var personService = ServicesFactory.Resolve<IPersonService>();

                    var userRights = personService.GetRights(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
                    trNumberNotYetAllocated.Visible = userRights.HasRight(Enum_UserRights.Specification_View_UnAllocated_Number);
                    lnkManageITURecommendations.Visible = userRights.HasRight(Enum_UserRights.Specification_ManageITURecommendations);
                    btnNewSpecification.Visible = userRights.HasRight(Enum_UserRights.Specification_Create);

                    var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    searchObj = new SpecificationSearch();
                    Technologies = specSvc.GetTechnologyList();
                    Series = specSvc.GetSeries();

                    BindControls();
                    SetTSGToCommunityControl();
                    ReleaseCtrl.Load += ReleaseCtrl_Load;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
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
                Specification currentSpecification = (Specification)e.Item.DataItem;
                Image img2G = (Image)dataItem.FindControl("img2G");
                Image img3G = (Image)dataItem.FindControl("img3G");
                Image imgLTE = (Image)dataItem.FindControl("imgLTE");

                if (img2G != null)
                    img2G.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "2g").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (img3G != null)
                    img3G.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "3g").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (imgLTE != null)
                    imgLTE.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "lte").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
            }
        }

        /// <summary>
        /// Load Specification list after the ReleaseCtrl load; if the request is from ShortURL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ReleaseCtrl_Load(object sender, EventArgs e)
        {
            //Load search control state if the request is from ShortURL
            if (isUrlSearch)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["title"]))
                    txtTitle.Text = searchObj.Title = Request.QueryString["title"];

                if (!String.IsNullOrEmpty(Request.QueryString["series"]))
                {
                    searchObj.Series = Request.QueryString["series"].Split(',').Select(n => int.Parse(n)).ToList();
                    foreach (RadComboBoxItem item in rcbSeries.Items)
                    {
                        int itemVal;
                        if (int.TryParse(item.Value, out itemVal))
                            item.Checked = searchObj.Series.Contains(itemVal);
                    }
                }

                if (!String.IsNullOrEmpty(Request.QueryString["type"]))
                {
                    searchObj.Type = Convert.ToBoolean(Request.QueryString["type"]);
                    if (searchObj.Type.Value)
                        cbTechnicalSpecification.Checked = true;
                    else
                        cbTechnicalReport.Checked = true;
                }
                if (!String.IsNullOrEmpty(Request.QueryString["communityIds"]))
                    CommunityCtrl.SelectedCommunityIds = searchObj.SelectedCommunityIds = Request.QueryString["communityIds"].Split(',').Select(n => int.Parse(n)).ToList();

                if (!String.IsNullOrEmpty(Request.QueryString["releases"]))
                    ReleaseCtrl.SelectedReleaseIds = searchObj.SelectedReleaseIds = Request.QueryString["releases"].Split(',').Select(n => int.Parse(n)).ToList();

                if (!String.IsNullOrEmpty(Request.QueryString["draft"]))
                    cbDraft.Checked = searchObj.IsDraft = Convert.ToBoolean(Request.QueryString["draft"]);
                if (!String.IsNullOrEmpty(Request.QueryString["underCC"]))
                    cbUnderCC.Checked = searchObj.IsUnderCC = Convert.ToBoolean(Request.QueryString["underCC"]);
                if (!String.IsNullOrEmpty(Request.QueryString["withACC"]))
                    cbWithdrawnAfterCC.Checked = searchObj.IsWithACC = Convert.ToBoolean(Request.QueryString["withACC"]);
                if (!String.IsNullOrEmpty(Request.QueryString["withBCC"]))
                    cbWithdrawnBeforeCC.Checked = searchObj.IsWithBCC = Convert.ToBoolean(Request.QueryString["withBCC"]);

                if (!String.IsNullOrEmpty(Request.QueryString["publication"]))
                {
                    searchObj.IsForPublication = Convert.ToBoolean(Request.QueryString["publication"]);
                    cbForPublication.Checked = searchObj.IsForPublication.Value;
                }

                if (!String.IsNullOrEmpty(Request.QueryString["tech"]))
                {
                    searchObj.Technologies = Request.QueryString["tech"].Split(',').Select(n => int.Parse(n)).ToList();
                    foreach (ListItem item in cblTechnology.Items)
                    {
                        int itemVal;
                        if (int.TryParse(item.Value, out itemVal))
                            item.Selected = searchObj.Technologies.Contains(itemVal);
                    }
                }

                // Management of the WiUid
                // Add it to the search object only if string is an integer
                if (!String.IsNullOrEmpty(Request.QueryString["WiUid"]))
                {
                    int tmpUid;
                    if (Int32.TryParse(Request.QueryString["WiUid"], out tmpUid))
                        searchObj.WiUid = tmpUid;
                }

            }
            else
            {
                searchObj.SelectedReleaseIds = ReleaseCtrl.SelectedReleaseIds;
            }

            LoadGridData();
        }

        /// <summary>
        /// On search btn click populate searchObj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //flag used in ShortURL generation
            fromSearch = true;
            searchObj = new SpecificationSearch();

            if (!String.IsNullOrEmpty(txtTitle.Text))
                searchObj.Title = txtTitle.Text;

            foreach (RadComboBoxItem item in rcbSeries.Items)
                if (item.Checked)
                {
                    int techId;
                    if (int.TryParse(item.Value, out techId))
                        searchObj.Series.Add(techId);
                }

            if (cbTechnicalSpecification.Checked != cbTechnicalReport.Checked)
                searchObj.Type = (cbTechnicalSpecification.Checked) ? true : ((cbTechnicalReport.Checked) ? (bool?)false : null);

            if (trNumberNotYetAllocated.Visible)
                searchObj.NumberNotYetAllocated = cbNumNotYetAllocated.Checked;

            if (CommunityCtrl.SelectedCommunityIds.Count > 0)
                searchObj.SelectedCommunityIds = CommunityCtrl.SelectedCommunityIds;

            searchObj.SelectedReleaseIds = ReleaseCtrl.SelectedReleaseIds;

            searchObj.IsDraft = cbDraft.Checked;
            searchObj.IsUnderCC = cbUnderCC.Checked;
            searchObj.IsWithACC = cbWithdrawnAfterCC.Checked;
            searchObj.IsWithBCC = cbWithdrawnBeforeCC.Checked;

            foreach (ListItem item in cblTechnology.Items)
                if (item.Selected)
                {
                    int techId;
                    if (int.TryParse(item.Value, out techId))
                        searchObj.Technologies.Add(techId);
                }

            if (cbForPublication.Checked != cbInternal.Checked)
                searchObj.IsForPublication = (cbForPublication.Checked) ? true : ((cbInternal.Checked) ? (bool?)false : null);

            ultShareUrl.IsShortUrlChecked = false;
            LoadGridData();
        }

        /// <summary>
        /// Need DataSource event for Specification List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void rgSpecificationList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            searchObj.SkipRecords = rgSpecificationList.CurrentPageIndex * rgSpecificationList.PageSize;
            searchObj.PazeSize = rgSpecificationList.PageSize;

            // Fetching the sort order:
            if (rgSpecificationList.MasterTableView.SortExpressions.Count != 0)
            {
                string name = rgSpecificationList.MasterTableView.SortExpressions[0].FieldName;
                GridSortOrder order = rgSpecificationList.MasterTableView.SortExpressions[0].SortOrder;

                if (name == "Number")
                {
                    if (order == GridSortOrder.Ascending)
                        searchObj.Order = SpecificationSearch.SpecificationOrder.Number;
                    else if (order == GridSortOrder.Descending)
                        searchObj.Order = SpecificationSearch.SpecificationOrder.NumberDesc;
                }
                else if (name == "Title")
                {
                    if (order == GridSortOrder.Ascending)
                        searchObj.Order = SpecificationSearch.SpecificationOrder.Title;
                    else if (order == GridSortOrder.Descending)
                        searchObj.Order = SpecificationSearch.SpecificationOrder.TitleDesc;
                }
            }


            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var result = specSvc.GetSpecificationBySearchCriteria(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);
            rgSpecificationList.VirtualItemCount = result.Key.Value;
            rgSpecificationList.DataSource = result.Key.Key;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Load Specification list data
        /// </summary>
        private void LoadGridData()
        {
            ManageShareUrl();
            fromSearch = true;
            ManageFullView();
            SetSearchLabel();
            rgSpecificationList.Rebind();
        }

        /// <summary>
        /// Retrieve person Id
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(CONST_DSID_KEY), out personID))
                    return personID;
            }
            return 0;
        }

        /// <summary>
        /// Bind ddl & list in the search panel
        /// </summary>
        private void BindControls()
        {
            cblTechnology.DataSource = Technologies;
            cblTechnology.DataTextField = "Description";
            cblTechnology.DataValueField = "Pk_Enum_TechnologyId";
            cblTechnology.DataBind();

            rcbSeries.DataSource = Series;
            rcbSeries.DataTextField = "Description";
            rcbSeries.DataValueField = "Pk_Enum_SerieId";
            rcbSeries.DataBind();
        }

        /// <summary>
        /// Construct ShortUrl
        /// </summary>
        private void ManageShareUrl()
        {
            ultShareUrl.ModuleId = ModuleId;
            ultShareUrl.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            ultShareUrl.BaseAddress = address;

            ultShareUrl.UrlParams = ManageUrlParams();
        }

        /// <summary>
        /// Construct FullUrl
        /// </summary>
        private void ManageFullView()
        {
            ultFullView.ModuleId = ModuleId;
            ultFullView.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            ultFullView.BaseAddress = address;

            ultFullView.UrlParams = ManageUrlParams();
            ultFullView.Display();
        }

        /// <summary>
        /// Set searched values into SearchPanel header
        /// </summary>
        private void SetSearchLabel()
        {
            if (searchObj != null)
            {
                StringBuilder sb = new StringBuilder();

                if (!String.IsNullOrEmpty(searchObj.Title))
                    sb.Append(searchObj.Title + ", ");
                if (searchObj.Series.Count > 0)
                    sb.Append("Series(" + searchObj.Series.Count + "), ");
                if (searchObj.Type != null)
                    sb.Append(((bool)searchObj.Type ? "TS" : "TR") + ", ");
                if (searchObj.NumberNotYetAllocated)
                    sb.Append("No. not yet allocated, ");
                if (searchObj.SelectedCommunityIds.Count > 0)
                    sb.Append("Primary responsible groups(" + searchObj.SelectedCommunityIds.Count + "), ");
                if (searchObj.SelectedReleaseIds.Count > 0)
                    sb.Append("Releases(" + searchObj.SelectedReleaseIds.Count + "), ");
                if (searchObj.IsDraft || searchObj.IsUnderCC || searchObj.IsWithACC || searchObj.IsWithBCC)
                {
                    StringBuilder tempSb = new StringBuilder();
                    tempSb.Append("(");
                    tempSb.Append(searchObj.IsDraft ? "Draft, " : string.Empty);
                    tempSb.Append(searchObj.IsUnderCC ? "Under change control, " : string.Empty);
                    tempSb.Append(searchObj.IsWithACC ? "Withdrawn before change control, " : string.Empty);
                    tempSb.Append(searchObj.IsWithBCC ? "Withdrawn under change control, " : string.Empty);

                    sb.Append(tempSb.ToString().Trim().Trim(',') + "), ");
                }
                if (searchObj.IsForPublication != null)
                    sb.Append((bool)searchObj.IsForPublication ? "For Publication" : "Internal" + ", ");
                if (searchObj.Technologies.Count > 0)
                    sb.Append("Technologies(" + searchObj.Technologies.Count + "), ");

                // WiUid case
                if (searchObj.WiUid != default(int))
                    sb.Append("WI #" + searchObj.WiUid + ",");

                if (sb.Length == 0)
                    sb.Append("Open Releases");

                lblSearchHeader.Text = String.Format("Search form ({0})", (sb.Length > 100) ? sb.ToString().Trim().TrimEnd(',').Substring(0, 100) + "..." : sb.ToString().Trim().TrimEnd(','));
            }
        }

        /// <summary>
        /// Generate Url parameters for Short/FullView Url
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> ManageUrlParams()
        {
            var nameValueCollection = HttpContext.Current.Request.QueryString;
            var urlParams = new Dictionary<string, string>();

            if (fromSearch)
            {
                fromSearch = false;

                urlParams.Add("s", "y");
                if (!String.IsNullOrEmpty(searchObj.Title))
                    urlParams.Add("title", searchObj.Title.Trim());

                if (searchObj.Series != null && searchObj.Series.Count > 0)
                    urlParams.Add("series", String.Join(",", searchObj.Series));

                if (searchObj.Type != null)
                    urlParams.Add("type", searchObj.Type.ToString());

                if (CommunityCtrl.SelectedCommunityIds != null && CommunityCtrl.SelectedCommunityIds.Count > 0)
                    urlParams.Add("communityIds", String.Join(",", CommunityCtrl.SelectedCommunityIds));

                if (searchObj.SelectedReleaseIds != null && searchObj.SelectedReleaseIds.Count > 0)
                    urlParams.Add("releases", String.Join(",", searchObj.SelectedReleaseIds));

                urlParams.Add("draft", searchObj.IsDraft.ToString());
                urlParams.Add("underCC", searchObj.IsUnderCC.ToString());
                urlParams.Add("withACC", searchObj.IsWithACC.ToString());
                urlParams.Add("withBCC", searchObj.IsWithBCC.ToString());

                if (searchObj.IsForPublication != null)
                    urlParams.Add("publication", searchObj.IsForPublication.ToString());

                if (searchObj.Technologies != null && searchObj.Technologies.Count > 0)
                    urlParams.Add("tech", String.Join(",", searchObj.Technologies));
            }
            else
            {
                foreach (var k in nameValueCollection.AllKeys)
                {
                    if (k != null && nameValueCollection[k] != null)
                        urlParams.Add(k, nameValueCollection[k]);
                }
            }

            return urlParams;
        }

        /// <summary>
        /// Extract query strings from Url (other than ShortUrl params)
        /// </summary>
        private void GetRequestParameters()
        {
            isUrlSearch = (Request.QueryString["s"] != null) ? Request.QueryString["s"] == "y" : false;
        }

        /// <summary>
        /// Set TSG values to Community Control
        /// </summary>
        private void SetTSGToCommunityControl()
        {
            //Set TSG values to community control only when it is Multi Selection Edit Mode
            if ((!CommunityCtrl.IsSingleSelection) && CommunityCtrl.IsEditMode)
            {
                string subTBId = Request.QueryString["SubTB"];
                if (!String.IsNullOrEmpty(subTBId))
                {
                    int value;
                    var tbList = subTBId.Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                    tbList.RemoveAll(x => x == -1);
                    CommunityCtrl.TBSelectorIds = tbList;
                }
            }
        }

        #endregion
    }
}