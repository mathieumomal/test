using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// Enum which contains modules settings
    /// Notice : 
    /// --- to get a setting anywhere : 
    /// if(Settings.Contains(Enum_Settings.WorkItem_UploadPath.ToString()))
    ///     importButton.Text = Settings[Enum_Settings.WorkItem_UploadPath.ToString()].ToString();
    /// --- to draw and set a setting : example in WorkItem/Settings.ascx.cs
    /// </summary>
    public enum Enum_Settings
    {
        //WorkItem settings
        WorkItem_UploadPath,
        WorkItem_ExportPath,
        
        //Specification settings
        Spec_ExportPath,

        //Version settings
        Version_UploadPath,
        Version_FtpBasePath
    }
}
