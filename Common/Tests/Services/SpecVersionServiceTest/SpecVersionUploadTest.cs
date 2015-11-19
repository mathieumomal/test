using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using System.IO;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Services
{
    [Category("Version_Upload")]
    public class SpecVersionUploadTest : BaseEffortTest
    {
        #region constantes
        const int USER_HAS_NO_RIGHT = 0;
        const int USER_HAS_RIGHT = 2;

        static string UPLOAD_PATH;
        #endregion

        SpecVersionService versionSvc;
        SpecVersion myVersion;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            versionSvc = new SpecVersionService();
            myVersion = CreateVersion();
            SetupMocks();

            if (String.IsNullOrEmpty(UPLOAD_PATH))
            {
                UPLOAD_PATH = Environment.CurrentDirectory + "\\TestData\\Versions\\ToUpload\\";
            }

            // Create directory structure
            var latestDraftsDirectory = "Ftp\\Specs\\latest-drafts";
            if (!Directory.Exists(latestDraftsDirectory))
                Directory.CreateDirectory(latestDraftsDirectory);

            var backupDirectory = "Ftp\\Backup\\Specs\\latest-drafts";
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            // Set up a mock for the possible email sent.
            var mailMock = MockRepository.GenerateMock<IMailManager>();
            mailMock.Stub(s => s.SendEmail(Arg<string>.Is.Anything,
                    Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything, Arg<List<string>>.Is.Anything,
                    Arg<string>.Is.Anything,Arg<string>.Is.Anything)).Return(true);
            UtilsFactory.Container.RegisterInstance<IMailManager>(mailMock);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Clear the FTP. 
            foreach (var folder in Directory.GetDirectories(Environment.CurrentDirectory+"\\Ftp\\Specs"))
            {
                Directory.Delete(folder, true);
            }

            // Clear the Backup. 
            foreach (var folder in Directory.GetDirectories(Environment.CurrentDirectory + "\\Ftp\\Backup"))
            {
                Directory.Delete(folder, true);
            }
        }
        
        #region tests
        [Test]
        public void Upload_Fails_If_User_Has_No_Right()
        {
            var result = versionSvc.UploadVersion(USER_HAS_NO_RIGHT, "token");
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
        }


        [Test]
        public void CheckVersionForUpload_Fails_If_User_Has_No_Right()
        {
            var result = versionSvc.CheckVersionForUpload(USER_HAS_NO_RIGHT, myVersion, "path");
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
        }

        /// <summary>
        /// Fixing bug: file name should be 22101-0d0300.zip, and not 22.101-0d0300.zip
        /// (hence the "." in the spec number should disappear)
        /// </summary>
        [Test]
        public void CheckVersionForUpload_Should_Rename_With_No_Dot()
        {
            // Let's try to upload version 13.3.0
            myVersion.TechnicalVersion = 3;

            var fileToUpload = UPLOAD_PATH + "22101-d30.zip";

            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(0, result.Report.WarningList.Where(w => w.Contains("Invalid file name.")).Count());
        }

        /// <summary>
        /// Version 13.1.0 is an already uploaded version.
        /// </summary>
        [Test]
        public void CheckVersionForUpload_ShouldForbidToUploadExistingUploadedVersion()
        {
            var fileToUpload = UPLOAD_PATH + "22101-d30.zip";

            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.IsTrue(result.Report.ErrorList.First().Contains(String.Format(Utils.Localization.Upload_Version_Error_Version_Already_Exists, "13.1.0")));
        }

        [Test]
        public void CheckVersionForUpload_UCC_ShouldNotAllowWhenMeetingNotProvided()
        {
            myVersion.Source = 0;

            var fileToUpload = UPLOAD_PATH + "22101-d30.zip";

            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.IsTrue(result.Report.ErrorList.First().Contains(Utils.Localization.Upload_Version_Error_Meeting_Id_Not_Provided));
        }

        [Test]
        public void CheckVersionForUpload_Draft_ShouldAllowWhenMeetingNotProvided()
        {
            myVersion = CreateDraftVersion();
            myVersion.Source = 0;

            var fileToUpload = UPLOAD_PATH + "22103-200.zip";

            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
        }

        /// <summary>
        /// Remarks concerning non conformity of the version are logged, except in the case of draft
        /// </summary>
        [Test]
        public void UploadVersion_ShouldNotLogQualityChecksInRemarksIfVersionIsDraft()
        {
            UploadDraft(CreateDraftVersion(), UPLOAD_PATH + "22103-200.zip");

            var dbVersion = Context.SpecVersions.Where(v => v.Fk_SpecificationId == myVersion.Fk_SpecificationId 
                && v.MajorVersion == myVersion.MajorVersion && v.TechnicalVersion == myVersion.TechnicalVersion
                && v.EditorialVersion == myVersion.EditorialVersion).FirstOrDefault();
            Assert.AreEqual(0, dbVersion.Remarks.Count);
        }

        /// <summary>
        /// Whenever uploading a draft, only the latest version should be kept in the Latest Draft folder.
        /// </summary>
        [Test]
        public void UploadVersion_ShouldClearOlderVersionInLatestDraftFolder()
        {
            

            // Create version 22103Version110 in the folder
            string createdFilePath="Ftp\\Specs\\latest-drafts\\22103-210.zip";
            File.Create(createdFilePath);

            // Create version 22103Version110 in the backup folder
            string createdBackupFilePath = "Ftp\\Backup\\Specs\\latest-drafts\\22103-210.zip";
            File.Create(createdBackupFilePath);

            myVersion = CreateDraftVersion();
            myVersion.TechnicalVersion = 4;
            var fileToUpload = UPLOAD_PATH + "22103-200.zip";
            UploadDraft(myVersion, fileToUpload);

            Assert.IsFalse(File.Exists(createdFilePath));
            Assert.IsFalse(File.Exists(createdBackupFilePath));
        }

        /// <summary>
        /// Whenever uploading a draft, only the oldest version should be kept in the Latest Draft folder.
        /// </summary>
        [Test]
        public void UploadVersion_ShouldNotClearMoreRecentVersionsInLatestDraftFolder()
        {
            // Create version 22103Version210 in the folder
            string createdFilePath = "Ftp\\Specs\\latest-drafts\\22103-210.zip";
            File.Create(createdFilePath);

            // Create version 22103Version210 in the backup folder
            string createdBackupFilePath = "Ftp\\Backup\\Specs\\latest-drafts\\22103-210.zip";
            File.Create(createdBackupFilePath);

            myVersion = CreateDraftVersion();
            var fileToUpload = UPLOAD_PATH + "22103-200.zip";
            UploadDraft(myVersion, fileToUpload);

            Assert.IsTrue(File.Exists(createdFilePath));
            Assert.IsTrue(File.Exists("Ftp\\Specs\\latest-drafts\\22103-210.zip"));
            Assert.IsFalse(File.Exists("Ftp\\Specs\\latest-drafts\\22103-200.zip"));

            Assert.IsTrue(File.Exists(createdBackupFilePath));
            Assert.IsTrue(File.Exists("Ftp\\Backup\\Specs\\latest-drafts\\22103-210.zip"));
            Assert.IsFalse(File.Exists("Ftp\\Backup\\Specs\\latest-drafts\\22103-200.zip"));
        }

        [Test]
        public void CheckVersion_MustAllowToUploadAllocatedVersion()
        {
            myVersion.EditorialVersion = 1;
            var fileToUpload = UPLOAD_PATH + "22103-200.zip";

            var checkResults = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(0, checkResults.Report.GetNumberOfErrors());
        }

        /// <summary>
        /// Version 13.1.0 is already uploaded, so system should allow version 13.0.2 to be uploaded.
        /// </summary>
        [Test]
        public void CheckVersion_MustAllowToUploadNonAllocatedPreviousVersion()
        {
            myVersion.TechnicalVersion = 0;
            myVersion.EditorialVersion = 2;
            var fileToUpload = UPLOAD_PATH + "22103-200.zip";

            var checkResults = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, fileToUpload);
            Assert.AreEqual(0, checkResults.Report.GetNumberOfErrors());
        }

       

        private void UploadDraft(SpecVersion version, string fileToUpload)
        {
            
            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, version, fileToUpload);
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.IsNotNullOrEmpty(result.Result);

            var uploadResult = versionSvc.UploadVersion(USER_HAS_RIGHT, result.Result);
            Assert.AreEqual(0, uploadResult.Report.GetNumberOfErrors());
        }

        
        
        #endregion

        #region datas
        private SpecVersion CreateVersion()
        {
            return new SpecVersion()
            {
                MajorVersion = 13,
                TechnicalVersion = 1,
                EditorialVersion = 0,
                Source = 22888,
                Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID,
                Fk_SpecificationId = EffortConstants.SPECIFICATION_ACTIVE_ID

            };
        }
        private SpecVersion CreateDraftVersion()
        {
            return new SpecVersion()
            {
                MajorVersion = 2,
                TechnicalVersion = 0,
                EditorialVersion = 0,
                Source = 22888,
                Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID,
                Fk_SpecificationId = EffortConstants.SPECIFICATION_DRAFT_WITH_EXISTING_DRAFTS_ID

            };
        }

        private void SetupMocks()
        {
            var noRights = new UserRightsContainer();
            var uploadRights = new UserRightsContainer();
            uploadRights.AddRight(Enum_UserRights.Versions_Upload);

            var rightsManager = MockRepository.GenerateMock<IRightsManager>();
            rightsManager.Stub(x => x.GetRights(USER_HAS_NO_RIGHT)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(USER_HAS_RIGHT)).Return(uploadRights);

            ManagerFactory.Container.RegisterInstance<IRightsManager>(rightsManager);
        }
        #endregion
    }
}
