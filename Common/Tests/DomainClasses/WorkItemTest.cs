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
    class WorkItemTest : BaseTest
    {
        [Test]
        public void GetLatestRemark()
        {
            var wi = new WorkItem()
            {
                Remarks = new List<Remark>() { 
                    new Remark() { Pk_RemarkId = 12, RemarkText="other test" },
                    new Remark() { Pk_RemarkId = 13, RemarkText="test" },
                }
            };

            Assert.AreEqual("test", wi.ShortLatestRemark);
        }
    }
}
