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
using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Tests.Business
{
    [Category("CR Tests")]
    public class CrManagerTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;
        private const int personID = 0;     
        private int specificationId = 136080;
        private int changeRequestId = 1;
        private IDbSet<ChangeRequest> changeRequest;

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

        [Test, Description("Checking alphanumeric number")]
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
            Assert.AreEqual("A0145", result);
        }

        [Test, Description("Checking numeric number")]
        public void Business_GenerateNumericCrNumberBySpecificationId()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            changeRequest.Fk_Specification = 136081;
            var crManager = new ChangeRequestManager();
            crManager.UoW = UoW;
            //Act
            var result = crManager.GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
            //Assert
            Assert.AreEqual("0002", result);
        }

        [Test]
        [TestCase(1, "0001")]
        [TestCase(2, "A002")]
        [TestCase(3, "A0012")]
        public void GetChangeRequestById(int changeRequestId, string crNumber)
        {

            var crManager = new ChangeRequestManager();
            crManager.UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var result = crManager.GetChangeRequestById(personID, changeRequestId);
            //Assert
            Assert.AreEqual(crNumber, result.CRNumber);
        }
        #endregion
    }
}
