using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Controls
{
    public partial class ReleaseSearchControl : System.Web.UI.UserControl
    {
        public int Width { get; set; }
        public int DropDownWidth { get; set; }

        public List<int> SelectedReleaseIds
        {
            get
            {
                RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");

                //All Releases
                RadioButton rbAllReleases = (RadioButton)rtvReleases.Nodes[0].FindControl("rbAllReleases");
                if (rbAllReleases.Checked)
                    return rtvReleases.Nodes[0].Value.Split(',').Select(int.Parse).ToList();

                //Open Releases
                RadioButton rbOpenReleases = (RadioButton)rtvReleases.Nodes[1].FindControl("rbOpenReleases");
                if(rbOpenReleases.Checked)
                    return rtvReleases.Nodes[1].Value.Split(',').Select(int.Parse).ToList();

                //Custom Releases
                List<int> customReleaseIds = new List<int>();
                foreach (RadTreeNode node in rtvReleases.Nodes[2].Nodes)
                {
                    if(node.Checked)
                        customReleaseIds.Add(Convert.ToInt32(node.Value));
                }
                return customReleaseIds;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rcbReleases.Width = Width;
                rcbReleases.DropDownWidth = DropDownWidth;
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseObjects = svc.GetAllReleases(0);

                RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");
                List<int> allReleaseIds = new List<int>();
                List<int> openReleaseIds = new List<int>();
                foreach (var release in releaseObjects.Key)
                {
                    RadTreeNode newNode = new RadTreeNode();
                    newNode.Text = release.ShortName;
                    newNode.Value = release.Pk_ReleaseId.ToString();
                    rtvReleases.Nodes[2].Nodes.Add(newNode);

                    //Add to Open Release List
                    if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open)
                        openReleaseIds.Add(release.Pk_ReleaseId);
                    //Add to All Release List
                    allReleaseIds.Add(release.Pk_ReleaseId);
                }

                rtvReleases.Nodes[0].Value = String.Join(",", allReleaseIds);
                rtvReleases.Nodes[1].Value = String.Join(",", openReleaseIds);
            }
        }

        /// <summary>
        /// Reset to All Releases
        /// </summary>
        public void Reset()
        {
            RadTreeView rtvReleases = (RadTreeView)this.rcbReleases.Items[0].FindControl("rtvReleases");

            //All Releases
            RadioButton rbAllReleases = (RadioButton)rtvReleases.Nodes[0].FindControl("rbAllReleases");
            rbAllReleases.Checked = true;
        }
    }
}