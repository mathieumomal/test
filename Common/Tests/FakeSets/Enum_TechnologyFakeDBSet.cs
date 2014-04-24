using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class Enum_TechnologyFakeDBSet : FakeDBSet<Enum_Technology>
    {
        public override Enum_Technology Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_Enum_TechnologyId == keyValue);
        }
    }
}
