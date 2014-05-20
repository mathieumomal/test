using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using System.Data.Entity;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Business
{
    class SpecVersionsManagerTest : BaseTest
    {
        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionForASpecReleaseTest(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager(uow);            
            List<SpecVersion> result = mng.GetVersionsForASpecRelease(1, 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result[0].Pk_VersionId);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsBySpecIdTest(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager(uow);
            List<SpecVersion> result = mng.GetVersionsBySpecId(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Pk_VersionId);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsByIdTest(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager(uow);
            SpecVersion result = mng.GetSpecVersionById(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Location1", result.Location);
        }

        [Test]
        public void UploadOrAllocateVersionTest()
        {
            //TODO
        }

        /// <summary>
        /// Get SpecVersions DATA
        /// </summary>
        public IEnumerable<SpecVersionFakeDBSet> SpecVersionsData
        {
            get
            {
                var rmkDbSet = new RemarkFakeDbSet();
                rmkDbSet.Add(new Remark() { Pk_RemarkId = 1, RemarkText = "Remark 1", Fk_VersionId = 1 });
                rmkDbSet.Add(new Remark() { Pk_RemarkId = 2, RemarkText = "Remark 2", Fk_VersionId = 2 });

                SpecVersionFakeDBSet specVersionFakeDBSet = new SpecVersionFakeDBSet();
                
                specVersionFakeDBSet.Add(new SpecVersion()
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
                specVersionFakeDBSet.Add(new SpecVersion()
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
                
                yield return specVersionFakeDBSet;
            }
        }
    }
}
