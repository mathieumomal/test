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
            
        } 


    }
}
