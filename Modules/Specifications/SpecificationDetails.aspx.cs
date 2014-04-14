using Etsi.Ultimate.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class SpecificationDetails : System.Web.UI.Page
    {
        protected HistoryControl specificationHistory;
        protected RemarksControl specificationRemarks;
        protected RapporteurControl specificationRapporteurs;

        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_RESPONSIBILITY_TAB = "Responsibility";
        private static String CONST_RELATED_TAB = "Related";
        private static String CONST_RELEASES_TAB = "Releases";
        private static String CONST_HISTORY_TAB = "History";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {               
                LoadReleaseDetails();                
            }
        }

        private void LoadReleaseDetails()
        {
            BuildTabsDisplay(null);
        }

        /// <summary>
        /// Set the tabs display
        /// </summary>
        /// <param name="userRights"></param>
        private void BuildTabsDisplay(DomainClasses.UserRightsContainer userRights)
        {

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected = true
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RESPONSIBILITY_TAB,
                    Text = CONST_RESPONSIBILITY_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELATED_TAB,
                    Text = CONST_RELATED_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELEASES_TAB,
                    Text = CONST_RELEASES_TAB,
                    Selected = false
                });

            SpecificationDetailsRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_HISTORY_TAB,
                    Text = CONST_HISTORY_TAB,
                    Selected = false
                });
        }

        protected void EditSpecificationDetails_Click(object sender, EventArgs e)
        {
        }


        protected void WithdrawSpecificatione_Click(object sender, EventArgs e)
        {
        }


        protected void ExitSpecificationDetails_Click(object sender, EventArgs e)
        {
        }
    }

}