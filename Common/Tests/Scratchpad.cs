using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
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

        [Test]
        public void OrderByListOfCrs()
        {
            var list = new List<string>
            {
                "CR: 0001 - Rev: 1",
                "CR: 0000 - Rev: 2",
                "CR: 0003",
                "CR: 0002 - Rev: 345",
                "CR: 0001a",
                "CR: 0001"
            };

            var result = list.OrderBy(x => x).ToList();

            Assert.AreEqual("CR: 0000 - Rev: 2", result.ElementAt(0));
            Assert.AreEqual("CR: 0001", result.ElementAt(1));
            Assert.AreEqual("CR: 0001 - Rev: 1", result.ElementAt(2));
            Assert.AreEqual("CR: 0001a", result.ElementAt(3));
            Assert.AreEqual("CR: 0002 - Rev: 345", result.ElementAt(4));
            Assert.AreEqual("CR: 0003", result.ElementAt(5));
        }
    }
}
