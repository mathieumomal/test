using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Module.Release
{
    public partial class ReleaseDetails : ReleaseModuleBase
    {
        protected RemarksControl RemarksControlComponent;
        protected HistoryControl HistoryControlComponent;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            RemarksControlComponent.AddRemarkHandler += RemarksControlComponent_AddRemarkHandler;
        }

        private void LoadData()
        {
            IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
            DomainClasses.Release releaseObject = svc.GetReleaseById(1, Convert.ToInt32(Request["ReleaseId"])).Key;

            lblReleaseCode.Text = releaseObject.Code;
            lblReleaseStatus.Text = releaseObject.Enum_ReleaseStatus.ReleaseStatus;
            lblReleaseName.Text = releaseObject.Name;
            lblReleaseDescription.Text = releaseObject.Description;
            lblReleaseShortName.Text = releaseObject.ShortName;
            lblStartDate.Text = releaseObject.StartDate.ToString();
            lblEndDate.Text = releaseObject.EndDate.ToString();
            lblClosureDate.Text = releaseObject.ClosureDate.ToString();

            RemarksControlComponent.LoadGrid(releaseObject.Remarks.ToList());
            HistoryControlComponent.LoadGrid(releaseObject.Histories.ToList());
        }

        private void RemarksControlComponent_AddRemarkHandler(object sender, EventArgs e)
        {

        }
    }
}