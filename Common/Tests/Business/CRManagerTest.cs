using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Etsi.Ultimate.Tests.FakeSets;
using System.Data.Entity;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
    [Category("CR Tests")]
    public class CrManagerTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;
        private const int personID = 0;
        private const string CACHE_KEY = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";
        private int specificationId = 136080;
        private int changeRequestId = 1;

        #endregion

        #region Tests

        [Test]
        public void Business_CreateChangeRequest_Success()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCRRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Business_CreateChangeRequest_Failure()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCRRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything)).Throw(new System.Exception());
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Business_GenerateCrNumberBySpecificationId()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            changeRequest.Fk_Specification = specificationId;
            var crManager = new ChangeRequestManager();
            crManager.UoW = UoW;
            //Act
            var result = crManager.GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
            //Assert
            Assert.AreNotSame("AC144", result);
        }
       
        [Test]
        public void GetChangeRequestById()
        {
            var mockChangeRequestRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockChangeRequestRepository);
            //Act Useing Effort
            var crManager = new ChangeRequestManager();
            crManager.UoW = UoW;
            var result = UoW.Context.ChangeRequests.Where(x => x.Pk_ChangeRequest == 6).FirstOrDefault();
            //Assert
            Assert.AreEqual(6, result.Pk_ChangeRequest);
            Assert.AreEqual("AC0144", result.CRNumber);
        }
        #endregion
      
    }
}
