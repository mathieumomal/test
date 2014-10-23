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

        public int Width { get; set; }
        public int DropDownWidth { get; set; }
        public bool IsLoadingNeeded { get; set; }
        public List<int> SelectedReleaseIds
        {
            get
            {
                //Get All Releases
                RadButton rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                if (rbAllReleases.Checked)
                    return rbAllReleases.Attributes["Value"].Split(',').Select(int.Parse).ToList();

                //Get Open Releases
                RadButton rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                if (rbOpenReleases.Checked)
                    return rbOpenReleases.Attributes["Value"].Split(',').Select(int.Parse).ToList();

                //Get Custom Releases
                RadTreeView rtvReleases = (RadTreeView)rcbReleases.Items[0].FindControl("rtvReleases");
                List<int> customReleaseIds = new List<int>();
                foreach (RadTreeNode node in rtvReleases.Nodes)
                {
                    RadButton rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
                    if (rbCustomReleases.Checked)
                        customReleaseIds.Add(Convert.ToInt32(node.Value));
                }
                return customReleaseIds;
            }
            set
            {
                var releaseIds = value;

                RadButton rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                RadButton rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                RadButton rbCustomSelection = (RadButton)rcbReleases.Items[0].FindControl("rbCustomSelection");
                rbAllReleases.Checked = rbOpenReleases.Checked = rbAllReleases.Checked = false;

                var AllReleases = rbAllReleases.Attributes["Value"].Split(',').Select(int.Parse).ToList();
                if (releaseIds.OrderBy(x => x).SequenceEqual(AllReleases.OrderBy(x => x)))
                    rbAllReleases.Checked = true;
                else
                {
                    var OpenReleases = rbOpenReleases.Attributes["Value"].Split(',').Select(int.Parse).ToList();
                    if (releaseIds.OrderBy(x => x).SequenceEqual(OpenReleases.OrderBy(x => x)))
                        rbOpenReleases.Checked = true;
                    else
                    {
                        rbCustomSelection.Checked = true;

                        RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");
                        foreach (RadTreeNode node in rtvReleases.Nodes)
                        {
                            RadButton rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
                            rbCustomReleases.Checked = releaseIds.Exists(x => x == Convert.ToInt32((node.Value ?? "0")));
                        }
                    }
                }
            }
        }

        public string SearchString
        {
            get
            {
                RadButton rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                if (rbAllReleases.Checked)
                {
                    return rbAllReleases.Text;
                }
                else
                {
                    RadButton rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                    if (rbOpenReleases.Checked)
                    {
                        return rbOpenReleases.Text;
                    }
                    else
                    {
                        RadButton rbCustomSelection = (RadButton)rcbReleases.Items[0].FindControl("rbCustomSelection");
                        if (rbCustomSelection.Checked)
                        {
                            var searchString = string.Empty;
                            RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");
                            foreach (RadTreeNode node in rtvReleases.Nodes)
                            {
                                RadButton rbCustomReleases = (RadButton)node.FindControl("rbCustomReleases");
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
                rcbReleases.Width = Width;
                rcbReleases.DropDownWidth = DropDownWidth;
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseObjects = svc.GetAllReleases(0);

                //Bind Custom Releases
                RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");
                List<int> allReleaseIds = new List<int>();
                List<int> openReleaseIds = new List<int>();
                foreach (var release in releaseObjects.Key.OrderByDescending(x => x.SortOrder))
                {
                    RadTreeNode newNode = new RadTreeNode();
                    newNode.Text = release.ShortName;
                    newNode.Value = release.Pk_ReleaseId.ToString();
                    newNode.Checkable = false;
                    rtvReleases.Nodes.Add(newNode);

                    //Add to Open Release List
                    if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open)
                        openReleaseIds.Add(release.Pk_ReleaseId);
                    //Add to All Release List
                    allReleaseIds.Add(release.Pk_ReleaseId);
                }

                rtvReleases.DataBind();

                //Bind All Releases
                RadButton rbAllReleases = (RadButton)rcbReleases.Items[0].FindControl("rbAllReleases");
                rbAllReleases.Attributes.Add("Value", String.Join(",", allReleaseIds));

                //Bind Open Releases
                RadButton rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
                rbOpenReleases.Attributes.Add("Value", String.Join(",", openReleaseIds));
                rbOpenReleases.Checked = true;



                //Assign javascript events - this.ClientId is added to make the js method unique to the calling control
                rcbReleases.OnClientDropDownClosed = "OnClientDropDownClosed" + this.ClientID;
                rcbReleases.OnClientDropDownOpened = "OnClientDropDownOpened" + this.ClientID;
                rcbReleases.OnClientLoad = "OnClientLoad" + this.ClientID;
                rbAllReleases.OnClientClicked = "ResetCheckBoxes" + this.ClientID;
                rbOpenReleases.OnClientClicked = "ResetCheckBoxes" + this.ClientID;
                foreach (RadTreeNode item in rtvReleases.Nodes)
                {
                    if (item.FindControl("rbCustomReleases") != null)
                        ((RadButton)item.FindControl("rbCustomReleases")).OnClientClicked = "ResetRadioButtons" + this.ClientID;
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
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "Reset", "ResetToAllReleases();", true);
            RadButton rbOpenReleases = (RadButton)rcbReleases.Items[0].FindControl("rbOpenReleases");
            this.SelectedReleaseIds = rbOpenReleases.Attributes["Value"].Split(',').Select(int.Parse).ToList();
        }
        #endregion
    }
}