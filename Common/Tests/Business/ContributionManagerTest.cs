using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Business
{
    public class ContributionManagerTest:BaseEffortTest
    {
        private object[] _sourceLists = {
            new object[] {new List<int> {1}},  //case 1
            new object[] {new List<int> {1, 2}} //case 2
        };

        [Test, TestCaseSource("_sourceLists")]
        public void GetUidsForCRs(List<int> crsId)
        {
            var mock = MockRepository.GenerateMock<IChangeRequestRepository>();
            mock.Stub(x => x.FindCrsByIds(Arg<List<int>>.Is.Equal(crsId))).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(mock);

            var manager = ManagerFactory.Resolve<IContributionManager>();
            manager.GetUidsForCRs(crsId);

            mock.VerifyAllExpectations();
        }

        [TestCase(1, Description = "CR should be successfully added")]
        [TestCase(2, Description = "CR should be successfully added")]
        public void GetUidForCrPackSuccess(int crPackId)
        {
            var mock = MockRepository.GenerateMock<ICrPackRepository>();
            mock.Stub(x => x.Find(Arg<int>.Is.Equal(crPackId))).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(mock);

            var manager = ManagerFactory.Resolve<IContributionManager>();
            manager.GetUidForCrPack(crPackId);

            mock.VerifyAllExpectations();
        }

        [TestCase(-1)]
        public void GetUidForCrPackFailed(int crPackId)
        {
            var manager = ManagerFactory.Resolve<IContributionManager>();
            var response = manager.GetUidForCrPack(crPackId);
            Assert.AreEqual(response.Result, "");

        }
        
    }
}
