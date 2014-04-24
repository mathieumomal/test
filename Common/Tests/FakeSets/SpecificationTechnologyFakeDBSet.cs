using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecificationTechnologyFakeDBSet : FakeDBSet<SpecificationTechnology>
    {
        public override SpecificationTechnology Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(speci => speci.Pk_SpecificationTechnologyId == keyValue);
        }
    }
}
