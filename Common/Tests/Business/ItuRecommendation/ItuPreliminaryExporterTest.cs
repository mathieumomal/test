using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    public class ItuPreliminaryExporterTest
    {
        #region Constants

        static string _exportPath;

        static string ExportFilePath
        {
            get { return _exportPath + "Q.1741_Preliminary_Test.xlsx"; }
        }

        #endregion

        #region setup
        [SetUp]
        public void SetUp()
        {
            if (String.IsNullOrEmpty(_exportPath))
                _exportPath = Environment.CurrentDirectory + "\\TestData\\ItuRecommendation\\";
        }
        
        #endregion

        #region unit tests
        [Test, Description("Eventhough there is no data, file should export with empty headers")]
        public void CreateItuPreliminaryFile_NoDatas()
        {
            var ituPreliminaryExporter = new ItuPreliminaryExporter();
            var result = ituPreliminaryExporter.CreateItuPreliminaryFile(ExportFilePath, new List<ItuPreliminaryRecord>());
            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(ExportFilePath));
        }

        [Test]
        public void CreateItuPreliminaryFile_EmptyFilePath()
        {
            var ituPreliminaryExporter = new ItuPreliminaryExporter();
            var result = ituPreliminaryExporter.CreateItuPreliminaryFile(null, new List<ItuPreliminaryRecord>());
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateItuPreliminaryFile_NominalCase()
        {
            var ituPreliminaryExporter = new ItuPreliminaryExporter();
            var result = ituPreliminaryExporter.CreateItuPreliminaryFile(ExportFilePath, new List<ItuPreliminaryRecord> { GetItuPreliminaryRecord() });

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(ExportFilePath));
        }

        [TestCase("TS", 2, 1, Description = "Type")]
        [TestCase("22.222", 2, 2, Description = "Spec number")]
        [TestCase("Test", 2, 3, Description = "Title")]
        public void CreateItuPreliminaryFile_DataTests(string expectedValue, int row, int col)
        {
            var ituPreliminaryExporter = new ItuPreliminaryExporter();
            var result = ituPreliminaryExporter.CreateItuPreliminaryFile(ExportFilePath, new List<ItuPreliminaryRecord> { GetItuPreliminaryRecord() });

            try
            {
                using (var package = new ExcelPackage(new FileInfo(ExportFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    Assert.AreEqual(expectedValue, worksheet.Cells[row, col].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured when reading document." + ex.Message);
            }
        }
        #endregion

        #region datas
        private ItuPreliminaryRecord GetItuPreliminaryRecord()
        {
            return new ItuPreliminaryRecord
            {
                SpecificationId = 1,
                SpecificationNumber = "22.222",
                Title = "Test",
                Type = "TS"
            };
        }
        #endregion
    }
}
