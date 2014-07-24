using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using NUnit.Framework;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using Etsi.Ultimate.Repositories;
using Effort.DataLoaders;
using System.Data.Entity.Core.EntityClient;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;


namespace Etsi.Ultimate.Tests.EffortTests
{
    [TestFixture]
    public class EffortTestsExample : BaseEffortTest
    {

        //REPO

        [Test]
        public void EnumTechnologyRepository_GetAll_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;

            var result = techRepo.All.ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("2G", result.FirstOrDefault().Code);
        }

        [Test]
        public void MeetingsRepository_GetAll_Test()
        {
            var mtgRepo = new MeetingRepository();
            mtgRepo.UoW = UoW;

            var result = mtgRepo.All.ToList();
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public void MeetingsRepository_FindById_Test()
        {
            var mtgRepo = new MeetingRepository();
            mtgRepo.UoW = UoW;

            var result = mtgRepo.Find(22888);
            Assert.AreNotEqual(null, result);
        }


        //BUSINESS

        [Test]
        public void MeetingsBusiness_GetMeetingById_Test()
        {
            var mtgMgr = new MeetingManager();
            mtgMgr.UoW = UoW;

            var result = mtgMgr.GetMeetingById(22888);
            Assert.AreNotEqual(null, result);
        }

        //SERVICE

        [Test]
        public void MeetingsService_GetMeetingById_Test()
        {
            var mtgSvc = new MeetingService();

            var result = mtgSvc.GetMeetingById(22888);
            Assert.AreNotEqual(null, result);
        }
    }
}
