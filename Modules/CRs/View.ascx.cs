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
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Etsi.Ultimate.DomainClasses;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace Etsi.Ultimate.Module.CRs
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CRsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Test data
                ChangeRequest c = new ChangeRequest();
                c.Specification = new Specification()
                {
                    Pk_SpecificationId = 1,
                    Number = "SPEC123"
                };
                c.Release = new Release()
                {
                    Pk_ReleaseId = 1,
                    Name = "test"
                };
                c.Revision = 12;
                c.CRNumber = "CR123";
                c.Subject = "New CR for test";
                c.WgMtgShortRef = "WG_MTG54";
                c.WgTDocLink = "#";
                c.Enum_TDocStatusWG = new Enum_TDocStatus() { Pk_EnumTDocStatus = 1, WGUsable = true, Code = "WG status" };
                c.TsgMtgShortRef = "TSG_MTG54";
                c.TsgTDocLink = "#";
                c.Enum_TDocStatusTSG = new Enum_TDocStatus() { Pk_EnumTDocStatus = 2, TSGUsable = true, Code = "TSG status" };
                c.ImplementationStatus = "Done";
                c.IsRevisionCreationEnabled = true;
                c.IsTDocCreationEnabled = true;
                ChangeRequest c1 = CloneCR(c);
                c1.IsRevisionCreationEnabled = false;
                ChangeRequest c2 = CloneCR(c);
                c2.IsTDocCreationEnabled = false;
                List<ChangeRequest> l = new List<ChangeRequest>() { c,c1 , c2};
                crsTable.DataSource = l;
                crsTable.DataBind();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Tmp operation to clone a ChangeRequest object for test purposes
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        private ChangeRequest CloneCR(ChangeRequest cr)
        {
            ChangeRequest c = new ChangeRequest();
            c.Specification = cr.Specification;
            c.Release = cr.Release;
            c.Revision = cr.Revision;
            c.CRNumber = cr.CRNumber;
            c.WgMtgShortRef = cr.WgMtgShortRef;
            c.WgTDocLink = cr.WgTDocLink;
            c.TsgMtgShortRef = cr.TsgMtgShortRef;
            c.TsgTDocLink = cr.TsgTDocLink;
            c.Enum_TDocStatusTSG = cr.Enum_TDocStatusTSG;
            c.ImplementationStatus = cr.ImplementationStatus;
            c.IsRevisionCreationEnabled = cr.IsRevisionCreationEnabled;
            c.IsTDocCreationEnabled = cr.IsTDocCreationEnabled;
            return c;
        }
        /// <summary>
        /// Speicification List ItemDataBound event
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void crsTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                //Get CR row
                DomainClasses.ChangeRequest currentCR = (DomainClasses.ChangeRequest)e.Item.DataItem;
                HyperLink createRevisionAction = e.Item.FindControl("CreateRevisionAction") as HyperLink;
                createRevisionAction.Enabled = currentCR.IsRevisionCreationEnabled;
                HyperLink createTDocAction = e.Item.FindControl("CreateTDocAction") as HyperLink;
                createTDocAction.Enabled = currentCR.IsRevisionCreationEnabled;
            }
        }
        
    }
}