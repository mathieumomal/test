﻿using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Tests.Repositories
{
    [Category("CR Tests")]
    public class CrRepositoryTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 6;
        int specificationId = 136080;
        #endregion

        #region Tests

        [Test]
        public void Repository_CR_InsertOrUpdate()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var changeRequest1 = new ChangeRequest() { CRNumber = "234.12", CR_WorkItems = new List<CR_WorkItems>() { new CR_WorkItems() { Fk_WIId = 2 } } };
            var changeRequest2 = new ChangeRequest() { CRNumber = "234.13", CR_WorkItems = new List<CR_WorkItems>() { new CR_WorkItems() { Fk_WIId = 2 }, new CR_WorkItems() { Fk_WIId = 3 } } };
            repo.InsertOrUpdate(changeRequest1);
            repo.InsertOrUpdate(changeRequest2);

            Assert.AreEqual(2, UoW.Context.GetEntities<ChangeRequest>(EntityState.Added).Count());
            Assert.AreEqual(3, UoW.Context.GetEntities<CR_WorkItems>(EntityState.Added).Count());
        }

        [Test]
        public void Repository_CR_All()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var allCRs = repo.All;

            Assert.AreEqual(totalNoOfCRsInCSV, allCRs.ToList().Count);
        }
        [Test]
        public void Repository_GenerateCrNumberBySpecificationId()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var repoResult = repo.FindCrNumberBySpecificationId(specificationId);
            Assert.AreEqual(5, repoResult.Count);
        }

        [Test]
        public void Repository_GetChangeRequestById()
        {
            var repo = new ChangeRequestRepository() { UoW = UoW };
            var repResult = repo.Find(1);
            Assert.AreEqual(1, repResult.Pk_ChangeRequest); 
            Assert.AreEqual("0001", repResult.CRNumber);
            Assert.AreEqual(136081, repResult.Fk_Specification);
        }      
        #endregion
    }
}