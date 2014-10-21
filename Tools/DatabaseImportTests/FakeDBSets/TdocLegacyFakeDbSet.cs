using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class TdocLegacyFakeDbSet : FakeDBSet<C2006_03_17_tdocs>
    {
        public override C2006_03_17_tdocs Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
