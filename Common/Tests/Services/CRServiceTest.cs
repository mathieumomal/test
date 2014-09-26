
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

namespace Etsi.Ultimate.Tests.Services
{
    [Category("CR Tests")]
    public class CRServiceTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 6;
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
        public void Service_UnitTest_GetchangeRequestCategories()
        {
            //Arrange
            var mockChangeRequestCategories = MockRepository.GenerateMock<IChangeRequestManager>();
            var changeRequestCategories = ChangeRequestCategoryDataObject();
            mockChangeRequestCategories.Stub(x => x.GetChangeRequestCategories(Arg<int>.Is.Anything)).Return(changeRequestCategories);
            ManagerFactory.Container.RegisterInstance(typeof(IChangeRequestManager), mockChangeRequestCategories);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            //Act
            var crCategoryService = new ChangeRequestService();
            var result = crCategoryService.GetChangeRequestCategories(personID);
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

            //Act
            var crService = new ChangeRequestService();
            var result = crService.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsTrue(result.Key);
            Assert.AreEqual(totalNoOfCRsInCSV + 1, result.Value);
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
        public void Service_IntegrationTest_GetchangeRequestCategories()
       {           
            //Act
            var crCategoryService = new ChangeRequestService();
            var result = crCategoryService.GetChangeRequestCategories(personID);
            //Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("CR", result.Value[0].Code);          
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

        private static List<Enum_CRCategory> ChangeRequestCategoryDataObject()
        {
            var changeRequestCategories = new List<Enum_CRCategory>
            {
                new Enum_CRCategory { Pk_EnumCRCategory = 1, Code = "CR", Description = "Change Request" },
                new Enum_CRCategory { Pk_EnumCRCategory = 2, Code = "PCR", Description = "Pack Change Request" },
            };
            return changeRequestCategories;
        }
        #endregion
    }
}
