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

namespace Etsi.Ultimate.Tests.Services
{
    public class UrlServiceTest : BaseTest
    {

        public const string tokenExample1 = "azer";
        public const string tokenExample2 = "azer";


        [Test]
        public void GetFullUrlForToken_()
        {
            ShortUrlFakeRepository shortUrlFakeRepository = new ShortUrlFakeRepository();
            
        }  
    }
}
