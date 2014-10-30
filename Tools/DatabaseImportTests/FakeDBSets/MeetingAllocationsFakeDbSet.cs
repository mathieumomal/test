using System.Linq;
using Etsi.Ngppdb.DomainClasses;

namespace DatabaseImportTests.FakeDBSets
{
    public class MeetingAllocationsFakeDbSet : FakeDBSet<MeetingAllocations>
    {
        public override MeetingAllocations Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.DOCALLOC_ID == keyValue);
        }
    }
}
