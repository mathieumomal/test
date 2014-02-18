using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
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
        public List<DomainClasses.Release> GetAllReleases()
        {
            using (var UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager() { UoW = UoW };
                var releases = releaseManager.GetAllReleases();

                // No save needed
                return releases;
            }
        }

        #endregion
    }
}
