using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class MeetingsWithEndDatesFakeDbSet : FakeDBSet<plenary_meetings_with_end_dates>
    {
        public override plenary_meetings_with_end_dates Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Row_id == keyValue);
        }
    }
}
