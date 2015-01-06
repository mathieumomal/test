using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    [Category("CR Tests")]
    public class CrRepositoryTest : BaseEffortTest
    {
        #region Constants
        private const int TotalNoOfCrsInCsv = 22;
        private const int SpecificationId = 136080;
        #endregion

        #region Tests

        [Test]
        public void Repository_CR_InsertOrUpdate()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var changeRequest1 = new ChangeRequest { CRNumber = "234.12", CR_WorkItems = new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 } } };
            var changeRequest2 = new ChangeRequest { CRNumber = "234.13", CR_WorkItems = new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 }, new CR_WorkItems { Fk_WIId = 3 } } };
            repo.InsertOrUpdate(changeRequest1);
            repo.InsertOrUpdate(changeRequest2);

            Assert.AreEqual(2, UoW.Context.GetEntities<ChangeRequest>(EntityState.Added).Count());
            Assert.AreEqual(3, UoW.Context.GetEntities<CR_WorkItems>(EntityState.Added).Count());
        }

        [Test]
        public void Repository_CR_All()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var allCRs = repo.All;

            Assert.AreEqual(TotalNoOfCrsInCsv, allCRs.ToList().Count);
        }

        [Test]
        public void Repository_CR_AllIncluding()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var allIncludingCRs = repo.AllIncluding(x => x.Enum_CRCategory);

            Assert.AreEqual(TotalNoOfCrsInCsv, allIncludingCRs.ToList().Count);
        }

        [Test]
        public void Repository_GenerateCrNumberBySpecificationId()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repoResult = repo.FindCrNumberBySpecificationId(SpecificationId);
            Assert.AreEqual(6, repoResult.Count);
        }

        [Test]
        public void Repository_GetChangeRequestById()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repResult = repo.Find(1);
            Assert.AreEqual(1, repResult.Pk_ChangeRequest);
            Assert.AreEqual("0001", repResult.CRNumber);
            Assert.AreEqual(136081, repResult.Fk_Specification);
        }

        [Test]
        public void Repository_GetChangeRequestByContributionUID()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repResult = repo.GetChangeRequestByContributionUID("TSG1");
            Assert.AreEqual(1, repResult.Pk_ChangeRequest);
            Assert.AreEqual("0001", repResult.CRNumber);
            Assert.AreEqual(136081, repResult.Fk_Specification);
        }

        [Test]
        [TestCase(0, "WG3")]
        [TestCase(5, "WG1")]
        public void Repository_FindByWgTDoc(int result, string wgTDoc)
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var respresult = repo.FindByWgTDoc(wgTDoc);


            if (respresult.Fk_TSGStatus == null)
            {
                Assert.AreEqual(result, 0);
                Assert.AreEqual(result, 0);
            }
            else
            {
                Assert.AreEqual(result, respresult.Fk_TSGStatus);
                Assert.AreEqual(result, respresult.Fk_TSGStatus);
            }
        }

        #endregion

        #region Get CR

        [TestCase(0, 0, 22, Description = "No paging")]
        [TestCase(0, 10, 10, Description = "Paging max 10 items")]
        [TestCase(5, 10, 10, Description = "Paging max 10 items (start from item 6)")]
        [TestCase(20, 10, 2, Description = "Paging start from item 20")]
        public void GetChangeRequestsTest_Paging(int skipRecords, int pageSize, int expectedResult)
        {
            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = pageSize, SkipRecords = skipRecords };

            //Call method
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            //Test
            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [Test]
        public void GetChangeRequestsTest_OrderBy()
        {
            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = 0, SkipRecords = 0 };

            //Call method
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            //Test order by spec # (ASCENDING)
            Assert.AreEqual("22.101",result.Key.FirstOrDefault().Specification.Number);//First spec
            Assert.AreEqual("22.107", result.Key.LastOrDefault().Specification.Number);//Last spec
            //Test order by CR # (DESCENDING) (Same spec between to first CR)
            Assert.AreEqual(result.Key.ElementAt(0).Specification.Number, result.Key.ElementAt(1).Specification.Number);
            Assert.AreEqual("AC014", result.Key.ElementAt(0).CRNumber);
            Assert.AreEqual("AB013", result.Key.ElementAt(1).CRNumber);
        }

        [TestCase("22.101", 6)]
        [TestCase("22.", 22)]
        public void GetChangeRequestsTest_SpecNumber(string specNumber, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { SpecificationNumber = specNumber };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase("22.107", 428927, 1)]
        [TestCase("22.101", 428926, 0)]
        public void GetChangeRequestsTest_SpecNumber(string specNumber, int specVersion, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { SpecificationNumber = specNumber, VersionId = specVersion};

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(2874, 2883, 16)]
        [TestCase(2882, 2884, 6)]
        public void GetChangeRequestsTest_Release(int releaseId1, int releaseId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { ReleaseIds = new List<int>() { releaseId1, releaseId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 1, 15)]
        [TestCase(1, 2, 17)]
        public void GetChangeRequestsTest_WgStatus(int wgStatusId1, int wgStatusId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { WgStatusIds = new List<int>() { wgStatusId1, wgStatusId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 5, 7)]
        [TestCase(2, 5, 13)]
        public void GetChangeRequestsTest_TsgStatus(int tsgStatusId1, int tsgStatusId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { TsgStatusIds = new List<int>() { tsgStatusId1, tsgStatusId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 1, 10)]
        [TestCase(2, 2, 5)]
        [TestCase(3, 3, 7)]
        [TestCase(4, 4, 2)]
        [TestCase(1, 2, 15)]
        [TestCase(1, 4, 10)]
        [TestCase(2, 3, 12)]
        public void GetChangeRequestsTest_Meeting(int meetingId1, int meetingId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { MeetingIds = new List<int>() { meetingId1, meetingId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1274, 1274, 1)]
        [TestCase(2, 2, 4)]
        [TestCase(3, 3, 2)]
        [TestCase(2, 3, 4)]
        [TestCase(2, 1274, 5)]
        public void GetChangeRequestsTest_WorkItem(int workItemId1, int workItemId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { WorkItemIds = new List<int>() { workItemId1, workItemId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase("22.101", 2874, 5)]
        [TestCase("22.101", 2884, 1)]
        public void GetChangeRequestsTest_SpecAndRelease(string specNumber, int releaseId, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { SpecificationNumber = specNumber, ReleaseIds = new List<int>() { releaseId } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 2, 3)]
        [TestCase(2, 2, 1)]
        [TestCase(1, 3, 2)]
        [TestCase(2, 3, 0)]
        public void GetChangeRequestsTest_WgStatusAndWorkItem(int wgStatusId, int workItemId, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { WgStatusIds = new List<int>() { wgStatusId }, WorkItemIds = new List<int>() { workItemId } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 2, 10)]
        [TestCase(2, 1, 2)]
        [TestCase(1, 5, 3)]
        public void GetChangeRequestsTest_WgStatusAndTsgStatus(int wgStatusId, int tsgStatusId, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { WgStatusIds = new List<int>() { wgStatusId }, TsgStatusIds = new List<int>() { tsgStatusId } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        #endregion
    }
}
