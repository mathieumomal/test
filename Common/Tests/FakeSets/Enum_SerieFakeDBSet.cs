using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class Enum_SerieFakeDBSet : FakeDBSet<Enum_Serie>
    {
        public override Enum_Serie Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_Enum_SerieId == keyValue);
        }
    }
}
