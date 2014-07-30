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

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecVersionUploadTest : BaseEffortTest
    {
        #region constantes
        const int USER_HAS_NO_RIGHT = 1;
        const int USER_HAS_RIGHT = 2;

        const int OPEN_RELEASE_ID = 2883;
        const int OPEN_SPEC_ID = 136080;
        #endregion

        #region tests
        [Test]
        public void Upload_Fails_If_User_Has_No_Right()
        {
            var versionSvc = new SpecVersionService();

            SetupMocks();

            var myVersion = CreateVersion();

            var result = versionSvc.UploadVersion(USER_HAS_NO_RIGHT, myVersion, "token");
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
        }

        [Test]
        public void Upload_NominalCase()
        {
            var versionSvc = new SpecVersionService();

            SetupMocks();

            var myVersion = CreateVersion();
            var result = versionSvc.UploadVersion(USER_HAS_RIGHT, myVersion, "token");
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(0, result.Report.GetNumberOfWarnings());
        }

        [Test]
        public void CheckVersionForUpload_Fails_If_User_Has_No_Right()
        {
            var versionSvc = new SpecVersionService();

            SetupMocks();

            var myVersion = CreateVersion();

            var result = versionSvc.CheckVersionForUpload(USER_HAS_NO_RIGHT, myVersion, "path");
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckVersionForUpload_NominalCase()
        {
            var versionSvc = new SpecVersionService();
            SetupMocks();
            var myVersion = CreateVersion();
            var result = versionSvc.CheckVersionForUpload(USER_HAS_RIGHT, myVersion, "path");
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(0, result.Report.GetNumberOfWarnings());
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
                Fk_ReleaseId = OPEN_RELEASE_ID,
                Fk_SpecificationId = OPEN_SPEC_ID

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
