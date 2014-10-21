using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class CRStatusValuesLegacyFakeDbSet : FakeDBSet<CR_status_values>
    {
        public override CR_status_values Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Row_id == keyValue);
        }
    }
}
