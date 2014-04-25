using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecificationReleaseFakeDBSet : FakeDBSet<Specification_Release>
    {
        public override Specification_Release Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_Specification_ReleaseId == keyValue);
        }
    }
}
