﻿using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the releases.
    /// </summary>
    public class ReleaseService : IReleaseService
    {

        #region IReleaseService Membres

        /// <summary>
        /// Returns the list of all releases.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<List<DomainClasses.Release>,UserRightsContainer> GetAllReleases(int personID)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                //Get list of releases
                return releaseManager.GetAllReleases(personID);
            }
        }

        public Release GetReleaseById(int id)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                var release = releaseManager.GetReleaseById(id);

                return release;
            }
        }

        #endregion
    }
}
