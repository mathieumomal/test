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
using Etsi.Ultimate.DomainClasses;
using Telerik.Web.UI;

namespace Etsi.Ultimate.Module.Specifications
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from SpecificationsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class SpecificationsList : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Speicification List ItemDataBound event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void rgSpecificationList_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                Specification currentSpecification = (Specification)e.Item.DataItem;
                Image img2G = (Image)dataItem.FindControl("img2G");
                Image img3G = (Image)dataItem.FindControl("img3G");
                Image imgLTE = (Image)dataItem.FindControl("imgLTE");

                if (img2G != null)
                    img2G.Visible = (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "2g").FirstOrDefault() != null);
                if (img3G != null)
                    img3G.Visible = (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "3g").FirstOrDefault() != null);
                if (imgLTE != null)
                    imgLTE.Visible = (currentSpecification.SpecificationTechnologies.ToList().Where(x => x.Enum_Technology.Code.ToLower() == "lte").FirstOrDefault() != null);
            }
        }

        /// <summary>
        /// Need DataSource event for Specification List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void rgSpecificationList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            ////TODO:: Remove the below code after implementation with real data
            //Etsi.Ultimate.Services.SpecificationServiceMock specMock = new Etsi.Ultimate.Services.SpecificationServiceMock();
            //var specDetails = specMock.GetSpecificationDetails(0);

            //rgSpecificationList.DataSource = specDetails.Key;
        }
    }
}