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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Services.Installer.Dependencies;
using Etsi.Ultimate.Module.Release.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Etsi.Ultimate.Services;
using Microsoft.Practices.ObjectBuilder2;
using Rhino.Mocks;
using Telerik.Web.UI;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Module.Release
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ReleaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : ReleaseModuleBase, IActionable
    {
        private static String freezeReach = "rgb(39, 116, 0)";

        private static List<String> freezeDates = new List<string>()
        {

        };

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Remplace ReleaseService by a mock (fake object)
                ServicesFactory.Container.RegisterType<IReleaseService, ReleaseServiceMock>(new TransientLifetimeManager());

                IReleaseService svc;
                svc = ServicesFactory.Resolve<IReleaseService>();//Get the mock instead service classe

                List<DomainClasses.Release> releaseObjects = svc.GetAllReleases();
                releasesTable.DataSource = releaseObjects;

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }

        protected string FormatDate(object date)
        {
            if (date != null)
                return Convert.ToDateTime(date).ToString("yyyy-MM-dd");
            return null;
        }

        public void releasesTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            //Analyse on row
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                DomainClasses.Release currentRelease = (DomainClasses.Release) e.Item.DataItem;
                //Analyse column : Closure date
                if (currentRelease.Enum_ReleaseStatus.ReleaseStatus.Equals("Closed"))
                {
                    TableCell closureDate = dataItem["ClosureDate"];
                    closureDate.Text =
                        new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.ClosureDate))
                            .Append(" (")
                            .Append(currentRelease.ClosureMtgRef)
                            .Append(")")
                            .ToString();
                    closureDate.CssClass = currentRelease.Enum_ReleaseStatus.ReleaseStatus.ToLower();
                }

                DateTime now = DateTime.Now;


                //Analyse column : Freeze 1
                if (currentRelease.Stage1FreezeDate != null)
                {
                    DateTime dateStage1FreezeDate = (DateTime)currentRelease.Stage1FreezeDate;
                    if (now > dateStage1FreezeDate.Date)
                    {
                        TableCell freeze1 = dataItem["Stage1FreezeDate"];
                        if (currentRelease.Stage1FreezeMtgRef != null)
                        {
                            freeze1.Text =
                            new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage1FreezeDate))
                                .Append(" (")
                                .Append(currentRelease.Stage1FreezeMtgRef)
                                .Append(")")
                                .ToString();
                        }
                        freeze1.CssClass = freezeReach;
                    }
                }
                

                //Analyse column : Freeze 2
                if (currentRelease.Stage2FreezeDate!=null)
                {
                    DateTime dateStage2FreezeDate = (DateTime)currentRelease.Stage2FreezeDate;
                    if (now > dateStage2FreezeDate.Date)
                    {
                        TableCell freeze2 = dataItem["Stage2FreezeDate"];
                        if (currentRelease.Stage2FreezeMtgRef != null)
                        {
                            freeze2.Text =
                            new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage2FreezeDate))
                                .Append(" (")
                                .Append(currentRelease.Stage2FreezeMtgRef)
                                .Append(")")
                                .ToString();
                        }
                        freeze2.CssClass = freezeReach;
                    }
                }
               

                //Analyse column : Freeze 3
                if (currentRelease.Stage3FreezeDate != null)
                {
                    DateTime dateStage3FreezeDate = (DateTime)currentRelease.Stage3FreezeDate;
                    if (now > dateStage3FreezeDate.Date)
                    {
                        TableCell freeze3 = dataItem["Stage3FreezeDate"];
                        if (currentRelease.Stage3FreezeMtgRef != null)
                        {
                            freeze3.Text =
                            new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage3FreezeDate))
                                .Append(" (")
                                .Append(currentRelease.Stage3FreezeMtgRef)
                                .Append(")")
                                .ToString();
                        }
                        freeze3.CssClass = freezeReach;
                    }
                }
                
                

            }
        }
    }


    #region Useful Objects
    public class ReleaseObjectView
    {
        public DomainClasses.Release release { get; set; }
        public Properties properties { get; set; }

        public ReleaseObjectView(DomainClasses.Release release, Properties properties)
        {
            this.release = release;
            this.properties = properties;
        }
    }

    public class Properties
    {
        public String statusColor_CSSClasse { get; set; }
        public String closureDateColor_CSSClasse { get; set; }
        public String closureDateCompl_String { get; set; }

        public Properties()
        {
            this.statusColor_CSSClasse = "";
            this.closureDateColor_CSSClasse = "";
            this.closureDateCompl_String = "";
        }
    }
    #endregion  
}