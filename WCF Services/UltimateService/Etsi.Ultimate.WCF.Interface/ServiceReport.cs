using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.WCF.Interface
{
    /// <summary>
    /// The report contains error, warning and information concerning the 
    /// operation that was requested.
    /// </summary>
    [DataContract]
    public class ServiceReport
    {

        public ServiceReport()
        {
            ErrorList = new List<string>();
            WarningList = new List<string>();
            InfoList = new List<string>();
        }

        /// <summary>
        /// Errors
        /// </summary>
        [DataMember]
        public List<string> ErrorList;

        /// <summary>
        /// Warnings
        /// </summary>
        [DataMember]
        public List<string> WarningList;

        /// <summary>
        /// Information
        /// </summary>
        [DataMember]
        public List<string> InfoList;
    }
}
