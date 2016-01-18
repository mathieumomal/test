using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils.Core;
using NUnit.Framework;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.DataAccess;
using System.IO;

namespace Etsi.Ultimate.Tests.Business
{
    class FtpFoldersManagerTest : BaseEffortTest
    {
        [Test]
        public void CheckLatestFolder_NominalCase()
        {
            /* Init Rights manager */
            var rightsManagerMock = MockRepository.GenerateMock<IRightsManager>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_SyncHardLinksOnLatestFolder);
            rightsManagerMock.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), rightsManagerMock);

            /* Mock GetLatestVersionGroupedBySpecRelease */
            var specVersionsRepositoryMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionsRepositoryMock.Stub(x => x.GetLatestVersionGroupedBySpecRelease()).Return(null);
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionsRepositoryMock);

            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.UoW = UoW;
            var response = ftpFoldersManager.CheckLatestFolder("test", UserRolesFakeRepository.MCC_MEMBER_ID);

            Assert.True(response.Result);
        }

        [Test]
        public void CheckLatestFolder_UserRightsError()
        {
            /* Init Rights manager */
            var rightsManagerMock = MockRepository.GenerateMock<IRightsManager>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Release_Close);
            rightsManagerMock.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), rightsManagerMock);

            /* Mock GetLatestVersionGroupedBySpecRelease */
            var specVersionsRepositoryMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionsRepositoryMock.Stub(x => x.GetLatestVersionGroupedBySpecRelease()).Return(null);
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionsRepositoryMock);

            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.UoW = UoW;
            var response = ftpFoldersManager.CheckLatestFolder("test", UserRolesFakeRepository.MCC_MEMBER_ID);

            Assert.False(response.Result);
            Assert.AreSame(Localization.RightError, response.Report.ErrorList.FirstOrDefault());
        }

        [Test]
        public void CheckLatestFolder_FolderNameError()
        {
            /* Init Rights manager */
            var rightsManagerMock = MockRepository.GenerateMock<IRightsManager>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_SyncHardLinksOnLatestFolder);
            rightsManagerMock.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), rightsManagerMock);

            /* Mock GetLatestVersionGroupedBySpecRelease */
            var specVersionsRepositoryMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionsRepositoryMock.Stub(x => x.GetLatestVersionGroupedBySpecRelease()).Return(null);
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionsRepositoryMock);

            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.UoW = UoW;
            var response = ftpFoldersManager.CheckLatestFolder(string.Empty, UserRolesFakeRepository.MCC_MEMBER_ID);

            Assert.False(response.Result);
            Assert.AreSame(Localization.LatestFolder_FolderName_IsMandatory, response.Report.ErrorList.FirstOrDefault());
        }

        [Test]
        public void CheckLatestFolder_FolderNameExistsError()
        {
            /* Init Rights manager */
            var rightsManagerMock = MockRepository.GenerateMock<IRightsManager>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_SyncHardLinksOnLatestFolder);
            rightsManagerMock.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), rightsManagerMock);

            /* Mock GetLatestVersionGroupedBySpecRelease */
            var specVersionsRepositoryMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionsRepositoryMock.Stub(x => x.GetLatestVersionGroupedBySpecRelease()).Return(null);
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionsRepositoryMock);

            /* Mock ILatestFolderRepository Exists */
            var latestFolderRepositoryMock = MockRepository.GenerateMock<ILatestFolderRepository>();
            latestFolderRepositoryMock.Stub(x => x.Exists(Arg<string>.Is.Anything)).Return(true);
            RepositoryFactory.Container.RegisterInstance(typeof(ILatestFolderRepository), latestFolderRepositoryMock);

            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.UoW = UoW;
            var response = ftpFoldersManager.CheckLatestFolder("test", UserRolesFakeRepository.MCC_MEMBER_ID);

            Assert.False(response.Result);
            Assert.AreSame(Localization.LatestFolder_FolderName_Already_Exist, response.Report.ErrorList.FirstOrDefault());
        }

        [Test]
        public void GetLatestVersionGroupedBySpecRelease_NominalCase() {
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;
            var SpecVersions = specVersionRepo.GetLatestVersionGroupedBySpecRelease();

            Assert.IsFalse(SpecVersions.Any(x => x.Specification.IsActive == false));
            Assert.IsFalse(SpecVersions.Any(x => x.Specification.IsUnderChangeControl.HasValue == false 
                || x.Specification.IsUnderChangeControl.Value == false));
            Assert.IsFalse(SpecVersions.Any(x => x.Release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed));
        }

        [Test]
        public void CreateAndFillVersionLatestFolderThread_NominalCase()
        {
            var folderName = "latestFolderTest";
            var ftpBasePath = ConfigVariables.FtpBasePhysicalPath;
            clearFolder(Path.Combine(ftpBasePath, "Specs", folderName));

            /* Mock GetLatestVersionGroupedBySpecRelease */
            var specVersionsRepositoryMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionsRepositoryMock.Stub(x => x.GetLatestVersionGroupedBySpecRelease()).Return(GetListSpecVersion());
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionsRepositoryMock);

            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.CreateAndFillVersionLatestFolderThread(folderName, UserRolesFakeRepository.MCC_MEMBER_ID);

            /* custom folder */
            var file1CustPath = Path.Combine(ftpBasePath, "Specs", folderName, @"14\14_series\1401-321.zip");
            var file2CustPath = Path.Combine(ftpBasePath, "Specs", folderName, @"15\15_series\1502-452.zip");

            /* latest folder */
            var file1LatestPath = Path.Combine(ftpBasePath, @"Specs\latest\14\14_series\1401-321.zip");
            var file2LatestPath = Path.Combine(ftpBasePath, @"Specs\latest\15\15_series\1502-452.zip");

            Assert.IsTrue(File.Exists(file1CustPath));
            Assert.IsTrue(File.Exists(file2CustPath));
            Assert.IsTrue(File.Exists(file1LatestPath));
            Assert.IsTrue(File.Exists(file2LatestPath));
        }

        private List<SpecVersion> GetListSpecVersion()
        {
            List<SpecVersion> result = new List<SpecVersion>();

            result.Add(new SpecVersion()
            {
                DocumentUploaded = DateTime.Now,
                Specification = new Specification()
                {
                    Number = "14.0.1"
                },
                Release = new Release()
                {
                    Code = "14"
                },
                MajorVersion = 3,
                TechnicalVersion = 2,
                EditorialVersion = 1
            });

            result.Add(new SpecVersion()
            {
                DocumentUploaded = DateTime.Now,
                Specification = new Specification()
                {
                    Number = "15.0.2"
                },
                Release = new Release()
                {
                    Code = "15"
                },
                MajorVersion = 4,
                TechnicalVersion = 5,
                EditorialVersion = 2
            });

            return result;
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    clearFolder(di.FullName);
                    di.Delete();
                }
            }
        }

        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            return iUnitOfWork;
        }
    }
}
