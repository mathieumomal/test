using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    public class ChangeRequestGetListTests: BaseEffortTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            InitializeUserRightsMock();
        }


        [Test]
        public void GetCrList_ImplementsCorrectOrder()
        {
            var crSvc = new ChangeRequestService();
            var response = crSvc.GetChangeRequests(0, new ChangeRequestsSearch {PageSize = 2, SkipRecords = 2}).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Count);
            Assert.AreEqual("A0144", response.First().ChangeRequestNumber);
            Assert.AreEqual("A002", response.Last().ChangeRequestNumber);
        }

        
    }
}
