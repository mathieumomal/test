using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class CRWIFakeDbSet : FakeDBSet<CR_WorkItems>
    {
        public override CR_WorkItems Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.Pk_CRWorkItems == keyValue);
        }
    }
}
