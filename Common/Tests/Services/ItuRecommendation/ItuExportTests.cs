using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Services.ItuRecommendation
{
    [TestFixture]
    class ItuExportTests: BaseTest
    {
        ItuRecommendationService ituService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            ituService = new ItuRecommendationService();

            InitializeUserRightsMock();
        }

        [Test, Description("In case no error happened, system must return path to file")]
        public void ItuExport_NominalCase()
        {
            var response = ituService.ExportItuRecommendation(UserRolesFakeRepository.SPECMGR_ID,
                "ITU-R 14.12", 1,2,65, "test.docx");

            Assert.IsNotNullOrEmpty(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());

        }

        [Test, Description("System must throw an error if user does not have right")]
        public void ItuExport_ThrowsErrorIfUserHasNoRight()
        {
            var response = ituService.ExportItuRecommendation(UserRolesFakeRepository.ANONYMOUS_ID, "test", 1, 2, 3, "test.docx");
            Assert.IsNullOrEmpty(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.RightError, response.Report.ErrorList.First());
        }
    }
}
