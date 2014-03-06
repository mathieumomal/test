using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class CommunityFakeDBSet : FakeDBSet<Community>
    {
        public override Community Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.TbId == keyValue);
        }
    }
}
