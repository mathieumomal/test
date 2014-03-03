using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Module.Release
{
    public partial class ReleaseDetails : System.Web.UI.Page
    {
        #region fields
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_ADMIN_TAB = "Administration";
        private static String CONST_HISTORY_TAB = "History";

        private int UserId;
        private int ReleaseId;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            getRequestParameters();
            if (!Page.IsPostBack)
            {
                BuildTabsDisplay();
            }
            IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();//Get the mock instead service classe

            //Example : mock to fake rights manager : ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId, ReleaseId);            

            //Mock
            var statusClosed = new Domain.Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" };
            Domain.Release release = releaseRightsObject.Key;            
            FillGeneralTab(release);
            ManageButtonDisplay(release);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        private void BuildTabsDisplay()
        {

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB

                });            

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_ADMIN_TAB,
                    Text = CONST_ADMIN_TAB
                });

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_HISTORY_TAB,
                    Text = CONST_HISTORY_TAB
                });

        }

        private void FillGeneralTab(Domain.Release release)
        {
            releaseCodeVal.Text = release.Code;
            ReleaseStatusVal.Text = release.Enum_ReleaseStatus.ReleaseStatus;
            ReleaseNameVal.Text = release.Name;
            ReleaseDescVal.Src = release.Description;
            ReleaseShortNameVal.Text = release.ShortName;
            if (release.StartDate != null)
                ReleaseStartDateVal.Text = Convert.ToDateTime(release.StartDate).ToString("yyyy-MM-dd");

            ReleaseFreezeStage1Meeting.Text = release.Stage1FreezeMtgRef;
            if (release.Stage1FreezeDate != null)
                ReleaseFreezeStage1Date.Text = Convert.ToDateTime(release.Stage1FreezeDate).ToString("yyyy-MM-dd");

            ReleaseFreezeStage2Meeting.Text = release.Stage2FreezeMtgRef;
            if (release.Stage2FreezeDate != null)
                ReleaseFreezeStage2Date.Text = Convert.ToDateTime(release.Stage2FreezeDate).ToString("yyyy-MM-dd");

            if (release.Stage3FreezeDate != null)
                ReleaseFreezeStage3Date.Text = Convert.ToDateTime(release.Stage3FreezeDate).ToString("yyyy-MM-dd");

            if (release.EndDate != null)
                ReleaseEndDateVal.Text = Convert.ToDateTime(release.EndDate).ToString("yyyy-MM-dd");

            if (release.ClosureDate != null)
                ReleaseClosureDateVal.Text = Convert.ToDateTime(release.ClosureDate).ToString("yyyy-MM-dd");

            
        }

        private void getRequestParameters()
        {
            UserId = (Request.QueryString["UserID"]!=null) ? int.Parse(Request.QueryString["UserID"]) : -1;
            ReleaseId = (Request.QueryString["releaseId"]!=null) ? int.Parse(Request.QueryString["releaseId"]) : -1;
        }


        private void ManageButtonDisplay(Domain.Release release)
        {
            string status = release.Enum_ReleaseStatus.ReleaseStatus;
            switch (status)
            {
                case "Closed":                    
                    CloseReleaseBtn.Visible = false;
                    FreezeReleaseBtn.Visible = false; 
                    EditBtn.Visible = false;
                    break;
                case "Frozen":
                    FreezeReleaseBtn.Visible = false;
                    break;                
                default:
                    CloseReleaseBtn.Visible = true;
                    FreezeReleaseBtn.Visible = true;
                    EditBtn.Visible = true;
                    break;
            }
        }
        protected void ReleaseDetailRadTabStrip_Click(object sender, EventArgs e)
        {
        }
    }
}