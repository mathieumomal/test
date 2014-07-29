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
    public class EnumCommunityShortnameRepositoryTest : BaseEffortTest
    {
        [Test]
        public void EnumCommunityShortNameRepository_All_Test()
        {
            var comShortNameRepo = new Enum_CommunitiesShortNameRepository();
            comShortNameRepo.UoW = UoW;

            var result = comShortNameRepo.All.ToList();
            Assert.AreEqual(91, result.Count);
            Assert.AreEqual(520, result.FirstOrDefault().Fk_TbId);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumCommunityShortNameRepository_Find_Test()
        {
            var comShortNameRepo = new Enum_CommunitiesShortNameRepository();
            comShortNameRepo.UoW = UoW;
            var result = comShortNameRepo.Find(1);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumCommunityShortNameRepository_AllIncluding_Test()
        {
            var comShortNameRepo = new Enum_CommunitiesShortNameRepository();
            comShortNameRepo.UoW = UoW;
            var result = comShortNameRepo.AllIncluding(null).ToList();
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumCommunityShortNameRepository_InsertOrUpdate_Test()
        {
            var comShortNameRepo = new Enum_CommunitiesShortNameRepository();
            comShortNameRepo.UoW = UoW;
            comShortNameRepo.InsertOrUpdate(null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnumCommunityShortNameRepository_delete_Test()
        {
            var comShortNameRepo = new Enum_CommunitiesShortNameRepository();
            comShortNameRepo.UoW = UoW;
            comShortNameRepo.Delete(1);
        }
    }
}
