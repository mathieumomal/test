using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
namespace Etsi.Ultimate.Tests.Repositories
{
    public class ReleaseRepositoryTests : BaseEffortTest
    {
        [Test]
        public void GetHighestNonClosedReleaseLinkedToASpec()
        {
            var repo = RepositoryFactory.Resolve<IReleaseRepository>();
            repo.UoW = UoW;

            var result = repo.GetHighestNonClosedReleaseLinkedToASpec(136080);
            //This spec is linked to 4 releases (one is closed)

            Context.Dispose();//Context closed to be sure that we have access to the Enum_ReleaseStatus 

            Assert.AreEqual(2884, result.Pk_ReleaseId);
            Assert.AreEqual(Enum_ReleaseStatus.Open.ToString(), result.Enum_ReleaseStatus.Code);
        }
    }
}
