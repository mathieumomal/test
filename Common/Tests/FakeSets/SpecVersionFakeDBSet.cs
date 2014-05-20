using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecVersionFakeDBSet : FakeDBSet<SpecVersion>
    {
        public override SpecVersion Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.Pk_VersionId == keyValue);
        }
    }
}
