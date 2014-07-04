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
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecVersionServiceTest : BaseTest
    {
        private const int USERID = 0;

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
            SpecVersion result = versionsSvc.GetVersionsById(2,0).Key;            
            Assert.AreEqual("Location2", result.Location);
            Assert.AreEqual("Remark 2", result.Remarks.ToList()[0].RemarkText);
        }

        [Test]
        public void UploadOrAllocateVersionTest_ExistingVersion_Uploaded()
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var versionsSvc = new SpecVersionService();
            var existingVersion = new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "L1",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() {Pk_RemarkId=1, Fk_VersionId = 1, RemarkText="R1"}},
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };
            Report r = versionsSvc.UploadOrAllocateVersion(existingVersion, false, USERID);
            Assert.AreEqual(String.Format("Document has already been uploaded to this version"), r.ErrorList.FirstOrDefault());
            //Assert.AreEqual(existingVersion.EditorialVersion, versionsSvc.GetVersionsById(1,0).Key.EditorialVersion) ; 
        }

        [Test]
        public void UploadOrAllocateVersionTest_ExistingVersion_Allocated()
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var versionsSvc = new SpecVersionService();
            var existingVersion = new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = "L2",
                MajorVersion = 20,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 10, 18),
                ProvidedBy = 1,
                Specification = new Specification() { Pk_SpecificationId = 1, IsActive = true, IsUnderChangeControl = false },
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };
            Report r = versionsSvc.UploadOrAllocateVersion(existingVersion, false, USERID);
            Assert.AreEqual(null, r.ErrorList.FirstOrDefault());
            Assert.AreEqual(existingVersion.Remarks.Count + 1, versionsDBSet.Where(v => v.Pk_VersionId==2).ToList().FirstOrDefault().Remarks.Count);
            Assert.AreEqual("L2", versionsDBSet.Where(v => v.Pk_VersionId == 2).ToList().FirstOrDefault().Location);
        }

        [Test]
        public void UploadOrAllocateVersionTest_Draft_InvalidVersion()
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);


            var versionsSvc = new SpecVersionService();
            var existingVersion = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = "L3",
                MajorVersion = 28, //Minor Major version
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Specification = new Specification() { Pk_SpecificationId = 2, IsActive = true, IsUnderChangeControl = false },
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1
            };
            Report r = versionsSvc.UploadOrAllocateVersion(existingVersion, true, USERID); //Draft            
            Assert.AreEqual("Invalid draft version number!", r.ErrorList.FirstOrDefault());                        
        }

        [Test]
        public void UploadOrAllocateVersionTest_New_InvalidVersion()
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);


            var versionsSvc = new SpecVersionService();
            var existingVersion = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = "L3",
                MajorVersion = 28, //Minor Major version
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Specification = new Specification() { Pk_SpecificationId = 2, IsActive = true, IsUnderChangeControl = false },
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1
            };
            Report r = versionsSvc.UploadOrAllocateVersion(existingVersion, false, USERID); //New
            Assert.AreEqual(String.Format("Invalid version number. Version number should be grater than {0}", versionsDBSet.Where(v => v.Pk_VersionId == 3).ToList().FirstOrDefault().Version), r.ErrorList.FirstOrDefault());            
        }

        [Test]
        public void UploadOrAllocateVersionTest_New_ValidVersion()
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);


            var versionsSvc = new SpecVersionService();
            var newVersion = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = "L3",
                MajorVersion = 30, //Minor Major version
                TechnicalVersion = 2,
                EditorialVersion = 2,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Specification = new Specification() { Pk_SpecificationId = 2, IsActive = true, IsUnderChangeControl = false },
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1
            };
            Report r = versionsSvc.UploadOrAllocateVersion(newVersion, false, USERID); //New
            Assert.AreEqual(null, r.ErrorList.FirstOrDefault());           

            mockDataContext.AssertWasCalled(x => x.SetAdded(newVersion));
        }

        [TestCase(null, null, 1)]
        [TestCase(null, 1, 1)]
        [TestCase(3,null, 1)]
        [TestCase(3, 1, 0)]
        [TestCase(3, 2, 0)]
        [TestCase(1, 2, 0)]
        [TestCase(2, 1, 0)]
        [TestCase(2, 2, 0)]
        [TestCase(2, 4, 0)]
        public void UploadTransposition_Transposition(int specID, int releaseID, int errorExpected)
        {
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(USERID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var versionsDBSet = GetSpecVersions();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)versionsDBSet);

            var specs = GetSpecs();
            var releases = GetReleases();
            var enumReleaseStatus = GetReleaseStatus();
            var specRelease = GetSpecReleases();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)enumReleaseStatus);
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specs);
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)specRelease);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var versionsSvc = new SpecVersionService();
            var newVersion = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = "L3",
                MajorVersion = 30, //Minor Major version
                TechnicalVersion = 2,
                EditorialVersion = 2,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = specID,
                Fk_ReleaseId = releaseID
            };

            Report r = versionsSvc.UploadOrAllocateVersion(newVersion, false, USERID);
            Assert.AreEqual(errorExpected, r.ErrorList.Count());
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

        // <summary>
        /// Get Fake SpecVersion Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecVersionFakeDBSet GetSpecVersions()
        {
            var versionDbSet = new SpecVersionFakeDBSet();
           
            var version = new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "L1",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() {Pk_RemarkId=1, Fk_VersionId = 1, RemarkText="R1"}},
                Fk_SpecificationId=1,
                Fk_ReleaseId = 1

            };
            var version2 = new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = null,
                MajorVersion = 20,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 10, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };
            var version3 = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = null,
                MajorVersion = 30,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1
            };
            versionDbSet.Add(version);
            versionDbSet.Add(version2);
            versionDbSet.Add(version3);

            return versionDbSet;
        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "1", IsUnderChangeControl = true });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "2", IsUnderChangeControl = true });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "2", IsUnderChangeControl = false });
            return list;
        }

        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            list.Add(new Release() { Pk_ReleaseId = 2, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 3, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 4, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            return list;
        }

        private IDbSet<Enum_ReleaseStatus> GetReleaseStatus()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen", Description = "Frozen" });
            return list;
        }

        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId =3, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 2, Fk_ReleaseId = 2, isTranpositionForced = true });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 4, Fk_SpecificationId = 2, Fk_ReleaseId = 1, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 5, Fk_SpecificationId = 2, Fk_ReleaseId = 4, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 6, Fk_SpecificationId = 3, Fk_ReleaseId = 1, isTranpositionForced = true });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 7, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isTranpositionForced = false });
            return list;
        }
    }
}
