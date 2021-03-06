﻿using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class PersonManager : IPersonManager
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public PersonManager() { }

        /// <summary>
        /// <see cref="Etsi.Ultimate.Business.IPersonManager.GetPersonDisplayName"/>
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public string GetPersonDisplayName(int personID)
        {
            IPersonRepository personRepo = RepositoryFactory.Resolve<IPersonRepository>();
            personRepo.UoW = UoW;
            try
            {
                View_Persons person = personRepo.Find(personID);

                if (person != null)
                    return person.FIRSTNAME + ' ' + person.LASTNAME;
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                LogManager.Error("Finding person using id failed", ex);
                return string.Empty;
            }
        }

        #region IPersonManager Membres

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personIds"></param>
        /// <returns></returns>
        public List<View_Persons> GetByIds(List<int> personIds)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;
            List<View_Persons> listPersonsFound = repo.FindByIds(personIds);
            listPersonsFound.Reverse();
            
            var orderResult = new List<View_Persons>();
            foreach (var pId in personIds)
            {
                var personToAdd = listPersonsFound.Find(p => p.PERSON_ID == pId);
                if (personToAdd != null)
                {
                    orderResult.Add(personToAdd);
                }
            }
            orderResult.Reverse();

            return orderResult;
        }

        public List<View_Persons> LookFor(string keywords)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;

            return repo
                    .All
                    .Where(x => (x.DELETED_FLG.Equals("N") && ((x.FIRSTNAME != null && x.FIRSTNAME.ToLower().Contains(keywords.ToLower())) ||
                                                               (x.LASTNAME != null && x.LASTNAME.ToLower().Contains(keywords.ToLower())) ||
                                                               (x.FIRSTNAME != null && x.LASTNAME != null && ((x.FIRSTNAME.ToLower() + " " + x.LASTNAME.ToLower()).Contains(keywords.ToLower()) 
                                                                                                           || (x.LASTNAME.ToLower() + " " + x.FIRSTNAME.ToLower()).Contains(keywords.ToLower()))) ||
                                                               (x.Email != null && x.Email.ToLower().Contains(keywords.ToLower())))))
                                                               .ToList();
        }

        public View_Persons FindPerson(int id)
        {
            var repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;
            return repo.Find(id);
        }

        public View_Persons FindPersonDeletedOrNot(int id)
        {
            var repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;
            return repo.FindDeletedOrNot(id);
        }

        public int GetChairmanIdByCommityId(int primeResponsibleGroupId)
        {
            var repoChairman = RepositoryFactory.Resolve<IUserRolesRepository>();
            repoChairman.UoW = UoW;
            return repoChairman.GetChairmanIdByCommitteeId(primeResponsibleGroupId);

        }
        #endregion

        public List<string> GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(int primeResponsibleGroupId)
        {
            var listSecretariesEmail = new List<string>();

            IResponsibleGroupSecretaryRepository repoSecretary = RepositoryFactory.Resolve<IResponsibleGroupSecretaryRepository>();
            repoSecretary.UoW = UoW;
            var secretaries = repoSecretary.FindAllByCommiteeId(primeResponsibleGroupId).Where(x => (!x.roleExpirationDate.HasValue) || (x.roleExpirationDate >= DateTime.UtcNow)).ToList();

            foreach(var secretary in secretaries)
            {
                listSecretariesEmail.Add(secretary.Email);
            }
            return listSecretariesEmail;
        }

        
    }
}
