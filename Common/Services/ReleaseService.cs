using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Linq;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the releases.
    /// </summary>
    public class ReleaseService : IReleaseService
    {

        #region IReleaseService Membres

        /// <summary>
        /// Get a pair of all releases and user's rights
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<List<DomainClasses.Release>,UserRightsContainer> GetAllReleases(int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                //Get list of releases
                return releaseManager.GetAllReleases(personId);
            }
        }

        public KeyValuePair<Release, UserRightsContainer> GetReleaseById(int personId, int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                return releaseManager.GetReleaseById(personId,releaseId); 
            }
        }

        public string GetPreviousReleaseCode(int personID, int releaseId)
        {
            List<DomainClasses.Release> allReleases = GetAllReleases(personID).Key.OrderBy(x => x.SortOrder).ToList();
            int nmbrOfReleases= allReleases.Count;
            for (int i = 0; i < nmbrOfReleases; i++)
            {
                if (allReleases[i].Pk_ReleaseId == releaseId)
                {
                    if (i > 0)
                        return allReleases[i - 1].Code;
                    else
                        break;
                }
            }
            return String.Empty;
        }

        public void FreezeRelease(int releaseId, DateTime endDate)
        {

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                releaseManager.FreezeRelease(releaseId, endDate);
            }
        }

        #endregion
    }
}
