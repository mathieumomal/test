using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

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

        [Test]
        public void GetSpecificationByNumber()
        {
            SpecificationFakeDBSet specFakeDBSet = new SpecificationFakeDBSet();
            specFakeDBSet.Add(new Specification() { Pk_SpecificationId = 1, Number = "23.001" });
            specFakeDBSet.Add(new Specification() { Pk_SpecificationId = 2, Number = "23.002" });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specFakeDBSet).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecificationManager manager = new SpecificationManager();
            manager.UoW = uow;
            var result = manager.GetSpecificationByNumber("23.001");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Pk_SpecificationId);
            Assert.AreEqual("23.001", result.Number);
            
            mockDataContext.VerifyAllExpectations();
        }

        #endregion

        #region Set prime and secondary responsible groups

        [Test]
        public void SetPrimeAndSecondaryResponsibleGroupsData()
        {
            var mock = MockRepository.GenerateMock<ICommunityManager>();
            mock.Stub(x => x.GetCommmunityByIds(Arg<List<int>>.Is.Anything)).Return(new List<Community>
            {
                new Community{TbId = 1, TbName = "3GPP AA", ShortName = "A", DetailsURL = "http://A.com"},
                new Community{TbId = 2, TbName = "3GPP BB", ShortName = "B", DetailsURL = "http://B.com"},
                new Community{TbId = 3, TbName = "3GPP CC", ShortName = "C", DetailsURL = "http://C.com"},
            });
            ManagerFactory.Container.RegisterInstance(mock);

            var mgr = new SpecificationManager();
            var result = mgr.SetPrimeAndSecondaryResponsibleGroupsData(new Specification{SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>
            {
                new SpecificationResponsibleGroup{Fk_commityId = 1, IsPrime = true},
                new SpecificationResponsibleGroup{Fk_commityId = 2, IsPrime = false},
                new SpecificationResponsibleGroup{Fk_commityId = 3, IsPrime = false}
            }});

            Assert.IsNotNull(result.PrimeResponsibleGroupFullName);
            Assert.AreEqual("3GPP AA", result.PrimeResponsibleGroupFullName);
            Assert.AreEqual("A", result.PrimeResponsibleGroupShortName);

            Assert.IsNotNull(result.SecondaryResponsibleGroupsFullNames);
            Assert.AreEqual("3GPP BB, 3GPP CC", result.SecondaryResponsibleGroupsFullNames);
        }

        [Test]
        public void SetParentAndChildrenPrimeResponsibleGroupsData()
        {
            var mock = MockRepository.GenerateMock<ICommunityManager>();
            mock.Stub(x => x.GetCommmunityByIds(Arg<List<int>>.Is.Anything)).Return(new List<Community>
            {
                new Community{TbId = 1, TbName = "3GPP AA", ShortName = "A", DetailsURL = "http://A.com"},
                new Community{TbId = 2, TbName = "3GPP BB", ShortName = "B", DetailsURL = "http://B.com"}
            });
            ManagerFactory.Container.RegisterInstance(mock);

            var mgr = new SpecificationManager();
            var result = mgr.SetParentAndChildrenPrimeResponsibleGroupsData(new Specification
            {
                SpecificationParents = new List<Specification>
                {
                    new Specification{SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>
                    {
                        new SpecificationResponsibleGroup{Fk_commityId = 1, IsPrime = true}
                    }}
                },
                SpecificationChilds = new List<Specification>
                {
                    new Specification{SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>
                    {
                        new SpecificationResponsibleGroup{Fk_commityId = 2, IsPrime = true}
                    }}
                }
            });

            Assert.IsNotNull(result.SpecificationParents);
            Assert.AreEqual(1, result.SpecificationParents.Count);
            Assert.AreEqual("A", result.SpecificationParents.First().PrimeResponsibleGroupShortName);

            Assert.IsNotNull(result.SpecificationChilds);
            Assert.AreEqual(1, result.SpecificationChilds.Count);
            Assert.AreEqual("B", result.SpecificationChilds.First().PrimeResponsibleGroupShortName);
        }

        #endregion

    }
}
