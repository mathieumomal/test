using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class UsersAdHocRolesFakeDBSet : FakeDBSet<Users_AdHoc_Roles>
    {
        public override Users_AdHoc_Roles Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(m => m.UserID == keyValue);
        }
    }
}
