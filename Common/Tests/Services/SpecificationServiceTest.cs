using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using System.Data.Entity;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecificationServiceTest : BaseTest
    {
        [Test, TestCaseSource("SpecificationData")]
        public void GetSpecificationDetailsById(SpecificationFakeDBSet specificationData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);            

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
             Assert.IsTrue(mockRightsManager.GetRights(0).HasRight(Enum_UserRights.Specification_Withdraw));

            Community c = new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" };
            Community c1 = new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" };
            Community c2 = new Community() { TbId = 3, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S2", TbName = "3GPP SA 2" };

            List<Community> communitiesSet = new List<Community>() { c, c1, c2 };
            //Mock Comminities
            var mockCommunitiesManager = MockRepository.GenerateMock<ICommunityManager>();
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(1)).Return(c.ShortName);
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(2)).Return(c1.ShortName);
            mockCommunitiesManager.Stub(x => x.GetCommmunityshortNameById(3)).Return(c2.ShortName);
            mockCommunitiesManager.Stub(x => x.GetCommunities()).Return(communitiesSet);
          
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specificationData);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();            
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            ManagerFactory.Container.RegisterInstance(typeof(ICommunityManager), mockCommunitiesManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specSVC = new SpecificationService();
            KeyValuePair<Specification, UserRightsContainer> result = specSVC.GetSpecificationDetailsById(0, 1);
            Assert.IsFalse(result.Value.HasRight(Enum_UserRights.Specification_Withdraw));
            Assert.AreEqual("Withdrawn before change control", result.Key.Status);
            Assert.AreEqual("SP", result.Key.PrimeResponsibleGroupShortName);
            Assert.AreEqual("S1,S2", result.Key.SecondaryResponsibleGroupsShortNames);
        }








        [Test, TestCaseSource("GetSpecificicationNumbersTestFormat")]
        public void TestCheckNumberIsValid(string specNumber, bool expectResult, int messageCount)
        {
            var service = new SpecificationService();
            var results = service.CheckNumber(specNumber.ToString());

            Assert.AreEqual(expectResult, results.Key);
            if (!results.Key)
            {
                Assert.AreEqual(messageCount, results.Value.Count());
            }
        }


        #region data
        private IEnumerable<object[]> GetSpecificicationNumbersTestFormat
        {
            get
            {
                //Good format
                yield return new object[] { "01.0", true, 0 };
                yield return new object[] { "01.00U", true, 0 };
                yield return new object[] { "42.0UU-P", true, 0 };
                yield return new object[] { "99.Uu-P-s", true, 0 };
                yield return new object[] { "00.abc-P-", true, 0 };
                yield return new object[] { "01.abc-P-s-", true, 0 };
                //Bad format
                yield return new object[] { "test", false, 1 };
                yield return new object[] { "9.A.", false, 1 };
                yield return new object[] { "9.A.-", false, 1 };
                yield return new object[] { "xy.abc-", false, 1 };
                yield return new object[] { "xy.abc-", false, 1 };
                
            }
        }



        /// <summary>
        /// Get the WorkItem Data from csv
        /// </summary>
        public IEnumerable<SpecificationFakeDBSet> SpecificationData
        {
            get
            {
                //var specificationList = GetAllTestRecords<Spec>(Directory.GetCurrentDirectory() + "\\TestData\\WorkItems\\WorkItem.csv");
                SpecificationFakeDBSet specificationFakeDBSet = new SpecificationFakeDBSet();
                specificationFakeDBSet.Add(new Specification()
                {
                    Pk_SpecificationId = 1,
                    Number = "00.01U",
                    Title = "First specification",
                    IsTS = new Nullable<bool>(true),

                    IsActive = false,
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
                    },
                    SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>(){
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 1,
                            IsPrime = true,
                            Fk_commityId = 1,
                            Fk_SpecificationId = 1
                        },
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 2,
                            IsPrime = false,
                            Fk_commityId = 2,
                            Fk_SpecificationId = 1
                        },
                        new SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 3,
                            IsPrime = false,
                            Fk_commityId = 3,
                            Fk_SpecificationId = 1
                        }
                    }
                });

                yield return specificationFakeDBSet;
            }
        }
        #endregion

    }

}
