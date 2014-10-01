
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

        private const int totalNoOfCRsInCSV = 6;
        private const int totalNoOfCRWorkItemsInCSV = 1;
        private const int personID = 0;
        MemoryAppender memoryAppender;

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
            memoryAppender = ((log4net.Core.LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
            memoryAppender.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            memoryAppender.Clear();
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
            var result = crService.CreateChangeRequest(personID, changeRequest);

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
            var result = crService.CreateChangeRequest(personID, changeRequest);

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
            var result = crService.EditChangeRequest(personID, changeRequest);

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
            var result = crService.EditChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsFalse(result);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test]
        public void Service_UnitTest_GetChangeRequestById()
        {
            var mockChangeRequestById = MockRepository.GenerateMock<IChangeRequestManager>();
            var changeRequestById = GetChangeRequestByIdDataObject(1);
            mockChangeRequestById.Stub(x => x.GetChangeRequestById(0, 1)).Return(changeRequestById);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockChangeRequestById);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var svcChangeRequestById = new ChangeRequestService();
            var result = svcChangeRequestById.GetChangeRequestById(personID, 1);
            //Assert
            Assert.IsTrue(result.Key);
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
            var result = crService.CreateChangeRequest(personID, changeRequest);
            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreEqual(totalNoOfCRsInCSV + 1, result.Value);
            Assert.AreEqual(totalNoOfCRWorkItemsInCSV + 2, UoW.Context.CR_WorkItems.Count());
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
            var result = crService.CreateChangeRequest(personID, changeRequest);
            LoggingEvent[] events = memoryAppender.GetEvents();

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
            var result = crService.EditChangeRequest(personID, changeRequest);
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
            var result = crService.EditChangeRequest(personID, changeRequest);
            LoggingEvent[] events = memoryAppender.GetEvents();

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
            var result = crService.CreateChangeRequest(personID, changeRequest);
            //Assert
            Assert.AreEqual(changeRequest.Pk_ChangeRequest, result.Value);
            Assert.AreEqual(changeRequest.CRNumber, "0001");
        }
     
        [Test]
        public void Service_IntegrationTest_GetChangeRequestById()
        {
            //Act
            var changeRequestById = new ChangeRequestService();
            var result = changeRequestById.GetChangeRequestById(personID, 3);
            //Assert
            Assert.AreEqual(3, result.Value.Pk_ChangeRequest);
            Assert.AreEqual("A0012", result.Value.CRNumber);
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
