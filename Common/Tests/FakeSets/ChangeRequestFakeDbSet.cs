using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ChangeRequestFakeDbSet : FakeDBSet<ChangeRequest>
    {
        public override ChangeRequest Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_ChangeRequest == keyValue);
        }
    }
}
