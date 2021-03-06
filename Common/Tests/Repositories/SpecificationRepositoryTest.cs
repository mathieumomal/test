﻿using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class SpecificationRepositoryTest : BaseTest
    {
        [Test]
        public void GetSpecificationsWithSpecRelease()
        {
            var repo = new SpecificationRepository { UoW = GetUnitOfWork() };
            var result = repo.GetSpecificationsWithSpecRelease(new List<int> {1});

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result.First().Specification_Release.Count);
        } 

        [Test]
        public void Specifications_GetAll()
        {
            var repo = new SpecificationRepository() { UoW = GetUnitOfWork() };
            var results = repo.All.ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results[0].SpecificationResponsibleGroups.ToList().Count);
        }

        [Test]
        public void Specification_Find()
        {
            var repo = new SpecificationRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual("00.01U", repo.Find(1).Number);
            Assert.AreEqual(1, repo.Find(1).SpecificationResponsibleGroups.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).SpecificationTechnologies.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).Remarks.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).Histories.ToList().Count);
        }

        [Test, Description("Get All specifications corresponding to a list of id")]
        public void GetSpecifications_NominalCase()
        {
            var repo = new SpecificationRepository { UoW = GetSimplifiedUnitOfWork() };
            var specificationsFound = repo.GetSpecifications(new List<int>() {1, 2});
            Assert.AreEqual(2, specificationsFound.Count);
            Assert.AreEqual("00.01", specificationsFound.FirstOrDefault(x => x.Pk_SpecificationId == 1).Number);
            Assert.AreEqual("00.02", specificationsFound.FirstOrDefault(x => x.Pk_SpecificationId == 2).Number);
        }

        [Test, Description("Get All specifications corresponding to a list of id but a spec not exist : we get only the list of specs found")]
        public void GetSpecifications_ASpecNoExist()
        {
            var repo = new SpecificationRepository { UoW = GetSimplifiedUnitOfWork() };
            var specificationsFound = repo.GetSpecifications(new List<int>() { 1, 4 });
            Assert.AreEqual(1, specificationsFound.Count);
            Assert.AreEqual("00.01", specificationsFound.FirstOrDefault(x => x.Pk_SpecificationId == 1).Number);
        }

        [Test]
        public void SpecificationGetAllSearchCriteria_SearchesOnWid()
        {
            var repo = new SpecificationRepository() { UoW = GetSimplifiedUnitOfWork() };

            // By default, returns all
            var searchCriteria0 = new SpecificationSearch();
            Assert.AreEqual(2, repo.GetSpecificationBySearchCriteria(searchCriteria0, false).Value);

            // Search on dedicated, non existing WiD returns nothing.
            var searchCriteria1 = new SpecificationSearch() { WiUid = 123456789};
            Assert.AreEqual(0, repo.GetSpecificationBySearchCriteria(searchCriteria1, false).Value);

            // Search on dedicated WI.
            var searchCriteria2 = new SpecificationSearch() { WiUid = 1 };
            Assert.AreEqual(1, repo.GetSpecificationBySearchCriteria(searchCriteria2, false).Value);

            // Search on dedicated WI, but that is not userAdded.
            var searchCriteria3 = new SpecificationSearch() { WiUid = 2 };
            Assert.AreEqual(0, repo.GetSpecificationBySearchCriteria(searchCriteria3, false).Value);
        }
                                                    //(IsActive, IsUnderChangeControl)
        [TestCase(false, false, false, false, 2)]   //(null, null)
        [TestCase(true, false, false, false, 0)]    //(true, false)
        [TestCase(false, true, false, false, 1)]    //(true, true)
        [TestCase(false, false, true, false, 0)]    //(false, true)
        [TestCase(false, false, false, true, 1)]    //(false, false)
        [TestCase(false, true, true, false, 1)]    
        [TestCase(true, false, true, false, 0)]
        [TestCase(false, true, false, true, 2)]
        [TestCase(true, true, true, true, 2)]
        [TestCase(false, false, false, false, 2)]  
        public void GetSpecificationBySearchCriteria_IsActiveAndIsUnderChangeControlCases(bool isDraft, bool isUnderCC, bool isWithACC, bool isWithBCC, int expectedResult)
        {
            var repo = new SpecificationRepository() { UoW = GetSimplifiedUnitOfWork() };

            var searchCriterias = new SpecificationSearch() { IsDraft = isDraft , IsUnderCC = isUnderCC, IsWithACC = isWithACC, IsWithBCC = isWithBCC};
            Assert.AreEqual(expectedResult, repo.GetSpecificationBySearchCriteria(searchCriterias, false).Value);
        }

        [Test]
        public void SpecificationGetAllSearchCriteria_HandlesOrderingCorrectly()
        {
            var repo = new SpecificationRepository() { UoW = GetSimplifiedUnitOfWork() };

            // Check that with no order, but 1 record per page, only specification 1 is output
            var searchCriteria = new SpecificationSearch() { PageSize = 1 };
            var speclist = repo.GetSpecificationBySearchCriteria(searchCriteria, false).Key;
            Assert.AreEqual(1, speclist.Count);
            Assert.AreEqual(1, speclist.First().Pk_SpecificationId);

            // Now reverse the order, and check that specification 2 is output
            var searchCriteria2 = new SpecificationSearch() { PageSize = 1, Order = SpecificationSearch.SpecificationOrder.NumberDesc };
            var specList2 = repo.GetSpecificationBySearchCriteria(searchCriteria2, false).Key;
            Assert.AreEqual(1, specList2.Count);
            Assert.AreEqual(2, specList2.First().Pk_SpecificationId);

        }

        [Test]
        public void Specification_InsertOrUpdate()
        {
            var repo = new SpecificationRepository() { UoW = GetSimplifiedUnitOfWork() };
            var spec = new Specification() { Pk_SpecificationId = 0, Number = "12.234" };
            repo.InsertOrUpdate(spec);
            Assert.AreEqual(3, repo.All.ToList().Count);
        }

        [Test]
        public void SpecRelease_Find()
        {
            var repo = new SpecificationRepository() { UoW = GetUnitOfWork() };
            Assert.IsNotNull(repo.GetSpecificationReleaseByReleaseIdAndSpecId(1, 1, true));
            Assert.IsNull(repo.GetSpecificationReleaseByReleaseIdAndSpecId(1, 3, false));
        }

        [Test]
        public void Specification_GetSpecificationByNumber()
        {
            var repo = new SpecificationRepository() { UoW = GetUnitOfWork() };
            var spec = repo.GetSpecificationByNumber("00.01U");
            Assert.IsNotNull(spec);
            Assert.AreEqual("00.01U", spec.Number);
            Assert.AreEqual(1, spec.SpecificationResponsibleGroups.ToList().Count);
        }

        [TestCase(99999, null, Description = "Spec doesn't exist ; should return null")]
        [TestCase(1, "00.01U", Description = "Spec exists ; should return the uid")]
        public void SpecExists(int specId, string expectedResult)
        {
            var repo = new SpecificationRepository{ UoW = GetUnitOfWork() };
            var uid = repo.SpecExists(specId);
            Assert.AreEqual(expectedResult, uid);
        }


        private IUltimateUnitOfWork GetSimplifiedUnitOfWork()
        {
            var unitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var ultimateContext = new FakeContext();
            unitOfWork.Stub(u => u.Context).Return(ultimateContext);
            var specDbSet = new SpecificationFakeDBSet();
            ultimateContext.Specifications = specDbSet;

            // Now create the Specs.

            // Spec 1 is associated to no work item.
            var spec1 = new Specification()
            {
                Pk_SpecificationId = 1,
                Number = "00.01",
                Title = "Spec 1",
                IsTS = true,
                IsActive = false,
                IsUnderChangeControl = false

            };
            specDbSet.Add(spec1);

            // Spec2 is associated to wiUid 1
            var spec2 = new Specification()
            {
                Pk_SpecificationId = 2,
                Number = "00.02",
                Title = "Spec 2",
                IsTS = true,
                IsActive = true,
                IsUnderChangeControl = true,
                Specification_WorkItem = new List<Specification_WorkItem>() { 
                    new Specification_WorkItem() { Pk_Specification_WorkItemId =1, Fk_WorkItemId = 1, Fk_SpecificationId = 2, IsSetByUser = true },
                    new Specification_WorkItem() { Pk_Specification_WorkItemId =2, Fk_WorkItemId = 2, Fk_SpecificationId = 2, IsSetByUser = false},
                }
            };
            specDbSet.Add(spec2);
            


            return unitOfWork;
        }


        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var specRelease1 = new Specification_Release()
            {
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 1,
                isWithdrawn = false
            };

            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();

            var specDbSet = new SpecificationFakeDBSet();
            specDbSet.Add(new Specification()
            {
                Pk_SpecificationId = 1,
                Number = "00.01U",
                Title = "First specification",
                IsTS = true,

                IsActive = true,
                IsUnderChangeControl = new Nullable<bool>(false),

                IsForPublication = new Nullable<bool>(false),
                Remarks = new List<Remark>
                {
                    new Remark(){
                        Pk_RemarkId =1,
                        RemarkText= "Remark1",
                        IsPublic = true,
                        CreationDate = DateTime.Now,
                        PersonName = "Author 1"
                    },
                    new Remark(){
                        Pk_RemarkId =2,
                        RemarkText= "Remark2",
                        IsPublic= true,
                        CreationDate = DateTime.Now,
                        PersonName = "Author 2"
                    }
                },
                SpecificationTechnologies = new List<SpecificationTechnology>()
                {
                    new SpecificationTechnology(){
                        Pk_SpecificationTechnologyId = 1,
                        Fk_Enum_Technology = 1,
                        Fk_Specification = 1,
                        Enum_Technology = new Enum_Technology()
                        {
                            Pk_Enum_TechnologyId = 1,
                            Code= "2G",
                            Description = "Second generation"
                        }
                    },
                    new SpecificationTechnology(){
                        Pk_SpecificationTechnologyId = 2,
                        Fk_Enum_Technology = 2,
                        Fk_Specification = 1,
                        Enum_Technology = new Enum_Technology()
                        {
                            Pk_Enum_TechnologyId = 3,
                            Code= "LTE",
                            Description = "Long Term Evolution"
                        }
                    }
                },
                SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>()
                {
                    new SpecificationResponsibleGroup(){
                        Pk_SpecificationResponsibleGroupId = 1,
                        Fk_commityId = 12,
                        Fk_SpecificationId = 1
                    }
                },
                Enum_Serie = new Enum_Serie() { Pk_Enum_SerieId = 1, Code = "S1", Description = "Serie 1" },
                ComIMS = new Nullable<bool>(true),
                EPS = null,
                C_2gCommon = null,
                CreationDate = null,
                MOD_TS = null,
                TitleVerified = null,
                Fk_SerieId = 1,
                
                Histories = new List<History>
                {
                    new History(){
                        Pk_HistoryId = 1,
                        PersonName = "Author 1",
                        HistoryText = "Text 1"
                    },
                    new History(){
                        Pk_HistoryId = 2,
                        PersonName = "Author 1",
                        HistoryText = "Text 2"
                    }
                },
                Specification_Release = new List<Specification_Release>{specRelease1}
            });

            iUltimateContext.Specifications = specDbSet;


            // Generate spec release

            var specRel = new SpecificationReleaseFakeDBSet();
            specRel.Add(specRelease1);
            specRel.Add(new Specification_Release() { Fk_ReleaseId = 2, Fk_SpecificationId = 2, isWithdrawn = true});
            iUltimateContext.Specification_Release = specRel;



            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }
    }
}
