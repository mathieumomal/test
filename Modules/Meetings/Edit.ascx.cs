using System;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;

namespace Etsi.Ultimate.Module.Meetings
{
    public partial class Edit : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}