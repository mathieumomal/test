using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class EnumTechnologiesRepositoryTest
    {
        [Test]
        public void EnumTechologiesGetAll(){
            var repo = new EnumTechnologiesRepository(GetUnitOfWork());
            Assert.AreEqual(3,repo.All.ToList().Count);
        }

        [Test]
        public void EnumTechnlogies_FindById()
        {
            var repo = new EnumTechnologiesRepository(GetUnitOfWork());
            Assert.AreEqual("2G", repo.Find(1).Code);
            Assert.AreEqual("3G", repo.Find(2).Code);
            Assert.AreEqual("LTE", repo.Find(3).Code);
        }


        public IUltimateUnitOfWork GetUnitOfWork(){

            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new EnumTechnologiesFakeDBSet();
            dbSet.Add(new Enum_Technology() { Pk_Enum_TechnologyId = 1, Code = "2G", Description = "2G" });
            dbSet.Add(new Enum_Technology() { Pk_Enum_TechnologyId = 2, Code = "3G", Description = "3G" });
            dbSet.Add(new Enum_Technology() { Pk_Enum_TechnologyId = 3, Code = "LTE", Description = "LTE" });

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.Enum_Technology).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
