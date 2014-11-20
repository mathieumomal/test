using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    public class ItuRecommendationExporterTest
    {
        #region Constants

        static string EXPORT_PATH;

        #endregion
        
        [SetUp]
        public void SetUp()
        {
            if (String.IsNullOrEmpty(EXPORT_PATH))
                EXPORT_PATH = Environment.CurrentDirectory + "\\TestData\\ItuRecommendation\\";
        }

        [Test]
        public void CreateItuFile_EmptyFilePath()
        {
            var ituRecommendationExporter = new ItuRecommendationExporter();
            var ituRecords = new List<ItuRecord>();
            var result = ituRecommendationExporter.CreateItuFile(String.Empty, ituRecords);
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateItuFile_NullItuRecords()
        {
            var ituRecommendationExporter = new ItuRecommendationExporter();
            var result = ituRecommendationExporter.CreateItuFile("Temp.xlsx", null);
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateItuFile_NominalCase()
        {
            var fileToExport = EXPORT_PATH + "ITU-R 2001_Test.xlsx";
            var ituRecords = new List<ItuRecord>();
            var ituRecord = new ItuRecord() {
                ClauseNumber = "5.1.2.1.1",
                SpecificationNumber = "25.201",
                Title = "3G Security; Report on the design and evaluation of the MILENAGE algorithm set; Deliverable 5: An example algorithm for the 3GPP authentication and key generation functions",
                Sdo = "ETSI",
                SdoVersionReleaase = "Release 99",
                SdoReference = "ETSI TS 125 201",
                SpecVersionNumber = "3.4.0",
                VersionPublicationStatus = "Published",
                PublicationDate = "2002-07-05",
                Hyperlink = "http://www.etsi.org/deliver/etsi_ts/125200_125299/125201/03.04.00_60/"
            };
            ituRecords.Add(ituRecord);
            var ituRecommendationExporter = new ItuRecommendationExporter();
            var result = ituRecommendationExporter.CreateItuFile(fileToExport, ituRecords);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(fileToExport));
        }
    }
}
