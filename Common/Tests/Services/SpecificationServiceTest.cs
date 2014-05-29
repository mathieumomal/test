using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.ModelMails;
using System.IO;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecificationServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;

        public override void Setup()
        {
            base.Setup();
            _editSpecInstance = null;
        }

        [Test, TestCaseSource("SpecificationData")]
        public void GetSpecificationDetailsById(SpecificationFakeDBSet specificationData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);

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
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(1)).Return(c.ShortName);
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(2)).Return(c1.ShortName);
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(3)).Return(c2.ShortName);
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
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            ManagerFactory.Container.RegisterInstance(typeof(ICommunityManager), mockCommunitiesManager);
            ManagerFactory.Container.RegisterInstance(typeof(ISpecificationTechnologiesManager), mockTechnologiesManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            KeyValuePair<Specification, UserRightsContainer> result = specSVC.GetSpecificationDetailsById(0, 1);
            Assert.IsFalse(result.Value.HasRight(Enum_UserRights.Specification_Withdraw));
            Assert.AreEqual("Withdrawn before change control", result.Key.Status);
            Assert.AreEqual("SP", result.Key.PrimeResponsibleGroupShortName);
            Assert.AreEqual("S1,S2", result.Key.SecondaryResponsibleGroupsShortNames);
            Assert.AreEqual("S1,S2", result.Key.SecondaryResponsibleGroupsShortNames);
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

        [Test, TestCaseSource("GetSpecificicationNumbersTestAlreadyExists")]
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
            repo.Stub(r => r.GetSpecificationBySearchCriteria(Arg<SpecificationSearch>.Matches(s => s.PageSize == 0))).Return( pair );
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(repo);

            var svc = new SpecificationService();
            var result = svc.ExportSpecification(EDIT_RIGHT_USER,new SpecificationSearch() { PageSize = 1 });
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
            userRights.AddRight(Enum_UserRights.Specification_InhibitPromote);
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
        private IEnumerable<object[]> GetSpecificicationNumbersTestAlreadyExists
        {
            get
            {
                //Already exist
                yield return new object[] { "01.05", true, 0 };
                yield return new object[] { "01.01", false, 1 };
                yield return new object[] { "03.98", true, 0 };
            }
        }
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
                //var specificationList = GetAllTestRecords<Spec>(Directory.GetCurrentDirectory() + "\\TestData\\WorkItems\\WorkItem.csv");
                SpecificationFakeDBSet specificationFakeDBSet = new SpecificationFakeDBSet();
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
                    Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
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
        /// Get Fake Releases
        /// </summary>
        /// <returns>Queryable Release list</returns>
        private IQueryable<Release> Releases()
        {
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            return releaseFakeRepository.All;
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
            personManager.Stub(p => p.FindPerson(1)).Return(new View_Persons() { PERSON_ID = 1, FIRSTNAME = "User", LASTNAME = "1" });
            personManager.Stub(p => p.FindPerson(3)).Return(new View_Persons() { PERSON_ID = 3, FIRSTNAME = "User", LASTNAME = "3" });
            personManager.Stub(p => p.FindPerson(4)).Return(new View_Persons() { PERSON_ID = 4, FIRSTNAME = "User", LASTNAME = "4" });
            ManagerFactory.Container.RegisterInstance<IPersonManager>(personManager);

            // Need a release repository
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Mock the UoW
            var uow = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            uow.Expect(r => r.Save());
            RepositoryFactory.Container.RegisterInstance<IUltimateUnitOfWork>(uow);


        }

        

    }

}
