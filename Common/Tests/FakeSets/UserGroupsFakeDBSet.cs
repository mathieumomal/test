using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class UserGroupsFakeDBSet : FakeDBSet<Users_Groups>
    {
        public override Users_Groups Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(m => m.PERSON_ID == keyValue);
        }
    }
}
