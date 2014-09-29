using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Etsi.Ultimate.Tests.FakeSets;
using System.Data.Entity;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
   public class CrCategoriesManagerTest:BaseEffortTest
    {
        #region Constants       
        private const string CACHE_KEY = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";      
        #endregion

        #region Tests          
   
        [Test, TestCaseSource("GetCRCategoryData")]
        public void Business_ChangeRequestCategory(IDbSet<Enum_CRCategory> changeRequestCategories)
        {
            var mockCRCategoryRepository = MockRepository.GenerateMock<IEnum_CrCategoryRepository>();
            mockCRCategoryRepository.Stub(x => x.All).Return(changeRequestCategories);
            RepositoryFactory.Container.RegisterInstance(typeof(IEnum_CrCategoryRepository), mockCRCategoryRepository);

            //Act
            var crManager = new CrCategoriesManager();
            crManager.UoW = UoW;
            var result = crManager.GetChangeRequestCategories();
            //Assert
            var svcChangeRequestCategory = (List<Enum_CRCategory>)CacheManager.Get(CACHE_KEY);

            Assert.IsNotNull(svcChangeRequestCategory);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("CR", result[0].Code);
        }

        #endregion
        #region Data object

        /// <summary>
        /// Provide Enum_CRCategory Data
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDbSet<Enum_CRCategory>> GetCRCategoryData
        {
            get
            {
                var crCategoryDBSet = new Enum_CRCategoryFakeDbSet();
                crCategoryDBSet.Add(new Enum_CRCategory() { Pk_EnumCRCategory = 1, Code = "CR", Description = "Change Request" });
                crCategoryDBSet.Add(new Enum_CRCategory() { Pk_EnumCRCategory = 1, Code = "CD", Description = "Change Description" });
                yield return (IDbSet<Enum_CRCategory>)crCategoryDBSet;
            }
        }
        #endregion
    }
}
