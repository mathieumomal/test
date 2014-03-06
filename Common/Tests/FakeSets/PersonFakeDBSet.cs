using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class PersonFakeDBSet : FakeDBSet<View_Persons>
    {
        public override View_Persons Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(p => p.PERSON_ID == keyValue);
        }
    }
}
