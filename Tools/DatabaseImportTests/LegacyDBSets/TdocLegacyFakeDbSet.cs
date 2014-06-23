using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.LegacyDBSets
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
