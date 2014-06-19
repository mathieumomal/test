using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.LegacyDBSets
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
