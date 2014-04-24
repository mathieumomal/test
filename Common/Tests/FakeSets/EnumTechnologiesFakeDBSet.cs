using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class EnumTechnologiesFakeDBSet : FakeDBSet<Enum_Technology>
    {
        public override Enum_Technology Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(releaStat => releaStat.Pk_Enum_TechnologyId == keyValue);
        }
    }
}
