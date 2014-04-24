using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class SpecificationRepositoryTest : BaseTest
    {
        [Test]
        public void Specifications_GetAll()
        {
            var repo = new SpecificationRepository(GetUnitOfWork());
            var results = repo.All.ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results[0].SpecificationResponsibleGroups.ToList().Count);
        }

        

        [Test]
        public void Specification_Find()
        {
            var repo = new SpecificationRepository(GetUnitOfWork());
            Assert.AreEqual("00.01U", repo.Find(1).Number);
            Assert.AreEqual(1, repo.Find(1).SpecificationResponsibleGroups.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).SpecificationTechnologies.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).Remarks.ToList().Count);
            Assert.AreEqual(2, repo.Find(1).Histories.ToList().Count);
        }


        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            //var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var specDbSet = new SpecificationFakeDBSet();
            specDbSet.Add(new Specification()
            {
                Pk_SpecificationId = 1,
                Number = "00.01U",
                Title = "First specification",
                IsTS = new Nullable<bool>(true),

                IsActive = true,
                IsUnderChangeControl = new Nullable<bool>(false),

                Enum_SpecificationStage = new Enum_SpecificationStage()
                {
                    Pk_Enum_SpecificationStage = 1,
                    Code = "Stage 3",
                    Description = "Third stage"
                },
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
                Fk_SpecificationStageId = 1,
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
                }
            });

            iUltimateContext.Specifications = specDbSet;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }
    }
}
