using Etsi.Ultimate.DomainClasses;
using System.Linq;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class HistoryFakeDBSet : FakeDBSet<History>
    {
        public override History Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Pk_HistoryId == keyValue);
        }
    }
}
