using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class ScheduleLegacyFakeDBSet : FakeDBSet<C2001_04_25_schedule>
    {
        public override C2001_04_25_schedule Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
