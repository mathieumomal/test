using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class SpecificationServiceMock : ISpecificationService
    {
        public KeyValuePair<Domain.Specification, Domain.UserRightsContainer> GetSpecificationDetailsById(int personID, int specificationId)
        {
            KeyValuePair<List<Domain.Specification>, Domain.UserRightsContainer> listSpecifications = GetAllSpecification(personID);
            return new KeyValuePair<Domain.Specification, Domain.UserRightsContainer>(listSpecifications.Key.Find(x => x.Pk_SpecificationId == specificationId), listSpecifications.Value);
        }

        public List<Enum_Technology> GetAllSpecificationTechnologies()
        {
            return new List<Enum_Technology>(){
                new Enum_Technology(){ Pk_Enum_TechnologyId=1, Code="2G", Description="2G"},
                new Enum_Technology(){ Pk_Enum_TechnologyId=2, Code="3G", Description="3G"},
                new Enum_Technology(){ Pk_Enum_TechnologyId=3, Code="LTE", Description="LTE"}
            };
        }
        

        public KeyValuePair<List<Domain.Specification>, Domain.UserRightsContainer> GetAllSpecification(int personID)
        {
            var specifications = new List<Domain.Specification>();
            var technlogies = new List<Domain.Enum_Technology>()
            {
                new Domain.Enum_Technology(){
                    Pk_Enum_TechnologyId = 1,
                    Code= "2G",
                    Description = "Second generation"
                },
                new Domain.Enum_Technology(){
                    Pk_Enum_TechnologyId = 2,
                    Code= "3G",
                    Description = "third generation"
                },
                new Domain.Enum_Technology(){
                    Pk_Enum_TechnologyId = 3,
                    Code= "LTE",
                    Description = "Long Term Evolution"
                }

            };
            var specTechnologies = new List<Domain.SpecificationTechnology>()
            {
                new Domain.SpecificationTechnology(){
                    Pk_SpecificationTechnologyId = 1,
                    Fk_Enum_Technology = 1,
                    Fk_Specification = 1,
                    Enum_Technology = new Domain.Enum_Technology()
                    {
                        Pk_Enum_TechnologyId = 1,
                        Code= "2G",
                        Description = "Second generation"
                    }
                },
                new Domain.SpecificationTechnology(){
                    Pk_SpecificationTechnologyId = 2,
                    Fk_Enum_Technology = 2,
                    Fk_Specification = 1,
                    Enum_Technology = new Domain.Enum_Technology()
                    {
                        Pk_Enum_TechnologyId = 3,
                        Code= "LTE",
                        Description = "Long Term Evolution"
                    }
                }
            };

            var remarks = new List<Domain.Remark>
            {
                new Domain.Remark(){
                    Pk_RemarkId =1,
                    RemarkText= "Remark1",
                    IsPublic = true,
                    CreationDate = DateTime.Now,
                    PersonName = "Author 1"
                },
                new Domain.Remark(){
                    Pk_RemarkId =2,
                    RemarkText= "Remark2",
                    IsPublic= true,
                    CreationDate = DateTime.Now,
                    PersonName = "Author 2"
                }
            };

            var histories = new List<Domain.History>
            {
                new Domain.History(){
                    Pk_HistoryId = 1,
                    PersonName = "Author 1",
                    HistoryText = "Text 1"
                },
                new Domain.History(){
                    Pk_HistoryId = 2,
                    PersonName = "Author 1",
                    HistoryText = "Text 2"
                }
            };

            specifications.Add(
                new Domain.Specification()
                {
                    Pk_SpecificationId = 1,
                    Number = "00.01U",
                    Title = "First specification",
                    IsTS = new Nullable<bool>(true),

                    IsActive = true,
                    IsUnderChangeControl = new Nullable<bool>(false),

                    
                    IsForPublication = new Nullable<bool>(false),
                    Remarks = new List<Domain.Remark>
                    {
                        new Domain.Remark(){
                            Pk_RemarkId =1,
                            RemarkText= "Remark1",
                            IsPublic = true,
                            CreationDate = DateTime.Now,
                            PersonName = "Author 1"
                        },
                        new Domain.Remark(){
                            Pk_RemarkId =2,
                            RemarkText= "Remark2",
                            IsPublic= true,
                            CreationDate = DateTime.Now,
                            PersonName = "Author 2"
                        }
                    },
                    SpecificationTechnologies = new List<Domain.SpecificationTechnology>()
                    {
                        new Domain.SpecificationTechnology(){
                            Pk_SpecificationTechnologyId = 1,
                            Fk_Enum_Technology = 1,
                            Fk_Specification = 1,
                            Enum_Technology = new Domain.Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 1,
                                Code= "2G",
                                Description = "Second generation"
                            }
                        },
                        new Domain.SpecificationTechnology(){
                            Pk_SpecificationTechnologyId = 2,
                            Fk_Enum_Technology = 2,
                            Fk_Specification = 1,
                            Enum_Technology = new Domain.Enum_Technology()
                            {
                                Pk_Enum_TechnologyId = 3,
                                Code= "LTE",
                                Description = "Long Term Evolution"
                            }
                        }
                    },
                    SpecificationResponsibleGroups = new List<Domain.SpecificationResponsibleGroup>()
                    {
                        new Domain.SpecificationResponsibleGroup(){
                            Pk_SpecificationResponsibleGroupId = 1,
                            Fk_commityId = 12,
                            Fk_SpecificationId = 1
                        }
                    },
                    Enum_Serie = new Domain.Enum_Serie() { Pk_Enum_SerieId = 1, Code = "S1", Description = "Serie 1" },
                    ComIMS = new Nullable<bool>(true),
                    EPS = null,
                    C_2gCommon = null,
                    CreationDate = null,
                    MOD_TS = null,
                    TitleVerified = null,
                    Fk_SerieId = 1,
                    Histories = new List<Domain.History>
                    {
                        new Domain.History(){
                            Pk_HistoryId = 1,
                            PersonName = "Author 1",
                            HistoryText = "Text 1"
                        },
                        new Domain.History(){
                            Pk_HistoryId = 2,
                            PersonName = "Author 1",
                            HistoryText = "Text 2"
                        }
                    },
                    SpecificationRapporteurs = new List<SpecificationRapporteur>(){
                        new SpecificationRapporteur(){
                            Pk_SpecificationRapporteurId = 1,
                            Fk_SpecificationId = 1,
                            IsPrime = true,
                            Fk_RapporteurId = 31
                        },
                        new SpecificationRapporteur(){
                            Pk_SpecificationRapporteurId = 2,
                            Fk_SpecificationId = 1,
                            IsPrime = false,
                            Fk_RapporteurId = 32
                        },
                        new SpecificationRapporteur(){
                            Pk_SpecificationRapporteurId = 3,
                            Fk_SpecificationId = 1,
                            IsPrime = false,
                            Fk_RapporteurId = 37
                        },
                        new SpecificationRapporteur(){
                            Pk_SpecificationRapporteurId = 4,
                            Fk_SpecificationId = 1,
                            IsPrime = false,
                            Fk_RapporteurId = 44
                        }
                    },
                    SpecificationChilds = new List<Domain.Specification>(),
                    SpecificationParents = new List<Domain.Specification>(){
                        new Domain.Specification()
                        {
                            Pk_SpecificationId = 4,
                            Number = "00.02U",
                            Title = "Third specification",
                            IsTS = new Nullable<bool>(true),

                            IsActive= true,
                            IsUnderChangeControl = new Nullable<bool>(false),

                            
                            IsForPublication = new Nullable<bool>(false),
                            Remarks = remarks,
                            SpecificationTechnologies = specTechnologies,  
                            Enum_Serie = new Domain.Enum_Serie(){ Pk_Enum_SerieId= 1, Code ="S1", Description="Serie 1"},
                            ComIMS = new Nullable<bool>(true),
                            EPS = null,
                            C_2gCommon = null ,
                            CreationDate =null,
                            MOD_TS =null,
                            TitleVerified =null,               
                            Fk_SerieId = 1,
                            Histories = histories,
                            PrimeResponsibleGroupShortName = "SP",
                            SecondaryResponsibleGroupsShortNames = "S1, S2, S3"
             
                        }   ,
                        new Domain.Specification()
                        {
                            Pk_SpecificationId = 3,
                            Number = "00.03U",
                            Title = "Third specification",
                            IsTS = new Nullable<bool>(true),

                            IsActive= true,
                            IsUnderChangeControl = new Nullable<bool>(false),

                            
                            IsForPublication = new Nullable<bool>(false),
                            Remarks = remarks,
                            SpecificationTechnologies = specTechnologies,  
                            Enum_Serie = new Domain.Enum_Serie(){ Pk_Enum_SerieId= 1, Code ="S1", Description="Serie 1"},
                            ComIMS = new Nullable<bool>(true),
                            EPS = null,
                            C_2gCommon = null ,
                            CreationDate =null,
                            MOD_TS =null,
                            TitleVerified =null,               
                            Fk_SerieId = 1,
                            Histories = histories,
                            PrimeResponsibleGroupShortName = "SP",
                            SecondaryResponsibleGroupsShortNames = "S1, S2, S3"
             
                        }
                    },
                    SpecificationInitialRelease = "Release 03",
                    PrimeResponsibleGroupShortName = "SP",
                    SecondaryResponsibleGroupsShortNames = "S1, S2, S3",
                    Specification_WorkItem = new List<Domain.Specification_WorkItem>()
                    {
                        new Domain.Specification_WorkItem(){
                            Pk_Specification_WorkItemId = 1,
                            isPrime = true,
                            WorkItem = new Domain.WorkItem(){
                                Pk_WorkItemUid = 1,
                                Acronym = "acro1",
                                WorkItems_ResponsibleGroups = new List<Domain.WorkItems_ResponsibleGroups>(){
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =1,
                                        ResponsibleGroup = "S1",
                                        IsPrimeResponsible = true
                                    },
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =2,
                                        ResponsibleGroup = "S3"
                                    }
                                }
                            }
                        },
                        new Domain.Specification_WorkItem(){
                            Pk_Specification_WorkItemId = 2,
                            isPrime = false,
                            WorkItem = new Domain.WorkItem(){
                                Pk_WorkItemUid = 2,
                                Acronym = "acro2",
                               WorkItems_ResponsibleGroups = new List<Domain.WorkItems_ResponsibleGroups>(){
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =1,
                                        ResponsibleGroup = "S2"
                                    },
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =2,
                                        ResponsibleGroup = "S1",
                                        IsPrimeResponsible = true
                                    }
                                }
                            }  
                        },
                        new Domain.Specification_WorkItem(){
                            Pk_Specification_WorkItemId = 3,
                            isPrime = false,
                            WorkItem = new Domain.WorkItem(){
                                Pk_WorkItemUid = 3,
                                Acronym = "acro3",
                               WorkItems_ResponsibleGroups = new List<Domain.WorkItems_ResponsibleGroups>(){
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =1,
                                        ResponsibleGroup = "S3"
                                    },
                                    new Domain.WorkItems_ResponsibleGroups(){
                                        Pk_WorkItemResponsibleGroups =2,
                                        ResponsibleGroup = "S2",
                                        IsPrimeResponsible = true
                                    }
                                }
                            }  
                        }
                    }                    
                });


            specifications.Add(new Domain.Specification()
            {
                Pk_SpecificationId = 4,
                Number = "00.04U",
                Title = "4th specification",
                IsTS = new Nullable<bool>(true),

                IsActive= true,
                IsUnderChangeControl = new Nullable<bool>(false),

                
                IsForPublication = new Nullable<bool>(false),
                Remarks = remarks,
                SpecificationTechnologies = specTechnologies,  
                Enum_Serie = new Domain.Enum_Serie(){ Pk_Enum_SerieId= 1, Code ="S1", Description="Serie 1"},
                ComIMS = new Nullable<bool>(true),
                EPS = null,
                C_2gCommon = null ,
                CreationDate =null,
                MOD_TS =null,
                TitleVerified =null,               
                Fk_SerieId = 1,
                Histories = histories
             
            });



            Domain.UserRightsContainer userRightsContainer = new Domain.UserRightsContainer();
            userRightsContainer.AddRight(Domain.Enum_UserRights.Specification_ViewDetails);
            userRightsContainer.AddRight(Domain.Enum_UserRights.Specification_EditFull);
            userRightsContainer.AddRight(Domain.Enum_UserRights.Specification_Withdraw);
            return new KeyValuePair<List<Domain.Specification>, Domain.UserRightsContainer>(specifications, userRightsContainer);
        }

        #region ISpecificationService Membres


        public KeyValuePair<bool, List<string>> CheckNumber(string specNumber)
        {
            throw new NotImplementedException();
        }

        public bool CheckInhibitedToPromote(string specNumber)
        {
            throw new NotImplementedException();
        }

        #endregion


        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj)
        {
            throw new NotImplementedException();
        }

        public List<Enum_Technology> GetTechnologyList()
        {
            throw new NotImplementedException();
        }

        public List<Enum_Serie> GetSeries()
        {
            throw new NotImplementedException();
        }

   
        KeyValuePair<int, Report> ISpecificationService.CreateSpecification(int personId, Specification spec)
        {
            throw new NotImplementedException();
        }


        KeyValuePair<bool, Report> ISpecificationService.EditSpecification(int personId, Specification spec)
        {
            throw new NotImplementedException();
        }

        public List<Specification> GetSpecificationBySearchCriteria(int personId, string searchString)
        {
            throw new NotImplementedException();
        }

        public List<Specification> GetSpecificationBySearchCriteriaWithExclusion(int personId, String searchString, List<string> toExclude)
        {
            throw new NotImplementedException();
        }

        public string ExportSpecification(int personId, SpecificationSearch searchobj)
        {
            throw new NotImplementedException();
        }

        #region ISpecificationService Membres
        public KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<bool, List<string>> LookForNumber(string specNumber)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ISpecificationService Members


        public bool ForceTranspositionForRelease(int personId, int releaseId, int specificationId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISpecificationService Members


        public List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISpecificationService Members


        public bool UnforceTranspositionForRelease(int personId, int releaseId, int specificationId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISpecificationService Members


        public bool WithdrawForRelease(int personId, int releaseId, int specificationId, int withdrawalMtgId)
        {
            throw new NotImplementedException();
        }

        #endregion


        public bool SpecificationInhibitPromote(int personId, int specificationId)
        {
            throw new NotImplementedException();
        }

        public bool SpecificationRemoveInhibitPromote(int personId, int specificationId)
        {
            throw new NotImplementedException();
        }


        public bool PromoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            throw new NotImplementedException();
        }
    }
}
