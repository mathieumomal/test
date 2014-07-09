using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class RemarkRepositoryTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("RemarkData")]
        public void RemarkRepository_GetAll(RemarkFakeDbSet remarkData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return((IDbSet<Remark>)remarkData).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new RemarkRepository();
            repo.UoW = uow;
            var allRemarks = repo.All;

            //Assert
            Assert.AreEqual(5, allRemarks.ToList().Count);
            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("RemarkData")]
        public void RemarkRepository_GetAllIncluding(RemarkFakeDbSet remarkData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return((IDbSet<Remark>)remarkData).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new RemarkRepository();
            repo.UoW = uow;
            var allRemarks = repo.AllIncluding();

            //Assert
            Assert.AreEqual(5, allRemarks.ToList().Count);
            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("RemarkData")]
        public void RemarkRepository_Find(RemarkFakeDbSet remarkData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return((IDbSet<Remark>)remarkData).Repeat.Times(3);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new RemarkRepository();
            repo.UoW = uow;
            var remark1 = repo.Find(1);
            var remark3 = repo.Find(3);
            var remark5 = repo.Find(5);

            //Assert
            Assert.AreEqual(1, remark1.Pk_RemarkId);
            Assert.AreEqual(121, remark1.Fk_PersonId);
            Assert.IsTrue(remark1.IsPublic ?? false);
            Assert.AreEqual("Remark 1", remark1.RemarkText);

            Assert.AreEqual(3, remark3.Pk_RemarkId);
            Assert.AreEqual(123, remark3.Fk_PersonId);
            Assert.IsTrue(remark3.IsPublic ?? false);
            Assert.AreEqual("Remark 3", remark3.RemarkText);

            Assert.AreEqual(5, remark5.Pk_RemarkId);
            Assert.AreEqual(125, remark5.Fk_PersonId);
            Assert.IsFalse(remark5.IsPublic ?? true);
            Assert.AreEqual("Remark 5", remark5.RemarkText);
            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot add or update a Remark entity as its own")]
        public void RemarkRepository_InsertOrUpdate()
        {
            //Arrange
            Remark remark = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };
            
            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new RemarkRepository();
            repo.UoW = uow;
            repo.InsertOrUpdate(remark);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot delete Remark entity as its own")]
        public void RemarkRepository_Delete()
        {
            //Arrange
            int remarkId = 1;

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new RemarkRepository();
            repo.UoW = uow;
            repo.Delete(remarkId);
        }

        #endregion

        #region Test Data

        /// <summary>
        /// Fake data for Remark
        /// </summary>
        public IEnumerable<RemarkFakeDbSet> RemarkData
        {
            get
            {
                // Create DBSet
                var dbSet = new RemarkFakeDbSet();
                dbSet.Add(new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" });
                dbSet.Add(new Remark() { Pk_RemarkId = 2, Fk_PersonId = 122, IsPublic = true, RemarkText = "Remark 2" });
                dbSet.Add(new Remark() { Pk_RemarkId = 3, Fk_PersonId = 123, IsPublic = true, RemarkText = "Remark 3" });
                dbSet.Add(new Remark() { Pk_RemarkId = 4, Fk_PersonId = 124, IsPublic = false, RemarkText = "Remark 4" });
                dbSet.Add(new Remark() { Pk_RemarkId = 5, Fk_PersonId = 125, IsPublic = false, RemarkText = "Remark 5" });

                yield return dbSet;
            }
        }

        #endregion
    }
}
