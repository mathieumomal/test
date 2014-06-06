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
    public class SpecificationEditServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;

        public override void Setup()
        {
            base.Setup();
            _editSpecInstance = null;
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
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

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
            specToEdit.Remarks.Add(new Remark() { IsPublic = false, Fk_PersonId = 0 });

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.IsTrue(specSvc.EditSpecification(EDIT_RIGHT_USER, specToEdit).Key);

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

        //SUCCESS
        [TestCase(false, false, EDIT_RIGHT_USER, "12.123", 0)]//Number don't changed
        [TestCase(false, false, EDIT_LIMITED_RIGHT_USER, "12.123", 0)]//Number don't changed
        [TestCase(true, true, EDIT_RIGHT_USER, "", 0)]//Number changed to null
        [TestCase(true, true, EDIT_RIGHT_USER, "12.145", 0)]//Number changed
        ////ERROR
        [TestCase(true, false, EDIT_LIMITED_RIGHT_USER, "12.145", 1)]//Number changed but user don't have full rights
        [TestCase(true, false, EDIT_RIGHT_USER, "ijfzeo989=)", 1)]//Number changed with format error
        [TestCase(true, false, EDIT_RIGHT_USER, "12.189", 1)]//Number changed but already exist
        public void EditSpecification_EmailTest(bool shouldMailBeSent, bool shouldMailSucceed, int person, String specToUpdateNumber, int error)
        {
            var specToEdit = GetCorrectSpecificationForEdit(true);
            specToEdit.Number = specToUpdateNumber;
            // Set up the rights
            RegisterAllMocks();
            //---MAIL
            //Specific mock for the email, because we want to check the call made to it.
            var roleManager = new RolesManager();//To get workplan managers email
            var toAddresss = roleManager.GetWpMgrEmail();
            var personManager = new PersonManager();//To get secretaries email
            var primeResponsibleGroupCommityId = specToEdit.PrimeResponsibleGroup.Fk_commityId;
            var listSecretariesEmail = personManager.GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(primeResponsibleGroupCommityId);
            toAddresss = toAddresss.Concat(listSecretariesEmail).ToList();

            var subject = String.Format(Localization.Specification_ReferenceNumberAssigned_Subject, specToEdit.Number);
            var body = new SpecReferenceNumberAssignedMailTemplate((String.IsNullOrEmpty(specToEdit.Number) ? "" : specToEdit.Number), (String.IsNullOrEmpty(specToEdit.Title) ? "" : specToEdit.Title), new List<string>() { });
            var bodyContent = body.TransformText();
            //Simulation send mail with test datas when we use 'SendEmail' method
            var mailMock = MockRepository.GenerateMock<IMailManager>();
            mailMock.Stub(r => r.SendEmail(
                Arg<string>.Is.Null,
                Arg<List<string>>.Is.Equal(toAddresss),
                Arg<List<string>>.Is.Null,
                Arg<List<string>>.Is.Null,
                Arg<string>.Is.Equal(subject),
                Arg<string>.Is.Equal(bodyContent)
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
            }

            if (shouldMailBeSent)
            {
                mailMock.VerifyAllExpectations();
            }
            if (shouldMailBeSent && !shouldMailSucceed)
                Assert.AreEqual(1, report.ErrorList.Count);
        }

        private Specification GetSpecsToCreate(int spec)
        {
            switch (spec)
            {
                case 1://WITH NUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 12,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "12.145",
                        Title = "SpecTitle"
                    };
                case 2://WITHOUTNUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 12,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "",
                        Title = "SpecTitle"
                    };
                case 3://BAD FORMAT NUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 12,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "vd-()=",
                        Title = "SpecTitle"
                    };
                case 4://ALREADY EXIST
                    return new Specification()
                    {
                        Pk_SpecificationId = 12,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "12.123",
                        Title = "SpecTitle"
                    };
                default:
                    return null;
            }
        }

        #region datas
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

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "01.01", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "12.123", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "12.189", IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 4, Number = "02.72", IsActive = true });
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
