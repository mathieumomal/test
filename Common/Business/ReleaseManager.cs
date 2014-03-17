using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;

namespace Etsi.Ultimate.Business
{
    public class ReleaseManager
    {
        // Used for the caching of the releases.
        private static string CACHE_KEY = "ULT_BIZ_RELEASES_ALL";

        public IUltimateUnitOfWork UoW { get; set; }

        public ReleaseManager() { }


        private int personId;


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

        public void CreateRelease(Release release, int previousReleaseId, int personId)
        {
            IReleaseRepository relRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            relRepo.UoW = UoW;
            List<Release> allReleases = new List<Release>();
            Release previousRelease;
            release.Histories.Add(new History
            {
                Fk_ReleaseId = release.Pk_ReleaseId,
                Fk_PersonId = personId,
                CreationDate = DateTime.Now,
                //Check for the used text
                HistoryText = "",
                PersonName = ""
            });

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
                var firstSortOrder = allReleases[0].SortOrder;
                foreach (Release r in allReleases)
                {                    
                    r.SortOrder += 10;
                    relRepo.InsertOrUpdate(r);                    
                }
                release.SortOrder = firstSortOrder;
                relRepo.InsertOrUpdate(release);
            }
            ClearCache();
        }

        public void EditRelease(Release release, int previousReleaseId, int personId)
        {
            this.personId = personId;

            IReleaseRepository relRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            relRepo.UoW = UoW;
            List<Release> allReleases = new List<Release>();
            Release previousRelease;

            var releaseToUpdate = relRepo.All.Where(r => r.Pk_ReleaseId ==release.Pk_ReleaseId).FirstOrDefault();
            UpdateReleaseAndHistory(releaseToUpdate, release);

            if (previousReleaseId != 0)
            {
                previousRelease = relRepo.Find(previousReleaseId);
                releaseToUpdate.SortOrder = previousRelease.SortOrder + 10;
                
                allReleases = relRepo.All.OrderByDescending(x => x.SortOrder).ToList();
                foreach (Release r in allReleases)
                {
                    if (r.Pk_ReleaseId == releaseToUpdate.Pk_ReleaseId)
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
                var firstSortOrder = allReleases[0].SortOrder;                
                foreach (Release r in allReleases)
                {
                    if (r.Pk_ReleaseId == releaseToUpdate.Pk_ReleaseId)
                        continue;                    
                    else
                    {
                        r.SortOrder += 10;
                        relRepo.InsertOrUpdate(r);
                    }
                }
                releaseToUpdate.SortOrder = firstSortOrder;                
            }

            // Finally save the release
            relRepo.InsertOrUpdate(releaseToUpdate);


            ClearCache();
        }

        /// <summary>
        /// Compares the old release with the new release that has been created by the UI.
        /// Logs an history entry if any change is detected.
        /// </summary>
        /// <param name="releaseToUpdate"></param>
        /// <param name="release"></param>
        private void UpdateReleaseAndHistory(Release releaseToUpdate, Release release)
        {
            // Store the list of changes
            var changes = new List<string>();
            var mtgMgr = new MeetingManager() { UoW = UoW };

            releaseToUpdate.Code = release.Code;
            releaseToUpdate.Description = release.Description;
            releaseToUpdate.Name = release.Name;
            releaseToUpdate.ShortName = release.ShortName;
            
            string newStartDate = release.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            string oldStartDate = releaseToUpdate.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            if (oldStartDate != newStartDate)
            {
                changes.Add("Start date: " + oldStartDate + " => " + newStartDate);
                releaseToUpdate.StartDate = release.StartDate;
            }

            // Freeze date 1
            releaseToUpdate.Stage1FreezeMtgId = release.Stage1FreezeMtgId;
            if (releaseToUpdate.Stage1FreezeMtgId.GetValueOrDefault() != 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage1FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage1FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage1FreezeDate = mtg.END_DATE;
                }
            }

            // Freeze stage 2
            releaseToUpdate.Stage2FreezeMtgId = release.Stage2FreezeMtgId;
            if (releaseToUpdate.Stage2FreezeMtgId.GetValueOrDefault() != 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage2FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage2FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage2FreezeDate = mtg.END_DATE;
                }
            }

            // Freeze stage 3
            releaseToUpdate.Stage3FreezeMtgId = release.Stage3FreezeMtgId;
            if (releaseToUpdate.Stage3FreezeMtgId.GetValueOrDefault() != 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage3FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage3FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage3FreezeDate = mtg.END_DATE;
                }
            }

            // End date ==> Logged
            if (release.EndMtgId != releaseToUpdate.EndMtgId)
            {
                string oldEndDate = "None";
                if (releaseToUpdate.EndMtgId.GetValueOrDefault() != 0)
                    oldEndDate = releaseToUpdate.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");

                DateTime newEndDate = new DateTime();
                string newRef = "";
                if (release.EndMtgId.GetValueOrDefault() != 0)
                {
                    var mtg = mtgMgr.GetMeetingById(release.EndMtgId.Value);
                    if (mtg != null)
                    {
                        newRef = mtg.MtgShortRef;
                        newEndDate= mtg.END_DATE.GetValueOrDefault();
                    }
                }
                changes.Add("End date: " + oldEndDate + " => " + newEndDate.ToString("yyyy-MM-dd"));

                releaseToUpdate.EndMtgId = release.EndMtgId;
                releaseToUpdate.EndDate = newEndDate;
                releaseToUpdate.EndMtgRef = newRef;
            }

            // Closure date ==> Logged
            if (releaseToUpdate.ClosureMtgId != release.ClosureMtgId)
            {
                string oldClosureDate = "None";
                if (releaseToUpdate.ClosureMtgId.GetValueOrDefault() != 0)
                    oldClosureDate = releaseToUpdate.ClosureDate.GetValueOrDefault().ToString("yyyy-MM-dd");

                DateTime newClosureDate = new DateTime();
                string newRef = "";
                if (release.ClosureMtgId.GetValueOrDefault() != 0)
                {
                    var mtg = mtgMgr.GetMeetingById(release.ClosureMtgId.Value);
                    if (mtg != null)
                    {
                        newRef = mtg.MtgShortRef;
                        newClosureDate = mtg.END_DATE.GetValueOrDefault();
                    }
                }
                changes.Add("Closure date: " + oldClosureDate + " => " + newClosureDate.ToString("yyyy-MM-dd"));

                releaseToUpdate.ClosureDate = newClosureDate;
                releaseToUpdate.ClosureMtgId = release.ClosureMtgId;
                releaseToUpdate.ClosureMtgRef = newRef;
            }
            
            // To uncomment afterwards (one thing at a time :P)
            //releaseToUpdate.Remarks = release.Remarks;

            // Admin tab
            releaseToUpdate.IturCode = release.IturCode;
            releaseToUpdate.Version2g = release.Version2g;
            releaseToUpdate.Version3g = release.Version3g;
            releaseToUpdate.WpmCode2g = release.WpmCode2g;
            releaseToUpdate.WpmCode3g = release.WpmCode3g;

            //releaseToUpdate.LAST_MOD_BY = personId;
            releaseToUpdate.LAST_MOD_TS = DateTime.Now;

            if (changes.Count >0)
            {
                var historyEntry = new History()
                {
                    Fk_PersonId = personId,
                    Release = releaseToUpdate,
                    HistoryText = "Release updated:<br /> " + string.Join("<br />", changes),
                    CreationDate = DateTime.Now
                };

                var historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
                historyRepo.UoW = UoW;
                historyRepo.InsertOrUpdate(historyEntry);
            }
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
