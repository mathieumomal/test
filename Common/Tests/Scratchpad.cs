using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests
{
    class Scratchpad
    {
        [Test]
        public void Guid()
        {
            var id1 = System.Guid.NewGuid().ToString().Substring(0,8);
            var id2 = System.Guid.NewGuid().ToString().Substring(0, 8);
            Assert.AreEqual(8, id1.Length);
            Assert.AreNotEqual(id1, id2);
        }

        [Test(Description = "Just to confirm that contains method compare by reference and not by value by default")]
        public void FindObjectInsideListByReference()
        {
            var obj1 = new SpecVersion{Fk_ReleaseId = 1};
            var obj2 = new SpecVersion { Fk_ReleaseId = 1 };
            var list = new List<SpecVersion> {obj1};

            Assert.IsFalse(list.Contains(obj2));
        }
    }
}
