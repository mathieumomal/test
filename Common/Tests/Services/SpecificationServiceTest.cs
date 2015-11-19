using Etsi.Ultimate.Business.Specifications.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using System.Data.Entity;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using System.IO;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecificationServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;

        public override void SetUp()
        {
            base.SetUp();
            _editSpecInstance = null;

            //Spec rapporteur repo mock
            var rapporteurRepoMock = MockRepository.GenerateMock<ISpecificationRapporteurRepository>();
            rapporteurRepoMock.Stub(x => x.FindBySpecId(Arg<int>.Is.Anything)).Return(new List<SpecificationRapporteur>());
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecificationRapporteurRepository), rapporteurRepoMock);
        }

        [Test, Description("Get specifications by their ids, one of spec requested don't exist (id : 40)")]
        public void GetSpecifications_NominalCase()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)GetSpecs());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specSvc = new SpecificationService();
            var specifications = specSvc.GetSpecifications(0, new List<int> { 1, 2, 40}).Key;
            Assert.AreEqual(2, specifications.Count);
            Assert.AreEqual("01.01", specifications.FirstOrDefault(x => x.Pk_SpecificationId == 1).Number);
            Assert.AreEqual("12.123", specifications.FirstOrDefault(x => x.Pk_SpecificationId == 2).Number);
        }

        [Test, TestCaseSource("SpecificationData")]
        public void GetSpecificationDetailsById(SpecificationFakeDBSet specificationData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            userRights.AddRight(Enum_UserRights.Specification_Delete);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
            Assert.IsTrue(mockRightsManager.GetRights(0).HasRight(Enum_UserRights.Specification_Withdraw));

            Community c = new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" };
            Community c1 = new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" };
            Community c2 = new Community() { TbId = 3, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S2", TbName = "3GPP SA 2" };

            List<Community> communitiesSet = new List<Community>() { c, c1, c2 };
            //Mock Comminities
            var mockCommunitiesManager = MockRepository.GenerateMock<ICommunityManager>();
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(1)).Return(c);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(2)).Return(c1);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(3)).Return(c2);
            mockCommunitiesManager.Stub(x => x.GetCommunities()).Return(communitiesSet);

            Enum_Technology e = new Enum_Technology() { Pk_Enum_TechnologyId = 1, Code = "2G", Description = "2G" };
            Enum_Technology e1 = new Enum_Technology() { Pk_Enum_TechnologyId = 2, Code = "3G", Description = "3G" };
            Enum_Technology e2 = new Enum_Technology() { Pk_Enum_TechnologyId = 3, Code = "LTE", Description = "LTE" };
            List<Enum_Technology> techSet = new List<Enum_Technology>() { e, e1, e2 };

            var mockTechnologiesManager = MockRepository.GenerateMock<ISpecificationTechnologiesManager>();
            mockTechnologiesManager.Stub(x => x.GetASpecificationTechnologiesBySpecId(1)).Return(techSet);


            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specificationData);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            Enum_TechnologiesFakeRepository fakeRepo2 = new Enum_TechnologiesFakeRepository();
            mockDataContext.Stub(x => x.SpecificationTechnologies).Return((IDbSet<SpecificationTechnology>)fakeRepo2.All).Repeat.Once();
            SpecificationWIFakeRepository fakeRepo3 = new SpecificationWIFakeRepository();
            mockDataContext.Stub(x => x.Specification_WorkItem).Return((IDbSet<Specification_WorkItem>)fakeRepo3.All).Repeat.Once();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)GetWorkItems());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            ManagerFactory.Container.RegisterInstance(typeof(ICommunityManager), mockCommunitiesManager);
            ManagerFactory.Container.RegisterInstance(typeof(ISpecificationTechnologiesManager), mockTechnologiesManager);

            var mockReleaseService = MockRepository.GenerateMock<IReleaseManager>();
            mockReleaseService.Stub(x => x.GetReleasesLinkedToASpec(Arg<int>.Is.Anything)).Return(new List<Release> ());
            ManagerFactory.Container.RegisterInstance(typeof (IReleaseManager), mockReleaseService);

            var specSVC = new SpecificationService();
            KeyValuePair<Specification, UserRightsContainer> result = specSVC.GetSpecificationDetailsById(0, 1);
            Assert.IsFalse(result.Value.HasRight(Enum_UserRights.Specification_Withdraw));
            Assert.IsFalse(result.Value.HasRight(Enum_UserRights.Specification_Delete));
            Assert.AreEqual("Withdrawn before change control", result.Key.Status);
            Assert.AreEqual("3GPP SA", result.Key.PrimeResponsibleGroupFullName);
            Assert.AreEqual("3GPP SA 1,3GPP SA 2", result.Key.SecondaryResponsibleGroupsFullNames);
            Assert.AreEqual("3GPP SA 1,3GPP SA 2", result.Key.SecondaryResponsibleGroupsFullNames);
            Assert.AreEqual(1, result.Key.SpecificationParents.Count);
            Assert.AreEqual(1, result.Key.SpecificationChilds.Count);
            Assert.AreEqual("Stage 1 for RAN Sharing Enhancements", result.Key.SpecificationWIsList.FirstOrDefault().Name);
        }

        [Test, TestCaseSource("SpecificationData")]
        public void GetSpecificationBySearchCriteria(SpecificationFakeDBSet specificationData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            Community c = new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" };
            Community c1 = new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" };
            Community c2 = new Community() { TbId = 3, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S2", TbName = "3GPP SA 2" };

            List<Community> communitiesSet = new List<Community>() { c, c1, c2 };
            //Mock Comminities
            var mockCommunitiesManager = MockRepository.GenerateMock<ICommunityManager>();
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(1)).Return(c);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(2)).Return(c1);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(3)).Return(c2);
            mockCommunitiesManager.Stub(x => x.GetCommunities()).Return(communitiesSet);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specificationData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            ManagerFactory.Container.RegisterInstance(typeof(ICommunityManager), mockCommunitiesManager);

            SpecificationSearch searchObject = new SpecificationSearch()
            {
                Title= "First specification",
                Order = SpecificationSearch.SpecificationOrder.TitleDesc,
                PageSize = 1
            };
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            Assert.AreEqual(1, specSVC.GetSpecificationBySearchCriteria(0, searchObject).Key.Key.Count);

        }

        [Test, TestCaseSource("SpecificationData")]
        public void GetSpecificationBySearchText(SpecificationFakeDBSet specificationData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            Community c = new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" };
            Community c1 = new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" };
            Community c2 = new Community() { TbId = 3, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S2", TbName = "3GPP SA 2" };

            List<Community> communitiesSet = new List<Community>() { c, c1, c2 };
            //Mock Comminities
            var mockCommunitiesManager = MockRepository.GenerateMock<ICommunityManager>();
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(1)).Return(c);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(2)).Return(c1);
            mockCommunitiesManager.Stub(x => x.GetCommmunityById(3)).Return(c2);
            mockCommunitiesManager.Stub(x => x.GetCommunities()).Return(communitiesSet);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specificationData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            ManagerFactory.Container.RegisterInstance(typeof(ICommunityManager), mockCommunitiesManager);

           
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            Assert.AreEqual(1, specSVC.GetSpecificationBySearchCriteria(0, "specification").Count);
            Assert.AreEqual(0, specSVC.GetSpecificationBySearchCriteriaWithExclusion(0, "specification", new List<string>() { "00.01U" }).Count);

        }

        [Test]
        public void GetSpecificationsByNumbers()
        {
            var specData = new SpecificationFakeDBSet();
            specData.Add(new Specification { Pk_SpecificationId = 1, Number = "00.01", Title = "First specification" });
            specData.Add(new Specification { Pk_SpecificationId = 2, Number = "00.02", Title = "Second specification" });
            specData.Add(new Specification { Pk_SpecificationId = 3, Number = "00.03", Title = "Third specification" });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specService = new SpecificationService();
            var specNumbers = new List<string> { "00.01", "00.03" };
            var result = specService.GetSpecificationsByNumbers(0, specNumbers);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Pk_SpecificationId);
            Assert.AreEqual("00.01", result[0].Number);
            Assert.AreEqual("First specification", result[0].Title);
            Assert.AreEqual(3, result[1].Pk_SpecificationId);
            Assert.AreEqual("00.03", result[1].Number);
            Assert.AreEqual("Third specification", result[1].Title);
        }

        [Test]
        public void SpecificationInhibitPromote_Nominal()
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_InhibitPromote);
            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
           
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            Assert.True(specSVC.SpecificationInhibitPromote(0, 1));            
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void SpecificationInhibitPromote_MissingRights()
        {
            UserRightsContainer userRights = new UserRightsContainer();            
            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            Assert.False(specSVC.SpecificationInhibitPromote(0, 1));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test]
        public void SpecificationRemoveInhibitPromote_MissingRights()
        {
            UserRightsContainer userRights = new UserRightsContainer();
            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);

            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            Assert.False(specSVC.SpecificationRemoveInhibitPromote(0, 1));            
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test, TestCaseSource("GetSpecificicationNumbersTestFormat")]
        public void TestCheckFormatNumberIsValid(string specNumber, bool expectResult, int messageCount)
        {
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var results = specSvc.CheckFormatNumber(specNumber.ToString());

            Assert.AreEqual(expectResult, results.Key);
            if (!results.Key)
            {
                Assert.AreEqual(messageCount, results.Value.Count());
            }
        }

      
        [Test]
        [TestCase("01.05", false, 1)]
        [TestCase("01.01", false, 1)]
        [TestCase("03.98", true, 0)]
        public void TestCheckNumberAlreadyExist(string specNumber, bool expectResult, int messageCount)
        {
            var repo = MockRepository.GenerateMock<ISpecificationRepository>();
            repo.Stub(c => c.All).Return(GetSpecs());
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(repo);

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var results = specSvc.LookForNumber(specNumber.ToString());

            Assert.AreEqual(expectResult, results.Key);
            if (!results.Key)
            {
                Assert.AreEqual(messageCount, results.Value.Count());
            }
        }
        

        [Test]
        public void ExportSpecificationList_Nominal()
        {
            RegisterAllMocks();

            if (!Directory.Exists(Utils.ConfigVariables.DefaultPublicTmpPath))
                Directory.CreateDirectory(Utils.ConfigVariables.DefaultPublicTmpPath);

            var specList = new List<Specification>() 
            {
                new Specification() { 
                    Number = "29.253", 
                    IsTS = false, 
                    Title = "Test", 
                    IsActive = false, 
                    IsUnderChangeControl = true, 
                    IsForPublication = true,
                    ComIMS = false,
                    SpecificationTechnologies = new List<SpecificationTechnology>(){
                        new SpecificationTechnology() { Enum_Technology = new Enum_Technology() { Code="2G", Description="2G" } },
                        new SpecificationTechnology() { Enum_Technology = new Enum_Technology() { Code="LTE", Description="LTE"} },
                    },
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>() {
                        new SpecificationResponsibleGroup() { IsPrime = true, Fk_commityId = 1 },
                    },
                    SpecificationRapporteurs = new List<SpecificationRapporteur>() {
                        new SpecificationRapporteur() { IsPrime = true, Fk_RapporteurId = 1 },
                    }
                }
            };
            var pair = new KeyValuePair<List<Specification>, int>(specList, 1);

            var communityMgr = MockRepository.GenerateMock<ICommunityManager>();
            communityMgr.Stub(m => m.GetCommmunityshortNameById(1)).Return("S1");
            ManagerFactory.Container.RegisterInstance<ICommunityManager>(communityMgr);
            

            var repo = MockRepository.GenerateMock<ISpecificationRepository>();
            repo.Stub(r => r.GetSpecificationBySearchCriteria(Arg<SpecificationSearch>.Matches(s => s.PageSize == 0), Arg<bool>.Is.Anything)).Return( pair );
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(repo);

            var svc = new SpecificationService();
            var result = svc.ExportSpecification(EDIT_RIGHT_USER,new SpecificationSearch() { PageSize = 1 }, "BASEURL");
            Assert.IsTrue(result.Contains(Utils.ConfigVariables.DefaultPublicTmpAddress));
            Assert.IsTrue(result.Contains("SpecificationList"));
        }

        [TestCase(1, 1, 1)]
        public void PromoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            userRights.AddRight(Enum_UserRights.Specification_Promote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Initial Assert
            var spec = specDBSet.Find(specificationId);
            Assert.AreEqual(1, spec.Specification_Release.Count);

            //Act
            bool isSuccess = specificationService.PromoteSpecification(personId, specificationId, currentReleaseId);

            //Assert
            Assert.AreEqual(2, spec.Specification_Release.Count);
            var newSpecRelease = spec.Specification_Release.ToList().Where(x => x.Pk_Specification_ReleaseId == default(int)).FirstOrDefault();
            Assert.AreEqual(false, newSpecRelease.isWithdrawn);
            Assert.AreEqual(2, newSpecRelease.Fk_ReleaseId);
            Assert.IsNotNull(newSpecRelease.CreationDate);
            Assert.IsNotNull(newSpecRelease.UpdateDate);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [TestCase(1, 1, 1)]
        public void PromoteSpecificationWithoutRights(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Act
            bool isSuccess = specificationService.PromoteSpecification(personId, specificationId, currentReleaseId);

            //Assert
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [TestCase(1, 1, 2)]
        public void DemoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            userRights.AddRight(Enum_UserRights.Specification_Demote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications2();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Initial Assert
            var spec = specDBSet.Find(specificationId);
            Assert.AreEqual(1, spec.Specification_Release.Count);

            //Act
            bool isSuccess = specificationService.DemoteSpecification(personId, specificationId, currentReleaseId);

            //Assert
            Assert.AreEqual(2, spec.Specification_Release.Count);
            var newSpecRelease = spec.Specification_Release.ToList().Where(x => x.Pk_Specification_ReleaseId == default(int)).FirstOrDefault();
            Assert.AreEqual(false, newSpecRelease.isWithdrawn);
            Assert.AreEqual(1, newSpecRelease.Fk_ReleaseId);
            Assert.IsNotNull(newSpecRelease.CreationDate);
            Assert.IsNotNull(newSpecRelease.UpdateDate);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [TestCase(1, 1, 1)]
        public void DemoteSpecificationWithoutRights(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications2();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Act
            bool isSuccess = specificationService.DemoteSpecification(personId, specificationId, currentReleaseId);

            //Assert
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test]
        public void PerformMassivePromotion()
        {
            
            //Arrange
            //Params
            int personId = 1;
            
            int targetReleaseId = 2;
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_BulkPromote);            
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecificationsForMassivePromote();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Action
            bool result = specificationService.PerformMassivePromotion(personId, specDBSet.ToList(), targetReleaseId);

            //Assert
            Assert.IsTrue(result);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void PerformMassivePromotion_Exception()
        {

            //Arrange
            //Params
            int personId = 1;

            int targetReleaseId = 2;
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_BulkPromote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecificationsForMassivePromote();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var specificationService = new SpecificationService();

            //Action
            bool result = specificationService.PerformMassivePromotion(personId, specDBSet.ToList(), targetReleaseId);

            //Assert
            Assert.IsFalse(result);
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());
        }

        [Test]
        public void PerformMassivePromotionWithVersionsAllocations()
        {
            //Arrange
            //Params
            int personId = 1;            
            int intitialReleaseId = 1;
            int targetReleaseId = 2;
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_BulkPromote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecificationsForMassivePromote();
            var specVersionDBSet = GetSpecVersions();
            var releaseDBSet = GetReleases();
            specDBSet.ToList()[0].IsNewVersionCreationEnabled = true;
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releaseDBSet);
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionDBSet);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleases());
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)GetReleaseStatus());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var versionsSvc = new SpecVersionService();
            SpecVersion allocatedSpec = versionsSvc.GetVersionsForSpecRelease(1, targetReleaseId).FirstOrDefault();
            Assert.AreEqual(20, allocatedSpec.MajorVersion);

            //Action
            var specificationService = new SpecificationService();
            specificationService.PerformMassivePromotion(personId, specDBSet.ToList(), intitialReleaseId);

            //After            
            allocatedSpec = versionsSvc.GetVersionsForSpecRelease(specDBSet.ToList()[0].Pk_SpecificationId, targetReleaseId).OrderByDescending(v => v.MajorVersion).FirstOrDefault();
            
            var targetRelease = releaseDBSet.Where(r => r.Pk_ReleaseId == targetReleaseId).FirstOrDefault();

            //Asserts
            //Assert.AreEqual(targetRelease.Version3g , allocatedSpec.MajorVersion);
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void GetSpecificationByNumber()
        {
            SpecificationFakeDBSet specFakeDBSet = new SpecificationFakeDBSet();
            specFakeDBSet.Add(new Specification() { Pk_SpecificationId = 1, Number = "23.001" });
            specFakeDBSet.Add(new Specification() { Pk_SpecificationId = 2, Number = "23.002" });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specFakeDBSet).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            SpecificationService service = new SpecificationService();
            var result = service.GetSpecificationByNumber("23.001");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Pk_SpecificationId);
            Assert.AreEqual("23.001", result.Number);

            mockDataContext.VerifyAllExpectations();
        }

        #region data
//--- Check format number
        private IEnumerable<object[]> GetSpecificicationNumbersTestFormat
        {
            get
            {
                //Good format
                yield return new object[] { "01.0", true, 0 };
                yield return new object[] { "01.00U", true, 0 };
                yield return new object[] { "42.0UU-P", true, 0 };
                yield return new object[] { "99.Uu-P-s", true, 0 };
                yield return new object[] { "00.abc-P-", true, 0 };
                yield return new object[] { "01.abc-P-s-", true, 0 };
                //Bad format
                yield return new object[] { "test", false, 1 };
                yield return new object[] { "9.A.", false, 1 };
                yield return new object[] { "9.A.-", false, 1 };
                yield return new object[] { "xy.abc-", false, 1 };
                yield return new object[] { "xy.abc-", false, 1 };
            }
        }

//--- Check spec number already exists
       
        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "01.01", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "12.123", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "01.05", IsActive = false });
            list.Add(new Specification() { Pk_SpecificationId = 4, Number = "02.72", IsActive = true });
            return list;
        }



        /// <summary>
        /// Get the WorkItem Data from csv
        /// </summary>
        public IEnumerable<SpecificationFakeDBSet> SpecificationData
        {
            get
            {
                SpecificationFakeDBSet specificationFakeDBSet = new SpecificationFakeDBSet();
                Specification parentSpec = new Specification()
                {
                    Pk_SpecificationId = 2,
                    Number = "00.02P",
                    Title = "Parent specification",
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>() { new SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId =1,Fk_SpecificationId=2, IsPrime=true, Fk_commityId=1} }
                };
                Specification ChildSpec = new Specification()
                {
                    Pk_SpecificationId = 3,
                    Number = "00.03C",
                    Title = "Child specification",
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>() { new SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId = 2, Fk_SpecificationId = 3, IsPrime = true, Fk_commityId=2 } }
                };
                specificationFakeDBSet.Add(new Specification()
                {
                    Pk_SpecificationId = 1,
                    Number = "00.01U",
                    Title = "First specification",
                    IsTS = new Nullable<bool>(true),

                    IsActive = false,
                    IsUnderChangeControl = new Nullable<bool>(false),


                    IsForPublication = new Nullable<bool>(false),
                    Remarks = new List<Remark>
                    {
                        new Remark(){
                            Pk_RemarkId =1,
                            RemarkText= "Remark1",
                            IsPublic = true,
                            CreationDate = DateTime.Now,
                            PersonName = "Author 1"
                        },
                        new Remark(){
                            Pk_RemarkId =2,
                            RemarkText= "Remark2",
                            IsPublic= true,
                            CreationDate = DateTime.Now,
                            PersonName = "Author 2"
                        }
                    },
                    SpecificationTechnologies = new List<SpecificationTechnology>()
                    {
                        new SpecificationTechnology(){
                            Pk_SpecificationTechnologyId = 1,
                            Fk_Enum_Technology = 1,
                            Fk_Specification = 1,
                            Enum_Technology = new Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 1,
                                Code= "2G",
                                Description = "Second generation"
                            }
                        },
                        new SpecificationTechnology(){
                            Pk_SpecificationTechnologyId = 2,
                            Fk_Enum_Technology = 2,
                            Fk_Specification = 1,
                            Enum_Technology = new Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 3,
                                Code= "LTE",
                                Description = "Long Term Evolution"
                            }
                        }
                    },
                    Enum_Serie = new Enum_Serie() { Pk_Enum_SerieId = 1, Code = "S1", Description = "Serie 1" },
                    ComIMS = new Nullable<bool>(true),
                    EPS = null,
                    C_2gCommon = null,
                    CreationDate = null,
                    MOD_TS = null,
                    TitleVerified = null,
                    Fk_SerieId = 1,

                    Histories = new List<History>
                    {
                        new History(){
                            Pk_HistoryId = 1,
                            PersonName = "Author 1",
                            HistoryText = "Text 1"
                        },
                        new History(){
                            Pk_HistoryId = 2,
                            PersonName = "Author 1",
                            HistoryText = "Text 2"
                        }
                    },
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>(){
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 1,
                            IsPrime = true,
                            Fk_commityId = 1,
                            Fk_SpecificationId = 1
                        },
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 2,
                            IsPrime = false,
                            Fk_commityId = 2,
                            Fk_SpecificationId = 1
                        },
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 3,
                            IsPrime = false,
                            Fk_commityId = 3,
                            Fk_SpecificationId = 1
                        }
                    },
                    SpecificationChilds = new List<Specification>() { ChildSpec },
                    SpecificationParents = new List<Specification>() { parentSpec },
                    Specification_WorkItem = new List<Specification_WorkItem>
                    {
                        new Specification_WorkItem(){ Pk_Specification_WorkItemId=1, Fk_WorkItemId=105, isPrime=true}
                    }
                });

                yield return specificationFakeDBSet;
            }
        }

        private Specification _editSpecInstance;
        private Specification GetCorrectSpecificationForEdit(bool clone)
        {
            var spec = new Specification()
                {
                    Pk_SpecificationId = 12,
                    Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OpenedReleaseId } },
                    Number = "12.123",
                    SpecificationTechnologies = new List<SpecificationTechnology>() { 
                        new SpecificationTechnology() { Pk_SpecificationTechnologyId = 11, Fk_Enum_Technology=1 }, // Let's say it's 2G
                        new SpecificationTechnology() { Pk_SpecificationTechnologyId = 12, Fk_Enum_Technology=2 }, // Let's say it's 3G
                    },
                    Remarks = new List<Remark>() {
                        new Remark() { Pk_RemarkId = 1, Fk_PersonId=12, IsPublic = true }
                    },
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>()
                    {
                        new SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId=1, Fk_commityId=1, IsPrime = true },
                        new SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId=2, Fk_commityId=2, IsPrime = false },
                    },
                    SpecificationRapporteurs = new List<SpecificationRapporteur>()
                    {
                        new SpecificationRapporteur() { Pk_SpecificationRapporteurId = 1, Fk_RapporteurId=1, IsPrime = true },
                        new SpecificationRapporteur() { Pk_SpecificationRapporteurId = 2, Fk_RapporteurId=2, IsPrime = false },
                    },
                    Specification_WorkItem = new List<Specification_WorkItem>()
                    {
                        new Specification_WorkItem() { Pk_Specification_WorkItemId = 1, Fk_SpecificationId = 12, Fk_WorkItemId = 1, isPrime = true },
                        new Specification_WorkItem() { Pk_Specification_WorkItemId = 2, Fk_SpecificationId = 12, Fk_WorkItemId = 2, isPrime = false },
                    },

                };
            
            if (clone)
            {
                return spec;
            }
            else
            {

                if (_editSpecInstance == null)
                    _editSpecInstance = spec;
                return _editSpecInstance;
                
            }
        }

        /// <summary>
        /// Get Fake Specification Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecificationFakeDBSet GetSpecifications()
        {
            var specDbSet = new SpecificationFakeDBSet();
            var release = Releases().FirstOrDefault();
            var specRelease = new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = release.Pk_ReleaseId, isWithdrawn = false, Release = release };
            var specReleaseList = new List<Specification_Release>() { specRelease };
            var specification = new Specification() { Pk_SpecificationId = 1, Number = "00.01U", Title = "First specification", IsActive = true, Specification_Release = specReleaseList };
            specDbSet.Add(specification);
            return specDbSet;
        }

        /// <summary>
        /// Get Fake Specification Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecificationFakeDBSet GetSpecifications2()
        {
            var specDbSet = new SpecificationFakeDBSet();
            var release = Releases().Last();
            var specRelease = new Specification_Release() { Pk_Specification_ReleaseId = 2, 
                Fk_SpecificationId = 1, Fk_ReleaseId = release.Pk_ReleaseId, isWithdrawn = false, Release = release };
            var specReleaseList = new List<Specification_Release>() { specRelease };
            var specification = new Specification() { Pk_SpecificationId = 1, Number = "00.01U", Title = "First specification", IsActive = true, Specification_Release = specReleaseList };
            specDbSet.Add(specification);
            return specDbSet;
        }

        // <summary>
        /// Get Fake Specification Details for massive promote
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecificationFakeDBSet GetSpecificationsForMassivePromote()
        {
            var specDbSet = new SpecificationFakeDBSet();
            var release = GetReleases().FirstOrDefault();
            var specReleaseList = new List<Specification_Release>() { 
                new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isWithdrawn = false, Release = release}
                //new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isWithdrawn = false}
            };

            var specReleaseList2 = new List<Specification_Release>() {                 
                new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 2, Fk_ReleaseId = 1, isWithdrawn = false, Release = release}
            };

            specDbSet.Add(new Specification() { Pk_SpecificationId = 1, Number = "00.01U", Title = "First specification", IsActive = true, IsUnderChangeControl= false, Specification_Release = specReleaseList });
            specDbSet.Add(new Specification() { Pk_SpecificationId = 2, Number = "00.02U", Title = "Second specification", IsActive = true, IsUnderChangeControl = false, Specification_Release = specReleaseList2 });
            return specDbSet;
        }

        private SpecVersionFakeDBSet GetSpecVersions()
        {
            var specVersionFakeDBSet = new SpecVersionFakeDBSet();
            specVersionFakeDBSet.Add(new SpecVersion()
            {
                Pk_VersionId = 1,
                Multifile = false,
                ForcePublication = false,
                Location = "Location1",
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 1,
                MajorVersion = 10,
                Release = GetReleases().FirstOrDefault(),
                Specification = GetSpecificationsForMassivePromote().ToList()[0]
            });
            specVersionFakeDBSet.Add(new SpecVersion()
            {
                Pk_VersionId = 2,
                Multifile = false,
                ForcePublication = false,
                Location = "Location2",
                Fk_ReleaseId = 2,
                Fk_SpecificationId = 1,
                MajorVersion = 20,

                Release = GetReleases().FirstOrDefault(),
                Specification = GetSpecificationsForMassivePromote().ToList()[1]
            });
            return specVersionFakeDBSet;
        }

        private ReleaseFakeDBSet GetReleases()
        {
            
            var openStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" };
            var frozenStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen" };
            var closedStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, Code = "Closed" };

            var releaseFakeDBSet = new ReleaseFakeDBSet();
            releaseFakeDBSet.Add(new Release()
            {
                Pk_ReleaseId = 1, 
                Enum_ReleaseStatus = openStatus,
                Version3g = 20,
                SortOrder = 20,

            });

            releaseFakeDBSet.Add(new Release()
            {
                Pk_ReleaseId = 2,
                Enum_ReleaseStatus = openStatus,
                Version3g = 30,
                SortOrder = 30,

            });
            return releaseFakeDBSet;
        }
        /// <summary>
        /// Get Fake Releases
        /// </summary>
        /// <returns>Queryable Release list</returns>
        private IQueryable<Release> Releases()
        {
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            return releaseFakeRepository.All;
        }

        /// <summary>
        /// Get work items
        /// </summary>
        /// <returns></returns>
        private WorkItemFakeDBSet GetWorkItems()
        {
            var workItemList = GetAllTestRecords<WorkItem>(Directory.GetCurrentDirectory() + "\\TestData\\WorkItems\\WorkItem.csv");
            WorkItemFakeDBSet workItemFakeDBSet = new WorkItemFakeDBSet();
            workItemList.ForEach(x => workItemFakeDBSet.Add(x));
            return workItemFakeDBSet;
        }

        #endregion

        private void RegisterAllMocks()
        {
            var rights = new UserRightsContainer();
            rights.AddRight(Enum_UserRights.Specification_Create);
            rights.AddRight(Enum_UserRights.Specification_EditFull);
            rights.AddRight(Enum_UserRights.Specification_View_UnAllocated_Number);
            var rights_withLimitedEdit = new UserRightsContainer();
            rights_withLimitedEdit.AddRight(Enum_UserRights.Specification_Create);
            rights_withLimitedEdit.AddRight(Enum_UserRights.Specification_EditLimitted);
            rights_withLimitedEdit.AddRight(Enum_UserRights.Specification_View_UnAllocated_Number);

            var userRights = MockRepository.GenerateMock<IRightsManager>();
            userRights.Stub(r => r.GetRights(EDIT_RIGHT_USER)).Return(rights);
            userRights.Stub(r => r.GetRights(EDIT_LIMITED_RIGHT_USER)).Return(rights_withLimitedEdit);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);


            // Repository: set up the attribution of the Pk
            var repo = MockRepository.GenerateMock<ISpecificationRepository>();
            repo.Expect(r => r.InsertOrUpdate(Arg<Specification>.Is.Anything));
            repo.Stub(r => r.GetSeries()).Return(new List<Enum_Serie>() { new Enum_Serie() { Pk_Enum_SerieId = 1, Code = "SER_12", Description = "Serie12" } });
            repo.Stub(r => r.Find(12)).Return(GetCorrectSpecificationForEdit(false));
            repo.Stub(r => r.All).Return(GetSpecs());
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(repo);

            var communityManager = MockRepository.GenerateMock<ICommunityManager>();
            communityManager.Stub(c => c.GetCommunities()).Return(
                new List<Community>() {
                    new Community() { TbId = 1, TbName ="RAN 1", ShortName="R1" },
                    new Community() { TbId = 3, TbName ="RAN 2" },
                });
            ManagerFactory.Container.RegisterInstance<ICommunityManager>(communityManager);

            var personManager = MockRepository.GenerateMock<IPersonManager>();
            var user1 = new View_Persons() { PERSON_ID = 1, FIRSTNAME = "User", LASTNAME = "1" };
            personManager.Stub(p => p.FindPerson(1)).Return(user1);
            personManager.Stub(p => p.FindPerson(3)).Return(new View_Persons() { PERSON_ID = 3, FIRSTNAME = "User", LASTNAME = "3" });
            personManager.Stub(p => p.FindPerson(4)).Return(new View_Persons() { PERSON_ID = 4, FIRSTNAME = "User", LASTNAME = "4" });
            personManager.Stub(p => p.GetByIds(Arg<List<int>>.Matches(x => x.Count == 1 && x.First() == 1))).Return(new List<View_Persons>() { user1 });
            ManagerFactory.Container.RegisterInstance<IPersonManager>(personManager);

            // Need a release repository
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Mock the UoW
            var uow = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            uow.Expect(r => r.Save());
            RepositoryFactory.Container.RegisterInstance<IUltimateUnitOfWork>(uow);


        }

        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 3, Fk_ReleaseId = 2, isTranpositionForced = true });
            return list;
        }

        private IDbSet<Enum_ReleaseStatus> GetReleaseStatus()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen", Description = "Frozen" });
            return list;
        }

        

    }

}
