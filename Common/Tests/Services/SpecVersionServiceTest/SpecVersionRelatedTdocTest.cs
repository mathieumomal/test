using System;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Linq;
using Etsi.Ultimate.Business;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionRelatedTdocTest : BaseEffortTest
    {
        [TestCase(136080, 13, 0, 1, "R2-001", true, "")]
        [TestCase(136080, 13, 0, 2, "R2-001", true, "Version does not exist. So, Tdoc cannot be linked.")]
        public void UpdateVersionRelatedTdoc_Test(int specId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc, bool isSuccess, string infoMessage)
        {
            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.UpdateVersionRelatedTdoc(specId, majorVersion, technicalVersion,
                editorialVersion, relatedTdoc);

            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
            if (!String.IsNullOrEmpty(infoMessage))
                Assert.AreEqual(infoMessage, svcResponse.Report.InfoList.First());
        }

        [Test, Description("Error while linking TDoc to Version")]
        public void UpdateVersionRelatedTdoc_Failure_Test()
        {
            const string errorMessage = "Test Exception Raised";
            var mockVersionManager = MockRepository.GenerateMock<ISpecVersionManager>();
            mockVersionManager.Stub(x => x.UpdateVersionRelatedTdoc(Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything)).Throw(new Exception(errorMessage));
            ManagerFactory.Container.RegisterInstance(typeof(ISpecVersionManager), mockVersionManager);

            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.UpdateVersionRelatedTdoc(1, 0, 1, 2, "0001");

            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(errorMessage, svcResponse.Report.ErrorList.First());
        }
    }
}
