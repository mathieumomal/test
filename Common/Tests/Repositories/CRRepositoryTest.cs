using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Tests.Repositories
{
    [Category("CR Tests")]
    public class CRRepositoryTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 6;
        int specificationId = 136080;
        #endregion

        #region Tests

        [Test]
        public void Repository_CR_InsertOrUpdate()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var changeRequest1 = new ChangeRequest() { CRNumber = "234.12" };
            var changeRequest2 = new ChangeRequest() { CRNumber = "234.13" };
            repo.InsertOrUpdate(changeRequest1);
            repo.InsertOrUpdate(changeRequest2);

            Assert.AreEqual(2, UoW.Context.GetEntities<ChangeRequest>(EntityState.Added).Count());
        }

        [Test]
        public void Repository_CR_All()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var allCRs = repo.All;

            Assert.AreEqual(totalNoOfCRsInCSV, allCRs.ToList().Count);
        }
        [Test]
        public void Repository_GenerateCrNumberBySpecificationId()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var repoResult = repo.FindCrNumberBySpecificationId(specificationId);
            Assert.AreEqual(6, repoResult.Count);
        }

        [Test]
        public void Repository_GetChangeRequestById()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var repResult = repo.GetChangeRequestById(1);
            Assert.AreEqual(1, repResult.Pk_ChangeRequest); 
            Assert.AreEqual("A001", repResult.CRNumber);
            Assert.AreEqual(136080, repResult.Fk_Specification);
        }


        [Test]
        public void Repository_GetCRCategories()
        {
            var repo = new Enum_CrCategoryRepository() { UoW = UoW };
            var result = repo.All;
            Assert.AreEqual(2, result.Count());
        }

        #endregion
    }
}
