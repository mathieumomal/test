using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class ReleaseInfoFakeDBSet : FakeDBSet<Specs_GSM_3G_release_info>
    {
        public override Specs_GSM_3G_release_info Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
