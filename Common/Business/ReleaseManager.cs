using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class ReleaseManager
    {
        // Used for the caching of the releases.
        private static string CACHE_KEY = "ULT_BIZ_RELEASES_ALL";

        public IUltimateUnitOfWork UoW { get; set; }

        public ReleaseManager() { }



        /// <summary>
        /// Retrieves all the data for the releases.
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<List<Release>, UserRightsContainer> GetAllReleases(int personID)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personID);

            // Check in the cache
            var cachedData = (List<Release>)CacheManager.Get(CACHE_KEY);
            if (cachedData == null)
            {
                // if nothing in the cache, ask the repository, then cache it
                IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
                repo.UoW = UoW;
                cachedData = repo.All.ToList();

                // Check that cache is still empty
                if (CacheManager.Get(CACHE_KEY) == null)
                    CacheManager.Insert(CACHE_KEY, cachedData);
            }
            return new KeyValuePair<List<Release>, UserRightsContainer>(cachedData, personRights); ;
        }


        /// <summary>
        /// Retrieves the data from one single release, and computes the associated rights.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KeyValuePair<Release, UserRightsContainer> GetReleaseById(int personId, int id)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
            repo.UoW = UoW;
            var release = repo.AllIncluding(t => t.Remarks, t => t.Histories).Where(r => r.Pk_ReleaseId == id).FirstOrDefault();

            if (release == null)
                return new KeyValuePair<Release, UserRightsContainer>(null, null);

            // remove some rights depending on release status:
            // - a frozen release cannot be frozen.
            // - a closed release can be neither frozen nor closed
            if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Frozen)
            {
                personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
            }
            else if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
            {
                personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                personRights.RemoveRight(Enum_UserRights.Release_Close, true);
            }

            return new KeyValuePair<Release, UserRightsContainer>(repo.Find(id), personRights);
        }


        /// <summary>
        /// Freezes the release.
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="endDate"></param>
        public void FreezeRelease(int releaseId, DateTime endDate, int personId, int FreezeMtgId, string FreezeMtgRef)
        {
            IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
            repo.UoW = UoW;

            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;
            var frozen = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Frozen).FirstOrDefault();

            var updatedObj = repo.Find(releaseId);
            updatedObj.Fk_ReleaseStatus = frozen.Enum_ReleaseStatusId;
            updatedObj.Enum_ReleaseStatus = null;
            updatedObj.EndDate = endDate;
            updatedObj.EndMtgRef = FreezeMtgRef;
            updatedObj.EndMtgId = FreezeMtgId;
            


            repo.InsertOrUpdate(updatedObj);

            History history = new History() { Fk_ReleaseId = releaseId, Fk_PersonId = personId, CreationDate = DateTime.UtcNow, HistoryText = Utils.Localization.History_Release_Freeze };
            historyRepo.InsertOrUpdate(history);

            ClearCache();
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
            IReleaseRepository relRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            relRepo.UoW = UoW;

            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;

            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;

            var updatedObj = relRepo.Find(releaseId);

            updatedObj.Fk_ReleaseStatus = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Closed).FirstOrDefault().Enum_ReleaseStatusId;
            updatedObj.Enum_ReleaseStatus = null; //Set Null to avoid referential integrity error
            updatedObj.ClosureDate = closureDate;
            updatedObj.ClosureMtgRef = closureMtgRef;
            updatedObj.ClosureMtgId = closureMtgId;

            relRepo.InsertOrUpdate(updatedObj);

            History history = new History() { Fk_ReleaseId = releaseId, Fk_PersonId = personID, CreationDate = DateTime.UtcNow, HistoryText = Utils.Localization.History_Release_Close };
            historyRepo.InsertOrUpdate(history);

            ClearCache();
        }

        public void CreateRelease(Release release, int previousReleaseId)
        {
            IReleaseRepository relRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            relRepo.UoW = UoW;
            List<Release> allReleases = new List<Release>();
            Release previousRelease;


            if (previousReleaseId != 0)
            {
                previousRelease = relRepo.Find(previousReleaseId);
                release.SortOrder = previousRelease.SortOrder + 10;
                allReleases = relRepo.All.OrderByDescending(x => x.SortOrder).ToList();
                foreach (Release r in allReleases)
                {                    
                    if (r.SortOrder <= previousRelease.SortOrder)
                        break;
                    if (r.SortOrder > previousRelease.SortOrder)
                    {
                        r.SortOrder += 10;
                        relRepo.InsertOrUpdate(r);
                    }
                }
                relRepo.InsertOrUpdate(release);
            }
            else
            {
                allReleases = relRepo.All.OrderByDescending(x => x.SortOrder).ToList();
                var firstSortOrder = allReleases[allReleases.Count - 1].SortOrder;
                foreach (Release r in allReleases)
                {                    
                    r.SortOrder += 10;
                    relRepo.InsertOrUpdate(r);                    
                }
                release.SortOrder = firstSortOrder;
                relRepo.InsertOrUpdate(release);
            }


            relRepo.UoW.Save();

            ClearCache();
        }

        public void EditRelease(Release release, int previousReleaseId)
        {
            IReleaseRepository relRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            relRepo.UoW = UoW;
            List<Release> allReleases = new List<Release>();
            Release previousRelease;


            if (previousReleaseId != 0)
            {
                previousRelease = relRepo.Find(previousReleaseId);
                release.SortOrder = previousRelease.SortOrder + 10;
                relRepo.InsertOrUpdate(release);
                allReleases = relRepo.All.OrderByDescending(x => x.SortOrder).ToList();
                foreach (Release r in allReleases)
                {
                    if (r.Pk_ReleaseId == release.Pk_ReleaseId)
                        continue;
                    if (r.Pk_ReleaseId == previousRelease.Pk_ReleaseId)
                        break;
                    if (r.SortOrder > previousRelease.SortOrder)
                    {
                        r.SortOrder += 10;
                        relRepo.InsertOrUpdate(r);
                    }
                }
            }
            else
            {
                allReleases = relRepo.All.OrderByDescending(x => x.SortOrder).ToList();
                var firstSortOrder = allReleases[allReleases.Count-1].SortOrder;                
                foreach (Release r in allReleases)
                {
                    if (r.Pk_ReleaseId == release.Pk_ReleaseId)
                        continue;                    
                    else
                    {
                        r.SortOrder += 10;
                        relRepo.InsertOrUpdate(r);
                    }
                }
                release.SortOrder = firstSortOrder;                
                relRepo.InsertOrUpdate(release);
            }
            

            relRepo.UoW.Save();

            ClearCache();
        }

        /// <summary>
        /// Clears the cache each time an action is performed on the releases.
        /// </summary>
        private void ClearCache()
        {
            CacheManager.Clear(CACHE_KEY);
        }
    }
}
