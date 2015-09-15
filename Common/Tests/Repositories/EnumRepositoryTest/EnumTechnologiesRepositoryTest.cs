using System;
using System.Linq;
using NUnit.Framework;
using Etsi.Ultimate.Repositories;


namespace Etsi.Ultimate.Tests.Repositories.EnumRepositoryTest
{
    public class EnumTechnologiesRepositoryTest : BaseEffortTest
    {
        [Test]
        public void EnumTechnologyRepository_GetAll_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;

            var result = techRepo.All.OrderBy(x => x.SortOrder).ToList();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("2G", result.FirstOrDefault().Code);
            Assert.AreEqual("5G", result.LastOrDefault().Code);
        }

        [Test]
        public void EnumTechnologyRepository_GetAll_byReverseOrder_Test()
        {
            var techRepo = new EnumTechnologiesRepository();
            techRepo.UoW = UoW;

            var result = techRepo.All.OrderByDescending(x => x.SortOrder).ToList();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("5G", result.FirstOrDefault().Code);
            Assert.AreEqual("2G", result.LastOrDefault().Code);
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
