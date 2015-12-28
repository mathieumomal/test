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
    }
}
