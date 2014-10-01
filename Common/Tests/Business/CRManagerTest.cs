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
using log4net.Appender;
using System.IO;
using log4net.Core;

namespace Etsi.Ultimate.Tests.Business
{
    [Category("CR Tests")]
    public class CrManagerTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;
        private const int personID = 0;     
        private const int alphaNumericalAllocatedSpecId = 136080;
        private const int oneAllocatedCrSpecId = 136081;
        private int changeRequestId = 1;
        private IDbSet<ChangeRequest> changeRequest;
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
            memoryAppender = ((log4net.Core.LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
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
            mockCRRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything)).Throw(new System.Exception("Test Exception Raised"));
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.CreateChangeRequest(personID, changeRequest);
            LoggingEvent[] events = memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().Contains("Test Exception Raised"));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [TestCase(87541, "0001", Description = "Numbering system returns 0001 when there is no CR")]
        [TestCase(oneAllocatedCrSpecId, "0002", Description = "Numbering system must return next available number (here 0002)")]
        [TestCase(alphaNumericalAllocatedSpecId, "0001", Description = "System should allocate CR#0001 even if there are already some alphanumeric allocated numbers")]
        public void GenerateCrNumberReturns0001WhenNoExistingCr(int specId, string expectedCrNumber)
        { 
            var crManager = new ChangeRequestManager() { UoW = UoW };
        
            var result = crManager.GenerateCrNumberBySpecificationId(specId);
            Assert.AreEqual(expectedCrNumber, result);
        }

        [Test]
        public void Business_EditChangeRequest_Success()
        {
            //Arrange
            var uiChangeRequest = new ChangeRequest() { Pk_ChangeRequest = 1, CRNumber = "234" };
            var dbChangeRequest = new ChangeRequest() { Pk_ChangeRequest = 1, CRNumber = "432" };
            var mockCRRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCRRepository.Stub(x => x.Find(1)).Return(dbChangeRequest);
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.EditChangeRequest(personID, uiChangeRequest);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(uiChangeRequest.CRNumber, dbChangeRequest.CRNumber);
        }

        [Test]
        public void Business_EditChangeRequest_Failure()
        {
            //Arrange
            var uiChangeRequest = new ChangeRequest() { Pk_ChangeRequest = 1, CRNumber = "234" };

            //Act
            var crManager = new ChangeRequestManager();
            crManager.UoW = null; //Set context null to raise object reference exception
            var result = crManager.EditChangeRequest(personID, uiChangeRequest);
            LoggingEvent[] events = memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(result);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, events.Length);            
            Assert.IsTrue(events[0].MessageObject.ToString().Contains("Object reference not set to an instance of an object"));
            Assert.AreEqual(Level.Error, events[0].Level);
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
