using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using System.Data.Entity;

namespace Etsi.Ultimate.Tests.Business
{
    public class RolesManagerTest : BaseTest
    {
        private const string EmailTest = "emailTest@etsi.org";
        [Test]
        public void GetSpecMgrs()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            
            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);

            var manager = new RolesManager();
            var results = manager.GetSpecMgr();

            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void GetSpecMgrsEmail()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);

            var manager = new RolesManager();
            var results = manager.GetSpecMgrEmail();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.First().Contains(EmailTest));
        }

        [Test]
        public void GetWpMgrs()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);

            var manager = new RolesManager();
            var results = manager.GetWpMgr();

            Assert.AreEqual(3, results.Count);
        }

        [Test]
        public void GetWpMgrsEmail()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            mockDataContext.Stub(x => x.View_Persons).Return(Persons());
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);

            var manager = new RolesManager();
            var results = manager.GetWpMgrEmail();

            Assert.AreEqual(3, results.Count);
            Assert.IsTrue(results.First().Contains(EmailTest));
        }

        #region datas
        private IDbSet<View_Persons> Persons()
        {
            var dbSet = new PersonFakeDBSet();
            dbSet.Add(new View_Persons() { PERSON_ID = 1, Email = "un@etsi.org"});
            dbSet.Add(new View_Persons() { PERSON_ID = 2, Email = "deux@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 3, Email = "trois@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 4, Email = "quatre@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 5, Email = "cinq@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 6, Email = "six@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 7, Email = "sept@etsi.org" });
            dbSet.Add(new View_Persons() { PERSON_ID = 101, Email = EmailTest });
            return dbSet;
        }
        #endregion
    }
}
