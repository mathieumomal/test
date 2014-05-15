using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class VersionFakeDBSet : FakeDBSet<SpecVersion>
    {
        public override SpecVersion Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(version => version.Pk_VersionId == keyValue);
        }
    }
}
