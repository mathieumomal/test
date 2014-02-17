using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Tests.FakeSets
{
    public class MeetingFakeDBSet : FakeDBSet<Meeting>
    {
        public override Meeting Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(m => m.MTG_ID == keyValue);
        }
    }
}
