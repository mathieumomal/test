using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class SpecificationRepositoryEffortTests : BaseEffortTest
    {
        [Test]
        public void DeleteSpecification()
        {
            var repo = new SpecificationRepository { UoW = UoW };
            var specRelCount = UoW.Context.Specification_Release.Count();
            var remarksCount = UoW.Context.Remarks.Count();
            var versionsCount = UoW.Context.SpecVersions.Count();
            var wisCount = UoW.Context.Specification_WorkItem.Count();
            var historyCount = UoW.Context.Histories.Count();
            var rapporteursCount = UoW.Context.SpecificationRapporteurs.Count();
            var respGroupsCount = UoW.Context.SpecificationResponsibleGroups.Count();
            var specTechsCount = UoW.Context.SpecificationTechnologies.Count();

            var spec = UoW.Context.Specifications
                .Include(x => x.Specification_Release)
                .Include(x => x.Remarks)
                .Include(x => x.Histories)
                .Include(x => x.Versions)
                .Include(x => x.Specification_WorkItem)
                .Include(x => x.SpecificationRapporteurs)
                .Include(x => x.SpecificationResponsibleGroups)
                .Include(x => x.SpecificationTechnologies)
                .Include(x => x.SpecificationChilds)
                .Include(x => x.SpecificationParents)
                .First(s => s.Pk_SpecificationId == 200000);

            Assert.AreEqual(1, spec.Specification_Release.Count);
            Assert.AreEqual(1, spec.Remarks.Count);
            Assert.AreEqual(1, spec.Versions.Count);
            Assert.AreEqual(1, spec.Specification_WorkItem.Count);
            Assert.AreEqual(1, spec.Histories.Count);
            Assert.AreEqual(1, spec.SpecificationRapporteurs.Count);
            Assert.AreEqual(1, spec.SpecificationResponsibleGroups.Count);
            Assert.AreEqual(1, spec.SpecificationTechnologies.Count);
            Assert.AreEqual(1, spec.SpecificationChilds.Count);
            Assert.AreEqual(1, spec.SpecificationParents.Count);

            var result = repo.DeleteSpecification(200000);
            Assert.IsTrue(result);
            UoW.Save();

            Assert.IsNull(UoW.Context.Specifications.FirstOrDefault(x => x.Pk_SpecificationId == 200000));
            Assert.AreEqual(specRelCount - 1, UoW.Context.Specification_Release.Count());
            Assert.AreEqual(remarksCount - 1, UoW.Context.Remarks.Count());
            Assert.AreEqual(versionsCount - 1, UoW.Context.SpecVersions.Count());
            Assert.AreEqual(wisCount - 1, UoW.Context.Specification_WorkItem.Count());
            Assert.AreEqual(historyCount - 1, UoW.Context.Histories.Count());
            Assert.AreEqual(rapporteursCount - 1, UoW.Context.SpecificationRapporteurs.Count());
            Assert.AreEqual(respGroupsCount - 1, UoW.Context.SpecificationResponsibleGroups.Count());
            Assert.AreEqual(specTechsCount - 1, UoW.Context.SpecificationTechnologies.Count());
        }
    }
}
