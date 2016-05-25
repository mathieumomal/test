using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    public partial class ReleaseSearchControl : System.Web.UI.UserControl
    {
        #region Properties

        public const string All = "all";
        public const string Open = "open";

        //Width of the rad combo box
        public int Width { get; set; }

        //Width of the content of the rad combo box
        public int DropDownWidth { get; set; }

        //Force page load process
        public bool IsLoadingNeeded { get; set; }

        #endregion

        #region releases properties (get set)
        /// <summary>
        /// Get selected releases with keywords for URL purpose. Three cases:
        /// - All releases : "all" keyword
        /// - Open releases : "open" keyword
        /// - Custom selection of releases : "1,2,3",... ; list of releases ids
        /// </summary>
        public string SelectedReleasesWithKeywords
        {
            get
            {
                //Get All Releases
                var rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                if (rbAllReleases.Checked)
                    return All;

                //Get Open Releases
                var rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                if (rbOpenReleases.Checked)
                    return Open;

                //Get Custom Releases
                var rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                var customReleaseIds = (from RadTreeNode node in rtvReleases.Nodes
                                        let rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases")
                                        where rbCustomReleases.Checked
                                        select Convert.ToInt32(node.Value))
                                              .ToList();
                return string.Join(",", customReleaseIds);
            }
            set
            {
                var releases = value;

                var rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                var rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                var rbCustomSelection = (RadButton)rcbReleases.Items[0].FindControl("rbCustomSelection");
                rbAllReleases.Checked = rbOpenReleases.Checked = rbCustomSelection.Checked = false;

                switch (releases)
                {
                    case All:
                        rbAllReleases.Checked = true;
                        break;
                    case Open:
                        rbOpenReleases.Checked = true;
                        break;
                    default:
                        rbCustomSelection.Checked = true;

                        var releasesIds = releases.Split(',').Select(x => Convert.ToInt32(x ?? "0")).ToList();
                        var rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                        foreach (RadTreeNode node in rtvReleases.Nodes)
                        {
                            var rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
                            rbCustomReleases.Checked = releasesIds.Exists(x => x == Convert.ToInt32((node.Value ?? "0")));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Return list of ids of selected releases (all, open or custom)
        /// </summary>
        public List<int> SelectedReleasesIds
        {
            get
            {
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releases = svc.GetAllReleases(0);

                switch (SelectedReleasesWithKeywords)
                {
                    case All:
                        return releases.Key.Select(x => x.Pk_ReleaseId).ToList();
                    case Open:
                        return releases.Key.Where(x => x.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open).Select(x => x.Pk_ReleaseId).ToList();
                    default:
                        return SelectedReleasesWithKeywords.Split(',').Select(x => Convert.ToInt32(x ?? "0")).ToList();
                }
            }
            set
            {
                var rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                var rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                var rbCustomSelection = (RadButton)rcbReleases.Items[0].FindControl("rbCustomSelection");
                rbAllReleases.Checked = rbOpenReleases.Checked = rbCustomSelection.Checked = false;

                var svc = ServicesFactory.Resolve<IReleaseService>();
                var allReleasesIds = svc.GetAllReleases(0).Key.Select(x => x.Pk_ReleaseId).ToList();
                var openReleasesIds = svc.GetAllReleases(0).Key.Where(x => x.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open).Select(x => x.Pk_ReleaseId).ToList();

                var selectedReleasesIds = value;

                if (allReleasesIds.All(selectedReleasesIds.Contains) && selectedReleasesIds.All(allReleasesIds.Contains))
                {
                    rbAllReleases.Checked = true;
                }
                else if (openReleasesIds.All(selectedReleasesIds.Contains) &&
                         selectedReleasesIds.All(openReleasesIds.Contains))
                {
                    rbOpenReleases.Checked = true;
                }
                else
                {
                    rbCustomSelection.Checked = true;

                    var rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                    foreach (RadTreeNode node in rtvReleases.Nodes)
                    {
                        var rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
                        rbCustomReleases.Checked = selectedReleasesIds.Exists(x => x == Convert.ToInt32((node.Value ?? "0")));
                    }
                }
            }
        }

        /// <summary>
        /// Get search string acccording to selected releases
        /// </summary>
        public string SearchString
        {
            get
            {
                var rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                if (rbAllReleases.Checked)
                {
                    return rbAllReleases.Text;
                }
                else
                {
                    var rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                    if (rbOpenReleases.Checked)
                    {
                        return rbOpenReleases.Text;
                    }
                    else
                    {
                        var rbCustomSelection = (RadButton)rcbReleases.Items[0].FindControl("rbCustomSelection");
                        if (rbCustomSelection.Checked)
                        {
                            var searchString = string.Empty;
                            var rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                            foreach (RadTreeNode node in rtvReleases.Nodes)
                            {
                                var rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
                                if (rbCustomReleases.Checked)
                                    searchString += rbCustomReleases.Text + ", ";
                            }
                            return searchString.Trim().TrimEnd(',');
                        }

                    }
                }
                return rcbReleases.Text;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || IsLoadingNeeded)
            {
                //Set width of the rad combo box
                rcbReleases.Width = Width;
                rcbReleases.DropDownWidth = DropDownWidth;

                //Get all releases
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseObjects = svc.GetAllReleases(0);

                //Bind Custom Releases
                var rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                var allReleaseIds = new List<int>();
                var openReleaseIds = new List<int>();
                foreach (var release in releaseObjects.Key.OrderByDescending(x => x.SortOrder))
                {
                    var newNode = new RadTreeNode
                    {
                        Text = release.ShortName,
                        Value = release.Pk_ReleaseId.ToString(),
                        Checkable = false
                    };
                    rtvReleases.Nodes.Add(newNode);

                    //Add to Open Release List
                    if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open)
                        openReleaseIds.Add(release.Pk_ReleaseId);
                    //Add to All Release List
                    allReleaseIds.Add(release.Pk_ReleaseId);
                }

                rtvReleases.DataBind();

                //Bind All Releases
                var rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                rbAllReleases.Attributes.Add("Value", String.Join(",", allReleaseIds));
                rbAllReleases.Checked = true;

                //Bind Open Releases
                var rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                rbOpenReleases.Attributes.Add("Value", String.Join(",", openReleaseIds));


                //Assign javascript events - this.ClientId is added to make the js method unique to the calling control
                rcbReleases.OnClientDropDownClosed = "OnClientDropDownClosed" + ClientID;
                rcbReleases.OnClientDropDownOpened = "OnClientDropDownOpened" + ClientID;
                rcbReleases.OnClientLoad = "OnClientLoad" + ClientID;
                rbAllReleases.OnClientClicked = "ResetCheckBoxes" + ClientID;
                rbOpenReleases.OnClientClicked = "ResetCheckBoxes" + ClientID;
                foreach (RadTreeNode item in rtvReleases.Nodes)
                {
                    if (item.FindControl("rbCustomReleases") != null)
                        ((RadButton)item.FindControl("rbCustomReleases")).OnClientClicked = "ResetRadioButtons" + ClientID;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset to All Releases
        /// </summary>
        public void Reset()
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Reset", "ResetToAllReleases" + ClientID + "();", true);
            SelectedReleasesIds = new List<int>();
        }
        #endregion
    }
}