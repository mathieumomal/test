﻿using NUnit.Framework;
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

namespace Etsi.Ultimate.Tests.Services
{
    class SpecificationServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;

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
        public void TestCheckNumberIsValid(string specNumber, bool expectResult, int messageCount)
        {
            var service = new SpecificationService();
            var results = service.CheckNumber(specNumber.ToString());

            Assert.AreEqual(expectResult, results.Key);
            if (!results.Key)
            {
                Assert.AreEqual(messageCount, results.Value.Count());
            }
        }

        [Test]
        public void CreateSpecification_ReturnsErrorIfExistingPk()
        {
            var specification = new Specification() { Pk_SpecificationId = 14 };
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(0, specification).Key);
        }

        [Test]
        public void CreationSpecification_ReturnsErrorIfUserDoesNotHaveRight()
        {
            // Create the user rights repository.
            var userRights = MockRepository.GenerateMock<IRightsManager>();
            userRights.Stub(r => r.GetRights(NO_EDIT_RIGHT_USER)).Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);

            var specification = new Specification() { Pk_SpecificationId = 0 };
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(NO_EDIT_RIGHT_USER, specification).Key);
        }

        [Test]
        public void CreateSpecification_ReturnsErrorIfUserDidNotDefineRelease()
        {
            // Create the user rights repository.
            RegisterAllMocks();

            var specification = GetCorrectSpecificationForCreation();
            specification.Specification_Release.Clear();
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(EDIT_RIGHT_USER, specification).Key);
        }

        [Test]
        public void CreateSpecification_ReturnsErrorIfSpecNumberIsInvalid()
        {
            RegisterAllMocks();
            var specification = GetCorrectSpecificationForCreation();
            specification.Number = "12aaa";
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(EDIT_RIGHT_USER, specification).Key);
        }

        [Test]
        public void CreateSpecification_NominalCase()
        {
            // Set up the rights
            RegisterAllMocks();

            // Specific mock for the email, because we want to check the call made to it.
            /*var mailMgr = MockRepository.GenerateMock<IMailManager>();
            mailMgr.Stub(r => r.SendEmail(
                Arg<string>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<List<string>>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything
                )).Return(true);
            MailManager.Instance = mailMgr;*/


            var specification = GetCorrectSpecificationForCreation();
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var report = specSvc.CreateSpecification(EDIT_RIGHT_USER, specification).Value; // We can't change PK and check it, because it's assigned by EF in principle

            Assert.AreEqual(1, specification.Histories.Count);
            Assert.AreEqual(String.Format(Utils.Localization.History_Specification_Created, "R1"), specification.Histories.First().HistoryText);
            Assert.IsTrue(specification.IsActive);
            Assert.IsTrue(specification.IsUnderChangeControl.HasValue && !specification.IsUnderChangeControl.Value);
            Assert.IsTrue(specification.IsTS.GetValueOrDefault());
            Assert.AreEqual(1, specification.Fk_SerieId);

            Assert.AreEqual(0, report.WarningList.Count);
            Assert.AreEqual(0, report.ErrorList.Count);
        }

        [Test]
        public void EditSpecification_ReturnsFalseWhenUserHasNoRight()
        {
            RegisterAllMocks();

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var report = specSvc.EditSpecification(NO_EDIT_RIGHT_USER, GetCorrectSpecificationForEdit(false));
            Assert.IsFalse(report.Key);
            Assert.AreEqual(1, report.Value.GetNumberOfErrors());
        }

        [Test]
        public void EditSpecification_NonExistingSpecificationReturnsError()
        {
            RegisterAllMocks();
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var spec = GetCorrectSpecificationForEdit(false);
            spec.Pk_SpecificationId = 0;
            var report = specSvc.EditSpecification(EDIT_RIGHT_USER, spec);
            Assert.IsFalse(report.Key);
            Assert.AreEqual(1, report.Value.GetNumberOfErrors());
        }


        [Test]
        public void EditSpecification_NominalCase()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);
            specToEdit.Title = "New title";
            
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit ).Key);

            // From white box testing, we know that:
            // - spec that will be modified is the one provided by the Repository
            // - we can get it via GetCorrectSpecificationForEdit(true)
            // Thus we will test on it.
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            Assert.AreEqual(specToEdit.Title, modifiedSpec.Title);
           
        }

        [Test]
        public void SpecificationEdit_TestRemarksChange()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Change remarks
            specToEdit.Remarks.First().IsPublic = false;
            specToEdit.Remarks.Add(new Remark() { IsPublic = false, Fk_PersonId = 12 });

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // Test remarks
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            Assert.AreEqual(2, modifiedSpec.Remarks.Count);
            Assert.IsFalse(modifiedSpec.Remarks.First().IsPublic.GetValueOrDefault());
        }

        [Test]
        public void SpecificationEdit_TestTechnoChange()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Change spec technology
            specToEdit.SpecificationTechnologies.Remove(specToEdit.SpecificationTechnologies.First()); // Remove 2G
            specToEdit.SpecificationTechnologies.Add(new SpecificationTechnology() { Pk_SpecificationTechnologyId = 13, Fk_Enum_Technology = 3 }); // Let's say it's LTE

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            
            // Test specification technologies
            Assert.AreEqual(3, modifiedSpec.SpecificationTechnologies.Count);
        }

        [Test]
        public void SpecificationEdit_TestResponsibleGroupsChange()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Change responsible groups
            specToEdit.SpecificationResponsibleGroups.Remove(specToEdit.SpecificationResponsibleGroups.Last()); // Remove group 2
            specToEdit.SpecificationResponsibleGroups.First().IsPrime = false;  // Remove prime on group 1
            specToEdit.SpecificationResponsibleGroups.Add(new SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId = 3, Fk_commityId = 3, IsPrime = true }); // Set prime on group 3

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);
            var modifiedSpec = GetCorrectSpecificationForEdit(false);

            // Test responsible groups
            Assert.AreEqual(3, modifiedSpec.SpecificationResponsibleGroups.Count);
            Assert.IsFalse(modifiedSpec.SpecificationResponsibleGroups.Where(g => g.Fk_commityId == 1).FirstOrDefault().IsPrime);

            var createHistoryEntry = string.Format(Utils.Localization.History_Specification_Changed_Prime_Group, "RAN 2", "RAN 1");
            Assert.AreEqual(1, modifiedSpec.Histories.Where(h => h.HistoryText == createHistoryEntry).Count());
        }

        [Test]
        public void EditSpecification_TestRapporteurChange()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Changes to rapporteur
            // Remove rapporteur 2, change rapporteur 3 to be prime rapporteur
            specToEdit.SpecificationRapporteurs.Remove(specToEdit.SpecificationRapporteurs.Last());
            specToEdit.SpecificationRapporteurs.First().IsPrime = false;
            specToEdit.SpecificationRapporteurs.Add(new SpecificationRapporteur() { Pk_SpecificationRapporteurId = 3, Fk_RapporteurId = 3, IsPrime = true });

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // From white box testing, we know that:
            // - spec that will be modified is the one provided by the Repository
            // - we can get it via GetCorrectSpecificationForEdit(true)
            // Thus we will test on it.
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            
            // Check changes to rapporteur
            Assert.AreEqual(3, modifiedSpec.SpecificationRapporteurs.Count);
            Assert.IsFalse(modifiedSpec.SpecificationRapporteurs.Where(r => r.Fk_RapporteurId == 1).FirstOrDefault().IsPrime);

            var createHistoryEntry = string.Format(Utils.Localization.History_Specification_Changed_Prime_Rapporteur, "User 3", "User 1");
            Assert.AreEqual(1, modifiedSpec.Histories.Where(h => h.HistoryText == createHistoryEntry).Count());

        }

        [Test]
        public void EditSpecification_TestWorkItemChanges()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Changes to work items
            // remove WI 2, add WI 3.
            specToEdit.Specification_WorkItem.First().isPrime = false;
            specToEdit.Specification_WorkItem.Remove(specToEdit.Specification_WorkItem.Last());
            specToEdit.Specification_WorkItem.Add(new Specification_WorkItem() { Fk_WorkItemId = 3, isPrime = true });
          
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // From white box testing, we know that:
            // - spec that will be modified is the one provided by the Repository
            // - we can get it via GetCorrectSpecificationForEdit(true)
            // Thus we will test on it.
            var modifiedSpec = GetCorrectSpecificationForEdit(false);

            // Check changes to work items
            Assert.AreEqual(3, modifiedSpec.Specification_WorkItem.Count);
            Assert.IsFalse(modifiedSpec.Specification_WorkItem.Where(r => r.Fk_WorkItemId == 1).FirstOrDefault().isPrime.GetValueOrDefault());
        }
        

        #region data
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


        private Specification GetCorrectSpecificationForCreation()
        {
            return new Specification()
            {
                Pk_SpecificationId = 0,
                Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                Number = "12.123",
            };
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
        #endregion

        private void RegisterAllMocks()
        {
            var rights = new UserRightsContainer();
            rights.AddRight(Enum_UserRights.Specification_Create);
            rights.AddRight(Enum_UserRights.Specification_EditFull);
            var userRights = MockRepository.GenerateMock<IRightsManager>();
            userRights.Stub(r => r.GetRights(EDIT_RIGHT_USER)).Return(rights);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);


            // Repository: set up the attribution of the Pk
            var repo = MockRepository.GenerateMock<ISpecificationRepository>();
            repo.Expect(r => r.InsertOrUpdate(Arg<Specification>.Is.Anything));
            repo.Stub(r => r.GetSeries()).Return(new List<Enum_Serie>() { new Enum_Serie() { Pk_Enum_SerieId = 1, Code = "SER_12", Description = "Serie12" } });
            repo.Stub(r => r.Find(12)).Return(GetCorrectSpecificationForEdit(false));
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(repo);

            var communityManager = MockRepository.GenerateMock<ICommunityManager>();
            communityManager.Stub(c => c.GetCommunities()).Return(
                new List<Community>() {
                    new Community() { TbId = 1, TbName ="RAN 1" },
                    new Community() { TbId = 3, TbName ="RAN 2" },
                });
            ManagerFactory.Container.RegisterInstance<ICommunityManager>(communityManager);

            var personManager = MockRepository.GenerateMock<IPersonManager>();
            personManager.Stub(p => p.FindPerson(1)).Return(new View_Persons() { PERSON_ID = 1, FIRSTNAME = "User", LASTNAME = "1" });
            personManager.Stub(p => p.FindPerson(3)).Return(new View_Persons() { PERSON_ID = 3, FIRSTNAME = "User", LASTNAME = "3" });
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
