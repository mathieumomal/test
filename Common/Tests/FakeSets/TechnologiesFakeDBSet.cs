using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    class TechnologiesFakeDBSet : FakeDBSet<SpecificationTechnology>
    {
        public override SpecificationTechnology Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.Pk_SpecificationTechnologyId == keyValue);
        }
    }
}
