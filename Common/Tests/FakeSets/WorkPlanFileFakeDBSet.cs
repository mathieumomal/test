using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class WorkPlanFileFakeDBSet : FakeDBSet<WorkPlanFile>
    {
        public override WorkPlanFile Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(wpFile => wpFile.Pk_WorkPlanFileId == keyValue);
        }
    }
}
