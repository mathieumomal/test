using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            set {
                var width = (value >CONST_SELECT_WIDTH) ? value : CONST_SELECT_WIDTH;
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
                List<Community> dataSource = svc.GetCommunities();
                AddMissingParent(dataSource); // Add missing parents dynamically

                if (IsSingleSelection) //Single Selection
                {
                    imgBtnCommunity.Visible = false;

                    if (IsEditMode)
                    {
                        rcbCommunity.Visible = true;
                        lblCommunity.Visible = false;

                        rcbCommunity.DataSource = dataSource.OrderBy(x => x.TbId);
                        rcbCommunity.DataTextField = "TbName";
                        rcbCommunity.DataValueField = "TbId";
                        rcbCommunity.DataBind();

                        if (_selectedCommunityID != default(int))
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
                        var selectedCommunityNames = dataSource.FindAll(x => _selectedCommunityIds.Contains(x.TbId)).Select(x => x.TbName).ToList();
                        lblCommunity.Text = pnlCover.ToolTip = String.Join(", ", selectedCommunityNames);
                    }

                    if (IsEditMode)
                    {
                        imgBtnCommunity.Visible = true;

                        rtvCommunitySelector.DataTextField = "TbName";
                        rtvCommunitySelector.DataValueField = "TbId";
                        rtvCommunitySelector.DataFieldID = "TbId";
                        rtvCommunitySelector.DataFieldParentID = "ParentCommunityId";
                        rtvCommunitySelector.DataSource = dataSource.OrderBy(x => x.TbId);
                        rtvCommunitySelector.DataBind();

                        if (_selectedCommunityIds != null)
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
                    }
                }
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

        private void BindLabelText()
        {
            List<string> tbNames = new List<string>();
            foreach (RadTreeNode node in rtvCommunitySelector.CheckedNodes)
            {
                tbNames.Add(node.Text);
            }

            lblCommunity.Text = pnlCover.ToolTip = String.Join(", ", tbNames);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add missing parent record
        /// </summary>
        /// <param name="DataSource">Datasource</param>
        private void AddMissingParent(List<Community> DataSource)
        {
            List<Community> missingParentCommunities = new List<Community>();

            foreach (var community in DataSource)
            {
                if (community.ParentTbId != null && community.ParentTbId != 0) //Don't process the root nodes
                {
                    var parentCommunity = DataSource.Find(x => x.TbId == community.ParentTbId);
                    if ((parentCommunity == null) && (!missingParentCommunities.Exists(x => x.TbId == community.ParentCommunityId))) //If parent missing, add the same
                    {
                        Community missingParentCommunity = new Community();
                        missingParentCommunity.TbId = community.ParentCommunityId;
                        missingParentCommunity.ParentTbId = 0;
                        missingParentCommunity.TbName = community.TbName.Split(' ')[0];
                        missingParentCommunity.ShortName = community.TbName.Split(' ')[0];
                        missingParentCommunity.ActiveCode = "ACTIVE";
                        missingParentCommunities.Add(missingParentCommunity);
                    }
                }
            }

            DataSource.AddRange(missingParentCommunities);
        }

        #endregion
    }
}