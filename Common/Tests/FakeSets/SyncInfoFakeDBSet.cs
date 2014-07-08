using Etsi.Ultimate.DomainClasses;
using System.Linq;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SyncInfoFakeDBSet : FakeDBSet<SyncInfo>
    {
        public override SyncInfo Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(p => p.Pk_SyncId == keyValue);
        }
    }
}
