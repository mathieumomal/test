using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business.Facades
{
    public class SpecVersionFoundationCrs
    {
        public int VersionId { get; set; }
        public List<ChangeRequest> FoundationCrs { get; set; } 
    }
}
