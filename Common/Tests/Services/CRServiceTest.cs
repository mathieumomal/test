
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

namespace Etsi.Ultimate.Tests.Services
{
    [Category("CR Tests")]
    public class CrServiceTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 6;
        private const int totalNoOfCRWorkItemsInCSV = 0;
        private const int personID = 0;

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
            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(personID, changeRequest);
            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreNotEqual(6, result.Value);
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
            Assert.AreEqual(changeRequest.CRNumber, "AC0145");
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
