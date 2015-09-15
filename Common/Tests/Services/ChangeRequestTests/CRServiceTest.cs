using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils.Core;
using log4net.Appender;
using log4net.Core;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Services.ChangeRequestTests
{
    [Category("CR Tests")]
    public class CrServiceTest : BaseEffortTest
    {
        #region Constants
        private const int TotalNoOfCrsInCsv = 24;
        private const int TotalNoOfCrWorkItemsInCsv = 7;
        private const int PersonId = 0;       
        MemoryAppender _memoryAppender;
        #endregion

        #region Logger SetUp

        [TestFixtureSetUp]
        public void Init()
        {
            string configFileName = Directory.GetCurrentDirectory() + "\\TestData\\LogManager\\Test.log4net.config";
            LogManager.SetConfiguration(configFileName, "TestLogger");
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _memoryAppender = ((LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
            if (_memoryAppender != null) _memoryAppender.Clear();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            _memoryAppender.Clear();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            LogManager.SetConfiguration(String.Empty, String.Empty);
        }

        #endregion

        #region Unit Tests

        [Test]
        public void Service_UnitTest_CreateChangeRequest_Success()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            var mockCrManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCrManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(new ServiceResponse<bool> { Result = true });
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCrManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void Service_UnitTest_CreateChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            var mockCrManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCrManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(
                new ServiceResponse<bool> { Result = false, Report = new Report { ErrorList = new List<string> { "Error" } } });
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCrManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Result);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test]
        public void Service_UnitTest_EditChangeRequest_Success()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            var mockCrManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCrManager.Stub(x => x.EditChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCrManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsTrue(result);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void Service_UnitTest_EditChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            var mockCrManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCrManager.Stub(x => x.EditChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(false);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCrManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsFalse(result);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        // --- GetCRId ---
        [Test]
        public void Service_UnitTest_GetChangeRequestById_Success()
        {
            var mockChangeRequestById = MockRepository.GenerateMock<IChangeRequestManager>();
            var changeRequestById = GetChangeRequestByIdDataObject(1);
            mockChangeRequestById.Stub(x => x.GetChangeRequestById(0, 1)).Return(changeRequestById);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockChangeRequestById);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var svcChangeRequestById = new ChangeRequestService();
            var result = svcChangeRequestById.GetChangeRequestById(PersonId, 1);
            //Assert
            Assert.IsTrue(result.Key);
        }

        [Test, Description("Test if we try to get a CR which doesn't exist. We expect a true success flag and a null change request object.")]
        public void Service_UnitTest_GetChangeRequestById_Failure()
        {
            var mockChangeRequestById = MockRepository.GenerateMock<IChangeRequestManager>();
            var changeRequestById = GetChangeRequestByIdDataObject(100);
            mockChangeRequestById.Stub(x => x.GetChangeRequestById(0, 100)).Return(changeRequestById);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockChangeRequestById);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var svcChangeRequestById = new ChangeRequestService();
            var result = svcChangeRequestById.GetChangeRequestById(PersonId, 100);

            //Assert
            Assert.IsTrue(result.Key);
            Assert.IsNull(result.Value);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Service_IntegrationTest_CreateChangeRequest_Success()
        {
            //Arrange
            var changeRequest = new ChangeRequest
            {
                CRNumber = "234.12",
                CR_WorkItems =
                    new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 }, new CR_WorkItems { Fk_WIId = 3 } }
            };
            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(TotalNoOfCrsInCsv + 1, response.Result);
            Assert.AreEqual(TotalNoOfCrWorkItemsInCsv + 2, UoW.Context.CR_WorkItems.Count());
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(response.Result).CR_WorkItems.Any(x => x.Fk_WIId == 2));
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(response.Result).CR_WorkItems.Any(x => x.Fk_WIId == 3));
        }

        [TestCase("WG3", 136083, "AZE", 2, Description = "WG revision of uid, create a CR as Revision of another CR")]
        [TestCase("WG4", 136083, "AZEE", 4, Description = "WG revision of uid, create a CR as Revision of another CR")]
        [TestCase("TSG4", 136083, "AZEEE", 5, Description = "TSG revision of uid, create a CR as Revision of another CR")]
        public void Service_IntegrationTest_CreateChangeRequest_RevisionOf_Success(string wgTdocUid, int specId, string crNumberExpected, int revisionExpected)
        {
            //Arrange
            var changeRequest = new ChangeRequest
            {
                RevisionOf = wgTdocUid,
                Fk_Specification = specId
            };

            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(TotalNoOfCrsInCsv + 1, response.Result);
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(response.Result).CRNumber == crNumberExpected);
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(response.Result).Revision == revisionExpected);
        }

        [TestCase("WG3", 136083, 7, 6, null, Description = "Parent CR mark as 'revised'")]
        [TestCase("TSG4", 136083, 10, 0, 6, Description = "Parent CR mark as 'revised'")]
        public void Service_IntegrationTest_CreateChangeRequest_RevisionOfParentStatus_Success(string wgTdocUid, int specId, int parentId, int statusWgExpected, int? statusTsgExpected)
        {
            //Arrange
            var changeRequest = new ChangeRequest
            {
                RevisionOf = wgTdocUid,
                Fk_Specification = specId
            };

            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(TotalNoOfCrsInCsv + 1, response.Result);

            //Check parent status
            var parentCr = UoW.Context.ChangeRequests.Find(parentId);
            Assert.AreEqual(statusTsgExpected, parentCr.ChangeRequestTsgDatas.First().Fk_TsgStatus);
            Assert.AreEqual(statusWgExpected, parentCr.Fk_WGStatus.GetValueOrDefault());
        }

        [Test, Description("System sets CR WG and TSG decisions")]
        public void CreateChangeRequest_SetsDecisions()
        {
            var changeRequest = new ChangeRequest
            {
                CRNumber = "234.12",
                Revision = 1,
                Fk_WGStatus = 1,
                ChangeRequestTsgDatas = new List<ChangeRequestTsgData> {new ChangeRequestTsgData {Fk_TsgStatus = 2}}
            };
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());

            //Retrieve result.
            var newCr = UoW.Context.ChangeRequests.Find(response.Result);
            Assert.IsNotNull(newCr);
            Assert.AreEqual(1, newCr.Fk_WGStatus);
            Assert.AreEqual(2, newCr.ChangeRequestTsgDatas.First().Fk_TsgStatus);
        }

        [Test, Description("System checks CR WG decision is valid")]
        public void CreateChangeRequest_ErrorInvalidWgDecision()
        {
            var changeRequest = new ChangeRequest
            {
                CRNumber = "234.12",
                Revision = 1,
                Fk_WGStatus = 235684,
                ChangeRequestTsgDatas = new List<ChangeRequestTsgData> { new ChangeRequestTsgData { Fk_TsgStatus = 2 } }
            };
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
        }

        [Test]
        public void Service_IntegrationTest_CreateChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest { Fk_Specification = 136080, Fk_Release = 12345 };
            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            var events = _memoryAppender.GetEvents();

            //Assert
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Result);
            Assert.AreEqual(1, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().Contains("The key value [12345] does not exists in the referenced table [Releases :: Pk_ReleaseId]"));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test]
        public void Service_IntegrationTest_EditChangeRequest_Success()
        {
            //Arrange
            var crRepo = new ChangeRequestRepository {UoW = UoW};
            var initialCr = crRepo.Find(1);
            Assert.IsNotNull(initialCr);
            Assert.AreEqual(2, initialCr.ChangeRequestTsgDatas.First().TSGTarget);
            Assert.AreEqual(1, initialCr.ChangeRequestTsgDatas.First().TSGMeeting);
            Assert.AreEqual("Change request", initialCr.ChangeRequestTsgDatas.First().TSGSourceOrganizations);
            Assert.AreEqual(1, initialCr.ChangeRequestTsgDatas.First().Fk_TsgStatus);
            Assert.AreEqual("TSG1", initialCr.ChangeRequestTsgDatas.First().TSGTdoc);

            initialCr.Subject = "Subject Changed modified";
            initialCr.ChangeRequestTsgDatas.First().TSGTarget = 3;
            initialCr.ChangeRequestTsgDatas.First().TSGMeeting = 2;
            initialCr.ChangeRequestTsgDatas.First().TSGSourceOrganizations = "Change request 2";
            initialCr.ChangeRequestTsgDatas.First().Fk_TsgStatus = 2;
            initialCr.ChangeRequestTsgDatas.First().TSGTdoc = "TSG2";

            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, initialCr);
            var modifiedCr = crRepo.Find(1);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(initialCr.CRNumber, modifiedCr.CRNumber);
            Assert.AreEqual(initialCr.Revision, modifiedCr.Revision);
            Assert.AreEqual("Subject Changed modified", modifiedCr.Subject);
            Assert.AreEqual(initialCr.Fk_WGStatus, modifiedCr.Fk_WGStatus);
            Assert.AreEqual(initialCr.CreationDate, modifiedCr.CreationDate);
            Assert.AreEqual(initialCr.WGSourceOrganizations, modifiedCr.WGSourceOrganizations);
            Assert.AreEqual(initialCr.WGSourceForTSG, modifiedCr.WGSourceForTSG);
            Assert.AreEqual(initialCr.WGMeeting, modifiedCr.WGMeeting);
            Assert.AreEqual(initialCr.WGTarget, modifiedCr.WGTarget);
            Assert.AreEqual(initialCr.WGTDoc, modifiedCr.WGTDoc);
            Assert.AreEqual(initialCr.Fk_Enum_CRCategory, modifiedCr.Fk_Enum_CRCategory);
            Assert.AreEqual(initialCr.Fk_Specification, modifiedCr.Fk_Specification);
            Assert.AreEqual(initialCr.Fk_Release, modifiedCr.Fk_Release);
            Assert.AreEqual(initialCr.Fk_CurrentVersion, modifiedCr.Fk_CurrentVersion);
            Assert.AreEqual(initialCr.Fk_NewVersion, modifiedCr.Fk_NewVersion);
            Assert.AreEqual(initialCr.Fk_Impact, modifiedCr.Fk_Impact);

            Assert.AreEqual(1, modifiedCr.ChangeRequestTsgDatas.Count());
            Assert.IsTrue(modifiedCr.ChangeRequestTsgDatas.First().TSGTarget == 3);
            Assert.IsTrue(modifiedCr.ChangeRequestTsgDatas.First().TSGMeeting == 2);
            Assert.AreEqual("Change request 2", initialCr.ChangeRequestTsgDatas.First().TSGSourceOrganizations);
            Assert.AreEqual(2, initialCr.ChangeRequestTsgDatas.First().Fk_TsgStatus);
            Assert.AreEqual("TSG2", initialCr.ChangeRequestTsgDatas.First().TSGTdoc);
        }

        [Test]
        public void Service_IntegrationTest_EditChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest { Pk_ChangeRequest = 1, CRNumber = "#CRChangedMorethan10Characters" };

            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, changeRequest);
            LoggingEvent[] events = _memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().StartsWith("[Service] Failed to edit change request: Validation failed for one or more entities. See 'EntityValidationErrors' property for more details."));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test]
        public void Service_IntegrationTest_CreateChangeRequestwithCrNumber()
        {
            //Arange
            var changeRequest = ChangeRequestDataObject();
            //Act
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.AreEqual(changeRequest.Pk_ChangeRequest, response.Result);
            Assert.AreEqual("A001", changeRequest.CRNumber);
        }

        [Test]
        public void Service_IntegrationTest_GetChangeRequestById()
        {
            //Act
            var changeRequestById = new ChangeRequestService();
            var result = changeRequestById.GetChangeRequestById(PersonId, 3);
            //Assert
            Assert.AreEqual(3, result.Value.Pk_ChangeRequest);
            Assert.AreEqual("A0012", result.Value.CRNumber);
            Assert.AreEqual("22.101", result.Value.Specification.Number);
            Assert.AreEqual("R2000", result.Value.Release.ShortName);
            Assert.AreEqual("13.0.1", result.Value.CurrentVersion.Version);
            Assert.AreEqual("13.0.1", result.Value.NewVersion.Version);
            Assert.AreEqual("Agreed", result.Value.ChangeRequestTsgDatas.First().TsgStatus.Description);
            Assert.AreEqual("Approved", result.Value.WgStatus.Description);
        }

        [Test]
        public void Service_IntegrationTes_GetChangeRequestByContribUid()
        {
            const string contribUid = "TSG1";
            const string tdocNumber = "0001";
            const int tdocRevision = 1;
            //Act
            var svcCr = new ChangeRequestService();
            var result = svcCr.GetContributionCrByUid(contribUid);
            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreEqual(contribUid, result.Value.ChangeRequestTsgDatas.First().TSGTdoc);
            Assert.AreEqual(tdocNumber, result.Value.CRNumber);
            Assert.AreEqual(tdocRevision, result.Value.Revision);
            Assert.AreEqual("22.102", result.Value.Specification.Number);
            Assert.AreEqual("R2000", result.Value.Release.ShortName);
            Assert.AreEqual("13.0.1", result.Value.CurrentVersion.Version);
            Assert.AreEqual("13.0.1", result.Value.NewVersion.Version);
            Assert.AreEqual("Agreed", result.Value.ChangeRequestTsgDatas.First().TsgStatus.Description);
            Assert.AreEqual("Approved", result.Value.WgStatus.Description);
        }

        [Test]
        public void Service_IntegrationTes_GetChangeRequestListByContribUids()
        {
            var uids = new List<string> { "TSG1", "Change request description6" };
            var tdocNumbers = new List<string> { "0001", "A0144" };
            var tdocRevisions = new List<int> { 1, 2 };
            var tdocSpecNumbers = new List<string> { "22.102", "22.101" };
            var tdocReleaseShortNames = new List<string> { "R2000", "R2000" };
            var tdocCurrentVersions = new List<string> { "13.0.1", "13.0.1" };
            var tdocNewVersions = new List<string> { "13.0.1", "13.0.1" };
            var tdocTsgStatus = new List<string> { "Agreed", "Agreed" };
            var tdocWgStatus = new List<string> { "Approved", "Agreed" };
            //Act
            var svcCr = new ChangeRequestService();
            var result = svcCr.GetChangeRequestListByContributionUidList(uids);
            //Assert
            Assert.IsTrue(result.Key);
            var crList = result.Value;
            Assert.IsNotNull(crList);
            if (crList != null)
            {
                for (int i = 0; i < uids.Count; i++)
                {
                    Assert.AreEqual(uids[i], crList[i].ChangeRequestTsgDatas.First().TSGTdoc);
                    Assert.AreEqual(tdocNumbers[i], crList[i].CRNumber);
                    Assert.AreEqual(tdocRevisions[i], crList[i].Revision);
                    Assert.AreEqual(tdocSpecNumbers[i], crList[i].Specification.Number);
                    Assert.AreEqual(tdocReleaseShortNames[i], crList[i].Release.ShortName);
                    Assert.AreEqual(tdocCurrentVersions[i], crList[i].CurrentVersion.Version);
                    Assert.AreEqual(tdocNewVersions[i], crList[i].NewVersion.Version);
                    Assert.AreEqual(tdocTsgStatus[i], crList[i].ChangeRequestTsgDatas.First().TsgStatus.Description);
                    Assert.AreEqual(tdocWgStatus[i], crList[i].WgStatus.Description);
                }
            }
        }

        [Test, Description("System must fetch the list of statuses in database")]
        public void GetStatus_ReturnsTrueAndEnum()
        {
            var svcCr = new ChangeRequestService();
            var response = svcCr.GetChangeRequestStatuses();
            Assert.IsTrue(response.Key);
            Assert.IsNotNull(response.Value);

            var agreedStatus = response.Value.Find(s => s.Pk_EnumChangeRequestStatus == 1);
            Assert.IsNotNull(agreedStatus);
            Assert.AreEqual(1, agreedStatus.Pk_EnumChangeRequestStatus);
            Assert.AreEqual("Agreed", agreedStatus.Code);
            Assert.AreEqual("Agreed", agreedStatus.Description);
        }

        [Test, TestCaseSource("ChangeRequestsSearchData")]
        public void Service_IntegrationTest_GetChangeRequests_Success(ChangeRequestsSearch changeRequestsSearch, int searchResultCount, int searchQueryCount, string crNumber, string revisionNumber)
        {
            var svcCr = new ChangeRequestService();
            var result = svcCr.GetChangeRequests(PersonId, changeRequestsSearch);

            Assert.AreEqual(searchResultCount, result.Result.Key.Count);
            Assert.AreEqual(searchQueryCount, result.Result.Value);
            Assert.AreEqual(crNumber, result.Result.Key.FirstOrDefault().ChangeRequestNumber);
            Assert.AreEqual(revisionNumber, result.Result.Key.FirstOrDefault().Revision);
        }

        [TestCase("AZEE", 2, 136083, true, Description = "should exist")]
        [TestCase("AZEE", 22, 136083, false, Description = "should not exist")]
        public void Service_IntegrationTest_IsExistCrNumberRevisionCouple(string crNumber, int revision, int specId, bool expectedResult)
        {
            //Act
            var crService = new ChangeRequestService();
            var response = crService.DoesCrNumberRevisionCoupleExist(specId, crNumber, revision);
            //Assert
            Assert.AreEqual(expectedResult, response.Result);
        }

        [Test, TestCaseSource("ChangeRequestKeys")]
        public void Service_IntegrationTest_GetCrsByKeys(List<CrKeyFacade> crKeys, int resultCount)
        {
            //Act
            var crService = new ChangeRequestService();
            var response = crService.GetCrsByKeys(crKeys);
            //Assert
            Assert.AreEqual(resultCount, response.Result.Count);
        }

        [Test]
        public void Service_IntegrationTest_GetCrByKey()
        {
            var crKey = new CrKeyFacade { CrNumber = "0001", SpecNumber = "22.102", Revision = 1 };
            //Act
            var crService = new ChangeRequestService();
            var response = crService.GetCrByKey(crKey);
            //Assert
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(crKey.CrNumber, response.Result.CRNumber);
            Assert.AreEqual(crKey.SpecNumber, response.Result.Specification.Number);
            Assert.AreEqual(crKey.Revision, response.Result.Revision);
        }

        #endregion

        #region DataObject

        /// <summary>
        /// Changes the request data object.
        /// </summary>
        /// <returns></returns>
        private static ChangeRequest ChangeRequestDataObject()
        {
            var changeRequest = new ChangeRequest
            {
                CRNumber = "A001",
                Revision = 1,
                Subject = "Description",
                Fk_WGStatus = 1,
                CreationDate = DateTime.UtcNow,
                WGSourceOrganizations = "Ultimate",
                WGSourceForTSG = 2,
                WGMeeting = 2,
                WGTarget = 2,
                WGTDoc = "Work item",
                Fk_Enum_CRCategory = 1,
                Fk_Specification = 136080,
                Fk_Release = 2874,
                Fk_CurrentVersion = 428927,
                Fk_NewVersion = 428927,
                Fk_Impact = 1,
                ChangeRequestTsgDatas = new List<ChangeRequestTsgData>{new ChangeRequestTsgData
                {
                    TSGTdoc = "Change request",
                    TSGTarget = 2,
                    TSGMeeting = 2,
                    TSGSourceOrganizations = "Change request",
                    Fk_TsgStatus = 1
                }}
            };
            return changeRequest;
        }

        /// <summary>
        /// Gets the change request by identifier data object.
        /// </summary>
        /// <param name="changeRequestById">The change request by identifier.</param>
        /// <returns></returns>
        private static ChangeRequest GetChangeRequestByIdDataObject(int changeRequestById)
        {
            var changeRequest = new List<ChangeRequest>
            {

            new ChangeRequest{ Pk_ChangeRequest=1, CRNumber = "A001",Revision = 1,Subject = "Description",Fk_WGStatus = 1,CreationDate = DateTime.UtcNow,
                WGSourceOrganizations = "Ultimate", ChangeRequestTsgDatas = new List<ChangeRequestTsgData>{new ChangeRequestTsgData{Fk_TsgStatus = 1,TSGTdoc = "Change request",TSGSourceOrganizations = "Change request", TSGTarget = 2, TSGMeeting = 2}},WGSourceForTSG = 2,
                WGMeeting = 2,WGTarget = 2,WGTDoc = "Work item",Fk_Enum_CRCategory = 1,Fk_Specification = 136080,
                Fk_Release = 2874,Fk_CurrentVersion = 428927,Fk_NewVersion = 428927,Fk_Impact = 1},
            new ChangeRequest{ Pk_ChangeRequest=2, CRNumber = "A002",Revision = 1,Subject = "Description",Fk_WGStatus = 1,CreationDate = DateTime.UtcNow
                ,WGSourceOrganizations = "Ultimate Desc", ChangeRequestTsgDatas = new List<ChangeRequestTsgData>{new ChangeRequestTsgData{Fk_TsgStatus = 1, TSGTdoc = "Change request", TSGSourceOrganizations = "Change request Desc", TSGTarget = 2, TSGMeeting = 2}}, WGSourceForTSG = 2,
                WGMeeting = 2,WGTarget = 2,WGTDoc = "Work item",Fk_Enum_CRCategory = 1,Fk_Specification = 136080,
                Fk_Release = 2874,Fk_CurrentVersion = 428927,Fk_NewVersion = 428927,Fk_Impact = 1},

            };
            return changeRequest.Find(x => x.Pk_ChangeRequest == changeRequestById);
        }

        /// <summary>
        /// Gets the change requests search data.
        /// </summary>
        private IEnumerable<object[]> ChangeRequestsSearchData
        {
            get
            {
                yield return new object[] { new ChangeRequestsSearch() { PageSize = 2, SkipRecords = 0, SpecificationNumber = "22.101" }, 2, 6, "AC014", "1" };
                yield return new object[] { new ChangeRequestsSearch() { PageSize = 5, SkipRecords = 10, SpecificationNumber = "22.10" }, 5, 24, "AZEE", "2" };
                yield return new object[] { new ChangeRequestsSearch() { PageSize = 3, SkipRecords = 21, SpecificationNumber = "22.10" }, 3, 24, "3568", "16" };
                yield return new object[] { new ChangeRequestsSearch() { PageSize = 100, SkipRecords = 0, SpecificationNumber = "22.101", ReleaseIds = new List<int>{0}}, 6, 6, "AC014", "1" };
                yield return new object[] { new ChangeRequestsSearch() { PageSize = 100, SkipRecords = 0, SpecificationNumber = "22.101", ReleaseIds = new List<int> { 0, 2884 } }, 6, 6, "AC014", "1" };
            }
        }

        /// <summary>
        /// Gets the change request tuple data (combination of Spec#, CR #, Revision).
        /// </summary>
        private IEnumerable<object[]> ChangeRequestKeys
        {
            get
            {
                yield return new object[] { new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "0001", SpecId = 136081 , Revision = 1}, new CrKeyFacade { CrNumber = "0001", SpecId = 136081 , Revision = 1}}, 1 };
                yield return new object[] { new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "AB013", SpecId = 136080, Revision = 1 }, new CrKeyFacade { CrNumber = "AC014", SpecId = 136080, Revision = 1 }}, 2 };
                yield return new object[] { new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "0001", SpecId = 136081, Revision = 3 }, new CrKeyFacade { CrNumber = "0001", SpecId = 136081, Revision = 4 }}, 0 };
                yield return new object[] { new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "AZEE", SpecId = 136083, Revision = 3, TsgTdocNumber = "TSG_ABC" } }, 1 };
                yield return new object[] { new List<CrKeyFacade> { new CrKeyFacade { CrNumber = "AZEE", SpecId = 136083, Revision = 3, TsgTdocNumber = "DON'TEXIST" } }, 1 };
            }
        }

        #endregion
    }
}
