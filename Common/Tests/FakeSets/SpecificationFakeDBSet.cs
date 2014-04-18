using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecificationFakeDBSet : FakeDBSet<Specification>
    {
        public override Specification Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(speci => speci.Pk_SpecificationId == keyValue);
        }
    }
}
