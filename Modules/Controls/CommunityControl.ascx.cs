using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    public partial class CommunityControl : System.Web.UI.UserControl
    {
        #region Properties

        private List<int> _selectedCommunityIds;
        public List<int> SelectedCommunityIds
        {
            get
            {
                if (_selectedCommunityIds == null)
                    _selectedCommunityIds = new List<int>();
                return _selectedCommunityIds;
            }
            set
            {
                _selectedCommunityIds = value;
            }
        }

        private int _selectedCommunityID;
        public int SelectedCommunityID
        {
            get { return Convert.ToInt32(rcbCommunity.SelectedValue); }
            set { _selectedCommunityID = value; }
        }

        public bool IsSingleSelection { get; set; }
        public bool IsEditMode { get; set; }

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
                List<Community>  dataSource = svc.GetCommunities();
                AddMissingParent(dataSource);

                if (IsSingleSelection)
                {                
                    imgBtnCommunity.Visible = false;

                    if (IsEditMode)
                    {
                        rcbCommunity.Visible = true;
                        lblCommunity.Visible = false;

                        rcbCommunity.DataSource = dataSource;
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
                            lblCommunity.Text = (selectedCommunity == null) ? String.Empty : selectedCommunity.ShortName;
                        }
                    }
                }
                else
                {
                    rcbCommunity.Visible = false;
                    imgBtnCommunity.Visible = false;
                    lblCommunity.Visible = true;

                    if (_selectedCommunityIds != null)
                    {
                        var selectedCommunityShortNames = dataSource.FindAll(x => _selectedCommunityIds.Contains(x.TbId)).Select(x => x.ShortName).ToList();
                        lblCommunity.Text = String.Join(", ", selectedCommunityShortNames);
                    }

                    if (IsEditMode)
                    {
                        imgBtnCommunity.Visible = true;

                        rtvCommunitySelector.DataTextField = "TbName";
                        rtvCommunitySelector.DataValueField = "TbId";
                        rtvCommunitySelector.DataFieldID = "TbId";
                        rtvCommunitySelector.DataFieldParentID = "ParentCommunityId";
                        rtvCommunitySelector.DataSource = dataSource;
                        rtvCommunitySelector.DataBind();

                        if (_selectedCommunityIds != null)
                        {
                          //TODO::
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Click event of button confirm
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            List<string> tbNames = new List<string>();
            List<int> tbIds = new List<int>();
            foreach (RadTreeNode node in rtvCommunitySelector.CheckedNodes)
            {
                tbNames.Add(node.Text);
                tbIds.Add(Convert.ToInt32(node.Value));
            }

            lblCommunity.Text = String.Join(", ", tbNames);
            SelectedCommunityIds = tbIds;
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
                if (community.ParentTbId != null && community.ParentTbId != 0)
                {
                    var parentCommunity = DataSource.Find(x => x.TbId == community.ParentTbId);
                    if ((parentCommunity == null) && (!missingParentCommunities.Exists(x => x.TbId == community.ParentCommunityId)))
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