using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    public class ContributionServiceTest : BaseEffortTest
    {
        [TestCase(1, 0, 0, "nothing", 0, Description = "No CR-Packs found")]
        [TestCase(1, 1, 1, "16", 1, Description = "Find by uid")]
        [TestCase(1, 1, 2, "16", 0, Description = "Find by uid with incorrect tbid")]
        [TestCase(1, 2, 2, "CRPACK2", 1, Description = "Find by title")]
        [TestCase(1, 2, 1, "CRPACK2", 0, Description = "Find by title with incorrect tbid")]
        public void GetCrPacksByKeywords(int personId, int crPackIdExpected, int tbId, string keywords,
            int noOfRecordsExpected)
        {
            var service = ServicesFactory.Resolve<IContributionService>();
            var response = service.GetCrPacksByTbIdAndKeywords(personId, tbId, keywords);

            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(noOfRecordsExpected, response.Result.Count);
            if (noOfRecordsExpected > 0)
                Assert.AreEqual(crPackIdExpected, response.Result.First().pk_Contribution);
        }

        [Test]
        public void GenerateTdocListsAfterSendingCrsToCrPack()
        {
            var fakeResponse = new ServiceResponse<List<string>> {Result = new List<string> {"a","b","c"} };
            
            var mock = MockRepository.GenerateMock<IContributionManager>();
            mock.Stub(x => x.GetUidsForCRs(Arg<List<int>>.Is.Anything)).Repeat.Once().Return(fakeResponse);
            mock.Stub(x => x.GetUidForCrPack(Arg<int>.Is.Anything)).Repeat.Once().Return(new ServiceResponse<string> {Result = "d"});
            mock.Stub(
                    x =>
                        x.GenerateMeetingTdocListsAfterSendingCrsToCrPack(
                            Arg<string[]>.Is.Equal(new List<string> {"a", "b", "c", "d"}),
                            Arg<int>.Is.Anything))
                .Repeat.Once();
            ManagerFactory.Container.RegisterInstance(typeof(IContributionManager), mock);

            var service = ServicesFactory.Resolve<IContributionService>();
            service.GenerateTdocListsAfterSendingCrsToCrPack(637, 1, new List<int>{1, 2, 3});
            

            mock.VerifyAllExpectations();
        }

        [Test(Description = "Should regenerate list of tdocs only for CRs linked (unlinked) of an unknown CR-Pack (no generation of CR-Pack tdoc list because will be done to another place)")]
        public void GenerateTdocListsAfterSendingCrsToCrPack_CrPackUid_NotProvided()
        {
            var contribMgrMock = MockRepository.GenerateMock<IContributionManager>();
            contribMgrMock.Stub(
                x => x.GenerateMeetingTdocListsAfterSendingCrsToCrPack(Arg<string[]>.Is.Anything, Arg<int>.Is.Anything))
                .Return(new ServiceResponse<bool>{Result = true}).Repeat.Once();

            //Should be called anytime
            contribMgrMock.Stub(x => x.GetUidsForCRs(Arg<List<int>>.Is.Anything)).Repeat.Once().Return(new ServiceResponse<List<string>>{Result = new List<string>{"A"}});

            //Should not be called because CR-Pack UID not provided
            contribMgrMock.Stub(x => x.GetUidForCrPack(Arg<int>.Is.Anything)).Repeat.Never();

            ManagerFactory.Container.RegisterInstance(contribMgrMock);


            var service = ServicesFactory.Resolve<IContributionService>();
            service.GenerateTdocListsAfterSendingCrsToCrPack(0, 0, new List<int>{1, 2});


            contribMgrMock.VerifyAllExpectations();
        }
    }
}
