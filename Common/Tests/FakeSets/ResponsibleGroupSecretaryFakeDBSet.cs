using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ResponsibleGroupSecretaryFakeDBSet : FakeDBSet<ResponsibleGroup_Secretary>
    {
        public override ResponsibleGroup_Secretary Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(p => p.TbId == keyValue);
        }
    }
}
