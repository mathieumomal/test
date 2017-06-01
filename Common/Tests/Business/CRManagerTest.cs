using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Utils.Core;
using System;
using log4net.Appender;
using System.IO;
using log4net.Core;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Business
{
    [Category("CR Tests")]
    public class CrManagerTest : BaseEffortTest
    {
        #region Constants
        private const int PersonId = 0;
        private const int AlphaNumericalAllocatedSpecId = 136080;
        private const int OneAllocatedCrSpecId = 136081;
        MemoryAppender _memoryAppender;
        #endregion

        #region Logger SetUp

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var configFileName = Directory.GetCurrentDirectory() + "\\TestData\\LogManager\\Test.log4net.config";
            LogManager.SetConfiguration(configFileName, "TestLogger");
            _memoryAppender = ((LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            if(_memoryAppender != null)
                _memoryAppender.Clear();
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
            var changeRequest = new ChangeRequest();
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCrRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var response = crManager.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsTrue(response.Result);
        }

        [TestCase(true, null, null, Description = "System should not assign cr number & revision by itself")]
        [TestCase(false, "BBBB", 5, Description = "System should assign cr number & revision by itself")]
        public void Business_CreateChangeRequest_WithAutoNumbering_Feature(bool isAutoNumberingOff, string crNumber, int? revision)
        {
            var cr22 = UoW.Context.ChangeRequests.Find(22);
            //Arrange
            var changeRequest = new ChangeRequest() { IsAutoNumberingOff = isAutoNumberingOff, RevisionOf = cr22.ChangeRequestTsgDatas.First().TSGTdoc};
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCrRepository.Stub(x => x.GetChangeRequestByContributionUID(cr22.ChangeRequestTsgDatas.First().TSGTdoc)).Return(cr22);
            mockCrRepository.Stub(x => x.FindCrMaxRevisionBySpecificationIdAndCrNumber(cr22.Fk_Specification, cr22.CRNumber)).Return(4);
            mockCrRepository.Expect(x => x.InsertOrUpdate(Arg<ChangeRequest>.Matches(y => y.IsAutoNumberingOff == isAutoNumberingOff
                                                                                       && y.CRNumber == crNumber
                                                                                       && y.Revision == revision
                                                                                       && y.CreationDate == null)));//Creation date will be automaticaly setted by DB trigger
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);

            //Act
            var crManager = new ChangeRequestManager() { UoW = UoW };
            var response = crManager.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsTrue(response.Result);
            mockCrRepository.VerifyAllExpectations();
        }

        /**
         * Here test on TSG CR but same simple logic for WG CRs
         * */
        [TestCase(false, Description = "Revised CR without status :     System should set revised CR status to Revised")]
        [TestCase(true, Description = "Revised CR with status :         System should set revised CR status to Revised")]
        public void Business_CreateChangeRequest_Revision(bool revisedCrShouldHaveStatus)
        {
            var cr22 = UoW.Context.ChangeRequests.Find(22);
            if (revisedCrShouldHaveStatus)
            {
                cr22.ChangeRequestTsgDatas.First().Fk_TsgStatus = 1;
                UoW.Save();
            }

            //Arrange
            var changeRequest = new ChangeRequest { IsAutoNumberingOff = false, RevisionOf = cr22.ChangeRequestTsgDatas.First().TSGTdoc };
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCrRepository.Stub(x => x.GetChangeRequestByContributionUID(cr22.ChangeRequestTsgDatas.First().TSGTdoc)).Return(cr22);
            mockCrRepository.Stub(x => x.FindCrMaxRevisionBySpecificationIdAndCrNumber(cr22.Fk_Specification, cr22.CRNumber)).Return(4);
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);

            //Act
            var crManager = new ChangeRequestManager { UoW = UoW };
            var response = crManager.CreateChangeRequest(PersonId, changeRequest);

            //Assert
            Assert.IsTrue(response.Result);
            Assert.AreEqual(6, cr22.ChangeRequestTsgDatas.First().Fk_TsgStatus);//Revised status id : 6
        }

        [Test]
        public void Business_CreateChangeRequest_Failure()
        {
            //Arrange
            var changeRequest = new ChangeRequest();
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCrRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything)).Throw(new Exception("Test Exception Raised"));
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var response = crManager.CreateChangeRequest(PersonId, changeRequest);
            var events = _memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(response.Result);
            Assert.AreEqual(4, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().StartsWith("### EXCEPTION ###     CLASS: ChangeRequestManager, METHOD: CreateChangeRequest, MESSAGE: "));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test]
        public void Business_CreateChangeRequest_ReturnsError_IfSpecCrNumberCrRevAlreadyExist()
        {
            var changeRequest = new ChangeRequest();
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            var crs = new List<ChangeRequest>{ new ChangeRequest { Fk_Specification = 12, CRNumber="0001", Revision=null }};
            mockCrRepository.Stub(x => x.All).Return(crs.AsQueryable());
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);
            
            var crManager = new ChangeRequestManager { UoW = UoW };
            var result = crManager.CreateChangeRequest(PersonId, new ChangeRequest { Fk_Specification = 12, CRNumber = "0001", Revision = null });
            Assert.IsFalse(result.Result);
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(string.Format(Localization.ChangeRequest_Create_AlreadyExists, "0001", "none"), result.Report.ErrorList.First());
        }

        [TestCase(87541, "0001", Description = "Numbering system returns 0001 when there is no CR")]
        [TestCase(OneAllocatedCrSpecId, "0002", Description = "Numbering system must return next available number (here 0002)")]
        [TestCase(136085, "0001", Description = "System should allocate CR#0001 even if there are already some alphanumeric allocated numbers")]
        public void GenerateCrNumberReturns0001WhenNoExistingCr(int specId, string expectedCrNumber)
        {
            var crManager = new ChangeRequestManager { UoW = UoW };

            var result = crManager.GenerateCrNumberBySpecificationId(specId);
            Assert.AreEqual(expectedCrNumber, result);
        }

        [Test]
        public void Business_EditChangeRequest_Success()
        {
            //Arrange
            var uiChangeRequest = new ChangeRequest { Pk_ChangeRequest = 1, CRNumber = "234" };
            var dbChangeRequest = new ChangeRequest { Pk_ChangeRequest = 1, CRNumber = "432" };
            var mockCrRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCrRepository.Stub(x => x.Find(1)).Return(dbChangeRequest);
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCrRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.EditChangeRequest(PersonId, uiChangeRequest);

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(uiChangeRequest.CRNumber, dbChangeRequest.CRNumber);
        }

        [Test]
        public void Business_EditChangeRequest_Failure()
        {
            //Arrange
            var uiChangeRequest = new ChangeRequest { Pk_ChangeRequest = 1, CRNumber = "234" };

            //Act
            var crManager = new ChangeRequestManager { UoW = null };
            var result = crManager.EditChangeRequest(PersonId, uiChangeRequest);
            var events = _memoryAppender.GetEvents();

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(4, events.Length);
            Assert.IsTrue(events[0].MessageObject.ToString().StartsWith("### EXCEPTION ###     CLASS: ChangeRequestManager, METHOD: EditChangeRequest, MESSAGE: "));
            Assert.AreEqual(Level.Error, events[0].Level);
        }

        [Test, Description("Retrieve CR using TDoc(Contribution Uid)")]
        public void Business_GetChangeRequestByContributionUid()
        {
            const string uid = "TSG1";
            var crManager = new ChangeRequestManager { UoW = UoW };
            var cr = crManager.GetChangeRequestByContributionUid(uid);

            Assert.IsNotNull(cr);
            Assert.AreEqual(cr.ChangeRequestTsgDatas.First().TSGTdoc, uid);
        }

        [Test, Description("Retrieve CR using TDoc(Contribution Uid)")]
        public void Business_GetChangeRequestListByContributionUidList()
        {
            var uids = new List<string> { "TSG1", "Change request description6" };
            var crManager = new ChangeRequestManager { UoW = UoW };
            var crList = crManager.GetChangeRequestListByContributionUidList(uids);

            Assert.IsNotNull(crList);
            if (crList != null)
            {
                //First
                Assert.IsNotNull(crList[0]);
                Assert.AreEqual(crList[0].ChangeRequestTsgDatas.First().TSGTdoc, uids[0]);
                //Second
                Assert.IsNotNull(crList[1]);
                Assert.AreEqual(crList[1].ChangeRequestTsgDatas.First().TSGTdoc, uids[1]);
            }

        }

        [Test, Description("Checking numeric number")]
        public void Business_GenerateNumericCrNumberBySpecificationId()
        {
            //Arrange
            var changeRequest = new ChangeRequest { Fk_Specification = 136081 };
            var crManager = new ChangeRequestManager { UoW = UoW };
            //Act
            var result = crManager.GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
            //Assert
            Assert.AreEqual("0002", result);
        }

        // --- GetCRId ---
        [Test]
        [TestCase(1, "0001")]
        [TestCase(2, "A002")]
        [TestCase(3, "A0012")]
        public void GetChangeRequestById_Success(int changeRequestId, string crNumber)
        {
            var crManager = new ChangeRequestManager { UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>() };
            var result = crManager.GetChangeRequestById(PersonId, changeRequestId);
            //Assert
            Assert.AreEqual(crNumber, result.CRNumber);
        }
        [Test, Description("Test if we try to get a CR which doesn't exist. We expect a null change request object.")]
        public void GetChangeRequestById_Failure()
        {
            var crManager = new ChangeRequestManager { UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>() };
            var result = crManager.GetChangeRequestById(PersonId, 100);
            //Assert
            Assert.IsNull(result);
        }
        #endregion
    }
}
