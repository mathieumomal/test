using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecificationWorkItemFakeDBSet : FakeDBSet<Specification_WorkItem>
    {
        public override Specification_WorkItem Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(c => c.Pk_Specification_WorkItemId == keyValue);
        }
    }
}
