using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Rhino.Mocks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.DataAccess;
using System.Data.Entity;
using Etsi.Ultimate.Services;

namespace Etsi.Ultimate.Tests.Services
{
    public class UrlServiceTest : BaseTest
    {

        public const string tokenExample1 = "azer1";
        public const string tokenExample2 = "azer2";
        public const string tokenExample_dontexist = "azer8";

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            var mock = MockRepository.GenerateMock<IUrlRepository>();
            mock.Stub(m => m.GetTabIdAndPageNameForModuleId(1)).Return(new KeyValuePair<int, string>(12, "//WorkItem"));

            RepositoryFactory.Container.RegisterInstance(typeof(IUrlRepository), mock);
        }
        
        [Test]
        public void GetFullUrlForToken_exist()
        {
            RepositoryFactory.Container.RegisterType<IUrlRepository, ShortUrlFakeRepository>(new TransientLifetimeManager());
            var shorUrlService = new UrlService();
            string url1 = shorUrlService.GetFullUrlForToken(tokenExample1);
            string url2 = shorUrlService.GetFullUrlForToken(tokenExample2);
            Assert.AreEqual("address1", url1);
            Assert.AreEqual("address2", url2);

        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetFullUrlForToken_dontexist()
        {
            RepositoryFactory.Container.RegisterType<IUrlRepository, ShortUrlFakeRepository>(new TransientLifetimeManager());

            var shorUrlService = new UrlService();
            string url = shorUrlService.GetFullUrlForToken(tokenExample_dontexist);
        }

        [Test]
        public void CreateShortUrl()
        {
            RepositoryFactory.Container.RegisterType<IUrlRepository, ShortUrlFakeRepository>(new TransientLifetimeManager());
            var repo = new ShortUrlFakeRepository();

            var service = new UrlService();

            var urlParams = new Dictionary<string, string>() { { "p1", "v1" }, { "p2", "v2" }, { "TabID", "13" } };

            var shortUrl = service.CreateShortUrl(1, "http://portal.3gpp.org", urlParams);

            //get token
            string[] el = shortUrl.Split('=');
            var getFullUrlInDb = service.GetFullUrlForToken(el.Last());

            
        }

        [Test]
        public void GetPageIdAndFullAddressForModule_NominalCase()
        {
            var service = new UrlService();

            var urlParams = new Dictionary<string, string>() { { "p1", "v1" }, { "p2", "v2" }, {"TabID","13"} };

            var result = service.GetPageIdAndFullAddressForModule(1, "http://portal.3gpp.org", urlParams);

            Assert.AreEqual(12, result.Key);
            Assert.AreEqual("http://portal.3gpp.org/WorkItem.aspx?p1=v1&p2=v2", result.Value);
        }

        [Test]
        public void GetPageIdAndFullAddressForModule_ModuleIdNotFound()
        {
            var service = new UrlService();

            var urlParams = new Dictionary<string, string>() { { "p1", "v1" }, { "p2", "v2" }, { "tabId", "13" } };

            var result = service.GetPageIdAndFullAddressForModule(13, "http://portal.3gpp.org", urlParams);

            Assert.AreEqual(0, result.Key);
            Assert.IsNull(result.Value);
        }

    }
}
