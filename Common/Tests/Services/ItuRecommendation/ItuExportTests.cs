using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.ItuRecommendation;
using Etsi.Ultimate.DomainClasses;
using NUnit.Framework;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services.ItuRecommendation
{
    /// <summary>
    /// End to end integration tests for Itu recommendations.
    /// </summary>
    [TestFixture]
    class ItuExportTests: BaseTest
    {
        ItuRecommendationService _ituService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _ituService = new ItuRecommendationService();

            InitializeUserRightsMock();

        }

        [Test, Description("In case no error happened, system must return path to file")]
        public void ItuExport_NominalCase()
        {
            // Three mocks to set up to have test working
            var emptyResponse = new List<KeyValuePair<string, string>>();
            var seedFileParserMock = MockRepository.GenerateMock<ISeedFileParser>();
            seedFileParserMock.Stub(s => s.ExtractSpecificationNumbersFromSeedFile("test.docx"))
                .Return(new ServiceResponse<List<KeyValuePair<string, string>>>{Result = emptyResponse});
            ManagerFactory.Container.RegisterInstance(typeof(ISeedFileParser),seedFileParserMock);

            var emptyItuRecordList = new List<ItuRecord>();
            var specToItuRecordConverterMock = MockRepository.GenerateMock<ISpecToItuRecordConverter>();
            specToItuRecordConverterMock.Stub(s => s.BuildItuRecordsForSpec(emptyResponse, 1, 2, 65))
                .Return(new ServiceResponse<List<ItuRecord>>{ Result = emptyItuRecordList});
            ManagerFactory.Container.RegisterInstance(typeof (ISpecToItuRecordConverter), specToItuRecordConverterMock);

            var ituRecommandationExportMock = MockRepository.GenerateMock<IItuRecommendationExporter>();
            ituRecommandationExportMock.Stub(s => 
                s.CreateItuFile(Arg<string>.Matches(path => path.Contains(ConfigVariables.DefaultPublicTmpPath+"ITU-R 14.12")), 
                                Arg<List<ItuRecord>>.Matches(l => l.Count == 0))).Return(true);
            ManagerFactory.Container.RegisterInstance(typeof (IItuRecommendationExporter), ituRecommandationExportMock);

            var response = _ituService.ExportItuRecommendation(UserRolesFakeRepository.SPECMGR_ID,
                "ITU-R 14.12", 1,2,65, "test.docx");

            Assert.IsNotNullOrEmpty(response.Result);
            Assert.IsTrue(response.Result.Contains(ConfigVariables.DefaultPublicTmpAddress+"ITU-R 14.12"));
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());

        }

        [Test, Description("System must throw an error if user does not have right")]
        public void ItuExport_ThrowsErrorIfUserHasNoRight()
        {
            var response = _ituService.ExportItuRecommendation(UserRolesFakeRepository.ANONYMOUS_ID, "test", 1, 2, 3, "test.docx");
            Assert.IsNullOrEmpty(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.RightError, response.Report.ErrorList.First());
        }
    }
}
