﻿using System;
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
        const int USER_HAS_NO_RIGHT = 1;
        const int USER_HAS_RIGHT = 2;

        const int OPEN_RELEASE_ID = 2883;
        const int OPEN_SPEC_ID = 136080;

        [Test]
        public void Allocate_Fails_If_User_Has_No_Right()
        {
            var versionSvc = new SpecVersionService();

            SetupMocks();

            var myVersion = CreateVersion();

            var result = versionSvc.UploadVersion(USER_HAS_NO_RIGHT, myVersion);
            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
        }

        [Test]
        public void Allocate_NominalCase()
        {
            var versionSvc = new SpecVersionService();

            var myVersion = CreateVersion();
            var result = versionSvc.UploadVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(0, result.Value.GetNumberOfErrors());
            Assert.AreEqual(0, result.Value.GetNumberOfWarnings());
        }

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
    }
}
