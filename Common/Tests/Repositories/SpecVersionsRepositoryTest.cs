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
    public class SpecVersionsRepositoryTest : BaseTest
    {
        [Test]
        public void SpecVersionsRepository_GetAll()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;

            List<SpecVersion> buffer = repo.All.ToList();
            Assert.AreEqual(2, buffer.Count);
            Assert.AreEqual("Remark 1", buffer[0].Remarks.ToList()[0].RemarkText);
        }

        [Test]
        public void SpecVersionsRepository_GetAllIncluding()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;

            Assert.AreEqual(2, repo.AllIncluding().ToList().Count);

        }

        [Test]
        public void SpecVersionsRepository_Find()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;

            Assert.AreEqual("Location2", repo.Find(2).Location);
        }

        [Test]
        public void SpecVersionRepository_GetVersionForSpecrelease()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;

            Assert.AreEqual("Location1", repo.GetVersionForSpecRelease(1, 1)[0].Location);
        }
        

        [Test]
        public void SpecVersionsRepository_InsertOrUpdate_Insertion()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;


            var specVersion = new SpecVersion()
            {                
                Multifile = false,
                ForcePublication = false,
                Location = "Location3",
                Remarks = new List<Remark>(){ new Remark() { RemarkText = "New remark", Fk_VersionId = 1 } }
                 
            };
            repo.InsertOrUpdate(specVersion);
            Assert.AreEqual(3, repo.All.ToList().Count);
            Assert.AreEqual("New remark", repo.All.ToList()[2].Remarks.ToList()[0].RemarkText);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SpecVersionsRepository_Delete()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecVersionsRepository(uow);
            repo.UoW = uow;

            repo.Delete(1);
        }        

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            var rmkDbSet = new RemarkFakeDbSet();
            rmkDbSet.Add(new Remark() { Pk_RemarkId = 1, RemarkText = "Remark 1", Fk_VersionId = 1 });
            rmkDbSet.Add(new Remark() { Pk_RemarkId = 2, RemarkText = "Remark 2", Fk_VersionId = 2 });

            var dbSet = new SpecVersionFakeDBSet();
            dbSet.Add(new SpecVersion() 
            { 
                Pk_VersionId = 1, 
                Multifile = false, 
                ForcePublication = false, 
                Location = "Location1",
                Fk_ReleaseId = 1, 
                Fk_SpecificationId = 1, 
                Remarks = new List<Remark>() { rmkDbSet.ToList()[0] }, 
                Release = new Release(),
                Specification = new Specification()
            });
            dbSet.Add(new SpecVersion()
            {
                Pk_VersionId = 2,
                Multifile = false,
                ForcePublication = false,
                Location = "Location2",
                Fk_ReleaseId = 2,
                Fk_SpecificationId = 1,
                Remarks = new List<Remark>() { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
            });

            iUltimateContext.SpecVersions = dbSet;            
            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);

            return iUnitOfWork;
        }
    }
}
