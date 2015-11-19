using Etsi.Ultimate.Repositories;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Repositories
{
    class ViewContributionsWithAditionnalDataRepositoryTests : BaseEffortTest
    {
        [TestCase(1, 2, Description = "Should find 2 contributions related to this spec")]
        [TestCase(2, 1, Description = "Should find 1 contribution related to this spec")]
        [TestCase(3, 0, Description = "Should find 0 contribution related to this spec")]
        public void FindContributionsRelatedToASpecVersion(int specId, int expectedCountResult)
        {
            var repo = RepositoryFactory.Resolve<IViewContributionsWithAditionnalDataRepository>();
            repo.UoW = UoW;

            var result = repo.FindContributionsRelatedToASpec(specId);

            Assert.AreEqual(expectedCountResult, result.Count);
        }

        [TestCase(2, 2, 0, 2, 1, Description = "Should find 1 contribution related to this spec and this version number")]
        [TestCase(2, 1, 0, 0, 0, Description = "Should find 0 contribution related to this spec and this version number")]
        public void FindContributionsRelatedToASpecAndVersionNumber(int specId, int major, int technical, int editorial, int expectedCountResult)
        {
            var repo = RepositoryFactory.Resolve<IViewContributionsWithAditionnalDataRepository>();
            repo.UoW = UoW;

            var result = repo.FindContributionsRelatedToASpecAndVersionNumber(specId, major, technical, editorial);

            Assert.AreEqual(expectedCountResult, result.Count);
        }
    }
}
