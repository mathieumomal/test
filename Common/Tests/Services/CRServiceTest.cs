
using System.Security.Cryptography;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Etsi.Ultimate.Utils.Core;
using log4net.Appender;
using log4net.Core;

namespace Etsi.Ultimate.Tests.Services
{
    [Category("CR Tests")]
    public class CrServiceTest : BaseEffortTest
    {
        #region Constants
        private const int TotalNoOfCrsInCsv = 22;
        private const int TotalNoOfCrWorkItemsInCsv = 1;
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

        [TestCase("WG3", 136083, 7, 6, 0, Description = "Parent CR mark as 'revised'")]
        [TestCase("TSG4", 136083, 10, 0, 6, Description = "Parent CR mark as 'revised'")]
        public void Service_IntegrationTest_CreateChangeRequest_RevisionOfParentStatus_Success(string wgTdocUid, int specId, int parentId, int statusWgExpected, int statusTsgExpected)
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
            Assert.AreEqual(statusTsgExpected, parentCr.Fk_TSGStatus.GetValueOrDefault());
            Assert.AreEqual(statusWgExpected, parentCr.Fk_WGStatus.GetValueOrDefault());
        }

        [Test, Description("RevisionOfUid don't exist")]
        public void Service_IntegrationTest_CreateChangeRequest_RevisionOf_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest
            {
                RevisionOf = "uidDontExist",
                Fk_Specification = 136083
            };

            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(PersonId, changeRequest);
            var events = _memoryAppender.GetEvents();
            Assert.AreEqual(1, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().Contains("uidDontExist not found"));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test, Description("System sets CR WG and TSG decisions")]
        public void CreateChangeRequest_SetsDecisions()
        {
            var changeRequest = new ChangeRequest
            {
                CRNumber = "234.12",
                Revision = 1,
                Fk_WGStatus = 1,
                Fk_TSGStatus = 2,
            };
            var crService = new ChangeRequestService();
            var response = crService.CreateChangeRequest(PersonId, changeRequest);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());

            //Retrieve result.
            var newCr = UoW.Context.ChangeRequests.Find(response.Result);
            Assert.IsNotNull(newCr);
            Assert.AreEqual(1, newCr.Fk_WGStatus);
            Assert.AreEqual(2, newCr.Fk_TSGStatus);
        }

        [Test, Description("System checks CR WG decision is valid")]
        public void CreateChangeRequest_ErrorInvalidWgDecision()
        {
            var changeRequest = new ChangeRequest
            {
                CRNumber = "234.12",
                Revision = 1,
                Fk_WGStatus = 235684,
                Fk_TSGStatus = 2
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
            var changeRequest = new ChangeRequest
            {
                Pk_ChangeRequest = 1,
                CRNumber = "#CRChanged",
                Revision = 2,
                Subject = "Subject Changed",
                Fk_TSGStatus = 2,
                Fk_WGStatus = 2,
                CreationDate = Convert.ToDateTime("2009-01-30 12:12"),
                TSGSourceOrganizations = "Change request Changed",
                WGSourceOrganizations = "Ultimate Changed",
                TSGMeeting = 3,
                TSGTarget = 3,
                WGSourceForTSG = 3,
                TSGTDoc = "Change request description Changed",
                WGMeeting = 3,
                WGTarget = 3,
                WGTDoc = "Work item Changed",
                Fk_Enum_CRCategory = 2,
                Fk_Specification = 136082,
                Fk_Release = 2882,
                Fk_CurrentVersion = 428928,
                Fk_NewVersion = 428928,
                Fk_Impact = 2,
                CR_WorkItems =
                    new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 }, new CR_WorkItems { Fk_WIId = 3 } }
            };
            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, changeRequest);
            var modifiedCr = UoW.Context.ChangeRequests.Find(1);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(changeRequest.CRNumber, modifiedCr.CRNumber);
            Assert.AreEqual(changeRequest.Revision, modifiedCr.Revision);
            Assert.AreEqual(changeRequest.Subject, modifiedCr.Subject);
            Assert.AreEqual(changeRequest.Fk_TSGStatus, modifiedCr.Fk_TSGStatus);
            Assert.AreEqual(changeRequest.Fk_WGStatus, modifiedCr.Fk_WGStatus);
            Assert.AreEqual(changeRequest.CreationDate, modifiedCr.CreationDate);
            Assert.AreEqual(changeRequest.TSGSourceOrganizations, modifiedCr.TSGSourceOrganizations);
            Assert.AreEqual(changeRequest.WGSourceOrganizations, modifiedCr.WGSourceOrganizations);
            Assert.AreEqual(changeRequest.TSGMeeting, modifiedCr.TSGMeeting);
            Assert.AreEqual(changeRequest.TSGTarget, modifiedCr.TSGTarget);
            Assert.AreEqual(changeRequest.WGSourceForTSG, modifiedCr.WGSourceForTSG);
            Assert.AreEqual(changeRequest.TSGTDoc, modifiedCr.TSGTDoc);
            Assert.AreEqual(changeRequest.WGMeeting, modifiedCr.WGMeeting);
            Assert.AreEqual(changeRequest.WGTarget, modifiedCr.WGTarget);
            Assert.AreEqual(changeRequest.WGTDoc, modifiedCr.WGTDoc);
            Assert.AreEqual(changeRequest.Fk_Enum_CRCategory, modifiedCr.Fk_Enum_CRCategory);
            Assert.AreEqual(changeRequest.Fk_Specification, modifiedCr.Fk_Specification);
            Assert.AreEqual(changeRequest.Fk_Release, modifiedCr.Fk_Release);
            Assert.AreEqual(changeRequest.Fk_CurrentVersion, modifiedCr.Fk_CurrentVersion);
            Assert.AreEqual(changeRequest.Fk_NewVersion, modifiedCr.Fk_NewVersion);
            Assert.AreEqual(changeRequest.Fk_Impact, modifiedCr.Fk_Impact);
            Assert.AreEqual(2, modifiedCr.CR_WorkItems.Count());
            Assert.IsTrue(modifiedCr.CR_WorkItems.Any(x => x.Fk_WIId == 2));
            Assert.IsTrue(modifiedCr.CR_WorkItems.Any(x => x.Fk_WIId == 3));
            Assert.IsFalse(modifiedCr.CR_WorkItems.Any(x => x.Fk_WIId == 1274));
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
            Assert.AreEqual("Agreed", result.Value.TsgStatus.Description);
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
            Assert.AreEqual(contribUid, result.Value.TSGTDoc);
            Assert.AreEqual(tdocNumber, result.Value.CRNumber);
            Assert.AreEqual(tdocRevision, result.Value.Revision);
            Assert.AreEqual("22.102", result.Value.Specification.Number);
            Assert.AreEqual("R2000", result.Value.Release.ShortName);
            Assert.AreEqual("13.0.1", result.Value.CurrentVersion.Version);
            Assert.AreEqual("13.0.1", result.Value.NewVersion.Version);
            Assert.AreEqual("Agreed", result.Value.TsgStatus.Description);
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
                    Assert.AreEqual(uids[i], crList[i].TSGTDoc);
                    Assert.AreEqual(tdocNumbers[i], crList[i].CRNumber);
                    Assert.AreEqual(tdocRevisions[i], crList[i].Revision);
                    Assert.AreEqual(tdocSpecNumbers[i], crList[i].Specification.Number);
                    Assert.AreEqual(tdocReleaseShortNames[i], crList[i].Release.ShortName);
                    Assert.AreEqual(tdocCurrentVersions[i], crList[i].CurrentVersion.Version);
                    Assert.AreEqual(tdocNewVersions[i], crList[i].NewVersion.Version);
                    Assert.AreEqual(tdocTsgStatus[i], crList[i].TsgStatus.Description);
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

        [Test]
        public void GetChangeRequests_NominalCase()
        {
            //Get db object
            var crTotalCount = UoW.Context.ChangeRequests.Count();
            var concernedCr = UoW.Context.ChangeRequests.Find(5);

            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = 1, SkipRecords = 0 };

            //Call method
            var svc = new ChangeRequestService();
            var response = svc.GetChangeRequests(PersonId, searchObj);

            //Tests
            var crListFacade = response.Result.Key[0];
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.IsNotEmpty(response.Result.Key);
            Assert.AreEqual(crTotalCount, response.Result.Value);

            Assert.AreEqual(crListFacade.ChangeRequestId, concernedCr.Pk_ChangeRequest);
            Assert.AreEqual(crListFacade.SpecNumber, concernedCr.Specification.Number);
            Assert.AreEqual(crListFacade.ChangeRequestNumber, concernedCr.CRNumber);
            Assert.AreEqual(crListFacade.Revision, (concernedCr.Revision ?? 0) == 0 ? "-" : concernedCr.Revision.ToString());
            Assert.AreEqual(crListFacade.ImpactedVersion, string.Format("{0}.{1}.{2}", concernedCr.CurrentVersion.MajorVersion, concernedCr.CurrentVersion.TechnicalVersion,
                        concernedCr.CurrentVersion.EditorialVersion));
            Assert.AreEqual(crListFacade.TargetRelease, concernedCr.Release.ShortName);
            Assert.AreEqual(crListFacade.Title, concernedCr.Subject);
            Assert.AreEqual(crListFacade.WgTdocNumber, concernedCr.WGTDoc);
            Assert.AreEqual(crListFacade.TsgTdocNumber, concernedCr.TSGTDoc);
            Assert.AreEqual(crListFacade.WgStatus, concernedCr.WgStatus.Description);
            Assert.AreEqual(crListFacade.TsgStatus, concernedCr.TsgStatus.Description);
            Assert.AreEqual(crListFacade.NewVersion, string.Format("{0}.{1}.{2}", concernedCr.NewVersion.MajorVersion, concernedCr.NewVersion.TechnicalVersion,
                        concernedCr.NewVersion.EditorialVersion));
            Assert.AreEqual(crListFacade.SpecId, concernedCr.Fk_Specification ?? 0);
            Assert.AreEqual(crListFacade.TargetReleaseId, concernedCr.Release.Pk_ReleaseId);
            Assert.AreEqual(crListFacade.NewVersionPath, concernedCr.NewVersion.Location);
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
                Fk_TSGStatus = 1,
                Fk_WGStatus = 1,
                CreationDate = DateTime.UtcNow,
                TSGSourceOrganizations = "Change request",
                WGSourceOrganizations = "Ultimate",
                TSGMeeting = 2,
                TSGTarget = 2,
                WGSourceForTSG = 2,
                TSGTDoc = "Change request",
                WGMeeting = 2,
                WGTarget = 2,
                WGTDoc = "Work item",
                Fk_Enum_CRCategory = 1,
                Fk_Specification = 136080,
                Fk_Release = 2874,
                Fk_CurrentVersion = 428927,
                Fk_NewVersion = 428927,
                Fk_Impact = 1
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

            new ChangeRequest{ Pk_ChangeRequest=1, CRNumber = "A001",Revision = 1,Subject = "Description",Fk_TSGStatus = 1,Fk_WGStatus = 1,CreationDate = DateTime.UtcNow,
                TSGSourceOrganizations = "Change request",WGSourceOrganizations = "Ultimate",TSGMeeting = 2,TSGTarget = 2,WGSourceForTSG = 2,
                TSGTDoc = "Change request",WGMeeting = 2,WGTarget = 2,WGTDoc = "Work item",Fk_Enum_CRCategory = 1,Fk_Specification = 136080,
                Fk_Release = 2874,Fk_CurrentVersion = 428927,Fk_NewVersion = 428927,Fk_Impact = 1},
            new ChangeRequest{ Pk_ChangeRequest=2, CRNumber = "A002",Revision = 1,Subject = "Description",Fk_TSGStatus = 1,Fk_WGStatus = 1,CreationDate = DateTime.UtcNow,
                TSGSourceOrganizations = "Change request Desc",WGSourceOrganizations = "Ultimate Desc",TSGMeeting = 2,TSGTarget = 2,WGSourceForTSG = 2,
                TSGTDoc = "Change request",WGMeeting = 2,WGTarget = 2,WGTDoc = "Work item",Fk_Enum_CRCategory = 1,Fk_Specification = 136080,
                Fk_Release = 2874,Fk_CurrentVersion = 428927,Fk_NewVersion = 428927,Fk_Impact = 1},

            };
            return changeRequest.Find(x => x.Pk_ChangeRequest == changeRequestById);
        }
        #endregion
    }
}
