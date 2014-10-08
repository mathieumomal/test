using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class ChangeRequest
    {
        public string targetSpecificationNumber
        {
            get
            {
                return Specification.Number;
            }
        }

        public string crNumberAndRevision
        {
            get
            {
                return CRNumber+Revision;
            }
        }

        public string WgMtgShortRef { get; set; }

        public string WgTDocLink { get; set; }

        public string TsgMtgShortRef { get; set; }

        public string TsgTDocLink { get; set; }

        public bool IsRevisionCreationEnabled { get; set; }

        public bool IsTDocCreationEnabled { get; set; }

        public string ImplementationStatus { get; set; } 

        public string RevisionOf { get; set; }
    }
}
