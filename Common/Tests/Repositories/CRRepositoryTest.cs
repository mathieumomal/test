using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    [Category("CR Tests")]
    public class CrRepositoryTest : BaseEffortTest
    {
        #region Constants
        private const int TotalNoOfCrsInCsv = 10;
        private const int SpecificationId = 136080;
        #endregion

        #region Tests

        [Test]
        public void Repository_CR_InsertOrUpdate()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var changeRequest1 = new ChangeRequest { CRNumber = "234.12", CR_WorkItems = new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 } } };
            var changeRequest2 = new ChangeRequest { CRNumber = "234.13", CR_WorkItems = new List<CR_WorkItems> { new CR_WorkItems { Fk_WIId = 2 }, new CR_WorkItems { Fk_WIId = 3 } } };
            repo.InsertOrUpdate(changeRequest1);
            repo.InsertOrUpdate(changeRequest2);

            Assert.AreEqual(2, UoW.Context.GetEntities<ChangeRequest>(EntityState.Added).Count());
            Assert.AreEqual(3, UoW.Context.GetEntities<CR_WorkItems>(EntityState.Added).Count());
        }

        [Test]
        public void Repository_CR_All()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var allCRs = repo.All;

            Assert.AreEqual(TotalNoOfCrsInCsv, allCRs.ToList().Count);
        }

        [Test]
        public void Repository_CR_AllIncluding()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var allIncludingCRs = repo.AllIncluding(x => x.Enum_CRCategory);

            Assert.AreEqual(TotalNoOfCrsInCsv, allIncludingCRs.ToList().Count);
        }

        [Test]
        public void Repository_GenerateCrNumberBySpecificationId()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repoResult = repo.FindCrNumberBySpecificationId(SpecificationId);
            Assert.AreEqual(5, repoResult.Count);
        }

        [Test]
        public void Repository_GetChangeRequestById()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repResult = repo.Find(1);
            Assert.AreEqual(1, repResult.Pk_ChangeRequest); 
            Assert.AreEqual("0001", repResult.CRNumber);
            Assert.AreEqual(136081, repResult.Fk_Specification);
        }

        [Test]
        public void Repository_GetChangeRequestByContributionUID()
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var repResult = repo.GetChangeRequestByContributionUID("TSG1");
            Assert.AreEqual(1, repResult.Pk_ChangeRequest);
            Assert.AreEqual("0001", repResult.CRNumber);
            Assert.AreEqual(136081, repResult.Fk_Specification);
        } 

        #endregion
    }
}
