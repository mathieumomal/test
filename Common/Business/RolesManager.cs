﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public class RolesManager : IRolesManager
    {
        #region Get Email
        public List<string> GetSpecMgrEmail()
        {
            var specMgrsEmail = new List<string>();
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var specMgrsId = repo.GetSpecMgr();
            var specMgrs = personManager.GetByIds(specMgrsId);
            foreach(var specMgr in specMgrs){
                specMgrsEmail.Add(specMgr.Email);
            }
            return specMgrsEmail;
        }

        public List<string> GetWpMgrEmail()
        {
            var wpMgrsEmail = new List<string>();
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var wpMgrsId = repo.GetWpMgr();
            var wpMgrs = personManager.GetByIds(wpMgrsId);
            foreach (var wpMgr in wpMgrs)
            {
                wpMgrsEmail.Add(wpMgr.Email);
            }
            return wpMgrsEmail;
        }

        public List<string> GetSecretaryForComitteeEmail(int id)
        {
            var secretaryForComitteeEmail = new List<string>();
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var secretaryForComitteeId = repo.GetSecretaryForComittee(id);
            var secretaryForComittee = personManager.GetByIds(secretaryForComitteeId);
            foreach (var secretaryForComitteeMgr in secretaryForComittee)
            {
                secretaryForComitteeEmail.Add(secretaryForComitteeMgr.Email);
            }
            return secretaryForComitteeEmail;
        }
        #endregion


        #region Get person
        public List<View_Persons> GetSpecMgr()
        {
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var specMgrsId = repo.GetSpecMgr();
            return personManager.GetByIds(specMgrsId);
        }

        public List<View_Persons> GetWpMgr()
        {
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var wpMgrsId = repo.GetWpMgr();
            return personManager.GetByIds(wpMgrsId);
        }

        public List<View_Persons> GetSecretaryForComittee(int id)
        {
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            PersonManager personManager = new PersonManager();
            personManager.UoW = UoW;

            var secretariesForComitteeId = repo.GetSecretaryForComittee(id);
            return personManager.GetByIds(secretariesForComitteeId);
        }
        #endregion

        #region IRolesManager Membres

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        #endregion
    }

    public interface IRolesManager
    {
        /// <summary>
        /// Context to be provided in case the database needs to be targeted.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Return spec managers' email
        /// </summary>
        /// <returns></returns>
        List<String> GetSpecMgrEmail();

        /// <summary>
        /// Return workplan managers' email
        /// </summary>
        /// <returns></returns>
        List<String> GetWpMgrEmail();

        /// <summary>
        /// Return secretary's email for a comittee
        /// </summary>
        /// <returns></returns>
        List<String> GetSecretaryForComitteeEmail(int id);

        /// <summary>
        /// Return the list of spec manager
        /// </summary>
        /// <returns></returns>
        List<View_Persons> GetSpecMgr();

        /// <summary>
        /// Return the list of workplan manager
        /// </summary>
        /// <returns></returns>
        List<View_Persons> GetWpMgr();

        /// <summary>
        /// Return the list of secretary for a comittee 
        /// </summary>
        /// <returns></returns>
        List<View_Persons> GetSecretaryForComittee(int id);
    }
}
