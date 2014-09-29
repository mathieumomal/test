using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using System.Collections.Generic;
using System;

namespace Etsi.Ultimate.Tests.Services
{
    public class CrCategoriesServiceTest : BaseEffortTest
    {

        #region Tests
        [Test, Category("Change Request Category")]
        public void Service_UnitTest_GetchangeRequestCategories()
        {
            //Arrange
            var mockChangeRequestCategories = MockRepository.GenerateMock<ICrCategoriesManager>();
            var changeRequestCategories = ChangeRequestCategoryDataObject();
            mockChangeRequestCategories.Stub(x => x.GetChangeRequestCategories()).Return(changeRequestCategories);
            ManagerFactory.Container.RegisterInstance(typeof(ICrCategoriesManager), mockChangeRequestCategories);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            //Act
            var crCategoryService = new CrCategoriesService();
            var result = crCategoryService.GetChangeRequestCategories();
            //Assert
            Assert.IsTrue(result.Key);

        }

        [Test]
        public void Service_IntegrationTest_GetchangeRequestCategories()
        {
            //Act
            var crCategoryService = new CrCategoriesService();
            var result = crCategoryService.GetChangeRequestCategories();
            //Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("CR", result.Value[0].Code);
        }

        #endregion

        #region DataObject

        private static List<Enum_CRCategory> ChangeRequestCategoryDataObject()
        {
            var changeRequestCategories = new List<Enum_CRCategory>
            {
                new Enum_CRCategory { Pk_EnumCRCategory = 1, Code = "CR", Description = "Change Request" },
                new Enum_CRCategory { Pk_EnumCRCategory = 2, Code = "PCR", Description = "Pack Change Request" },
            };
            return changeRequestCategories;
        }
        #endregion
    }
}
