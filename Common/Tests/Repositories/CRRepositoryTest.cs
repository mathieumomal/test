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
        private const int TotalNoOfCrsInCsv = 24;
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
            Assert.AreEqual(1, repResult.ChangeRequestTsgDatas.Count);
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


            if (respresult.ChangeRequestTsgDatas.First().Fk_TsgStatus == null)
            {
                Assert.AreEqual(result, 0);
                Assert.AreEqual(result, 0);
            }
            else
            {
                Assert.AreEqual(result, respresult.ChangeRequestTsgDatas.First().Fk_TsgStatus);
                Assert.AreEqual(result, respresult.ChangeRequestTsgDatas.First().Fk_TsgStatus);
                Assert.AreEqual("Rejected", respresult.ChangeRequestTsgDatas.First().TsgStatus.Code);
            }
        }

        [Test(Description = "System should get Crs according to list of UIDs")]
        [TestCase(1, Description = "System should execute one request each 1 uid(s)")]
        [TestCase(2, Description = "System should execute one request each 2 uid(s)")]
        [TestCase(3, Description = "System should execute one request each 3 uid(s)")]
        [TestCase(4, Description = "System should execute one request each 4 uid(s)")]
        [TestCase(5, Description = "System should execute one request each 5 uid(s)")]
        public void GetChangeRequestListByContributionUidList(int limit)
        {
            var repo = new ChangeRequestRepository { UoW = UoW, LimitOfTdocsPerRequest = limit};
            var result = repo.GetChangeRequestListByContributionUidList(new List<string> { "ABC", "DEF", "GHI", "RP-CR0004", "RP-CR0005" });

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(x => x.WGTDoc == "ABC"));
            Assert.IsTrue(result.Any(x => x.WGTDoc == "DEF"));
            Assert.IsTrue(result.Any(x => x.WGTDoc == "GHI"));
            Assert.IsTrue(result.Any(x => x.ChangeRequestTsgDatas.Any(y => y.TSGTdoc == "RP-CR0004")));
            Assert.IsTrue(result.Any(x => x.ChangeRequestTsgDatas.Any(y => y.TSGTdoc == "RP-CR0005")));
        }

        #endregion

        #region Get CR

        [TestCase(0, 0, 24, Description = "No paging")]
        [TestCase(0, 10, 10, Description = "Paging max 10 items")]
        [TestCase(5, 10, 10, Description = "Paging max 10 items (start from item 6)")]
        [TestCase(20, 10, 4, Description = "Paging start from item 20")]
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
            Assert.AreEqual("22.108", result.Key.LastOrDefault().Specification.Number);//Last spec
            //Test order by CR # (DESCENDING) (Same spec between to first CR)
            Assert.AreEqual(result.Key.ElementAt(0).Specification.Number, result.Key.ElementAt(1).Specification.Number);
            Assert.AreEqual("AC014", result.Key.ElementAt(0).CRNumber);
            Assert.AreEqual("AB013", result.Key.ElementAt(1).CRNumber);
        }

        [TestCase("22.101", 6)]
        [TestCase("22.", 24)]
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

        [TestCase(2874, 2883, 17)]
        [TestCase(2882, 2884, 6)]
        public void GetChangeRequestsTest_Release(int releaseId1, int releaseId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { ReleaseIds = new List<int>() { releaseId1, releaseId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 1, 16)]
        [TestCase(1, 2, 18)]
        [TestCase(0, 0, 6)]
        [TestCase(0, 2, 8)]
        public void GetChangeRequestsTest_WgStatus(int wgStatusId1, int wgStatusId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { WgStatusIds = new List<int>() { wgStatusId1, wgStatusId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 5, 8)]
        [TestCase(2, 5, 14)]
        [TestCase(0, 0, 6)]
        [TestCase(0, 5, 10)]
        public void GetChangeRequestsTest_TsgStatus(int tsgStatusId1, int tsgStatusId2, int expectedResult)
        {
            var searchObj = new ChangeRequestsSearch { TsgStatusIds = new List<int>() { tsgStatusId1, tsgStatusId2 } };

            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            Assert.AreEqual(expectedResult, result.Key.Count);
        }

        [TestCase(1, 1, 10)]
        [TestCase(2, 2, 5)]
        [TestCase(3, 3, 8)]
        [TestCase(4, 4, 2)]
        [TestCase(1, 2, 15)]
        [TestCase(1, 4, 10)]
        [TestCase(2, 3, 13)]
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

        #region Get CR(s) for MM

        [Test]
        public void GetLightChangeRequestForMinuteMan()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetLightChangeRequestForMinuteMan("ABC");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CurrentVersion);
            Assert.IsNotNull(result.NewVersion);
            Assert.IsNotNull(result.ChangeRequestTsgDatas);
        }

        [Test]
        public void GetLightChangeRequestsForMinuteMan()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetLightChangeRequestsForMinuteMan(new List<string>{"ABC"});

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.First().CurrentVersion);
            Assert.IsNotNull(result.First().NewVersion);
            Assert.IsNotNull(result.First().ChangeRequestTsgDatas);
        }

        #endregion

        #region Get WgTdoc number of parent CRs
        [Test]
        public void GetCrWgTdocNumberOfParent_SearchForNothing()
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var crKeys = new List<CrKeyFacade>();

            var result = crRepository.GetCrWgTdocNumberOfParent(crKeys);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetCrWgTdocNumberOfParent_SearchForOneCr()
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var crKeys = new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "0001", SpecNumber = "22.102" } };

            var result = crRepository.GetCrWgTdocNumberOfParent(crKeys);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("MNO", result.First().Value);
        }

        [Test]
        public void GetCrWgTdocNumberOfParent_SearchForOneCrWhichUnexist()
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var crKeys = new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "0001", SpecNumber = "22.103" } };

            var result = crRepository.GetCrWgTdocNumberOfParent(crKeys);

            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region Get CRs inside CRPack for MM
        [Test]
        public void GetLightChangeRequestsInsideCrPackForMinuteMan()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetLightChangeRequestsInsideCrPackForMinuteMan("Change request description6");

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsNotNull(result.First().Specification.Number);
            Assert.AreEqual("22.101", result.First().Specification.Number);
            Assert.IsNotNull(result.First().ChangeRequestTsgDatas);
        }

        [Test]
        public void GetLightChangeRequestsInsideCrPacksForMinuteMan()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetLightChangeRequestsInsideCrPacksForMinuteMan(new List<string> { "Change request description6" });

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsNotNull(result.First().Specification.Number);
            Assert.AreEqual("22.101", result.First().Specification.Number);
            Assert.IsNotNull(result.First().ChangeRequestTsgDatas);
        }
        #endregion
    }
}
