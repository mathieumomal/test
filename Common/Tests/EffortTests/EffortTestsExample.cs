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
    public class EffortTestsExample
    {
        private IUltimateContext context { get; set; }

        /// <summary>
        /// Initialization of the context by a set of csv files
        /// </summary>
        //[TestFixtureSetUp]
        //public void init()
        //{
        //    //CSV files are stored in a specific folder : Data_For_EffortUnitTests
        //    string dir = Environment.CurrentDirectory + "\\Data_For_EffortUnitTests\\";
        //    //Use Effort framework
        //    IDataLoader loader = new Effort.DataLoaders.CsvDataLoader(dir);
        //    EntityConnection connection = Effort.EntityConnectionFactory.CreateTransient("name=UltimateContext", loader);
        //    //Context creation
        //    context = new UltimateContext(connection);
        //}

        ////REPO

        //[Test]
        //public void EnumTechnologyRepository_GetAll_Test()
        //{
        //    RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), context);
        //    var techRepo = new EnumTechnologiesRepository(RepositoryFactory.Resolve<IUltimateUnitOfWork>());

        //    var result = techRepo.All.ToList();
        //    Assert.AreEqual(3, result.Count);
        //    Assert.AreEqual("2G", result.FirstOrDefault().Code);
        //}

        //[Test]
        //public void MeetingsRepository_GetAll_Test()
        //{
        //    RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), context);
        //    var mtgRepo = new MeetingRepository();
        //    mtgRepo.UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

        //    var result = mtgRepo.All.ToList();
        //    Assert.AreEqual(10, result.Count);
        //}

        //[Test]
        //public void MeetingsRepository_FindById_Test()
        //{
        //    RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), context);
        //    var mtgRepo = new MeetingRepository();
        //    mtgRepo.UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

        //    var result = mtgRepo.Find(22888);
        //    Assert.AreNotEqual(null, result);
        //}


        ////BUSINESS

        //[Test]
        //public void MeetingsBusiness_GetMeetingById_Test()
        //{
        //    RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), context);
        //    var mtgMgr = new MeetingManager();
        //    mtgMgr.UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

        //    var result = mtgMgr.GetMeetingById(22888);
        //    Assert.AreNotEqual(null, result);
        //}

        ////SERVICE

        //[Test]
        //public void MeetingsService_GetMeetingById_Test()
        //{
        //    RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), context);
        //    var mtgSvc = new MeetingService();

        //    var result = mtgSvc.GetMeetingById(22888);
        //    Assert.AreNotEqual(null, result);
        //}
    }
}
