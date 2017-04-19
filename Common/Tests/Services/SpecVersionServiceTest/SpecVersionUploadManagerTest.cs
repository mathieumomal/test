using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Business.Versions;
using System.Configuration;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version_Upload")]
    public class SpecVersionUploadManagerTest : BaseEffortTest
    {
        #region contants
        public string InitialPath_SimpleDoc = "./TestData/Versions/temp/C1-234567.doc";
        public string InitialPath_Zip = "./TestData/Versions/temp/C1-234567.zip";
        public string ArchiveFolder = ConfigVariables.UploadVersionArchiveFolder;
        public string TempUploadFolder = "./TestData/Versions/temp/";
        #endregion

        #region Init methods
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            if (!Directory.Exists(TempUploadFolder))
            {
                Directory.CreateDirectory(TempUploadFolder);
            }

            var simpleDoc = File.Create(InitialPath_SimpleDoc);
            simpleDoc.Dispose();
            var zip = File.Create(InitialPath_Zip);
            zip.Dispose();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            if (File.Exists(InitialPath_SimpleDoc))
            {
                File.Delete(InitialPath_SimpleDoc);
            }
            if (File.Exists(InitialPath_Zip))
            {
                File.Delete(InitialPath_Zip);
            }

            if (Directory.Exists(ArchiveFolder))
            {
                DirectoryInfo archiveFolder = new DirectoryInfo(ArchiveFolder);
                foreach (FileInfo file in archiveFolder.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo directory in archiveFolder.GetDirectories())
                {
                    directory.Delete(true);
                }
            }
            if (Directory.Exists(TempUploadFolder))
            {
                Directory.Delete(TempUploadFolder, true);
            }
            ConfigurationManager.AppSettings["UploadVersionSystemShouldKeepUnZipFolder"] = "True";
        }
        #endregion

        #region SpecVersionUpload MANAGER
        [Test]
        public void GetUniquePath()
        {
            //When
            var specVersionUploadMgr = ManagerFactory.Resolve<ISpecVersionUploadManager>();
            var result = specVersionUploadMgr.GetUniquePath(InitialPath_SimpleDoc);

            //Then
            Assert.IsTrue(File.Exists(result));
            Assert.AreEqual(6, Path.GetFileNameWithoutExtension(result).Length);
        }

        [Test(Description="System should archive only file uploaded in case of a ZIP, [1]without and [2]with already existing folder")]
        public void ArchiveUploadedVersion_ArchiveZip_WithAndWithoutAlreadyExistingFolder_WithoutUnzipFolder()
        {
            var specVersionUploadMgr = ManagerFactory.Resolve<ISpecVersionUploadManager>();
            ConfigurationManager.AppSettings["UploadVersionSystemShouldKeepUnZipFolder"] = "False";

            //[1]
            //Given (ArchiveUploadedVersion requires GetUniquePath step...)
            var uniquePath = specVersionUploadMgr.GetUniquePath(InitialPath_Zip);

            //When
            var result = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_Zip, uniquePath);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            var expectedResult = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.zip");
            Assert.IsTrue(File.Exists(expectedResult));
            Assert.AreEqual(expectedResult, result);

            //[2]
            var zip = File.Create(InitialPath_Zip);
            zip.Dispose();
            var uniquePathTry2 = specVersionUploadMgr.GetUniquePath(InitialPath_Zip);

            Assert.IsTrue(Directory.Exists(ArchiveFolder));

            //When
            var resultTry2 = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_Zip, uniquePathTry2);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            var expectedResultTry2 = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.zip");
            Assert.IsTrue(File.Exists(expectedResultTry2));
            Assert.AreEqual(expectedResultTry2, resultTry2);
        }

        [Test(Description = "System should archive file uploaded + unzip content in case of a ZIP, [1]without and [2]with already existing folder")]
        public void ArchiveUploadedVersion_ArchiveZip_WithAndWithoutAlreadyExistingFolder_WithUnzipFolder()
        {
            var specVersionUploadMgr = ManagerFactory.Resolve<ISpecVersionUploadManager>();
            ConfigurationManager.AppSettings["UploadVersionSystemShouldKeepUnZipFolder"] = "True";
            
            //[1]
            //Given (ArchiveUploadedVersion requires GetUniquePath step...)
            var uniquePath = specVersionUploadMgr.GetUniquePath(InitialPath_Zip);

            //Simulating action of CheckUpload method by creating fake unzip of the file content...
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(uniquePath), Path.GetFileNameWithoutExtension(uniquePath)));

            //When
            var result = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_Zip, uniquePath);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567", "unzip")));
            var expectedResult = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.zip");
            Assert.IsTrue(File.Exists(expectedResult));
            Assert.AreEqual(expectedResult, result);

            //[2]
            var zip = File.Create(InitialPath_Zip);
            zip.Dispose();
            var uniquePathTry2 = specVersionUploadMgr.GetUniquePath(InitialPath_Zip);

            //Simulating action of CheckUpload method by creating fake unzip of the file content...
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(uniquePathTry2), Path.GetFileNameWithoutExtension(uniquePathTry2)));

            Assert.IsTrue(Directory.Exists(ArchiveFolder));

            //When
            var resultTry2 = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_Zip, uniquePathTry2);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567", "unzip")));
            var expectedResultTry2 = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.zip");
            Assert.IsTrue(File.Exists(expectedResultTry2));
            Assert.AreEqual(expectedResultTry2, resultTry2);
        }

        [Test(Description = "System should archive file uploaded in case of a doc, [1]without and [2]with already existing folder")]
        public void ArchiveUploadedVersion_ArchiveDoc_WithAndWithoutAlreadyExistingFolder()
        {
            var specVersionUploadMgr = ManagerFactory.Resolve<ISpecVersionUploadManager>();

            //[1]
            //Given (ArchiveUploadedVersion requires GetUniquePath step...)
            var uniquePath = specVersionUploadMgr.GetUniquePath(InitialPath_SimpleDoc);

            //When
            var result = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_SimpleDoc, uniquePath);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            var expectedResult = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.doc");
            Assert.IsTrue(File.Exists(expectedResult));
            Assert.AreEqual(expectedResult, result);

            //[2]
            var zip = File.Create(InitialPath_SimpleDoc);
            zip.Dispose();
            var uniquePathTry2 = specVersionUploadMgr.GetUniquePath(InitialPath_SimpleDoc);

            Assert.IsTrue(Directory.Exists(ArchiveFolder));

            //When
            var resultTry2 = specVersionUploadMgr.ArchiveUploadedVersion(InitialPath_SimpleDoc, uniquePathTry2);

            //Then
            Assert.IsTrue(Directory.Exists(ArchiveFolder));
            Assert.IsTrue(Directory.Exists(Path.Combine(ArchiveFolder, "C1-234567")));
            var expectedResultTry2 = Path.Combine(ArchiveFolder, "C1-234567", "C1-234567.doc");
            Assert.IsTrue(File.Exists(expectedResultTry2));
            Assert.AreEqual(expectedResultTry2, resultTry2);
        }
        #endregion
    }
}
