using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class SpecificationWILegacyFakeDBSet : FakeDBSet<C2008_03_08_Specs_vs_WIs>
    {
        public override C2008_03_08_Specs_vs_WIs Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
