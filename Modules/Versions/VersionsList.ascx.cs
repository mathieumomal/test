/*
' Copyright (c) 2014  Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Module.Versions
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from VersionsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase
    {
        #region Constants

        private const string CONST_LATEST_VERSIONS_FTP_MAPPED_PATH = "LatestVersionsFTPMappedPath";
        private const string CONST_SPEC_VERSIONS_FTP_PATH = "FtpSpecVersions";
        private const string CONST_FTP_LATEST_VERSIONS_PATH = "{0}\\Specs\\latest";
        private const string CONST_FTP_VERSIONS_BASE_PATH = "{0}\\Specs\\";
        private const string CONST_FTP_VERSIONS_MAP_PATH = "{0}\\Specs\\{1}";

        #endregion

        #region Events

        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    ConfigureControls();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Click event for Confirm button
        /// </summary>
        /// <param name="sender">Confirm Button</param>
        /// <param name="e">Click event arguments</param>
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string ftpBasePath = String.Empty;
                if (ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH] != null)
                    ftpBasePath = ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH].ToString();
                if (!String.IsNullOrEmpty(ftpBasePath))
                {
                    var sourcePath = String.Format(CONST_FTP_VERSIONS_MAP_PATH, ftpBasePath, rcbConfigFTP.Text);
                    var mapPath = String.Format(CONST_FTP_LATEST_VERSIONS_PATH, ftpBasePath);
                    
                    //Delete & Recreate 'Latest' directory
                    if (Directory.Exists(mapPath))
                        Directory.Delete(mapPath, true);
                    Directory.CreateDirectory(mapPath);

                    string[] fileNames = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);

                    //Create hard links for all files
                    foreach (string fileName in fileNames)
                    {
                        string hardLinkPath = fileName.Replace(sourcePath, mapPath);
                        string hardLinkDirectory = Path.GetDirectoryName(hardLinkPath);
                        if (!Directory.Exists(hardLinkDirectory))
                            Directory.CreateDirectory(hardLinkDirectory);

                        CreateHardLink(hardLinkPath, fileName, IntPtr.Zero);
                    }

                    var module = new ModuleController();
                    module.UpdateModuleSetting(ModuleId, CONST_LATEST_VERSIONS_FTP_MAPPED_PATH, sourcePath);

                    //Refresh Page with full postback
                    Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("Failed to configure FTP path for latest versions. \nError: " + ex.Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load initial values for FTP configuration
        /// </summary>
        private void ConfigureControls()
        {
            string ftpBasePath = String.Empty;
            if (ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH] != null)
                ftpBasePath = ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH].ToString();

            lblLatestVersionsPath.Text = String.Format(CONST_FTP_LATEST_VERSIONS_PATH, ftpBasePath);
            string currentPath = (Settings[CONST_LATEST_VERSIONS_FTP_MAPPED_PATH] == null) ? String.Empty : Settings[CONST_LATEST_VERSIONS_FTP_MAPPED_PATH].ToString();
            lblCurrentPath.Text = currentPath;
            lblNewPath.Text = currentPath;

            if (!String.IsNullOrEmpty(ftpBasePath))
            {
                string versionsFolder = String.Format(CONST_FTP_VERSIONS_BASE_PATH, ftpBasePath);
                Regex reg = new Regex(@"\d{4}_\d{2}");
                DirectoryInfo di = new DirectoryInfo(versionsFolder);
                var versionDirectories = di.GetDirectories().Where(path => reg.IsMatch(path.Name)).Select(x => new { x.FullName, x.Name }).ToList();
                rcbConfigFTP.DataTextField = "Name";
                rcbConfigFTP.DataValueField = "FullName";
                rcbConfigFTP.DataSource = versionDirectories;
                rcbConfigFTP.DataBind();

                if (!String.IsNullOrEmpty(currentPath))
                {
                    DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
                    if (versionDirectories.Exists(x => x.Name == currentDirectory.Name))
                        rcbConfigFTP.SelectedValue = currentDirectory.FullName;
                }
                else
                    lblNewPath.Text = rcbConfigFTP.SelectedValue;
            }
        }

        /// <summary>
        /// Create Hard links between files
        /// </summary>
        /// <param name="lpFileName">New File Name(Hard Link Path)</param>
        /// <param name="lpExistingFileName">Original File Name</param>
        /// <param name="lpSecurityAttributes">Security Attributes</param>
        /// <returns>True/False</returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );

        #endregion
    }
}