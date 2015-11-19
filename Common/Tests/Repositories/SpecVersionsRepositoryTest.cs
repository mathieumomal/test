using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class SpecVersionsRepositoryTest : BaseTest
    {
        private IUltimateUnitOfWork _uow;
        private ISpecVersionsRepository _repo;
        private const int _versionsGlobalCount = 5;

        [SetUp]
        public void Setup()
        {
            _uow = GetUnitOfWork();
            _repo = new SpecVersionsRepository {UoW = _uow};
        }

        [Test]
        public void SpecVersionsRepository_GetAll()
        {
            List<SpecVersion> buffer = _repo.All.ToList();
            Assert.AreEqual(_versionsGlobalCount, buffer.Count);
            Assert.AreEqual("Remark 1", buffer[0].Remarks.ToList()[0].RemarkText);
        }

        [Test]
        public void SpecVersionsRepository_GetAllIncluding()
        {
            Assert.AreEqual(_versionsGlobalCount, _repo.AllIncluding().ToList().Count);

        }

        [Test]
        public void SpecVersionsRepository_Find()
        {
            Assert.AreEqual("Location2", _repo.Find(2).Location);
        }

        [Test]
        public void SpecVersionRepository_GetVersionsForSpecrelease()
        {
            Assert.AreEqual("Location1", _repo.GetVersionsForSpecRelease(1, 1)[0].Location);
        }
        

        [Test]
        public void SpecVersionsRepository_InsertOrUpdate_Insertion()
        {
            var specVersion = new SpecVersion
            {                
                Multifile = false,
                ForcePublication = false,
                Location = "Location3",
                Remarks = new List<Remark>{ new Remark { RemarkText = "New remark", Fk_VersionId = 1 } }
                 
            };
            _repo.InsertOrUpdate(specVersion);
            Assert.AreEqual(_versionsGlobalCount + 1, _repo.All.ToList().Count);
            Assert.AreEqual("New remark", _repo.All.Last().Remarks.ToList()[0].RemarkText);
        }

        [Test]
        public void SpecVersionsRepository_Delete()
        {
            var specVesionToDelete = _repo.All.First();
            _repo.Delete(specVesionToDelete);
            Assert.IsFalse(_repo.All.Any(x=> x.Pk_VersionId == specVesionToDelete.Pk_VersionId));
        }

        [Test]
        public void SpecVersionsRepository_GetVersionsByReleaseId()
        {
            Assert.AreEqual(1, _repo.GetVersionsByReleaseId(2).Count);
            Assert.AreEqual(2, _repo.GetVersionsByReleaseId(2).First().Pk_VersionId);
        }

        [TestCase(1, 2, Description = "Should return 2 versions because have location setted")]
        [TestCase(2, 1, Description = "Should return 1 version because both have not location setted but one of them have a DocumentUploaded date")]
        public void AlreadyUploadedVersionsForSpec(int specId, int expectedResult)
        {
            Assert.AreEqual(expectedResult, _repo.AlreadyUploadedVersionsForSpec(specId).Count);
        }

        [TestCase(1, 0, Description = "Should return 0 version because no versions linked to a CR")]
        [TestCase(2, 1, Description = "Should return 1 version because one of them linked to a CR (current version setted)")]
        [TestCase(3, 1, Description = "Should return 1 version because one of them linked to a CR (new version setted)")]
        public void VersionsLinkedToChangeRequestsForSpec(int specId, int expectedResult)
        {
            Assert.AreEqual(expectedResult, _repo.VersionsLinkedToChangeRequestsForSpec(specId).Count);
        }

        [TestCase(1, 0, 0, Description = "Should return 0 cr linked to this version")]
        [TestCase(4, 1, 0, Description = "Should return 1 cr linked to this version (current version)")]
        [TestCase(5, 0, 1, Description = "Should return 1 cr linked to this version (new version)")]
        public void FindCrsLinkedToAVersion(int versionId, int expectedResultCurrentVersion, int expectedResultNewVersion)
        {
            Assert.AreEqual(expectedResultCurrentVersion, _repo.FindCrsLinkedToAVersion(versionId).CurrentChangeRequests.Count);
            Assert.AreEqual(expectedResultNewVersion, _repo.FindCrsLinkedToAVersion(versionId).FoundationsChangeRequests.Count);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            var rmkDbSet = new RemarkFakeDbSet
            {
                new Remark {Pk_RemarkId = 1, RemarkText = "Remark 1", Fk_VersionId = 1},
                new Remark {Pk_RemarkId = 2, RemarkText = "Remark 2", Fk_VersionId = 2}
            };

            var dbSet = new SpecVersionFakeDBSet();
            dbSet.Add(new SpecVersion
            { 
                Pk_VersionId = 1, 
                Multifile = false, 
                ForcePublication = false, 
                Location = "Location1",
                Fk_ReleaseId = 1, 
                Fk_SpecificationId = 1, 
                Remarks = new List<Remark> { rmkDbSet.ToList()[0] }, 
                Release = new Release(),
                Specification = new Specification()
            });
            dbSet.Add(new SpecVersion
            {
                Pk_VersionId = 2,
                Multifile = false,
                ForcePublication = false,
                Location = "Location2",
                Fk_ReleaseId = 2,
                Fk_SpecificationId = 1,
                Remarks = new List<Remark> { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
            });
            dbSet.Add(new SpecVersion
            {
                Pk_VersionId = 3,
                Multifile = false,
                ForcePublication = false,
                Location = null,
                Fk_ReleaseId = 3,
                Fk_SpecificationId = 2,
                Remarks = new List<Remark> { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
            });
            dbSet.Add(new SpecVersion
            {
                Pk_VersionId = 4,
                Multifile = false,
                ForcePublication = false,
                Location = null,
                DocumentUploaded = DateTime.Now,
                Fk_ReleaseId = 3,
                Fk_SpecificationId = 2,
                Remarks = new List<Remark> { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
                CurrentChangeRequests = new List<ChangeRequest> { new ChangeRequest { CRNumber = "ABC"} }
            });
            dbSet.Add(new SpecVersion
            {
                Pk_VersionId = 5,
                Multifile = false,
                ForcePublication = false,
                Location = null,
                DocumentUploaded = DateTime.Now,
                Fk_ReleaseId = 3,
                Fk_SpecificationId = 3,
                Remarks = new List<Remark> { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
                FoundationsChangeRequests = new List<ChangeRequest> { new ChangeRequest { CRNumber = "ABC" } }
            });

            iUltimateContext.SpecVersions = dbSet;            
            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);

            return iUnitOfWork;
        }
    }
}
