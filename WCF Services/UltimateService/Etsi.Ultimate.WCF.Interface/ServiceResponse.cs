using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.WCF.Interface
{
    /// <summary>
    /// Represents a response that can be made by the service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ServiceResponse<T>
    {
        /// <summary>
        /// The report, containing possible errors, warnings and infos.
        /// </summary>
        [DataMember]
        public ServiceReport Report { get; set; }

        /// <summary>
        /// The result of the operation.
        /// </summary>
        [DataMember]
        public T Result { get; set; }
    }
}
