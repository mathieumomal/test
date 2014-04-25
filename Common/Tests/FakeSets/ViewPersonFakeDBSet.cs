using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ViewPersonFakeDBSet : FakeDBSet<View_Persons>
    {
        public override View_Persons Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(speci => speci.PERSON_ID == keyValue);
        }
    }
}
