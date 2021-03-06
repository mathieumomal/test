﻿using System.Runtime.Serialization;

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
