
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
        private const int TotalNoOfCrsInCsv = 6;
        private const int TotalNoOfCrWorkItemsInCsv = 1;
        private const int PersonId = 0;
        MemoryAppender _memoryAppender;
        #endregion

        #region Logger Setup

        [TestFixtureSetUp]
        public void Init()
        {
            string configFileName = Directory.GetCurrentDirectory() + "\\TestData\\LogManager\\Test.log4net.config";
            LogManager.SetConfiguration(configFileName, "TestLogger");
        }

        [SetUp]
        public override void Setup()
        {
            base.SetUp();
            _memoryAppender = ((log4net.Core.LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
            _memoryAppender.Clear();
        }

        [TearDown]
        public void TearDown()
        {
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
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCRManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCRManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsTrue(result.Key);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void Service_UnitTest_CreateChangeRequest_Failure()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRManager = MockRepository.GenerateMock<IChangeRequestManager>();
            mockCRManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(false);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockCRManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsFalse(result.Key);
            Assert.AreEqual(0, result.Value);
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
            ChangeRequest changeRequest = new ChangeRequest();
            changeRequest.CRNumber = "234.12";
            changeRequest.CR_WorkItems = new List<CR_WorkItems>() { new CR_WorkItems() { Fk_WIId = 2 }, new CR_WorkItems() { Fk_WIId = 3 } };
            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreEqual(TotalNoOfCrsInCsv + 1, result.Value);
            Assert.AreEqual(TotalNoOfCrWorkItemsInCsv + 2, UoW.Context.CR_WorkItems.Count());
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(result.Value).CR_WorkItems.Any(x => x.Fk_WIId == 2));
            Assert.IsTrue(UoW.Context.ChangeRequests.Find(result.Value).CR_WorkItems.Any(x => x.Fk_WIId == 3));
        }

        [Test]
        public void Service_IntegrationTest_CreateChangeRequest_Failure()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            changeRequest.Fk_Specification = 136080;
            changeRequest.Fk_Release = 12345;
            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(PersonId, changeRequest);
            LoggingEvent[] events = _memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(result.Key);
            Assert.AreEqual(0, result.Value);
            Assert.AreEqual(1, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().Contains("The key value [12345] does not exists in the referenced table [Releases :: Pk_ReleaseId]"));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test]
        public void Service_IntegrationTest_EditChangeRequest_Success()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            changeRequest.Pk_ChangeRequest = 1;
            changeRequest.CRNumber = "#CRChanged";
            changeRequest.Revision = 2;
            changeRequest.Subject = "Subject Changed";
            changeRequest.Fk_TSGStatus = 2;
            changeRequest.Fk_WGStatus = 2;
            changeRequest.CreationDate = Convert.ToDateTime("2009-01-30 12:12");
            changeRequest.TSGSourceOrganizations = "Change request Changed";
            changeRequest.WGSourceOrganizations = "Ultimate Changed";
            changeRequest.TSGMeeting = 3;
            changeRequest.TSGTarget = 3;
            changeRequest.WGSourceForTSG = 3;
            changeRequest.TSGTDoc = "Change request description Changed";
            changeRequest.WGMeeting = 3;
            changeRequest.WGTarget = 3;
            changeRequest.WGTDoc = "Work item Changed";
            changeRequest.Fk_Enum_CRCategory = 2;
            changeRequest.Fk_Specification = 136082;
            changeRequest.Fk_Release = 2882;
            changeRequest.Fk_CurrentVersion = 428928;
            changeRequest.Fk_NewVersion = 428928;
            changeRequest.Fk_Impact = 2;
            changeRequest.CR_WorkItems = new List<CR_WorkItems>() { new CR_WorkItems() { Fk_WIId = 2 }, new CR_WorkItems() { Fk_WIId = 3 } };
            //Act
            var crService = new ChangeRequestService();
            var result = crService.EditChangeRequest(PersonId, changeRequest);
            var modifiedCR = UoW.Context.ChangeRequests.Find(1);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(changeRequest.CRNumber, modifiedCR.CRNumber);
            Assert.AreEqual(changeRequest.Revision, modifiedCR.Revision);
            Assert.AreEqual(changeRequest.Subject, modifiedCR.Subject);
            Assert.AreEqual(changeRequest.Fk_TSGStatus, modifiedCR.Fk_TSGStatus);
            Assert.AreEqual(changeRequest.Fk_WGStatus, modifiedCR.Fk_WGStatus);
            Assert.AreEqual(changeRequest.CreationDate, modifiedCR.CreationDate);
            Assert.AreEqual(changeRequest.TSGSourceOrganizations, modifiedCR.TSGSourceOrganizations);
            Assert.AreEqual(changeRequest.WGSourceOrganizations, modifiedCR.WGSourceOrganizations);
            Assert.AreEqual(changeRequest.TSGMeeting, modifiedCR.TSGMeeting);
            Assert.AreEqual(changeRequest.TSGTarget, modifiedCR.TSGTarget);
            Assert.AreEqual(changeRequest.WGSourceForTSG, modifiedCR.WGSourceForTSG);
            Assert.AreEqual(changeRequest.TSGTDoc, modifiedCR.TSGTDoc);
            Assert.AreEqual(changeRequest.WGMeeting, modifiedCR.WGMeeting);
            Assert.AreEqual(changeRequest.WGTarget, modifiedCR.WGTarget);
            Assert.AreEqual(changeRequest.WGTDoc, modifiedCR.WGTDoc);
            Assert.AreEqual(changeRequest.Fk_Enum_CRCategory, modifiedCR.Fk_Enum_CRCategory);
            Assert.AreEqual(changeRequest.Fk_Specification, modifiedCR.Fk_Specification);
            Assert.AreEqual(changeRequest.Fk_Release, modifiedCR.Fk_Release);
            Assert.AreEqual(changeRequest.Fk_CurrentVersion, modifiedCR.Fk_CurrentVersion);
            Assert.AreEqual(changeRequest.Fk_NewVersion, modifiedCR.Fk_NewVersion);
            Assert.AreEqual(changeRequest.Fk_Impact, modifiedCR.Fk_Impact);
            Assert.AreEqual(2, modifiedCR.CR_WorkItems.Count());
            Assert.IsTrue(modifiedCR.CR_WorkItems.Any(x => x.Fk_WIId == 2));
            Assert.IsTrue(modifiedCR.CR_WorkItems.Any(x => x.Fk_WIId == 3));
            Assert.IsFalse(modifiedCR.CR_WorkItems.Any(x => x.Fk_WIId == 1274));
        }

        [Test]
        public void Service_IntegrationTest_EditChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            changeRequest.Pk_ChangeRequest = 1;
            changeRequest.CRNumber = "#CRChangedMorethan10Characters";

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
            var result = crService.CreateChangeRequest(PersonId, changeRequest);
            //Assert
            Assert.AreEqual(changeRequest.Pk_ChangeRequest, result.Value);
            Assert.AreEqual(changeRequest.CRNumber, "0001");
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
        }

        [Test]
        public void ervice_IntegrationTes_GetChangeRequestByContribUid()
        {
            const string contribUid = "Change request description1";
            const string tdocNumber = "0001";
            const int tdocRevision = 1;
            //Act
            var svcCr= new ChangeRequestService();
            var result = svcCr.GetContributionCrByUid(contribUid);
            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreEqual(contribUid, result.Value.TSGTDoc);
            Assert.AreEqual(tdocNumber, result.Value.CRNumber);
            Assert.AreEqual(tdocRevision, result.Value.Revision);
        }
        
        #endregion

        #region DataObject

        private static ChangeRequest ChangeRequestDataObject()
        {
            ChangeRequest changeRequest = new ChangeRequest
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
