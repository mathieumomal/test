using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecVersionServiceTest : BaseTest
    {
        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsBySpecId(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            List<SpecVersion> result =  versionsSvc.GetVersionsBySpecId(1);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Remarks.Count);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsForSpecRelease(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            List<SpecVersion> result = versionsSvc.GetVersionsForSpecRelease(1, 2);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Location2", result[0].Location);
            Assert.AreEqual("Remark 2", result[0].Remarks.ToList()[0].RemarkText);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsById(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            SpecVersion result = versionsSvc.GetVersionsById(2);            
            Assert.AreEqual("Location2", result.Location);
            Assert.AreEqual("Remark 2", result.Remarks.ToList()[0].RemarkText);
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
