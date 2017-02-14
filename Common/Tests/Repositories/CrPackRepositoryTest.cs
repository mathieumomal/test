using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class CrPackRepositoryTest : BaseEffortTest
    {

        [TestCase(1, "16")]
        [TestCase(2, "17")]
        public void Find(int crPackId, string expectedCrPackId)
        {
            var repo = new CrPackRepository { UoW = UoW };
            var result = repo.Find(crPackId);

            Assert.AreEqual(expectedCrPackId, result.uid);
        }
    }
}
