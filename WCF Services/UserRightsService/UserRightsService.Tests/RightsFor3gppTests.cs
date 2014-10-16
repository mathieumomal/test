using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.UserRights.Service;
using System.IO;

namespace UserRightsService.Tests
{
    [TestFixture]
    class RightsFor3gppTests: BaseEffortTest
    {
        public const int MCC_MEMBER_ID = 32337;
        public const int ADMINISTRATOR_ID = 53388;
        public const int WORKPLANMGR_ID = 915;
        public const int SPECMGR_ID = 637;
        public const int MTG_MGR_ID = 3060;
        public const int EOL_ACCOUNT_OWNER_ID = 4101;
        public const int U3GPPMEMBER_ID = 758;

        public const int SA1ChairmanId = 35011;
        public const int SASecretary = 10343;

        public const int SaTsgId = 375;
        public const int Sa1WgId = 384;

        public const int CHAIRMAN_ID = 12;
        public const int VICECHAIRMAN_ID = 13;
        public const int SECRETARY_ID = 15;

        [Test, TestCaseSource("GetNonCommitteeRightsData")]
        public void GetRights_NonCommitteeReleated_Test(int personId, List<string> expectedRights)
        {
            var rightsRetriever = new UltimateRights();

            var actualRights = rightsRetriever.GetRights(personId);

            Assert.IsNotNull(actualRights);
            Assert.IsNotNull(actualRights.ApplicationRights);
            Assert.AreEqual(expectedRights.Count, actualRights.ApplicationRights.Count);
            Assert.IsFalse(actualRights.ApplicationRights.Any(x => !expectedRights.Contains(x)));
            Assert.IsFalse(expectedRights.Any(x => !actualRights.ApplicationRights.Contains(x)));
        }

        [Test, TestCaseSource("GetCommitteeRightsData")]
        public void GetRights_CommitteeReleated_Test(int personId, int groupId, bool hasRight, List<string> expectedRights)
        {
            var rightsRetriever = new UltimateRights();
            rightsRetriever.UserRightsXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database\\TestRights.xml");
            var actualRights = rightsRetriever.GetRights(personId);

            Assert.IsNotNull(actualRights);
            Assert.AreEqual(hasRight, actualRights.CommitteeRights.ContainsKey(groupId));
            if (hasRight)
            {
                Assert.AreEqual(expectedRights.Count, actualRights.CommitteeRights[groupId].Count);
                Assert.IsFalse(actualRights.CommitteeRights[groupId].Any(x => !expectedRights.Contains(x)));
                Assert.IsFalse(expectedRights.Any(x => !actualRights.CommitteeRights[groupId].Contains(x)));
            }
        }

        #region Data

        private IEnumerable<object[]> GetNonCommitteeRightsData
        {
            get
            {
                var anonymousRights = new List<string>() { "Release_ViewPartialList", "Release_ViewLimitedDetails", "Specification_ViewDetails", "Meeting_ViewMeetingList" };
                var eolAccountOwnerRights = new List<string>() { "Release_ViewPartialList", "Release_ViewLimitedDetails", "Specification_ViewDetails", "Meeting_ViewMeetingList" };
                var ultimate3gppMemberRights = new List<string>() { "General_ViewPersonalData", "Release_ViewCompleteList", "Release_ViewDetails", "Specification_ViewDetails", "Meeting_ViewMeetingList", "Contribute_Restricted", "Contribution_Upload", "Contribution_Withdraw_Restricted" };
                var staffMemberRights = new List<string>() { "General_ViewPersonalData", "Release_ViewCompleteList", "Release_ViewDetails", "Remarks_ViewPrivate", "Remarks_AddPrivateByDefault", 
                                                             "Specification_Create", "Specification_EditLimitted", "Specification_View_UnAllocated_Number", "Specification_ViewDetails", 
                                                             "Versions_Allocate", "Versions_Upload", "Versions_Modify_MajorVersion", "Versions_AddRemark", "Meeting_ViewMeetingList", 
                                                             "Meeting_CreateMeeting", "Manage_AgendaItems", "Contribute_Anytime", "Contribution_Edit", "Contribution_Upload", "Contribution_Upload_Anytime", 
                                                             "Contribution_Withdraw", "Contribution_Import", "Contribution_Edit_CR_Number" };
                var meetingManagerRights = new List<string>() { "Meeting_ViewMeetingList", "Meeting_CreateMeeting", "Meeting_CreateEvent", "Meeting_EditEvent", "Manage_AgendaItems" };
                var workPlanManagerRights = new List<string>() { "Release_ViewCompleteDetails", "WorkItem_ImportWorkplan" };
                var specificationManagerRights = new List<string>() { "General_ViewPersonalData", "Release_ViewCompleteList", "Release_ViewCompleteDetails", "Release_Create", "Release_Edit", 
                                                                      "Release_Freeze", "Release_Close", "Remarks_ViewPrivate", "Remarks_AddPrivateByDefault", "WorkItem_ImportWorkplan", "Specification_BulkPromote", 
                                                                      "Specification_Create", "Specification_EditFull", "Specification_ForceTransposition", "Specification_InhibitPromote", "Specification_ManageITURecommendations", 
                                                                      "Specification_RemoveInhibitPromote", "Specification_SyncHardLinksOnLatestFolder", "Specification_Withdraw", "Specification_WithdrawFromRelease", "Specification_UnforceTransposition", 
                                                                      "Specification_View_UnAllocated_Number", "Specification_ViewDetails", "Versions_Allocate", "Versions_Upload", "Versions_AddRemark", "Manage_AgendaItems", "Meeting_ViewMeetingList", 
                                                                      "Meeting_CreateMeeting", "Meeting_CreateEvent", "Meeting_EditEvent", "Contribute_Anytime", "Contribution_Edit", "Contribution_Upload", "Contribution_Upload_Anytime", "Contribution_Withdraw", 
                                                                      "Contribution_Import" , "Contribution_Edit_CR_Number" };
                var adminRights = new List<string>() { "General_ViewPersonalData", "Release_ViewCompleteList", "Release_ViewCompleteDetails", "Release_Create", "Release_Edit", "Release_Freeze", "Release_Close", "Remarks_ViewPrivate", 
                                                       "Remarks_AddPrivateByDefault", "WorkItem_ImportWorkplan", "Specification_BulkPromote", "Specification_Create", "Specification_EditFull", "Specification_ForceTransposition", 
                                                       "Specification_InhibitPromote", "Specification_ManageITURecommendations", "Specification_RemoveInhibitPromote", "Specification_SyncHardLinksOnLatestFolder", "Specification_Withdraw", 
                                                       "Specification_WithdrawFromRelease", "Specification_UnforceTransposition", "Specification_View_UnAllocated_Number", "Specification_ViewDetails", "Versions_Allocate", "Versions_Upload", 
                                                       "Versions_AddRemark", "Manage_AgendaItems", "Meeting_ViewMeetingList", "Meeting_CreateMeeting", "Meeting_CreateEvent", "Meeting_EditEvent", "Contribute_Anytime", "Contribution_Edit", 
                                                       "Contribution_Upload", "Contribution_Upload_Anytime", "Contribution_Withdraw" , "Contribution_Import" , "Contribution_Edit_CR_Number" };

                ultimate3gppMemberRights.AddRange(eolAccountOwnerRights);
                staffMemberRights.AddRange(eolAccountOwnerRights);
                meetingManagerRights.AddRange(eolAccountOwnerRights);
                workPlanManagerRights.AddRange(eolAccountOwnerRights);
                specificationManagerRights.AddRange(eolAccountOwnerRights);                
                adminRights.AddRange(eolAccountOwnerRights);                

                yield return new object[] { 0, anonymousRights };
                yield return new object[] { -1, anonymousRights };
                yield return new object[] { EOL_ACCOUNT_OWNER_ID, eolAccountOwnerRights };
                yield return new object[] { U3GPPMEMBER_ID, ultimate3gppMemberRights.Distinct().ToList() };
                yield return new object[] { MCC_MEMBER_ID, staffMemberRights.Distinct().ToList() };
                yield return new object[] { MTG_MGR_ID, meetingManagerRights.Distinct().ToList() };
                yield return new object[] { WORKPLANMGR_ID, workPlanManagerRights.Distinct().ToList() };
                yield return new object[] { SPECMGR_ID, specificationManagerRights.Distinct().ToList() };
                yield return new object[] { ADMINISTRATOR_ID, adminRights.Distinct().ToList() };
            }
        }

        private IEnumerable<object[]> GetCommitteeRightsData
        {
            get
            {
                var committeeOfficialRights = new List<string>() { "CommitteeOfficial_TestRight" };

                yield return new object[] { CHAIRMAN_ID, SaTsgId, false, committeeOfficialRights };
                yield return new object[] { VICECHAIRMAN_ID, SaTsgId, false, committeeOfficialRights };
                yield return new object[] { SECRETARY_ID, SaTsgId, false, committeeOfficialRights };
                yield return new object[] { SASecretary, Sa1WgId, true, committeeOfficialRights };
                yield return new object[] { SA1ChairmanId, Sa1WgId, true, committeeOfficialRights };
                yield return new object[] { SA1ChairmanId, SaTsgId, true, committeeOfficialRights };
            }
        }

        #endregion
    }
}
