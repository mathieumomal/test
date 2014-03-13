using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
{
    interface IUrlBusiness
    {
        String GetUrl();
        String GetShortUrl();
    }
}
