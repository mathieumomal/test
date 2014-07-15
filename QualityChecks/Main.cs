using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QualityChecks
{
    /// <summary>
    /// Class to process documents for Quality Checks
    /// </summary>
    public partial class frmMain : Form
    {
        #region Constructor

        /// <summary>
        /// Default Constructor to initialize controls
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        } 

        #endregion

        #region Events

        /// <summary>
        /// Click event of QCFolder button
        /// </summary>
        /// <param name="sender">QCFolder button</param>
        /// <param name="e">Event arguments</param>
        private void btnQCFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (DialogResult.OK == folderDialog.ShowDialog())
                txtQCFolder.Text = folderDialog.SelectedPath;
        }

        /// <summary>
        /// Click event of Process Quality Checks button
        /// </summary>
        /// <param name="sender">Process Quality Checks button</param>
        /// <param name="e">Event arguments</param>
        private void btnProcessQualityChecks_Click(object sender, EventArgs e)
        {
            rtbQCResults.Clear();
            btnExport.Visible = false;

            this.Cursor = Cursors.WaitCursor;
            string QCPath = txtQCFolder.Text;
            if (!String.IsNullOrEmpty(QCPath))
            {
                if (Directory.Exists(QCPath))
                {
                    Regex reg = new Regex(@"^[0-9]{5,}-([A-Za-z0-9]{6}|[A-Za-z0-9]{3})\.zip$");
                    var files = Directory.EnumerateFiles(QCPath, "*.zip", SearchOption.AllDirectories).Where(x => reg.IsMatch(Path.GetFileName(x)));

                    StringBuilder sbFileNames = new StringBuilder();
                    foreach (var fileName in files)
                    {
                        //Specification
                        ISpecificationService specService = ServicesFactory.Resolve<ISpecificationService>();
                        string specNumber = Path.GetFileNameWithoutExtension(fileName).Split('-')[0];
                        var spec = specService.GetSpecificationByNumber(String.Format("{0}.{1}", specNumber.Substring(0, 2), specNumber.Substring(2, specNumber.Length - 2)));

                        if (spec == null)
                            continue;

                        //SpecVersion
                        var utilsService = new UtilsService();
                        string base36VersionNumber = Path.GetFileNameWithoutExtension(fileName).Split('-')[1];
                        int majorVersion = Convert.ToInt32(utilsService.DecodeBase36ToDecimal((base36VersionNumber.Length == 3) ? base36VersionNumber.Substring(0, 1) : base36VersionNumber.Substring(0, 2)));
                        int technicalVersion = Convert.ToInt32(utilsService.DecodeBase36ToDecimal((base36VersionNumber.Length == 3) ? base36VersionNumber.Substring(1, 1) : base36VersionNumber.Substring(2, 2)));
                        int editorialVersion = Convert.ToInt32(utilsService.DecodeBase36ToDecimal((base36VersionNumber.Length == 3) ? base36VersionNumber.Substring(2, 1) : base36VersionNumber.Substring(4, 2)));
                        var version = String.Format("{0}.{1}.{2}", majorVersion, technicalVersion, editorialVersion);
                        var specVersion = spec.Versions.Where(x => x.MajorVersion == majorVersion && x.TechnicalVersion == technicalVersion && x.EditorialVersion == editorialVersion).FirstOrDefault();

                        //Meeting
                        IMeetingService meetingService = ServicesFactory.Resolve<IMeetingService>();
                        var meeting = meetingService.GetMeetingById(specVersion.Source ?? 0);
                        DateTime meetingStartDate = DateTime.MinValue;
                        if (meeting != null)
                            meetingStartDate = meeting.START_DATE ?? DateTime.MinValue;

                        //Community
                        ICommunityService communityService = ServicesFactory.Resolve<ICommunityService>();
                        string tsgTitle = String.Empty;
                        var communities = communityService.GetCommunities();
                        var community = communities.Where(x => x.TbId == spec.PrimeResponsibleGroup.Fk_commityId).FirstOrDefault();
                        if (community != null)
                            tsgTitle = GetParentTSG(community, communities).TbTitle.Replace("Technical Specification Group -", "Technical Specification Group");

                        //File
                        var extension = String.Empty;
                        bool isValidFile = false;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ZipArchive zip = ZipFile.OpenRead(fileName);
                            foreach (ZipArchiveEntry entry in zip.Entries)
                            {
                                extension = Path.GetExtension(entry.Name);
                                if (extension.Equals(".doc", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    isValidFile = true;
                                    using (var stream = entry.Open())
                                    {
                                        stream.CopyTo(ms);
                                        ms.Position = 0;
                                    }
                                    break;
                                }
                            }

                            if (isValidFile)
                            {
                                SpecVersionsManager specVersionsManager = new SpecVersionsManager();
                                Report report = specVersionsManager.ValidateVersionDocument(extension, ms, System.IO.Path.GetTempPath(), version, spec.Title, specVersion.Release.Name, meetingStartDate, tsgTitle, spec.IsTS ?? true);

                                sbFileNames.AppendLine(fileName);
                                sbFileNames.AppendLine("=========================================================");
                                if (report.ErrorList.Count > 0)
                                {
                                    sbFileNames.AppendLine("Errors::");
                                    sbFileNames.AppendLine("========");
                                    sbFileNames.AppendLine(String.Join("\n", report.ErrorList));
                                }
                                if (report.WarningList.Count > 0)
                                {
                                    sbFileNames.AppendLine("Warnings::");
                                    sbFileNames.AppendLine("==========");
                                    sbFileNames.AppendLine(String.Join("\n", report.WarningList));
                                }
                                if (report.ErrorList.Count == 0 && report.WarningList.Count == 0)
                                {
                                    sbFileNames.AppendLine("No Errors & Warnings");
                                }
                                sbFileNames.AppendLine("=========================================================");
                            }
                        }
                    }

                    rtbQCResults.AppendText(sbFileNames.ToString());
                }
                else
                {
                    rtbQCResults.Text = "Selected path didn't exists";
                }
            }
            else
            {
                rtbQCResults.Text = "Please select QC path";
            }

            this.Cursor = Cursors.Default;
            btnExport.Visible = true;
        }

        /// <summary>
        /// Click event of Export button
        /// </summary>
        /// <param name="sender">Export button</param>
        /// <param name="e">Event arguments</param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Rich text files (*.rtf)|*.rtf|All files (*.*)|*.*";
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                rtbQCResults.SaveFile(sfd.FileName, RichTextBoxStreamType.RichText);
            }
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Parent TSG
        /// </summary>
        /// <param name="community">Community</param>
        /// <param name="communities">List of Communities</param>
        /// <returns>Parent TSG community</returns>
        private Community GetParentTSG(Community community, List<Community> communities)
        {
            if (community.TbType == "TSG")
                return community;
            else
            {
                var parentCommunity = communities.Where(x => x.TbId == community.ParentCommunityId).FirstOrDefault();
                if (parentCommunity != null)
                    return GetParentTSG(parentCommunity, communities);
                else
                    return community;
            }
        } 

        #endregion
    }
}
