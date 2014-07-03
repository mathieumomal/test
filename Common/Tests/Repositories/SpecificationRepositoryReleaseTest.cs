using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Repositories
{
    /// <summary>
    /// Find this method in the SpecificationRepository (because specrelease object have no repo)
    /// </summary>
    public class SpecificationRepositoryReleaseTest
    {
        [TestCase(1, 2, true)]
        [TestCase(40, 2, false)]
        [TestCase(2, 8, true)]
        public void GetSpecificationReleaseTest(int specId, int releaseId, bool resultExpected)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new SpecificationRepository();
            repo.UoW = uow;

            var specReleaseFound = repo.GetSpecificationReleaseByReleaseIdAndSpecId(specId, releaseId, true);
            if (resultExpected)
            {
                Assert.IsNotNull(specReleaseFound);
            }
            else
            {
                Assert.IsNull(specReleaseFound);
            }
        }

        [TestCase(2, 2)]
        [TestCase(1, 1)]
        public void GetAllSpecificationsByReleaseIdTest(int releaseId, int numberOfSpecExpected)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleases());
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)GetReleases());
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)GetSpecs());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new SpecificationRepository();
            repo.UoW = uow;

            var specsFound = repo.GetAllRelatedSpecificationsByReleaseId(releaseId);

            Assert.AreEqual(numberOfSpecExpected, specsFound.Count);
        }

        #region datas
        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 3, Fk_ReleaseId = 1, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 2, Fk_ReleaseId = 8, isTranpositionForced = true });

            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 4, Fk_SpecificationId = 2, Fk_ReleaseId = 2, isTranpositionForced = true });
            return list;
        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1 });
            list.Add(new Specification() { Pk_SpecificationId = 2 });
            list.Add(new Specification() { Pk_SpecificationId = 3 });
            return list;
        }

        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1 });
            list.Add(new Release() { Pk_ReleaseId = 2 });
            return list;
        }
        #endregion
    }
}
