using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class CrCategoriesRepository : BaseEffortTest
    {      
        #region Tests

        [Test,Category("Change Request Category")]
        public void Repository_GetCRCategories()
        {
            var repo = new Enum_CrCategoryRepository { UoW = UoW };
            var result = repo.All;
            Assert.AreEqual(2, result.Count());
        }

        [TestCase(136083, "AZE", 1, Description = "Nominal case")]
        [TestCase(136083, "AZEE", 3, Description = "Nominal case")]
        [TestCase(2, "EZA", 0, Description = "Not found")]
        [TestCase(null, null, 0, Description = "Not found")]
        public void FindCrBySpecificationIdAndCrNumber(int specId, string crNumber, int expectedResult)
        {
            var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            repo.UoW = UoW;
            var result = repo.FindCrMaxRevisionBySpecificationIdAndCrNumber(specId, crNumber);
            Assert.AreEqual(expectedResult, result);
        }
        #endregion

    }
}
