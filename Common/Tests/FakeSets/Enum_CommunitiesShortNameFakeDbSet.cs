using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class Enum_CommunitiesShortNameFakeDbSet : FakeDBSet<Enum_CommunitiesShortName>
    {
        public override Enum_CommunitiesShortName Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.Pk_EnumCommunitiesShortNames == keyValue);
        }
    }
}
