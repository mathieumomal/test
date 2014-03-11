using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.DomainClasses
{
    class RightsContainerTest : BaseTest
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

        [TestCase(true,false)]      // If we remove from committee, it should no longer appear.
        [TestCase(false, true)]     // If we do not remove, it does appear.
        public void RemoveRight_IndependentOfCommittee(bool removeFromCommittee, bool result)
        {
            var aContainer = new UserRightsContainer();
            aContainer.AddRight(Enum_UserRights.Release_Create);
            aContainer.AddRight(Enum_UserRights.Release_Edit);

            aContainer.AddRight(Enum_UserRights.Release_Create, 12);

            aContainer.RemoveRight(Enum_UserRights.Release_Create, removeFromCommittee);

            Assert.IsFalse(aContainer.HasRight(Enum_UserRights.Release_Create));
            Assert.IsTrue(aContainer.HasRight(Enum_UserRights.Release_Edit));
            Assert.AreEqual(result, aContainer.HasRight(Enum_UserRights.Release_Create, 12));
        }

        [Test]
        public void RemoveRight_DependingOnCommittee()
        {
            var aContainer = new UserRightsContainer();
            aContainer.AddRight(Enum_UserRights.Release_Create, 12);
            aContainer.AddRight(Enum_UserRights.Release_Create, 13);

            aContainer.RemoveRight(Enum_UserRights.Release_Create, 12);

            Assert.IsFalse(aContainer.HasRight(Enum_UserRights.Release_Create, 12));
            Assert.IsTrue(aContainer.HasRight(Enum_UserRights.Release_Create, 13));

        }
    }
}
