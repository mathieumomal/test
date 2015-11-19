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
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.ModelMails;

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecificationEditServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;

        public override void SetUp()
        {
            base.SetUp();
            _editSpecInstance = null;
        }

        [Test]
        public void EditSpecification_ReturnsFalseWhenUserHasNoRight()
        {
            RegisterAllMocks();
            var spec = GetCorrectSpecificationForEdit(false);

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var report = specSvc.EditSpecification(NO_EDIT_RIGHT_USER, spec);
            Assert.AreEqual(-1, report.Key);
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
            Assert.AreEqual(-1, report.Key);
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
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // From white box testing, we know that:
            // - spec that will be modified is the one provided by the Repository
            // - we can get it via GetCorrectSpecificationForEdit(true)
            // Thus we will test on it.
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            Assert.AreEqual(specToEdit.Title, modifiedSpec.Title);

            Assert.IsNotNull(modifiedSpec.MOD_BY);
            Assert.IsTrue((modifiedSpec.MOD_TS.GetValueOrDefault() - DateTime.UtcNow).TotalMinutes < 1);
        }

        [Test]
        public void SpecificationEdit_TestRemarksChange()
        {
            RegisterAllMocks();

            // Get a fresh copy of the spec.
            var specToEdit = GetCorrectSpecificationForEdit(true);

            // Change remarks
            specToEdit.Remarks.First().IsPublic = false;
            specToEdit.Remarks.Add(new Remark() { IsPublic = false, Fk_PersonId = 0 });

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // Test remarks
            var modifiedSpec = GetCorrectSpecificationForEdit(false);
            Assert.AreEqual(2, modifiedSpec.Remarks.Count);
            Assert.AreEqual(EDIT_RIGHT_USER, modifiedSpec.Remarks.Last().Fk_PersonId);
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
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);
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
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);
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
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

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
            Assert.AreEqual(specToEdit.Pk_SpecificationId, specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

            // From white box testing, we know that:
            // - spec that will be modified is the one provided by the Repository
            // - we can get it via GetCorrectSpecificationForEdit(true)
            // Thus we will test on it.
            var modifiedSpec = GetCorrectSpecificationForEdit(false);

            // Check changes to work items
            Assert.AreEqual(3, modifiedSpec.Specification_WorkItem.Count);
            Assert.IsFalse(modifiedSpec.Specification_WorkItem.Where(r => r.Fk_WorkItemId == 1).FirstOrDefault().isPrime.GetValueOrDefault());
        }

        //SUCCESS
        [TestCase(false, false, EDIT_RIGHT_USER, "12.123", 0, 1)]//Number don't changed
        [TestCase(false, false, EDIT_LIMITED_RIGHT_USER, "12.123", 0, 1)]//Number don't changed
        [TestCase(true, true, EDIT_RIGHT_USER, "", 0, null)]//Number changed to null
        [TestCase(true, true, EDIT_RIGHT_USER, "23.145", 0, 2)]//Number changed
        ////ERROR
        [TestCase(true, false, EDIT_LIMITED_RIGHT_USER, "12.145", 1, null)]//Number changed but user don't have full rights
        [TestCase(true, false, EDIT_RIGHT_USER, "ijfzeo989=)", 1, null)]//Number changed with format error
        [TestCase(true, false, EDIT_RIGHT_USER, "12.189", 1, null)]//Number changed but already exist
        public void EditSpecification_EmailTest(bool shouldMailBeSent, bool shouldMailSucceed, int person, String specToUpdateNumber, int error, int? serie)
        {
            var specToEdit = GetCorrectSpecificationForEdit(true);
            specToEdit.Number = specToUpdateNumber;
            // Set up the rights
            RegisterAllMocks();
            //---MAIL
            //Specific mock for the email, because we want to check the call made to it.
            var primeResponsibleGroupCommityId = specToEdit.PrimeResponsibleGroup.Fk_commityId;

            var subject = String.Format(Localization.Specification_ReferenceNumberAssigned_Subject, specToEdit.Number);
            var body = new SpecReferenceNumberAssignedMailTemplate((String.IsNullOrEmpty(specToEdit.Number) ? "" : specToEdit.Number), (String.IsNullOrEmpty(specToEdit.Title) ? "" : specToEdit.Title), new List<string>() { });
            var bodyContent = body.TransformText();
            //Simulation send mail with test datas when we use 'SendEmail' method
            var mailMock = MockRepository.GenerateMock<IMailManager>();
            mailMock.Stub(r => r.SendEmail(
                Arg<string>.Is.Null,
                Arg<List<string>>.Matches(to => to.Count==5),
                Arg<List<string>>.Is.Null,
                Arg<List<string>>.Is.Null,
                Arg<string>.Is.Equal(subject),
                Arg<string>.Is.Anything
                )).Return(shouldMailSucceed);
            UtilsFactory.Container.RegisterInstance<IMailManager>(mailMock);
            //---MAIL

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var report = specSvc.EditSpecification(person, specToEdit).Value;

            Assert.AreEqual(0, report.WarningList.Count);
            Assert.AreEqual(error, report.ErrorList.Count);

            var modifiedSpec = GetCorrectSpecificationForEdit(false);

            if(error==0){
                Assert.AreEqual(specToUpdateNumber, modifiedSpec.Number);
                Assert.AreEqual(serie, modifiedSpec.Fk_SerieId);
            }

            if (shouldMailBeSent)
            {
                mailMock.VerifyAllExpectations();
            }
            if (shouldMailBeSent && !shouldMailSucceed)
                Assert.AreEqual(1, report.ErrorList.Count);
        }

        //Rule : default prime rapporteur
        [TestCase(1, EDIT_RIGHT_USER, "12.123", 1, 1, 1, Description = "if no prime rapporteur manually define and only one rapporteur define : this one should be prime rapporteur by default")]
        [TestCase(2, EDIT_RIGHT_USER, "12.123", 1, 0, 2, Description = "if no prime rapporteur manually define and two rapporteurs are define : no one should be prime rapporteur")]
        [TestCase(3, EDIT_RIGHT_USER, "12.123", 1, 3, 2, Description = "if prime rapporteur manually define : system should keep this one as primary rapporteur")]
        public void CreateSpecification_PrimeRapporteurByDefaultRule(int specRapporteur, int person, String number, int? serie, int primaryRapporteurId, int rapporteurCount)
        {
            var specToEdit = GetCorrectSpecificationForEdit(true);
            specToEdit.SpecificationRapporteurs = GetSpecRapporteurs(specRapporteur);
            // Set up the rights
            RegisterAllMocks();

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var response = specSvc.EditSpecification(person, specToEdit);

            Assert.AreEqual(0, response.Value.WarningList.Count);
            Assert.AreEqual(0, response.Value.ErrorList.Count);

            Assert.AreEqual(rapporteurCount, specToEdit.SpecificationRapporteurs.Count);
            if (primaryRapporteurId != 0)
            {
                Assert.AreEqual(primaryRapporteurId,
                    specToEdit.SpecificationRapporteurs.First(x => x.IsPrime).Fk_RapporteurId);
            }
            else
            {
                Assert.IsNull(specToEdit.SpecificationRapporteurs.FirstOrDefault(x => x.IsPrime));
            }
        }

        //Rule : default prime WI
        [TestCase(1, EDIT_RIGHT_USER, "12.123", 1, 1, 1, Description = "if no prime WI manually define and only one WI define : this one should be prime WI by default")]
        [TestCase(2, EDIT_RIGHT_USER, "12.123", 1, 0, 2, Description = "if no prime WI manually define and two WI are define : no one should be prime WI")]
        [TestCase(3, EDIT_RIGHT_USER, "12.123", 1, 3, 2, Description = "if prime WI manually define : system should keep this one as primary WI")]
        public void CreateSpecification_PrimeWiByDefaultRule(int specWis, int person, String number, int? serie, int primaryWiId, int wiCount)
        {
            var specToEdit = GetCorrectSpecificationForEdit(true);
            specToEdit.Specification_WorkItem = GetSpecWis(specWis);
            // Set up the rights
            RegisterAllMocks();

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var response = specSvc.EditSpecification(person, specToEdit);

            Assert.AreEqual(0, response.Value.WarningList.Count);
            Assert.AreEqual(0, response.Value.ErrorList.Count);

            Assert.AreEqual(wiCount, specToEdit.Specification_WorkItem.Count);
            if (primaryWiId != 0)
            {
                Assert.AreEqual(primaryWiId,
                    specToEdit.Specification_WorkItem.First(x => x.isPrime ?? false).Fk_WorkItemId);
            }
            else
            {
                Assert.IsNull(specToEdit.Specification_WorkItem.FirstOrDefault(x => x.isPrime ?? false));
            }
        }

        #region datas
        private List<Specification_WorkItem> GetSpecWis(int index)
        {
            switch (index)
            {
                case 1://ONE WI
                    return new List<Specification_WorkItem>
                        {
                            new Specification_WorkItem{Fk_WorkItemId = 1, isPrime = false}
                    };
                case 2://TWO WIs
                    return new List<Specification_WorkItem>
                        {
                            new Specification_WorkItem{Fk_WorkItemId = 1, isPrime = false},
                            new Specification_WorkItem{Fk_WorkItemId = 2, isPrime = false}
                    };
                case 3://TWO WIs AND ONE MANUALLY DEFINE AS PRIME WI
                    return new List<Specification_WorkItem>
                    {
                        new Specification_WorkItem {Fk_WorkItemId = 1, isPrime = false},
                        new Specification_WorkItem {Fk_WorkItemId = 3, isPrime = true}
                    };
                default:
                    return null;
            }
        }

        private List<SpecificationRapporteur> GetSpecRapporteurs(int index)
        {
            switch (index)
            {
                case 1://ONE RAPPORTEUR
                    return new List<SpecificationRapporteur>
                        {
                            new SpecificationRapporteur{Fk_RapporteurId = 1, IsPrime = false}
                    };
                case 2://TWO RAPPORTEURS
                    return  new List<SpecificationRapporteur>
                        {
                            new SpecificationRapporteur{Fk_RapporteurId = 1, IsPrime = false},
                            new SpecificationRapporteur{Fk_RapporteurId = 2, IsPrime = false}
                    };
                case 3://TWO RAPPORTEURS AND ONE MANUALLY DEFINE AS PRIME RAPPORTEUR
                    return new List<SpecificationRapporteur>
                    {
                        new SpecificationRapporteur {Fk_RapporteurId = 1, IsPrime = false},
                        new SpecificationRapporteur {Fk_RapporteurId = 3, IsPrime = true}
                    };
                default:
                    return null;
            }
        }

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
            repo.Stub(r => r.GetSeries()).Return(new List<Enum_Serie>() { 
                new Enum_Serie() { Pk_Enum_SerieId = 1, Code = "SER_12", Description = "Serie12" },
                new Enum_Serie() { Pk_Enum_SerieId = 2, Code = "SER_23", Description = "Serie23" }}
            );
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
            personManager.Stub(p => p.FindPerson(1)).Return(new View_Persons() { PERSON_ID = 1, FIRSTNAME = "User", LASTNAME = "1", Username = "User1"});
            personManager.Stub(p => p.FindPerson(3)).Return(new View_Persons() { PERSON_ID = 3, FIRSTNAME = "User", LASTNAME = "3", Username = "User3" });
            personManager.Stub(p => p.FindPerson(4)).Return(new View_Persons() { PERSON_ID = 4, FIRSTNAME = "User", LASTNAME = "4", Username = "User4" });
            ManagerFactory.Container.RegisterInstance<IPersonManager>(personManager);

            // Need a release repository
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Mock the UoW
            var uow = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            uow.Expect(r => r.Save());
            RepositoryFactory.Container.RegisterInstance<IUltimateUnitOfWork>(uow);

            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            
            RepositoryFactory.Container.RegisterType<ISpecificationWorkItemRepository, SpecificationWIFakeRepository>(new TransientLifetimeManager());

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            mockDataContext.Stub(x => x.ResponsibleGroupSecretaries).Return((IDbSet<ResponsibleGroup_Secretary>)GetResponsibleGroupSecretary());
            uow.Stub(s => s.Context).Return(mockDataContext);
        }

        private Specification _editSpecInstance;
        private Specification GetCorrectSpecificationForEdit(bool clone)
        {
            var spec = new Specification()
            {
                Pk_SpecificationId = 12,
                Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OpenedReleaseId } },
                Number = "12.123",
                Fk_SerieId = 1,
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

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "01.01", Title="Spec 1", IsActive = true, IsUnderChangeControl = true });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "12.123", Title = "Spec 2", IsActive = true, IsUnderChangeControl = false });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "12.189", Title = "Spec 3", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 4, Number = "02.72", Title = "Spec 4", IsActive = false });
            return list;
        }
        private IDbSet<View_Persons> Persons()
        {
            var dbSet = new PersonFakeDBSet();
            dbSet.Add(new View_Persons() { PERSON_ID = 1, Email = "un@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 2, Email = "deux@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 3, Email = "trois@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 4, Email = "quatre@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 5, Email = "cinq@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 6, Email = "six@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 7, Email = "sept@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 101, Email = "101@etsi.org" });
            return dbSet;
        }

        private static ResponsibleGroupSecretaryFakeDBSet GetResponsibleGroupSecretary()
        {
            var ResponsibleGroupSecretaries = new ResponsibleGroupSecretaryFakeDBSet { 
                new ResponsibleGroup_Secretary() { TbId = 1, Email = "one@capgemini.com", PersonId = 1 },
                new ResponsibleGroup_Secretary() { TbId = 1, Email = "onBis@capgemini.com", PersonId = 11 },
                new ResponsibleGroup_Secretary() { TbId = 2, Email = "two@capgemini.com", PersonId = 2 },
            };
            return ResponsibleGroupSecretaries;
        }

        #endregion
    }
}
