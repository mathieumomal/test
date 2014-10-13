using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.UserRights.Service;

namespace UserRightsService.Tests
{
    [TestFixture]
    class RolesFor3gppTests: BaseEffortTest
    {
        public const int MCC_MEMBER_ID = 32337;
        public const int ADMINISTRATOR_ID = 53388;
        public const int WORKPLANMGR_ID = 915;
        public const int SPECMGR_ID = 637;

        public const int SA1ChairmanId = 35011;
        public const int SASecretary = 10343;

        public const int SaTsgId = 375;
        public const int Sa1WgId = 384;

        public const int CHAIRMAN_ID = 12;
        public const int VICECHAIRMAN_ID = 13;
        public const int SECRETARY_ID = 15;

        [Test, Description("For user id <= 0, system must return Anonymous role")]
        public void GetNonCommitteeRelated_RolesReturnAnonymousOnPersonIdZero()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(0);
            var roles2 = rightsRetriever.GetApplicationRoles(-1);

            Assert.AreEqual(1, roles.Count);
            Assert.AreEqual(1, roles2.Count);

            Assert.AreEqual(Enum_UserRoles.Anonymous, roles.FirstOrDefault());
            Assert.AreEqual(Enum_UserRoles.Anonymous, roles2.FirstOrDefault());
        }

        [Test, Description("If user does not belong to Mcc list, system must return EolAccountOwner")]
        public void GetNonCommitteeRelated_RolesReturnEolOnPersonIdPositive()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(2335);
            Assert.AreEqual(1, roles.Count);
            Assert.AreEqual(Enum_UserRoles.EolAccountOwner, roles.FirstOrDefault());
        }

        [Test, Description("If user belongs to Mcc list (PLIST_ID=5240), system must return StaffMember (such as 32337 = Xavier Piednoir)")]
        public void GetNonCommitteeRelated_RolesReturnStaffMemberIfUserBelongsToEtsi()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(MCC_MEMBER_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.StaffMember, roles);
        }

        [Test]
        public void GetNonCommitteeRelated_RolesChecksForAdministrators()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(ADMINISTRATOR_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.Administrator, roles);
        }

        [Test, Description("System must return Workplan Manager role for workplan manager (such as 915 = Adrian Zoicas)")]
        public void GetNonCommitteeRelated_RolesChecksForWorkplanManagers()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(WORKPLANMGR_ID);
            Assert.Contains(Enum_UserRoles.WorkPlanManager, roles);
        }

        [Test, Description("System must return SuperUser roles for specifications managers (such as 637 = John M. Meredith)")]
        public void GetNonCommitteeRelated_RolesChecksForSpecManager()
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetApplicationRoles(SPECMGR_ID);
            Assert.Contains(Enum_UserRoles.SuperUser, roles);
        }

        [TestCase(SA1ChairmanId, Sa1WgId, true, Description="System should return Committee Official for Mona Mustapha, for SA 1 (as she's Chairman)") ]
        [TestCase(SA1ChairmanId, SaTsgId, true, Description = "System should also return Committee Official for Mona Mustapha, for SA (as she's Vice Chairman)")]
        [TestCase(SASecretary, Sa1WgId, true, Description = "System should return Committee Official for Alain Sultan, for SA 1 (as he's secretary)")]
        [TestCase(SASecretary, SaTsgId, false, Description = "System should not return Committee Official for Alain Sultan, for SA (as he's not secretary of this TSG)")]
        public void GetCommitteeRelated_RolesChecksForCommitteeOfficials(int personId, int groupId, bool shouldExist)
        {
            var rightsRetriever = new UltimateRights();

            var roles = rightsRetriever.GetCommitteeRoles(personId);
            Assert.Contains(Enum_UserRoles.CommitteeOfficial, roles.Keys);

            Assert.AreEqual(shouldExist, roles[Enum_UserRoles.CommitteeOfficial].Contains(groupId));
        }
    }
}
