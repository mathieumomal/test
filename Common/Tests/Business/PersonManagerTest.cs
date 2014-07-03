﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Business
{
    public class PersonManagerTest
    {
        [Test]
        [TestCase(2, 1)]//Two secretaries for the comittee id = 1 (TbId)
        [TestCase(1, 2)]//One secretary for the commitee id = 2
        [TestCase(0, 3)]//0 secretary for the ommitee id which isn't in the database
        public void GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId_Test(int result, int tbId)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.ResponsibleGroupSecretaries).Return((IDbSet<ResponsibleGroup_Secretary>)GetResponsibleGroupSecretary());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var personManager = ManagerFactory.Resolve<IPersonManager>();
            personManager.UoW = uow;

            var listSecretariesEmail = personManager.GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(tbId);

            Assert.AreEqual(result, listSecretariesEmail.Count);
        }

        [Test]
        [TestCase(1, 1)]//One chairman for the commitee id = 1
        [TestCase(2, 2)]//One chairman for the commitee id = 2 (but two chairman are here)
        [TestCase(0, 0)]//0 chairman for the commitee id which isn't in the database
        public void GetChairmanIdByCommityIdTest(int result, int tbId)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.ResponsibleGroupChairmans).Return((IDbSet<ResponsibleGroup_Chairman>)GetResponsibleGroupChairman());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var personManager = ManagerFactory.Resolve<IPersonManager>();
            personManager.UoW = uow;

            var chairmanId = personManager.GetChairmanIdByCommityId(tbId);

            Assert.AreEqual(result, chairmanId);
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

        private static ResponsibleGroupChairmanFakeDBSet GetResponsibleGroupChairman()
        {
            var ResponsibleGroupChairmans = new ResponsibleGroupChairmanFakeDBSet { 
                new ResponsibleGroup_Chairman() { TbId = 1, Email = "one@capgemini.com", PersonId = 1 },
                new ResponsibleGroup_Chairman() { TbId = 1, Email = "onBis@capgemini.com", PersonId = 11 },
                new ResponsibleGroup_Chairman() { TbId = 2, Email = "two@capgemini.com", PersonId = 2 },
            };
            return ResponsibleGroupChairmans;
        }

    }
}
