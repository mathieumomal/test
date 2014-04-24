using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.LegacyDBSets
{
    public class FiliusPatrisFakeDBSet : FakeDBSet<C2001_11_06_filius_patris>
    {
        public override C2001_11_06_filius_patris Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
