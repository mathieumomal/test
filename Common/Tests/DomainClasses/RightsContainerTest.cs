using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.DomainClasses
{
    class RightsContainerTest
    {

        [Test]
        public void UserRightsContainer_RetrieveCommitteeIndependentRight()
        {
            var aContainer = new UserRightsContainer();
            aContainer.AddRight(Enum_UserRights.Release_Create);

            Assert.IsTrue(aContainer.HasRight(Enum_UserRights.Release_Create));
            Assert.IsFalse(aContainer.HasRight(Enum_UserRights.Release_Edit));
        }

        [Test]
        public void UserRightsContainer_RetrieveCommitteeRight()
        {
            var aContainer = new UserRightsContainer();
            aContainer.AddRight(Enum_UserRights.Release_Create);

            aContainer.AddRight(Enum_UserRights.Release_Edit, 22);

            Assert.IsTrue(aContainer.HasRight(Enum_UserRights.Release_Create, 22));
            Assert.IsTrue(aContainer.HasRight(Enum_UserRights.Release_Edit, 22));
            Assert.IsFalse(aContainer.HasRight(Enum_UserRights.Release_Edit, 23));
        }
    }
}
