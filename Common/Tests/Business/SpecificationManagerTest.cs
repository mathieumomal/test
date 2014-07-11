using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Business
{
    public class SpecificationManagerTest
    {
        #region tests
        [TestCase("", false)]
        [TestCase("30.-kdd", true)]
        [TestCase("50.dzhzd987", true)]
        [TestCase("50.8zhzd987", true)]
        [TestCase("99.8zhzd987", true)]
        [TestCase("999.8zhzd987", false)]
        public void CheckInhibitedToPromote_Test(string specNumber, bool expectedResult)
        {
            var specSvc = ManagerFactory.Resolve<ISpecificationManager>();
            var result = specSvc.CheckInhibitedToPromote(specNumber);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void PutSpecAsInhibitedToPromote_Test()
        {
            //Case where the number is format to be inhibited to promote
            var spec1 = new Specification()
            {
                Pk_SpecificationId = 1,
                Number = "30.890",
                promoteInhibited = null,
                IsForPublication = null
            };
            //Case where the number isn't format to be inhibited to promote
            var spec2 = new Specification()
            {
                Pk_SpecificationId = 2,
                Number = "40.090",
                promoteInhibited = null,
                IsForPublication = null
            };

            var specSvc = ManagerFactory.Resolve<ISpecificationManager>();

            var result1 = specSvc.PutSpecAsInhibitedToPromote(spec1);
            Assert.AreEqual(result1.promoteInhibited, true);
            Assert.AreEqual(result1.IsForPublication, false);

            var result2 = specSvc.PutSpecAsInhibitedToPromote(spec2);
            Assert.AreEqual(result2.promoteInhibited, false);
            Assert.AreEqual(result2.IsForPublication, true);
        }
        #endregion

    }
}
