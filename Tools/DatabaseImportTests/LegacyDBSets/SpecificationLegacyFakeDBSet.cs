using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImportTests.LegacyDBSets
{
    public class SpecificationLegacyFakeDBSet : FakeDBSet<Specs_GSM_3G>
    {
        public override Specs_GSM_3G Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Row_id == keyValue);
        }
    }
}
