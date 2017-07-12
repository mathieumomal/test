using Etsi.Ultimate.DomainClasses;
using System;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Module.Specifications
{
    public partial class ReleaseHeaderControl : System.Web.UI.UserControl
    {
        #region Public Properties

        /// <summary>
        /// Specification Release
        /// </summary>
        public Specification_Release SpecRelease { get; set; }

        /// <summary>
        /// Edit Mode - True
        /// View Mode - False
        /// </summary>
        public bool IsEditMode { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Page Load event of Release Header Control
        /// </summary>
        /// <param name="sender">Release Header Control</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SpecRelease == null) return;
            SpecificationBasePage basePage = (SpecificationBasePage)this.Page;
            var userRights = basePage.SpecReleaseRights.Where(x => x.Key == SpecRelease.Pk_Specification_ReleaseId).FirstOrDefault().Value;

            if (!IsPostBack)
            {
                lblReleaseName.Text = SpecRelease.Release.Name;
                string status = string.Empty;
                if (SpecRelease.Specification.IsActive)
                {
                    bool? isWithdrawn = SpecRelease.isWithdrawn;
                    if (isWithdrawn != null && !isWithdrawn.Value)
                    {
                        if (SpecRelease.Specification.IsUnderChangeControl != null && SpecRelease.Specification.IsUnderChangeControl.Value)
                            status = "Spec is UCC for this Release";
                        else
                            status = "Spec is in Draft status";
                    }
                    else                    
                        status = "Spec is Withdrawn from this Release";                       
                    
                }
                else
                    status = "Spec is Withdrawn from this Release";

                if (SpecRelease.isWithdrawn.HasValue && SpecRelease.isWithdrawn.Value && SpecRelease.WithdrawMeetingId.HasValue && SpecRelease.WithdrawMeetingId > 0)
                {
                    var svc = ServicesFactory.Resolve<IMeetingService>();
                    string meetingName = svc.GetMeetingById(SpecRelease.WithdrawMeetingId.Value).MTG_REF;
                    if (meetingName.ToUpper().StartsWith("3GPP"))
                        meetingName = meetingName.Remove(0, 4);                        
                    status += " at " + meetingName;
                }


                lblStatus.Text = string.Format("({0})", status);

                if (SpecRelease.Remarks != null && SpecRelease.Remarks.Count > 0)
                {
                    //Latest remark case
                    Remark latestRemark;
                    if (userRights.HasRight(Enum_UserRights.Remarks_ViewPrivate))
                    {
                        latestRemark = SpecRelease.Remarks.OrderByDescending(x => x.CreationDate ?? DateTime.MinValue).FirstOrDefault();
                        if (latestRemark != null)
                        {
                            lblLatestRemark.Text = string.Format("({0}) {1}", (latestRemark.CreationDate != null) ? latestRemark.CreationDate.Value.ToString("yyyy-MM-dd") : String.Empty, UtilsFactory.TruncString(latestRemark.RemarkText, 50));
                            lblLatestRemark.ToolTip = latestRemark.RemarkText;
                        }
                    }
                    else if (SpecRelease.Remarks.Where(r => r.IsPublic.GetValueOrDefault()).ToList().Count > 0)
                    {
                        latestRemark = SpecRelease.Remarks.Where(r => r.IsPublic.GetValueOrDefault()).OrderByDescending(x => x.CreationDate ?? DateTime.MinValue).FirstOrDefault();
                        if (latestRemark != null)
                        {
                            lblLatestRemark.Text = string.Format("({0}) {1}", (latestRemark.CreationDate != null) ? latestRemark.CreationDate.Value.ToString("yyyy-MM-dd") : String.Empty, UtilsFactory.TruncString(latestRemark.RemarkText, 50));
                            lblLatestRemark.ToolTip = latestRemark.RemarkText;
                        }
                    }
                }
                imgRemarks.OnClientClick = "openRemarksPopup('specrelease','" + SpecRelease.Pk_Specification_ReleaseId + "','" + IsEditMode + "', 'Specification Release Remarks'); StopEvent(event); return false;";                    
            }                
        }

        #endregion
    }
}