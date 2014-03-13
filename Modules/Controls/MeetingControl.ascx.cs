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
        public bool DisplayLabel { get; set; }

        /// <summary>
        /// Default selected meeting id
        /// </summary>
        public int SelectedMeetingId { get; set; }

        /// <summary>
        /// Set style to EndDate label
        /// </summary>
        public string CssClass { get; set; }
        #endregion

        #region Page/Control Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            SetDropdownProperties();

            if (!IsPostBack)
            {
                if (DataSource == null)
                {
                    IMeetingService svc = ServicesFactory.Resolve<IMeetingService>();
                    if (SelectedMeetingId != default(int))
                        DataSource = svc.GetLatestMeetings(SelectedMeetingId);
                    else
                        DataSource = svc.GetLatestMeetings();
                }
                BindDropDownData(DataSource);
            }
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
                combo.Items.FindItem(x => x.Value.StartsWith(SelectedMeetingId.ToString())).Selected = true;

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
            var CustomDataSource = (from x in meetingsList
                                    orderby x.START_DATE descending
                                    select new
                                    {
                                        Value = x.MTG_ID.ToString() + "|" + x.END_DATE.Value.ToString("yyyy-MM-dd"),
                                        Text = x.LOC_CITY.Length > 0 ? String.Format("{0} ({1} - {2}({3}))",
                                        x.MtgShortRef, x.START_DATE.Value.ToString("yyyy-MM-dd"), x.LOC_CITY, x.LOC_CTY_CODE)
                                        : String.Format("{0} ({1})", x.MtgShortRef, x.START_DATE.Value.ToString("yyyy-MM-dd"))
                                    });
            rcbMeetings.DataSource = CustomDataSource;
            rcbMeetings.DataTextField = "Text";
            rcbMeetings.DataValueField = "Value";

            rcbMeetings.DataBind();
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
        #endregion
    }
}