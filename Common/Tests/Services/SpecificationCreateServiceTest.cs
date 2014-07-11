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
    public class SpecificationCreateServiceTest : BaseTest
    {
        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;
        private const int EDIT_SPECMGR_RIGHT_USER = UserRolesFakeRepository.SPECMGR_ID;

        [Test]
        public void CreateSpecification_ReturnsErrorIfExistingPk()
        {
            var specification = new Specification() { Pk_SpecificationId = 14 };
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(0, specification, "#baseurl#").Key);
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
            Assert.AreEqual(-1, specSvc.CreateSpecification(NO_EDIT_RIGHT_USER, specification, "#baseurl#").Key);
        }

        [Test]
        public void CreateSpecification_ReturnsErrorIfUserDidNotDefineRelease()
        {
            // Create the user rights repository.
            RegisterAllMocks();

            var specification = GetCorrectSpecificationForCreation();
            specification.Specification_Release.Clear();
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(EDIT_RIGHT_USER, specification, "#baseurl#").Key);
        }

        [Test]
        public void CreateSpecification_ReturnsErrorIfSpecNumberIsInvalid()
        {
            RegisterAllMocks();
            var specification = GetCorrectSpecificationForCreation();
            specification.Number = "12aaa";
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            Assert.AreEqual(-1, specSvc.CreateSpecification(EDIT_RIGHT_USER, specification, "#baseurl#").Key);
        }

        //Success
        [TestCase(1, false, false, EDIT_RIGHT_USER, "12.145", 1, 0)] //Right to edit number, so mail doesn't be send, no errors
        [TestCase(2, true, true, EDIT_LIMITED_RIGHT_USER, "", null, 0)]//Limited right, an no errors, a mail is send
        [TestCase(2, false, false, EDIT_SPECMGR_RIGHT_USER, "", null, 0)]         // User is spec Mgr, spec number is not assigned => No mail sent
        //Errors
        [TestCase(1, false, false, EDIT_LIMITED_RIGHT_USER, "", 0, 1)]//Same that the first one, but the user don't have the right to edit the number, so an error is thrown
        [TestCase(3, false, false, EDIT_RIGHT_USER, "", 0, 1)]//BAD FORMAT
        [TestCase(4, false, false, EDIT_RIGHT_USER, "", 0, 1)]//ALREADY EXIST
        public void CreateSpecification_NominalCase(int spec, bool shouldMailBeSent, bool shouldMailSucceed, int person, String number, int ? serie, int error)
        {
            var specification = GetSpecsToCreate(spec);
            // Set up the rights
            RegisterAllMocks();
            //---MAIL
            //Specific mock for the email, because we want to check the call made to it.
            var subject = String.Format(Localization.Specification_AwaitingReferenceNumberMail_Subject, specification.Title);

            var specUrl = new StringBuilder().Append("/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=").Append(specification.Pk_SpecificationId).ToString();
            var specLink = new StringBuilder().Append("#baseurl#").Append(specUrl).ToString();

            var body = new SpecAwaitingReferenceNumberMailTemplate("X Y", specification.Title, specLink);
            var bodyContent = body.TransformText();
            //Simulation send mail with test datas when we use 'SendEmail' method
            var mailMock = MockRepository.GenerateMock<IMailManager>();
            mailMock.Stub(r => r.SendEmail(
                Arg<string>.Is.Null,
                Arg<List<string>>.Matches(to => to.Count == 2),
                Arg<List<string>>.Is.Null,
                Arg<List<string>>.Is.Null,
                Arg<string>.Is.Equal(subject),
                Arg<string>.Is.Equal(bodyContent)
                )).Return(shouldMailSucceed);
            UtilsFactory.Container.RegisterInstance<IMailManager>(mailMock);
            //---MAIL

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var report = specSvc.CreateSpecification(person, specification, "#baseurl#").Value; // We can't change PK and check it, because it's assigned by EF in principle

            Assert.AreEqual(0, report.WarningList.Count);
            Assert.AreEqual(error, report.ErrorList.Count);

            //We don't test when an error occured
            if (error == 0)
            {
                Assert.AreEqual(1, specification.Histories.Count);
                Assert.AreEqual(String.Format(Utils.Localization.History_Specification_Created, "R1"), specification.Histories.First().HistoryText);
                Assert.IsTrue(specification.IsActive);
                Assert.IsTrue(specification.IsUnderChangeControl.HasValue && !specification.IsUnderChangeControl.Value);
                Assert.IsTrue(specification.IsTS.GetValueOrDefault());
                Assert.AreEqual(serie, specification.Fk_SerieId);
                Assert.AreEqual(number, specification.Number);
            }

            if (shouldMailBeSent)
            {
                mailMock.VerifyAllExpectations();
                Assert.AreEqual(1, report.InfoList.Count);
            }
            if (shouldMailBeSent && !shouldMailSucceed)
                Assert.AreEqual(1, report.ErrorList.Count);
        }

        #region Arguments tests
        private Specification GetSpecsToCreate(int spec)
        {
            switch (spec)
            {
                case 1://WITH NUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 0,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "12.145",
                        Title = "SpecTitle"
                    };
                case 2://WITHOUTNUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 0,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "",
                        Title = "SpecTitle"
                    };
                case 3://BAD FORMAT NUMBER
                    return new Specification()
                    {
                        Pk_SpecificationId = 0,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "vd-()=",
                        Title = "SpecTitle"
                    };
                case 4://ALREADY EXIST
                    return new Specification()
                    {
                        Pk_SpecificationId = 0,
                        Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                        Number = "12.123",
                        Title = "SpecTitle"
                    };
                default:
                    return null;
            }
        }
        #endregion

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
            userRights.Stub(r => r.GetRights(EDIT_SPECMGR_RIGHT_USER)).Return(rights);
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
            personManager.Stub(p => p.FindPerson(EDIT_SPECMGR_RIGHT_USER)).Return(new View_Persons() { PERSON_ID = EDIT_SPECMGR_RIGHT_USER, FIRSTNAME = "SpecMgr", LASTNAME = "5" });
            ManagerFactory.Container.RegisterInstance<IPersonManager>(personManager);

            // Need a release repository
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Mock the UoW
            var uow = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            uow.Expect(r => r.Save());
            RepositoryFactory.Container.RegisterInstance<IUltimateUnitOfWork>(uow);

            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            uow.Stub(s => s.Context).Return(mockDataContext);

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

        private Specification GetCorrectSpecificationForCreation()
        {
            return new Specification()
            {
                Pk_SpecificationId = 0,
                Specification_Release = new List<Specification_Release>() { new Specification_Release() { Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID } },
                Number = "",
                Title = "SpecTitle"
            };
        }
        private IDbSet<View_Persons> Persons()
        {
            var dbSet = new PersonFakeDBSet();
            dbSet.Add(new View_Persons() { PERSON_ID = 1, Email = "un@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 2, Email = "deux@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 3, Email = "trois@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 4, Email = "quatre@etsi.org", FIRSTNAME = "X", LASTNAME = "Y" });
            dbSet.Add(new View_Persons() { PERSON_ID = 5, Email = "cinq@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 6, Email = "six@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 7, Email = "sept@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 101, Email = "101@etsi.org" });
            return dbSet;
        }

        #endregion

        
    }
}
