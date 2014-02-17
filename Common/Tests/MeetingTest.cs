using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using NUnit.Framework;

namespace Tests
{
    [Category("Domain")]
    class MeetingTest
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
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ToFullReference_MustContainDash()
        {
            Meeting.ToFullReference("SA75");
        }
    }
}
