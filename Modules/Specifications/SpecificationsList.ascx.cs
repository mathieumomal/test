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
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Etsi.Ultimate.Utils;
using System.Runtime.InteropServices;


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
        private const string CONST_FTP_LATEST_VERSIONS_PATH = "{0}\\Specs\\latest";
        private const string CONST_FTP_VERSIONS_MAP_PATH = "{0}\\Specs\\{1}";
        private const string CONST_PROGRESS_BAR_CONTENT = "<pre><code><div class=\"meter\"><span style=\"width: {0}%\"></span></div></code></pre>";

        private bool _isUrlSearch;
        private bool fromSearch;

        private const string VS_SEARCHOBJ = "VS_SEARCHOBJ";
        private SpecificationSearch searchObj
        {
            get
            {
                return (SpecificationSearch)ViewState[VS_SEARCHOBJ];
            }
            set
            {
                ViewState[VS_SEARCHOBJ] = value;
            }
        }
        private static string PathExportSpec;

        protected Etsi.Ultimate.Controls.FullView ultFullView;
        protected Etsi.Ultimate.Controls.ShareUrlControl ultShareUrl;
        protected Etsi.Ultimate.Controls.ReleaseSearchControl ReleaseCtrl;


        private const string VS_TB_ID = "VS_TB_ID";
        private const string VS_SUBTB_ID = "VS_SUBTB_ID";
        private string TbId
        {
            get
            {
                return (string)ViewState[VS_TB_ID];
            }
            set
            {
                ViewState[VS_TB_ID] = value;
            }
        }
        private string SubTBId
        {
            get
            {
                return (string)ViewState[VS_SUBTB_ID];
            }
            set
            {
                ViewState[VS_SUBTB_ID] = value;
            }
        }

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

        /// <summary>
        /// First page load flag
        /// </summary>
        private bool FirstLoad { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetRequestParameters();
                if (!IsPostBack || !componentSpecList.Visible)
                {
                    FirstLoad = true;
                    ReleaseCtrl.IsLoadingNeeded = !componentSpecList.Visible;
                    updateProgressSpecificationGrid.Visible = true;

                    //Set panel visibility to true
                    componentSpecList.Visible = true;

                    //Init page size component
                    InitPageSizeComponent();

                    // Display or not NumberNotYetAllocated
                    var personService = ServicesFactory.Resolve<IPersonService>();

                    if (Settings.Contains(Enum_Settings.Spec_ExportPath.ToString()))
                        PathExportSpec = Settings[Enum_Settings.Spec_ExportPath.ToString()].ToString();

                    var userRights =
                        personService.GetRights(
                            GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
                    trNumberNotYetAllocated.Visible =
                        userRights.HasRight(Enum_UserRights.Specification_View_UnAllocated_Number);
                    lnkManageITURecommendations.Visible =
                        userRights.HasRight(Enum_UserRights.Specification_ManageITURecommendations);
                    btnNewSpecification.Visible = userRights.HasRight(Enum_UserRights.Specification_Create);
                    imgBtnFTP.Visible = userRights.HasRight(Enum_UserRights.Specification_SyncHardLinksOnLatestFolder);

                    var specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    searchObj = new SpecificationSearch();

                    Technologies = specSvc.GetTechnologyList().OrderBy(t => t.SortOrder).ToList();
                    Series = specSvc.GetSeries().OrderBy(x => x.Description).ToList();

                    TbId = Request.QueryString["tbid"];
                    SubTBId = Request.QueryString["SubTB"];

                    BindControls();
                    ReleaseCtrl.Load += ReleaseCtrl_Load;

                    //Searching for user cookie which save last filters used
                    //!!! EXCEPT if this is a URL search request !!! (condition: !_isUrlSearch): 
                    //in fact, URL GET parameters should contain all required filters and system should not ask for last user's filters stored inside a cookie
                    if (!_isUrlSearch)
                    {
                        var searchObjFromCookie = CookiesHelper.GetCookie<SpecificationSearch>(Page.Request, ConfigVariables.CookieNameSpecsList);
                        if (searchObjFromCookie != null && searchObjFromCookie.GetType() == typeof(SpecificationSearch))
                        {
                            searchObj = searchObjFromCookie;
                            LoadControlsFromSearchObj();
                        }
                    }

                    /* Latest Folder PopUp */
                    progressBarContent.Text = string.Format(CONST_PROGRESS_BAR_CONTENT, 0);
                    var specVersionService = ServicesFactory.Resolve<ISpecVersionService>();
                    currentFolderLabel.Text = specVersionService.GetFTPLatestFolderName();
                }
                else
                {
                    FirstLoad = false;
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
                var dataItem = e.Item as GridDataItem;
                var currentSpecification = (Specification)e.Item.DataItem;
                var img2G = (Image)dataItem.FindControl("img2G");
                var img3G = (Image)dataItem.FindControl("img3G");
                var img5G = (Image)dataItem.FindControl("img5G");
                var imgLte = (Image)dataItem.FindControl("imgLTE");
                var lnkCr = (HyperLink) dataItem.FindControl("lnkCr");


                if (img2G != null)
                    img2G.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "2g").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (img3G != null)
                    img3G.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "3g").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (img5G != null)
                    img5G.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "5g").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (imgLte != null)
                    imgLte.Attributes.Add("style", (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "lte").FirstOrDefault() != null) ? "opacity:1" : "opacity:0.1");
                if (lnkCr != null)
                {
                    var classes = new StringBuilder(lnkCr.CssClass);
                    if (!(currentSpecification.IsUnderChangeControl ?? false))
                    {
                        lnkCr.Enabled = false;
                        lnkCr.CssClass = classes.Append(" disabled").ToString();
                    }
                    else
                    {
                        lnkCr.NavigateUrl = string.Format(ConfigVariables.RelativeUrlSpecRelatedCrs, currentSpecification.Number);
                    }
                }
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
            if (_isUrlSearch)
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

                if (!String.IsNullOrEmpty(Request.QueryString["releases"]))
                {
                    ReleaseCtrl.SelectedReleasesWithKeywords = Request.QueryString["releases"];
                    searchObj.SelectedReleaseIds = ReleaseCtrl.SelectedReleasesIds;
                }
                    

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
                    cbInternal.Checked = !searchObj.IsForPublication.Value;
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

                if (!String.IsNullOrEmpty(Request.QueryString["numberNYA"]))
                {
                    searchObj.NumberNotYetAllocated = Convert.ToBoolean(Request.QueryString["numberNYA"]);
                    cbNumNotYetAllocated.Checked = searchObj.NumberNotYetAllocated;
                }

                // Management of the WiUid
                ParseWiUid(searchObj);

            }
            else
            {
                if (FirstLoad && searchObj != null && searchObj.SelectedReleaseIds != null &&
                    searchObj.SelectedReleaseIds.Count > 0)
                {
                    ReleaseCtrl.SelectedReleasesIds = searchObj.SelectedReleaseIds;
                }
                else
                {
                    searchObj.SelectedReleaseIds = ReleaseCtrl.SelectedReleasesIds;
                }
            }

            if (!String.IsNullOrEmpty(SubTBId))
            {
                List<int> tbList = new List<int>();
                int value;
                tbList = SubTBId.Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                tbList.RemoveAll(x => x == -1);
                searchObj.SelectedCommunityIds = tbList;
            }
            else { searchObj.SelectedCommunityIds = new List<int>();}

            LoadGridData(true);
        }

        /// <summary>
        /// On search btn click populate searchObj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDataWithSearchParameters(true);
        }

      
        /// <summary>
        /// On Refresh btn click populate searchObj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataWithSearchParameters(false);
        }

        /// <summary>
        /// Fills in search object from the search form.
        /// </summary>
        /// <returns></returns>
        private SpecificationSearch FillInSearchObj(bool shouldResetPageNumber)
        {
            searchObj = new SpecificationSearch();
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

            if (!String.IsNullOrEmpty(SubTBId))
            {
                List<int> tbList = new List<int>();
                int value;
                tbList = SubTBId.Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                tbList.RemoveAll(x => x == -1);
                searchObj.SelectedCommunityIds = tbList;
            }
            else
            {
                searchObj.SelectedCommunityIds = new List<int>();
            }

            searchObj.SelectedReleaseIds = ReleaseCtrl.SelectedReleasesIds;

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

            searchObj.PageSize = Convert.ToInt32(SelectPageSize.SelectedValue);

            if (!shouldResetPageNumber)
            {
                searchObj.SkipRecords = rgSpecificationList.CurrentPageIndex * rgSpecificationList.PageSize;
            }

            // There might be a filter by WI Id.
            ParseWiUid(searchObj);

            return searchObj;
        }

        /// <summary>
        /// Fills controls from the searchObj
        /// </summary>
        /// <returns></returns>
        private void LoadControlsFromSearchObj()
        {
            if (!String.IsNullOrEmpty(searchObj.Title))
                txtTitle.Text = searchObj.Title;

            SetSelectedValue(rcbSeries, searchObj.Series);
            cbNumNotYetAllocated.Checked = searchObj.NumberNotYetAllocated;

            if (searchObj.Type.HasValue)
            {
                cbTechnicalSpecification.Checked = searchObj.Type.Value;
                cbTechnicalReport.Checked = !searchObj.Type.Value;
            }

            cbDraft.Checked = searchObj.IsDraft;
            cbUnderCC.Checked = searchObj.IsUnderCC;
            cbWithdrawnAfterCC.Checked = searchObj.IsWithACC;
            cbWithdrawnBeforeCC.Checked = searchObj.IsWithBCC;
            SetSelectedValue(cblTechnology, searchObj.Technologies);
            cbInternal.Checked = !searchObj.IsForPublication == true;
            cbForPublication.Checked = searchObj.IsForPublication == true;
            SetSelectedValue(SelectPageSize, searchObj.PageSize.ToString());
        }

        private void SetSelectedValue(RadComboBox rcb, string value)
        {
            RadComboBoxItem item = rcb.FindItemByValue(value);
            if (item != null)
                item.Selected = true;
        }

        private void SetSelectedValue(RadComboBox rcb, List<int> values)
        {
            foreach (int val in values)
            {
                RadComboBoxItem item = rcb.FindItemByValue(val.ToString());
                if (item != null)
                    item.Checked = true;
            }
        }

        private void SetSelectedValue(CheckBoxList cbl, List<int> values)
        {
            foreach (int val in values)
            {
                ListItem item = cbl.Items.FindByValue(val.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }

        /// <summary>
        /// Need DataSource event for Specification List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void rgSpecificationList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (FirstLoad && !_isUrlSearch)
            {
                rgSpecificationList.DataSource = new List<Specification>();
            }
            else
            {
                searchObj.SkipRecords = rgSpecificationList.CurrentPageIndex*rgSpecificationList.PageSize;
                searchObj.PageSize = Convert.ToInt32(SelectPageSize.SelectedValue);

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
                var result =
                    specSvc.GetSpecificationBySearchCriteria(
                        GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj);

                rgSpecificationList.VirtualItemCount = result.Key.Value;

                if (searchObj.PageSize > rgSpecificationList.VirtualItemCount &&
                    rgSpecificationList.VirtualItemCount > 0)
                {
                    rgSpecificationList.PageSize = rgSpecificationList.VirtualItemCount;
                }
                else
                {
                    rgSpecificationList.PageSize = searchObj.PageSize;
                }

                rgSpecificationList.CurrentPageIndex = searchObj.SkipRecords / searchObj.PageSize;
                rgSpecificationList.DataSource = result.Key.Key;
            }
        }

        /// <summary>
        /// Export spec list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSpecExport_Click(object sender, ImageClickEventArgs e)
        {
            ISpecificationService svc = ServicesFactory.Resolve<ISpecificationService>();
            searchObj = FillInSearchObj(false);
            var filepath = svc.ExportSpecification(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), searchObj, Request.Url.GetLeftPart(UriPartial.Authority));

            hidSpecAddress.Value = filepath;
            /*Response.Redirect(Server.UrlEncode(filepath));
            Response.End();*/
        }

        /// <summary>
        /// Page size change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectPageSize_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            rgSpecificationList.PageSize = Convert.ToInt32(e.Value);
            LoadDataWithSearchParameters(true);
        }
        #endregion

        #region Helper methods

        /// <summary>
        /// Retrieves Uid from address
        /// </summary>
        /// <param name="searchObj"></param>
        protected void ParseWiUid(SpecificationSearch searchObj)
        {
            // Add it to the search object only if string is an integer
            if (!String.IsNullOrEmpty(Request.QueryString["WiUid"]))
            {
                int tmpUid;
                if (Int32.TryParse(Request.QueryString["WiUid"], out tmpUid))
                    searchObj.WiUid = tmpUid;
            }
        }

        /// <summary>
        /// This method is in charge of loading the data following a search or a refresh.
        /// </summary>
        /// <param name="shouldResetPageNumber">Whether pagination should be back to 1.</param>
        private void LoadDataWithSearchParameters(bool shouldResetPageNumber)
        {
            //flag used in ShortURL generation
            fromSearch = true;
            searchObj = FillInSearchObj(shouldResetPageNumber);
            CookiesHelper.SetCookie(Page.Response, ConfigVariables.CookieNameSpecsList, searchObj);
            ultShareUrl.IsShortUrlChecked = false;
            LoadGridData(shouldResetPageNumber);
        }


        /// <summary>
        /// Load Specification list data
        /// </summary>
        private void LoadGridData(bool shouldReinitializePage)
        {
            ManageShareUrl();
            fromSearch = true;
            ManageFullView();
            SetSearchLabel();
            if (shouldReinitializePage)
                rgSpecificationList.CurrentPageIndex = 0;
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
            cblTechnology.DataTextField = "Code";
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
        /// Construct FullView url
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
                {
                    var communityService = ServicesFactory.Resolve<ICommunityService>();                    
                    List<string> communityShortNames = new List<string>();
                    searchObj.SelectedCommunityIds.ForEach(x => {
                                                                   var shortName = communityService.GetCommmunityshortNameById(x);
                                                                   if (!String.IsNullOrEmpty(shortName)) communityShortNames.Add(shortName);
                                                                });
                    sb.Append("Primary responsible groups(" + String.Join(",", communityShortNames) + "), ");
                }
                if (searchObj.SelectedReleaseIds.Count > 0)
                    sb.Append("Releases(" + searchObj.SelectedReleaseIds.Count + "), ");
                if (searchObj.IsDraft || searchObj.IsUnderCC || searchObj.IsWithACC || searchObj.IsWithBCC)
                {
                    StringBuilder tempSb = new StringBuilder();
                    tempSb.Append("(");
                    tempSb.Append(searchObj.IsDraft ? "Draft, " : string.Empty);
                    tempSb.Append(searchObj.IsUnderCC ? "Under change control, " : string.Empty);
                    tempSb.Append(searchObj.IsWithACC ? "Withdrawn under change control, " : string.Empty);
                    tempSb.Append(searchObj.IsWithBCC ? "Withdrawn before change control, " : string.Empty);

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

                urlParams.Add("q", "1");//URL QUERY ON SPECIFICATIONS : data need to be loaded

                if (!String.IsNullOrEmpty(searchObj.Title))
                    urlParams.Add("title", searchObj.Title.Trim());

                if (searchObj.Series != null && searchObj.Series.Count > 0)
                    urlParams.Add("series", String.Join(",", searchObj.Series));

                if (searchObj.Type != null)
                    urlParams.Add("type", searchObj.Type.ToString());

                if (!String.IsNullOrEmpty(TbId))
                    urlParams.Add("tbid", TbId);
                if (!String.IsNullOrEmpty(SubTBId))
                    urlParams.Add("SubTB", SubTBId);

                if (!string.IsNullOrEmpty(ReleaseCtrl.SelectedReleasesWithKeywords))
                    urlParams.Add("releases", ReleaseCtrl.SelectedReleasesWithKeywords);

                urlParams.Add("draft", searchObj.IsDraft.ToString());
                urlParams.Add("underCC", searchObj.IsUnderCC.ToString());
                urlParams.Add("withACC", searchObj.IsWithACC.ToString());
                urlParams.Add("withBCC", searchObj.IsWithBCC.ToString());

                if (searchObj.IsForPublication != null)
                    urlParams.Add("publication", searchObj.IsForPublication.ToString());

                if (searchObj.Technologies != null && searchObj.Technologies.Count > 0)
                    urlParams.Add("tech", String.Join(",", searchObj.Technologies));

                urlParams.Add("numberNYA", searchObj.NumberNotYetAllocated.ToString());
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
            _isUrlSearch = (Request.QueryString["q"] != null);
        }

        /// <summary>
        /// Init page size component:
        /// - load default value 
        /// - add default value from the web.config
        /// - display data
        /// </summary>
        private void InitPageSizeComponent()
        {
            SelectPageSize.Items.AddRange(ConfigVariables.ItemsPerPageList.Select(
                x => new RadComboBoxItem(x.Key, x.Value.ToString())));

            var defaultItem =
                SelectPageSize.Items.FindItemByValue(ConfigVariables.ItemsPerPageListDefaultValue.ToString());

            if (defaultItem != null)
            {
                defaultItem.Selected = true;
                defaultItem.Checked = true;
            }
        }

        /// <summary>
        /// Create Hard links between files
        /// </summary>
        /// <param name="lpFileName">New File Name(Hard Link Path)</param>
        /// <param name="lpExistingFileName">Original File Name</param>
        /// <param name="lpSecurityAttributes">Security Attributes</param>
        /// <returns>True/False</returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );

        #endregion

        public void ramVersions_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            var specVersionService = ServicesFactory.Resolve<ISpecVersionService>();

            if (e.Argument == "CreateLatestFolder")
            {
                /* Create command */
                var folderName = txtFTPFolderName.Text;
                var response = specVersionService.CreateAndFillVersionLatestFolder(folderName, 
                    GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));

                if (response.Result)
                {
                    ramVersions.ResponseScripts.Add("RefreshCreateLatestFolderStatus();");
                }
                else
                {
                    FtpStatusLabel.Text = string.Join("<br/>", response.Report.ErrorList);
                    FtpStatusLabel.CssClass = "block messageBox error";
                }
            }
            else if (e.Argument == "RefreshCreateLatestFolderStatus")
            {
                /* copy in progress, get current status */
                var ftpFoldersManagerStatus = specVersionService.GetFtpFoldersManagerStatus();

                if (ftpFoldersManagerStatus != null)
                {
                    if (ftpFoldersManagerStatus.Finished)
                    {
                        if (ftpFoldersManagerStatus.ErrorMessages != null && ftpFoldersManagerStatus.ErrorMessages.Count > 0)
                        {
                            /* copy is finish with errors */
                            FtpStatusLabel.Text = "Error(s) occured while create latest folder :<br />";
                            FtpStatusLabel.Text += string.Join("<br />", ftpFoldersManagerStatus.ErrorMessages);
                            FtpStatusLabel.CssClass = "block messageBox error";
                            progressBarContent.Text = string.Empty;
                            specVersionService.ClearFtpFoldersManagerStatus();
                        }
                        else
                        {
                            /* copy is finish */
                            FtpStatusLabel.Text = "Finished.";
                            FtpStatusLabel.CssClass = "block messageBox success";
                            progressBarContent.Text = string.Empty;
                            specVersionService.ClearFtpFoldersManagerStatus();
                        }
                        
                    }
                    else
                    {
                        /* recall refresh */
                        if (ftpFoldersManagerStatus.TotalFiles > 0)
                        {
                            FtpStatusLabel.Text = string.Format("Copy in progress ({0}/{1} files)...", ftpFoldersManagerStatus.CurrentNumberFile, ftpFoldersManagerStatus.TotalFiles);
                        }
                        else
                        {
                            FtpStatusLabel.Text = "Please wait...";
                        }

                        FtpStatusLabel.CssClass = "block messageBox info";
                        ramVersions.ResponseScripts.Add("setTimeout(function () { RefreshCreateLatestFolderStatus(); }, 1000);");
                        
                        /* Progress bar content */
                        progressBarContent.Text = string.Format(CONST_PROGRESS_BAR_CONTENT, ftpFoldersManagerStatus.Percent.ToString());
                    }
                }
            }
        }
    }
}