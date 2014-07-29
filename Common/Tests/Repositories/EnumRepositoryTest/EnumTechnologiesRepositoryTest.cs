using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using NUnit.Framework;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using Etsi.Ultimate.Repositories;
using Effort.DataLoaders;
using System.Data.Entity.Core.EntityClient;


namespace Etsi.Ultimate.Tests.Repositories.EnumRepositoryTest
{
    public class EnumTechnologiesRepositoryTest : BaseEffortTest
    {
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
        public void EnumTechnologyRepository_Find_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;

            var result = techRepo.All.ToList();
            var technos = techRepo.Find(result.FirstOrDefault().Pk_Enum_TechnologyId);
            Assert.NotNull(technos);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumTechnologyRepository_AllIncluding_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;
            var result = techRepo.AllIncluding(null).ToList();
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumTechnologyRepository_InsertOrUpdate_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;
            techRepo.InsertOrUpdate(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnumTechnologyRepository_delete_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;
            techRepo.Delete(1);
        }
    }
}
