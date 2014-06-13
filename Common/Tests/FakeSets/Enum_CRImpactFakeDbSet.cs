using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class Enum_CRImpactFakeDbSet : FakeDBSet<Enum_CRImpact>
    {
        public override Enum_CRImpact Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_EnumCRImpact == keyValue);
        }
    }
}
