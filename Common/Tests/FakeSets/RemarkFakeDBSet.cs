using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Tests.FakeSets
{
    public class RemarkFakeDbSet : FakeDBSet<Remark>
    {
        public override Remark Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.Pk_RemarkId == keyValue);
        }
    }
}
