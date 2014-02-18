using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class Enum_ReleaseStatusFakeDBSet : FakeDBSet<Enum_ReleaseStatus>
    {
        public override Enum_ReleaseStatus Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(releaStat => releaStat.Enum_ReleaseStatusId == keyValue);
        }
    }
}
