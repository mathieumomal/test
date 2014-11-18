using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Etsi.Ultimate.Services;
using DotNetNuke.Entities.Users;
using Telerik.Web.UI;
using System.Data;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// Meeting Selector Control
    /// </summary>
    public partial class MeetingControl : UserControl
    {
        #region Public
        /// <summary>
        /// Unit in pixel
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// Unit in pixel
        /// </summary>
        public int width { get; set; }

        /// <summary>
        /// Data soruce for the dropdown
        /// </summary>
        public List<Meeting> DataSource { get; set; }

        /// <summary>
        /// Display meeting EndDate label (true)
        /// </summary>
        public bool DisplayLabel
        {
            get
            {
                bool display;
                if (bool.TryParse((string)ViewState["MTG_" + ID + "DisplayLabel"], out display))
                    return display;
                return false;
            }
            set
            {
                ViewState["MTG_" + ID + "DisplayLabel"] = value.ToString();
            }
        }

        /// <summary>
        /// Default selected meeting id
        /// </summary>
        public int SelectedMeetingId
        {
            get
            {
                int id;
                if (Int32.TryParse((string)ViewState["MTG_" + ID + "Selected"], out id))
                    return id;
                return 0;
            }
            set
            {
                ViewState["MTG_" + ID + "Selected"] = value.ToString();
            }
        }

        /// <summary>
        /// Selected meeting object
        /// </summary>
        public Meeting SelectedMeeting
        {
            get
            {
                var svc = ServicesFactory.Resolve<IMeetingService>();
                return svc.GetMeetingById(this.SelectedMeetingId);
            }
        }

        /// <summary>
        /// Set style to EndDate label
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// ClientID of Meeting Dropdown list
        /// </summary>
        public string MeetingClientID 
        {
            get { return this.rcbMeetings.ClientID; }
        }

        public bool NewestFirst { get; set; }
        public string TbRestriction { get; set; }

        #endregion

        #region Page/Control Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            SetDropdownProperties();

            if (!IsPostBack)
            {
                if (DataSource == null)
                {
                    var svc = ServicesFactory.Resolve<IMeetingService>();
                    if (SelectedMeetingId != default(int))
                        DataSource = svc.GetLatestMeetings(SelectedMeetingId);
                    else
                        DataSource = svc.GetLatestMeetings();
                }
                BindDropDownData(DataSource);

                rcbMeetings.OnClientSelectedIndexChanged = "OnClientSelectedIndexChanged" + lblEndDate.ClientID;
                rcbMeetings.OnClientTextChange = "OnClientTextChanged" + lblEndDate.ClientID;
            }
            if (!string.IsNullOrEmpty(rcbMeetings.SelectedValue) && !string.IsNullOrEmpty(rcbMeetings.SelectedValue.Split('|')[1]))
                lblEndDate.Text = rcbMeetings.SelectedValue.Split('|')[1];
            
        }
        protected void rcbMeetings_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (e.Text.Length > 1)
            {
                if (DataSource == null)
                {
                    IMeetingService svc = ServicesFactory.Resolve<IMeetingService>();
                    DataSource = svc.GetMatchingMeetings(e.Text);
                }
                BindDropDownData(DataSource);
            }
        }
        protected void rcbMeetings_DataBound(object sender, EventArgs e)
        {
            var combo = (RadComboBox)sender;

            if (!IsPostBack && SelectedMeetingId != default(int) && combo.Items.Count > 0)
            {
                if (combo.Items.FindItem(x => x.Value.StartsWith(SelectedMeetingId.ToString())) != null)
                    combo.Items.FindItem(x => x.Value.StartsWith(SelectedMeetingId.ToString())).Selected = true;
            }

            combo.Items.Insert(0, new RadComboBoxItem(" ", "-1|-"));
        }
        protected void rcbMeetings_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = (RadComboBox)sender;
            if (combo.SelectedValue != null && !String.IsNullOrEmpty(combo.SelectedValue.Split('|')[0]))
                SelectedMeetingId = Convert.ToInt32(combo.SelectedValue.Split('|')[0]);
        }
        #endregion

        #region Private
        private void BindDropDownData(List<DomainClasses.Meeting> meetingsList)
        {
            rcbMeetings.DataSource = FilterByParameters(meetingsList);
            rcbMeetings.DataTextField = "MtgDdlText";
            rcbMeetings.DataValueField = "MtgDdlValue";
            rcbMeetings.DataBind();

            if (SelectedMeetingId != 0 && meetingsList.Where(m => m.MTG_ID == SelectedMeetingId).Count() > 0
                && meetingsList.Where(m => m.MTG_ID == SelectedMeetingId).FirstOrDefault().END_DATE != null)
                lblEndDate.Text = meetingsList.Where(m => m.MTG_ID == SelectedMeetingId).FirstOrDefault().END_DATE.Value.ToString("yyyy-MM-dd");
        }

        private void SetDropdownProperties()
        {
            if (MaxHeight > 0)
                rcbMeetings.MaxHeight = Unit.Pixel(MaxHeight);
            if (width > 0)
                rcbMeetings.Width = Unit.Pixel(width);

            //Set meeting end date properties
            pnlEndDate.Visible = DisplayLabel;
            pnlEndDate.CssClass = CssClass;
        }

        private List<Meeting> FilterByParameters(List<Meeting> datasource)
        {
            //Control parameters (added for the occasion of ITU recommendations):
            if (NewestFirst)
            {
                datasource = datasource.OrderByDescending(x => x.Creation_Date).ToList();
            }
            if (!string.IsNullOrEmpty(TbRestriction))
            {
                datasource = datasource.Where(x => x.ShortName.Trim().Equals(TbRestriction.Trim())).ToList();
            }
            return datasource;
        } 
        
        #endregion
    }
}