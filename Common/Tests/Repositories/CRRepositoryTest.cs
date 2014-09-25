using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    [Category("CR Tests")]
    public class CRRepositoryTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;

        #endregion

        #region Tests

        [Test]
        public void Repository_CR_InsertOrUpdate()
        {
            var repo = new CRRepository() { UoW = UoW };
            var changeRequest1 = new ChangeRequest() { CRNumber = "234.12" };
            var changeRequest2 = new ChangeRequest() { CRNumber = "234.13" };
            repo.InsertOrUpdate(changeRequest1);
            repo.InsertOrUpdate(changeRequest2);

            Assert.AreEqual(2, UoW.Context.GetEntities<ChangeRequest>(EntityState.Added).Count());
        }

        [Test]
        public void Repository_CR_All()
        {
            var repo = new CRRepository() { UoW = UoW };
            var allCRs = repo.All;

            Assert.AreEqual(totalNoOfCRsInCSV, allCRs.ToList().Count);
        }


        [Test]
        public void Repository_GetCRCategories()
        {
            var repo = new Enum_CRCategoryRepository() { UoW = UoW };
            var result = repo.All;
            Assert.AreEqual(2, result.Count());
        }

        #endregion
    }
}
