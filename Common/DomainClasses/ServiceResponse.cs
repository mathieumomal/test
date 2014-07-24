using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class ServiceResponse<T>
    {
        public UserRightsContainer Rights { get; set; }
        public Report Report { get; set; }
        public T Result { get; set; }

        public ServiceResponse()
        {
            Rights = new UserRightsContainer();
            Report = new Report();
        }
    }
}
