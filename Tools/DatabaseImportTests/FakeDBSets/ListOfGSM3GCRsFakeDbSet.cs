using System.Linq;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.FakeDBSets
{
    public class ListOfGSM3GCRsFakeDbSet : FakeDBSet<List_of_GSM___3G_CRs>
    {
        public override List_of_GSM___3G_CRs Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Row_id == keyValue);
        }
    }
}
