
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

namespace Etsi.Ultimate.Tests.Business
{
    [Category("CR Tests")]
    public class CRManagerTest : BaseEffortTest
    {
        #region Constants

        private const int totalNoOfCRsInCSV = 0;
        private const int personID = 0;
        private const string CACHE_KEY = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";
        private int specificationId = 136080;

        #endregion

        #region Tests

        [Test]
        public void Business_CreateChangeRequest_Success()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCRRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsTrue(result);
        }
             
        [Test]
        public void Business_CreateChangeRequest_Failure()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            var mockCRRepository = MockRepository.GenerateMock<IChangeRequestRepository>();
            mockCRRepository.Stub(x => x.InsertOrUpdate(Arg<ChangeRequest>.Is.Anything)).Throw(new System.Exception());
            RepositoryFactory.Container.RegisterInstance(typeof(IChangeRequestRepository), mockCRRepository);

            //Act
            var crManager = new ChangeRequestManager();
            var result = crManager.CreateChangeRequest(personID, changeRequest);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Business_CrNumberGeneration()
        {
            //Arrange
            ChangeRequest changeRequest = new ChangeRequest();
            changeRequest.Fk_Specification = specificationId;
            var crManager = new ChangeRequestManager();
            crManager.UoW = UoW;
            //Act
            var result = crManager.GenerateCrNumber(changeRequest.Fk_Specification);
            //Assert
            Assert.AreNotSame("AC144", result);
        }

        [Test, TestCaseSource("GetCRCategoryData")]
        public void Business_ChangeRequestCategory(IDbSet<Enum_CRCategory> changeRequestCategories)
        {
            var mockCRCategoryRepository = MockRepository.GenerateMock<IEnum_CrCategoryRepository>();
            mockCRCategoryRepository.Stub(x => x.All).Return(changeRequestCategories);
            RepositoryFactory.Container.RegisterInstance(typeof(IEnum_CrCategoryRepository), mockCRCategoryRepository);

            //Act
            var crManager = new ChangeRequestManager();
            crManager.UoW = UoW;
            var result = crManager.GetChangeRequestCategories(personID);
            //Assert
            var svcChangeRequestCategory = (List<Enum_CRCategory>)CacheManager.Get(CACHE_KEY);

            Assert.IsNotNull(svcChangeRequestCategory);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("CR", result[0].Code);

        }

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
