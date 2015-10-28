using Etsi.Ultimate.Repositories;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class SpecificationRapporteurTests : BaseEffortTest
    {
        [TestCase(136082, 1, Description = "Should find one rapporteur")]
        [TestCase(136083, 0, Description = "Should not find rapporteur")]
        public void FindBySpecId(int specId, int expectedRapporteurs)
        {
            var specRapporteurRepo = RepositoryFactory.Resolve<ISpecificationRapporteurRepository>();
            specRapporteurRepo.UoW = UoW;

            Assert.AreEqual(expectedRapporteurs, specRapporteurRepo.FindBySpecId(specId).Count);
        }
    }
}
