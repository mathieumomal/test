using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class CRCategoryLegacyFakeDbSet : FakeDBSet<CR_categories>
    {
        public override CR_categories Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Row_id == keyValue);
        }
    }
}
