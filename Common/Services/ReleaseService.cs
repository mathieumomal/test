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

        public KeyValuePair<int, string> GetPreviousReleaseCode(int personID, int releaseId)
        {
            List<DomainClasses.Release> allReleases = GetAllReleases(personID).Key.OrderBy(x => x.SortOrder).ToList();
            int nmbrOfReleases= allReleases.Count;
            if (nmbrOfReleases > 0)
            {
                for (int i = 0; i < nmbrOfReleases; i++)
                {
                    if (allReleases[i].Pk_ReleaseId == releaseId)
                    {
                        if (i == 0)
                            break;
                        else
                            return new KeyValuePair<int, string>(allReleases[i - 1].Pk_ReleaseId, allReleases[i - 1].Code);
                    }
                }
            }
            return new KeyValuePair<int, string>(0,String.Empty);
        }

        public void FreezeRelease(int releaseId, DateTime endDate, int personId, int FreezeMtgId, string FreezeMtgRef)
        {

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                releaseManager.FreezeRelease(releaseId, endDate, personId, FreezeMtgId, FreezeMtgRef);
                uoW.Save();
            }
        }

        /// <summary>
        /// Close Release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="closureDate">Closure Date</param>
        /// <param name="closureMtgRef">Closure Meeting Reference</param>
        /// <param name="closureMtgId">Closure Meeting Reference ID</param>
        /// <param name="personID">Person ID</param>
        public void CloseRelease(int releaseId, DateTime closureDate, string closureMtgRef, int closureMtgId, int personID)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                releaseManager.CloseRelease(releaseId, closureDate, closureMtgRef, closureMtgId, personID);
                uoW.Save();
            }
        }

        /// <summary>
        /// Get the list of all releases' ids and codes except the current one
        /// An additional element is added for the case of a release that does not have any previous Release
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetAllReleasesCodes(int personId, int releaseId)
        {
            Dictionary<int, string> allReleasesCodes = new Dictionary<int, string>();
            List<DomainClasses.Release> allReleases = GetAllReleases(personId).Key.OrderByDescending(x => x.SortOrder).ToList();
            foreach (Release r in allReleases)
            {

                if (releaseId == r.Pk_ReleaseId) continue;
                allReleasesCodes.Add(r.Pk_ReleaseId, r.Code);
            }
            allReleasesCodes.Add(0, "No previous Release");
            return allReleasesCodes;
        }

        public void EditRelease(Release release, int previousReleaseId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                releaseManager.EditRelease(release, previousReleaseId, personId);
                uoW.Save();
            }
        }

        public void CreateRelease(Release release, int previousReleaseId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = new ReleaseManager();
                releaseManager.UoW = uoW;
                releaseManager.CreateRelease(release, previousReleaseId, personId);
            }
        }
        #endregion
    }
}
