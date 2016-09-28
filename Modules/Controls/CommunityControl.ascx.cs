using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Utils;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Community Control - Display the list of Communities
    ///     Single Selection - View Mode - Selected community will display in label
    ///                      - Edit Mode - Combobox will allow to select one community
    ///     Multi Selection  - View Mode - Selected communities will display in label
    ///                      - Edit Mode - Pencil icon will allow to open a popup & allow to select multiple communities
    /// </summary>
    public partial class CommunityControl : System.Web.UI.UserControl
    {
        #region Properties
        private const int CONST_SELECT_WIDTH = 100;

        private List<int> _selectedCommunityIds;
        /// <summary>
        /// SelectedCommunityIds will be used in Multi Selection mode
        /// to get/set the communitiy ids
        /// </summary>
        public List<int> SelectedCommunityIds
        {
            get
            {
                if (_selectedCommunityIds == null)
                    _selectedCommunityIds = new List<int>();

                _selectedCommunityIds.Clear();
                foreach (RadTreeNode node in rtvCommunitySelector.CheckedNodes)
                {
                    if (node.Nodes.Count == 0)
                        _selectedCommunityIds.Add(Convert.ToInt32(node.Value));
                }

                return _selectedCommunityIds;
            }
            set
            {
                _selectedCommunityIds = value;
                if (_selectedCommunityIds != null && _selectedCommunityIds.Count > 0)
                {
                    foreach (RadTreeNode node in rtvCommunitySelector.GetAllNodes().ToList())
                    {
                        int nodeValue = Convert.ToInt32(node.Value);
                        if (_selectedCommunityIds.Exists(x => x == nodeValue))
                        {
                            node.Checked = true;
                        }
                    }
                }
                BindLabelText();
            }
        }

        private int _selectedCommunityID;
        /// <summary>
        /// SelectedCommunityID will be used in Single Selection mode
        /// to get/set the community id
        /// </summary>
        public int SelectedCommunityID
        {
            get { return Convert.ToInt32(rcbCommunity.SelectedValue); }
            set { _selectedCommunityID = value; }
        }

        /// <summary>
        /// TBSelectorIds will be used in Multi Selection mode
        /// to get/set the default communities (usually it should be from TSG selector)
        /// </summary>
        public List<int> TBSelectorIds
        {
            set
            {
                defaultTbIds.Value = String.Join(",", value);
            }
        }

        /// <summary>
        /// True - Control will be rendered in Single Selection
        /// False - Control will be rendered in Multi Selection
        /// </summary>
        public bool IsSingleSelection { get; set; }

        /// <summary>
        /// True - Control will be rendered in Edit Mode
        /// False - Control will be rendered in View Mode
        /// </summary>
        public bool IsEditMode { get; set; }

        public int Width
        {
            set
            {
                var width = (value > CONST_SELECT_WIDTH) ? value : CONST_SELECT_WIDTH;
                pnlCover.Width = Unit.Pixel(width);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Community Control
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ICommunityService svc = ServicesFactory.Resolve<ICommunityService>();
                List<Community> communityList = svc.GetCommunities();
                Remove3GppFromCommunitiesTbName(communityList);

                List<Community> dataSource = new List<Community>(); //Create deep copy of list to avoid cache modifications
                dataSource.AddRange(communityList.Select(x => new Community { TbId = x.TbId, TbName = x.TbName, ParentTbId = x.ParentTbId, Order = x.Order, ActiveCode = x.ActiveCode }));

                if (IsSingleSelection) //Single Selection
                {
                    imgBtnCommunity.Visible = false;

                    var rootNodes = dataSource.Where(x => x.ParentTbId == 0);
                    rootNodes.ToList().ForEach(x => dataSource.FindAll(y => y.ParentTbId == x.TbId).ForEach(z => z.TbName = "Plenary " + z.TbName));
                    dataSource.RemoveAll(x => rootNodes.Contains(x));

                    if (IsEditMode)
                    {
                        rcbCommunity.Visible = true;
                        lblCommunity.Visible = false;
                        //Add element to Combobox that refer to no community is selected
                        List<Community> rcbCommunityDataSource = new List<Community>(){new Community() { TbName = "", TbId = default(int) }};
                        rcbCommunityDataSource.AddRange(dataSource.Where(x => x.ActiveCode == "ACTIVE" || x.TbId == _selectedCommunityID).OrderBy(x => x.Order));
                        rcbCommunity.DataSource = rcbCommunityDataSource;
                        rcbCommunity.DataTextField = "TbName";
                        rcbCommunity.DataValueField = "TbId";
                        rcbCommunity.DataBind();

                        //if (_selectedCommunityID != default(int))
                            rcbCommunity.SelectedValue = _selectedCommunityID.ToString();
                    }
                    else
                    {
                        rcbCommunity.Visible = false;
                        lblCommunity.Visible = true;

                        if (_selectedCommunityID != default(int))
                        {
                            var selectedCommunity = dataSource.Find(x => x.TbId == _selectedCommunityID);
                            lblCommunity.Text = pnlCover.ToolTip = (selectedCommunity == null) ? String.Empty : selectedCommunity.TbName;
                        }
                    }
                }
                else // Multi Selection
                {
                    rcbCommunity.Visible = false;
                    imgBtnCommunity.Visible = false;
                    lblCommunity.Visible = true;

                    if (_selectedCommunityIds != null)
                    {
                        var selectedCommunityNames = dataSource.FindAll(x => _selectedCommunityIds.Contains(x.TbId)).Select(x => dataSource.Any(y => y.ParentTbId == x.TbId) ? "Plenary " + x.TbName : x.TbName).ToList();
                        lblCommunity.Text = pnlCover.ToolTip = String.Join(", ", selectedCommunityNames);
                    }

                    if (IsEditMode)
                    {
                        imgBtnCommunity.Visible = true;

                        AddPlenaryRecords(dataSource);

                        List<Community> rtvSource;
                        if (dataSource.Count > 0 && _selectedCommunityIds != null)
                            rtvSource = dataSource.OrderBy(x => x.Order).Where(x => x.ActiveCode == "ACTIVE" || _selectedCommunityIds.Contains(x.TbId)).OrderBy(x => x.Order).ToList();
                        else
                            rtvSource = dataSource.OrderBy(x => x.Order).Where(x => x.ActiveCode == "ACTIVE").ToList();

                        rtvCommunitySelector.DataTextField = "TbName";
                        rtvCommunitySelector.DataValueField = "TbId";
                        rtvCommunitySelector.DataFieldID = "TbId";
                        rtvCommunitySelector.DataFieldParentID = "ParentCommunityId";
                        rtvCommunitySelector.DataSource = rtvSource;
                        rtvCommunitySelector.DataBind();

                        if (_selectedCommunityIds != null)
                        {
                            foreach (RadTreeNode node in rtvCommunitySelector.GetAllNodes().ToList())
                            {
                                int nodeValue = Convert.ToInt32(node.Value);
                                if (_selectedCommunityIds.Exists(x => x == nodeValue))
                                {
                                    if (node.Nodes.Count == 0)
                                    {
                                        node.Checked = true;
                                        UpdateParent(node);
                                    }
                                }
                            }
                        }
                    }
                }

                //Assign javascript events - this.ClientId is added to make the js method unique to the calling control
                imgBtnCommunity.OnClientClick = "openCommunitySelector" + this.ClientID + "(); return false;";
                rtvCommunitySelector.OnClientNodeChecked = "clientNodeChecked" + this.ClientID;
                btnAll.OnClientClicked = "function(button, args) { UpdateNodes" + this.ClientID + "(true); }";
                btnDefault.OnClientClicked = "SetDefaultItems" + this.ClientID;
                btnClear.OnClientClicked = "function(button, args) { UpdateNodes" + this.ClientID + "(false); }";
                btnCancel.OnClientClicked = "closeCommunitySelector" + this.ClientID;
            }
        }

        /// <summary>
        /// OnInit Event
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            //Fix for adding Update panel on Rad window
            ScriptManager sm = ScriptManager.GetCurrent(Page);
            MethodInfo m = (
            from methods in typeof(ScriptManager).GetMethods(
                BindingFlags.NonPublic | BindingFlags.Instance
                )
            where methods.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")
            select methods).First<MethodInfo>();

            m.Invoke(sm, new object[] { upCommunityPanel });
            base.OnInit(e);
        }

        /// <summary>
        /// Click event of button confirm
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            BindLabelText();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Rename all communities without 3GPP because not useful to display
        /// </summary>
        /// <param name="communityList"></param>
        private void Remove3GppFromCommunitiesTbName(List<Community> communityList)
        {
            foreach (var com in communityList)
            {
                com.TbName = com.TbName.Remove3GppAtTheBeginningOfAString();
            }
        }

        /// <summary>
        /// Add Plenary TSG records
        /// </summary>
        /// <param name="DataSource">Datasource</param>
        private void AddPlenaryRecords(List<Community> DataSource)
        {
            List<Community> plenaryCommunities = new List<Community>();
            var rootNodes = DataSource.Where(x => x.ParentTbId == 0);
            rootNodes.ToList().ForEach(rootNode => DataSource.FindAll(node => node.ParentTbId == rootNode.TbId).ForEach(level1node =>
            {
                if (DataSource.Any(x => x.ParentTbId == level1node.TbId))
                {
                    Community plenaryCommunity = new Community();
                    plenaryCommunity.TbId = level1node.TbId;
                    plenaryCommunity.ParentTbId = level1node.TbId;
                    plenaryCommunity.TbName = "Plenary " + level1node.TbName;
                    plenaryCommunity.ShortName = level1node.ShortName;
                    plenaryCommunity.ActiveCode = "ACTIVE";
                    plenaryCommunities.Add(plenaryCommunity);
                }
            }));

            // Add the plenary communities at the beginning.
            DataSource.Reverse();
            DataSource.AddRange(plenaryCommunities);
            DataSource.Reverse();
        }

        /// <summary>
        /// Bind Label of Community Control
        /// </summary>
        private void BindLabelText()
        {
            List<string> tbNames = new List<string>();
            foreach (RadTreeNode node in rtvCommunitySelector.CheckedNodes)
            {
                if (node.Nodes.Count == 0)
                    tbNames.Add(node.Text);
            }

            lblCommunity.Text = pnlCover.ToolTip = String.Join(", ", tbNames);
        }

        /// <summary>
        /// Update Parent node to checked, if all sibling nodes checked
        /// </summary>
        /// <param name="node">Tree Node</param>
        private void UpdateParent(RadTreeNode node)
        {
            if (node.ParentNode != null)
            {
                if (node.ParentNode.Nodes.Cast<RadTreeNode>().All(x => x.Checked))
                {
                    node.ParentNode.Checked = true;
                    UpdateParent(node.ParentNode);
                }
            }
        }

        #endregion
    }
}