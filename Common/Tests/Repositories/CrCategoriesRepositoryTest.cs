using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Tests.Repositories
{
     [Category("CRCategory Tests")]
    public class CrCategoriesRepositoryTest : BaseEffortTest
    {      
        #region Tests

        [Test,Category("Change Request Category")]
        public void Repository_GetCRCategories()
        {
            var repo = new Enum_CrCategoryRepository() { UoW = UoW };
            var result = repo.All;
            Assert.AreEqual(2, result.Count());
        }
        #endregion

    }
}
