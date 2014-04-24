using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.LegacyDBSets
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
