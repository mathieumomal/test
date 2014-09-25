
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using System.Collections.Generic;

namespace Etsi.Ultimate.Tests.Services
{
    [Category("CR Tests")]
    public class CRServiceTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;
        private const int personID = 0;

        #endregion

        #region Unit Tests

        [Test]
        public void Service_UnitTest_CreateChangeRequest_Success()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRManager = MockRepository.GenerateMock<ICRManager>();
            mockCRManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance(typeof(ICRManager), mockCRManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new CRService();
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
            var mockCRManager = MockRepository.GenerateMock<ICRManager>();
            mockCRManager.Stub(x => x.CreateChangeRequest(Arg<int>.Is.Anything, Arg<ChangeRequest>.Is.Anything)).Return(false);
            ManagerFactory.Container.RegisterInstance(typeof(ICRManager), mockCRManager);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Act
            var crService = new CRService();
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
            var mockChangeRequestCategories = MockRepository.GenerateMock<ICRManager>();
            var changeRequestCategories = new List<Enum_CRCategory>
            {
                new Enum_CRCategory { Pk_EnumCRCategory = 1, Code = "CR", Description = "Change Request" },
                new Enum_CRCategory { Pk_EnumCRCategory = 2, Code = "PCR", Description = "Pack Change Request" },
            };
            mockChangeRequestCategories.Stub(x => x.GetChangeRequestCategories(Arg<int>.Is.Anything)).Return(changeRequestCategories);
            ManagerFactory.Container.RegisterInstance(typeof(ICRManager), mockChangeRequestCategories);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            //Act
            var crCategoryService = new CRService();
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
            var crService = new CRService();
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
            var crService = new CRService();
            var result = crService.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsFalse(result.Key);
            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void Service_IntegrationTest_GetchangeRequestCategories()
        {
            //Arrange
            var mockChangeRequestCategories = MockRepository.GenerateMock<ICRManager>();
            var changeRequestCategories = new List<Enum_CRCategory>
            {
                new Enum_CRCategory { Pk_EnumCRCategory = 1, Code = "CR", Description = "Change Request" },
                new Enum_CRCategory { Pk_EnumCRCategory = 2, Code = "PCR", Description = "Pack Change Request" },
            };
            mockChangeRequestCategories.Stub(x => x.GetChangeRequestCategories(Arg<int>.Is.Anything)).Return(changeRequestCategories);
            ManagerFactory.Container.RegisterInstance(typeof(ICRManager), mockChangeRequestCategories);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            //Act
            var crCategoryService = new CRService();
            var result = crCategoryService.GetChangeRequestCategories(personID);
            //Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("CR", result.Value[0].Code);
            Assert.AreEqual("PCR", result.Value[1].Code);
        }

        #endregion
    }
}
