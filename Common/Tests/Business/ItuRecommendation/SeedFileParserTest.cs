using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;
using System;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    public class SeedFileParserTest
    {
        #region Constants

        static string SEED_FILE_DIRECTORY;

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            if (String.IsNullOrEmpty(SEED_FILE_DIRECTORY))
            {
                SEED_FILE_DIRECTORY = Environment.CurrentDirectory + "\\TestData\\ItuRecommendation\\";
            }
        } 

        #endregion

        #region Tests

        [Test]
        public void ExtractSpecificationNumbersFromSeedFile_NotExists()
        {
            var seedFilePath = SEED_FILE_DIRECTORY + "SeedFile1.docx";
            var seedFileParser = new SeedFileParser();
            var result = seedFileParser.ExtractSpecificationNumbersFromSeedFile(seedFilePath);
            Assert.AreEqual(1, result.Report.ErrorList.Count);
            Assert.AreEqual("Seed file 'SeedFile1.docx' not present", result.Report.ErrorList[0]);
        }

        [Test]
        public void ExtractSpecificationNumbersFromSeedFile_InvalidExtension()
        {
            var seedFilePath = SEED_FILE_DIRECTORY + "InvalidSeedFile.doc";
            var seedFileParser = new SeedFileParser();
            var result = seedFileParser.ExtractSpecificationNumbersFromSeedFile(seedFilePath);
            Assert.AreEqual(1, result.Report.ErrorList.Count);
            Assert.AreEqual("Seed file should be in 'docx' format", result.Report.ErrorList[0]);
        }

        [TestCase("SeedFile.docx", 813)]
        [TestCase("SeedFile-Tr.docx", 2)]
        [TestCase("SeedFile-Ts.docx", 6)]
        [TestCase("SeedFile-TrAndTs.docx", 3)]
        [TestCase("SeedFile-NoValidHeadings.docx", 0)]
        public void ExtractSpecificationNumbersFromSeedFile(string fileName, int result)
        {
            var seedFilePath = SEED_FILE_DIRECTORY + fileName;
            var seedFileParser = new SeedFileParser();
            var specList = seedFileParser.ExtractSpecificationNumbersFromSeedFile(seedFilePath);
            Assert.AreEqual(0, specList.Report.ErrorList.Count);
            Assert.AreEqual(result, specList.Result.Count);
        } 

        #endregion
    }
}
