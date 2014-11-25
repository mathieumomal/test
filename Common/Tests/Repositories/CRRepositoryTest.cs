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
        private const int TotalNoOfCrsInCsv = 22;
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
            Assert.AreEqual(6, repoResult.Count);
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

        [Test]
        [TestCase(0, "WG3")]
        [TestCase(5, "WG1")]
        public void Repository_FindByWgTDoc(int result, string wgTDoc)
        {
            var repo = new ChangeRequestRepository { UoW = UoW };
            var respresult = repo.FindByWgTDoc(wgTDoc);


            if (respresult.Fk_TSGStatus == null)
            {
                Assert.AreEqual(result, 0);
                Assert.AreEqual(result, 0);
            }
            else
            {
                Assert.AreEqual(result, respresult.Fk_TSGStatus);
                Assert.AreEqual(result, respresult.Fk_TSGStatus);
            }
        }
        #endregion

        #region Get CR
        [TestCase(0, 0, 22, Description = "No paging")]
        [TestCase(0, 10, 10, Description = "Paging max 10 items")]
        [TestCase(5, 10, 10, Description = "Paging max 10 items (start from item 6)")]
        [TestCase(20, 10, 2, Description = "Paging start from item 20")]
        public void GetChangeRequestsTest_Paging(int skipRecords, int pageSize, int expectedResult)
        {
            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = pageSize, SkipRecords = skipRecords };

            //Call method
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            //Test
            Assert.AreEqual(expectedResult, result.Key.Count);
        }
        [Test]
        public void GetChangeRequestsTest_OrderBy()
        {
            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = 0, SkipRecords = 0 };

            //Call method
            var repo = new ChangeRequestRepository { UoW = UoW };
            var result = repo.GetChangeRequests(searchObj);

            //Test order by spec # (ASCENDING)
            Assert.AreEqual("22.101",result.Key.FirstOrDefault().Specification.Number);//First spec
            Assert.AreEqual("22.107", result.Key.LastOrDefault().Specification.Number);//Last spec
            //Test order by CR # (DESCENDING) (Same spec between to first CR)
            Assert.AreEqual(result.Key.ElementAt(0).Specification.Number, result.Key.ElementAt(1).Specification.Number);
            Assert.AreEqual("AC014", result.Key.ElementAt(0).CRNumber);
            Assert.AreEqual("AB013", result.Key.ElementAt(1).CRNumber);
        }
        #endregion
    }
}
