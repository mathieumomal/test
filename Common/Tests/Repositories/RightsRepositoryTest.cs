using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils.Core;
using NUnit.Framework;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Repositories
{
    class RightsRepositoryTest : BaseTest
    {
        private static readonly string CACHE_VALUE="ULT_REPO_RIGHTS_ALL";

        [Test]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void GetRightsPerRole_NonExistingXmlFile()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "Nowhere.xml" };
            repo.GetRightsForRoles(new List<Enum_UserRoles>());
        }

        [Test]
        public void GetRightsPerRole_LoadSimpleData()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "../../TestData/Rights/SimpleData.xml" };

            var roles = new List<Enum_UserRoles>();
            roles.Add(Enum_UserRoles.Anonymous);

            var rights = repo.GetRightsForRoles(roles);

            Assert.AreEqual(1,rights.Count);
            Assert.IsTrue(rights.Contains(Enum_UserRights.Release_ViewPartialList));

        }

        [Test]
        public void GetRightsPerRole_ReturnsNoData()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "../../TestData/Rights/SimpleData.xml" };

            var roles = new List<Enum_UserRoles>();
            roles.Add(Enum_UserRoles.Administrator);

            var rights = repo.GetRightsForRoles(roles);

            Assert.AreEqual(0, rights.Count);

        }

        [Test]
        public void GetRightsPerRole_StoresDataInCache()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "../../TestData/Rights/SimpleData.xml" };

            var roles = new List<Enum_UserRoles>();
            roles.Add(Enum_UserRoles.Administrator);

            repo.GetRightsForRoles(roles);

            Assert.IsNotNull(CacheManager.Get("ULT_REPO_RIGHTS_ALL"));
        }

        [Test]
        public void GetRightsPerRole_IgnoresInvalidData()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "../../TestData/Rights/InvalidData.xml" };

            var roles = new List<Enum_UserRoles>();
            roles.Add(Enum_UserRoles.Anonymous );

            var rights = repo.GetRightsForRoles(roles);

            Assert.AreEqual(1, rights.Count);
        }

        [Test]
        public void GetRightsPerRole_ManagesDuplicates()
        {
            CacheManager.Clear(CACHE_VALUE);
            UserRightsRepository repo = new UserRightsRepository() { XmlDocumentPath = "../../TestData/Rights/DuplicateData.xml" };

            var roles = new List<Enum_UserRoles>();
            roles.Add(Enum_UserRoles.CommitteeOfficial);
            roles.Add(Enum_UserRoles.SuperUser);

            var rights = repo.GetRightsForRoles(roles);

            Assert.AreEqual(1, rights.Count);
        }

    }
}
