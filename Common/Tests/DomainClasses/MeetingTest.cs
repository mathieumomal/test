using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.DomainClasses
{
    [Category("Domain")]
    class MeetingTest : BaseTest
    {
        [Test]
        public void ToFullReference_OnPlenary()
        {
            Assert.AreEqual("3GPPSA#65", Meeting.ToFullReference("SP-65"));
            Assert.AreEqual("3GPPSMG#65", Meeting.ToFullReference("SMG-65"));
            Assert.AreEqual("3GPPGSM#65", Meeting.ToFullReference("GSM-65"));
            Assert.AreEqual("3GPPCT#65", Meeting.ToFullReference("CP-65"));
            Assert.AreEqual("3GPPCN#65", Meeting.ToFullReference("NP-65"));
            Assert.AreEqual("3GPPGERAN#65", Meeting.ToFullReference("GP-65"));
            Assert.AreEqual("3GPPRAN#65", Meeting.ToFullReference("RP-65"));
            Assert.AreEqual("3GPPT#65", Meeting.ToFullReference("TP-65"));
            Assert.AreEqual("3GPPPCG#65", Meeting.ToFullReference("PCG-65"));
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ToFullReference_MustContainDash()
        {
            Meeting.ToFullReference("SA75");
        }

        [Test]
        public void ToShortReference_OnPlenary()
        {
            Assert.AreEqual("SP-65", Meeting.ToShortReference("3GPPSA#65"));
            Assert.AreEqual("SMG-65", Meeting.ToShortReference("3GPPSMG#65"));
            Assert.AreEqual("GSM-65", Meeting.ToShortReference("3GPPGSM#65"));
            Assert.AreEqual("CP-65", Meeting.ToShortReference("3GPPCT#65"));
            Assert.AreEqual("NP-65", Meeting.ToShortReference("3GPPCN#65"));
            Assert.AreEqual("GP-65", Meeting.ToShortReference("3GPPGERAN#65"));
            Assert.AreEqual("RP-65", Meeting.ToShortReference("3GPPRAN#65"));
            Assert.AreEqual("TP-65", Meeting.ToShortReference("3GPPT#65"));
            Assert.AreEqual("PCG-65", Meeting.ToShortReference("3GPPPCG#65"));
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ToShortReference_MustContainHash()
        {
            Meeting.ToShortReference("SA75");
        }
    }
}
