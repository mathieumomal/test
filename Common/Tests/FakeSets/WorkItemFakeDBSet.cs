using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class WorkItemFakeDBSet : FakeDBSet<WorkItem>
    {
        public override WorkItem Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(relea => relea.Pk_WorkItemUid == keyValue);
        }
    }
}
