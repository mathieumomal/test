using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class ShortUrlRepositoryTest : BaseTest
    {
        public const string tokenExample_exist = "azer";
        public const string tokenExample_dontexist = "azex";

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindByToken_noResult()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var results = repo.FindByToken(tokenExample_dontexist);
        }

        [Test]
        public void FindByToken_oneFind()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var results = repo.FindByToken(tokenExample_exist);
            Assert.AreEqual(results.Token, tokenExample_exist);
        }

        [Test]
        public void InsertOrUpdate_add()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var shorturl = new ShortUrl();
            shorturl.Token = tokenExample_dontexist;
            shorturl.Url = "adresse";
            repo.InsertOrUpdate(shorturl);
            Assert.AreEqual(3, repo.All.Count());
        }
        



        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();

            var suDbSet = new ShortUrlFakeDBSet();
            suDbSet.Add(new ShortUrl()
            {
                Pk_Id = 1,
                Token = tokenExample_exist,
                Url = "/release.ascx?params"
            });
            suDbSet.Add(new ShortUrl()
            {
                Pk_Id = 2,
                Token = tokenExample_exist,
                Url = "/release.ascx?params"
            });

            iUltimateContext.ShortUrls = suDbSet;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }
    }
}
