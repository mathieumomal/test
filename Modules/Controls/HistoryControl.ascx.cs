using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Controls
{
    /// <summary>
    /// History Control Component
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        #region Public
        /// <summary>
        /// Unit in pixel
        /// </summary>
        public int ScrollHeight { get; set; }

        private List<History> _dataSource;
        /// <summary>
        /// Data source for the RadGrid
        /// </summary>
        public List<History> DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                LoadGrid();
            }
        } 
        #endregion

        #region Page Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            SetGridProperties();
        } 
        #endregion

        #region Private
        private void LoadGrid()
        {
            historyTable.DataSource = DataSource;
            historyTable.DataBind();
        }
        private void SetGridProperties()
        {
            if (ScrollHeight > 0)
                historyTable.ClientSettings.Scrolling.ScrollHeight = Unit.Pixel(ScrollHeight);
        } 
        #endregion
    }
}