using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class ReleaseManager : Etsi.Ultimate.Business.IReleaseManager
    {
        #region Variables / Properties

        // Used for the caching of the releases.
        private static string CACHE_KEY = "ULT_BIZ_RELEASES_ALL";
        private int personId;
        private IReleaseRepository releaseRepo;
        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region Constructor

        public ReleaseManager() {}

        #endregion

        #region IReleaseManager Membres

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
                releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                cachedData = releaseRepo.All.ToList();

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

            releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;
            var release = releaseRepo.AllIncluding(t => t.Remarks, t => t.Histories, t => t.Specification_Release).Where(r => r.Pk_ReleaseId == id).FirstOrDefault();

            if (release == null)
                return new KeyValuePair<Release, UserRightsContainer>(null, null);

            // remove some rights depending on release status:
            // - a frozen release cannot be frozen.
            // - a closed release can be neither frozen nor closed
            if (release.Enum_ReleaseStatus == null)
            {
                var releaseStatus = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
                releaseStatus.UoW = UoW;
                var frozen = releaseStatus.All.Where(x => x.Enum_ReleaseStatusId == 2).FirstOrDefault();
                var closed = releaseStatus.All.Where(x => x.Enum_ReleaseStatusId == 3).FirstOrDefault();
                if (release.Fk_ReleaseStatus == frozen.Enum_ReleaseStatusId)
                {
                    personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                }
                else if (release.Fk_ReleaseStatus == closed.Enum_ReleaseStatusId)
                {
                    personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                    personRights.RemoveRight(Enum_UserRights.Release_Close, true);
                }
            }
            else
            {
                if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Frozen)
                {
                    personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                }
                else if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
                {
                    personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                    personRights.RemoveRight(Enum_UserRights.Release_Close, true);
                }
            }
            
            return new KeyValuePair<Release, UserRightsContainer>(releaseRepo.Find(id), personRights);
        }

        /// <summary>
        /// Freezes the release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="endDate">End Date</param>
        /// <param name="personId">Person ID</param>
        /// <param name="FreezeMtgId">Freeze Meeting ID</param>
        /// <param name="FreezeMtgRef">Freeze Meeting Reference</param>
        public void FreezeRelease(int releaseId, DateTime? endDate, int personId, int? FreezeMtgId, string FreezeMtgRef)
        {
            releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;

            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            ISpecVersionManager versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;

            
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;

            var frozen = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Frozen).FirstOrDefault();

            var updatedObj = releaseRepo.Find(releaseId);

            string oldEndDate = (updatedObj.EndDate != null) ? updatedObj.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd") : String.Empty;
            string newEndDate = (endDate != null) ? endDate.GetValueOrDefault().ToString("yyyy-MM-dd") : String.Empty;
            string historyText = String.Format(Utils.Localization.History_Release_Freeze,
                ((oldEndDate != newEndDate) || (updatedObj.EndMtgRef != FreezeMtgRef))
                ? String.Format("Changes:<br />End date: {0} ({1}) => {2} ({3})", oldEndDate, updatedObj.EndMtgRef ?? "None", newEndDate, FreezeMtgRef ?? "None")
                : String.Empty);

            updatedObj.Fk_ReleaseStatus = frozen.Enum_ReleaseStatusId;
            updatedObj.Enum_ReleaseStatus = null;
            updatedObj.EndDate = endDate;
            updatedObj.EndMtgRef = FreezeMtgRef;
            updatedObj.EndMtgId = FreezeMtgId;

            releaseRepo.InsertOrUpdate(updatedObj);

            History history = new History() { Fk_ReleaseId = releaseId, Fk_PersonId = personId, CreationDate = DateTime.UtcNow, HistoryText = historyText };
            historyRepo.InsertOrUpdate(history);

            //Transposition
            var transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
            transposeMgr.UoW = UoW;
            //Get release related specs
            var relatedSpecs = specMgr.GetSpecsRelatedToARelease(updatedObj.Pk_ReleaseId);
            foreach (var spec in relatedSpecs)
            {
                //for each of these specs which are UCC and "forPublication"
                if ((spec.IsUnderChangeControl ?? false) && (spec.IsForPublication ?? false))
                {
                    var versions = versionMgr.GetVersionsForASpecRelease(spec.Pk_SpecificationId, updatedObj.Pk_ReleaseId);
                    //We get "the latest version of this spec"
                    var latestVersion = versions.OrderByDescending(x => x.MajorVersion ?? 0)
                                        .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                        .ThenByDescending(z => z.EditorialVersion ?? 0)
                                        .FirstOrDefault();
                    //"is allocated and not yet uploaded"
                    if (latestVersion != null && latestVersion.ETSI_WKI_ID == null)
                    {
                        transposeMgr.Transpose(spec, latestVersion);
                    }
                }
            }
            //Transposition

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
        public void CloseRelease(int releaseId, DateTime? closureDate, string closureMtgRef, int? closureMtgId, int personID)
        {
            releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;

            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;

            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;

            var updatedObj = releaseRepo.Find(releaseId);

            string oldClosureDate = (updatedObj.ClosureDate != null) ? updatedObj.ClosureDate.GetValueOrDefault().ToString("yyyy-MM-dd") : String.Empty;
            string newClosureDate = (closureDate != null) ? closureDate.GetValueOrDefault().ToString("yyyy-MM-dd") : String.Empty;
            string historyText = String.Format(Utils.Localization.History_Release_Close,
                ((oldClosureDate != newClosureDate) || (updatedObj.ClosureMtgRef != closureMtgRef))
                ? String.Format("Changes:<br />Closure date: {0} ({1}) => {2} ({3})", oldClosureDate, updatedObj.ClosureMtgRef ?? "None" , newClosureDate, closureMtgRef ?? "None")
                : String.Empty);

            updatedObj.Fk_ReleaseStatus = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Closed).FirstOrDefault().Enum_ReleaseStatusId;
            updatedObj.Enum_ReleaseStatus = null; //Set Null to avoid referential integrity error
            updatedObj.ClosureDate = closureDate;
            updatedObj.ClosureMtgRef = closureMtgRef;
            updatedObj.ClosureMtgId = closureMtgId;

            releaseRepo.InsertOrUpdate(updatedObj);

            History history = new History() { Fk_ReleaseId = releaseId, Fk_PersonId = personID, CreationDate = DateTime.UtcNow, HistoryText = historyText };
            historyRepo.InsertOrUpdate(history);

            ClearCache();
        }

        /// <summary>
        /// Creates the release
        /// </summary>
        /// <param name="release">Release</param>
        /// <param name="previousReleaseId">Previous Release ID</param>
        /// <param name="personId">Person ID</param>
        /// <returns>New Release</returns>
        public Release CreateRelease(Release release, int previousReleaseId, int personId)
        {
            ClearCache();

            releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;

            this.personId = personId;
            // Synchronize the release with a new release. This will help ensure all the dates are present
            // Then cleanup the history and add the correct entry.
            var aReleaseToAdd = new Release();
            UpdateReleaseAndHistory(aReleaseToAdd, release);
            aReleaseToAdd.Histories.Clear();

            // Add the release status
            var enumStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            enumStatusRepo.UoW = UoW;
            aReleaseToAdd.Fk_ReleaseStatus = enumStatusRepo.All.Where(e => e.Code == Enum_ReleaseStatus.Open).FirstOrDefault().Enum_ReleaseStatusId;
            
            aReleaseToAdd.Histories.Add(new History
            {
                Fk_ReleaseId = release.Pk_ReleaseId,
                Fk_PersonId = personId,
                CreationDate = DateTime.Now,
                HistoryText = Utils.Localization.History_Release_Created,
            });

            ManageSortOrder(aReleaseToAdd, previousReleaseId);

            releaseRepo.InsertOrUpdate(aReleaseToAdd);
            
            return aReleaseToAdd;
        }

        /// <summary>
        /// Edits the release
        /// </summary>
        /// <param name="release">Release</param>
        /// <param name="previousReleaseId">Previous Release ID</param>
        /// <param name="personId">Person ID</param>
        public void EditRelease(Release release, int previousReleaseId, int personId)
        {
            // We are in edit mode, therefore we do not want the cache to be targeted.
            ClearCache();
            
            // Initializing
            this.personId = personId;
            releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;
            
            // Fetch the release in database and synchronize the fields.
            var releaseToUpdate = releaseRepo.Find(release.Pk_ReleaseId);
            UpdateReleaseAndHistory(releaseToUpdate, release);

            ManageSortOrder(releaseToUpdate, previousReleaseId);

            // Finally save the release
            releaseRepo.InsertOrUpdate(releaseToUpdate);
        }

        /// <summary>
        /// Lists all the releases
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <returns>Release Codes</returns>
        public Dictionary<int, string> GetAllReleasesCodes(int releaseId)
        {
            Dictionary<int, string> allReleasesCodes = new Dictionary<int, string>();
            // Check in the cache
            var cachedData = (List<Release>)CacheManager.Get(CACHE_KEY);
            if (cachedData == null)
            {
                // if nothing in the cache, ask the repository, then cache it
                releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                cachedData = releaseRepo.All.ToList();                
                // Check that cache is still empty
                if (CacheManager.Get(CACHE_KEY) == null)
                    CacheManager.Insert(CACHE_KEY, cachedData);
            }
            //Excluding the current Release and Sorting the collection
            cachedData = cachedData.Where(r => r.Pk_ReleaseId != releaseId).OrderByDescending(r => r.SortOrder).ToList();
            foreach (Release r in cachedData)
            {
                allReleasesCodes.Add(r.Pk_ReleaseId, r.Code);
            }
            //A rlease that does not have any previous one
            allReleasesCodes.Add(0, "No previous Release");
            return allReleasesCodes;
        }

        /// <summary>
        /// Computes the release code for the previous release.
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <returns>Previous Release Code</returns>
        public KeyValuePair<int, string> GetPreviousReleaseCode(int releaseId)
        {
            // Check in the cache
            var cachedData = (List<Release>)CacheManager.Get(CACHE_KEY);
            if (cachedData == null)
            {
                // if nothing in the cache, ask the repository, then cache it
                releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                cachedData = releaseRepo.All.ToList();
                // Check that cache is still empty
                if (CacheManager.Get(CACHE_KEY) == null)
                    CacheManager.Insert(CACHE_KEY, cachedData);
            }
            //Sorting the collection
            cachedData = cachedData.OrderBy(r => r.SortOrder).ToList();
            int nmbrOfReleases = cachedData.Count;
            if (nmbrOfReleases > 0)
            {
                
                for (int i = 0; i < nmbrOfReleases; i++)
                {
                    if (cachedData[i].Pk_ReleaseId == releaseId)
                    {
                        if (i == 0)
                            //If the current Release is the first one ==> no previous Release 
                            break;
                        else
                            //Once the current Release is reached, the previous one is returned (id and code)
                            return new KeyValuePair<int, string>(cachedData[i - 1].Pk_ReleaseId, cachedData[i - 1].Code);
                    }
                }
            }
            return new KeyValuePair<int, string>(0, String.Empty);
        }

        /// <summary>
        /// Get next release details for the given current release id.
        /// </summary>
        /// <param name="releaseId">Relese ID</param>
        /// <returns>Next Release</returns>
        public Release GetNextRelease(int releaseId)
        {
            var allReleases = GetAllReleases();
            var currentRelease = allReleases.Where(x => x.Pk_ReleaseId == releaseId).FirstOrDefault();
            int currentReleaseSortOrder = (currentRelease == null) ? 0 : (currentRelease.SortOrder ?? 0);
            var nextRelease = allReleases.Where(x => x.SortOrder > currentReleaseSortOrder).OrderBy(y => y.SortOrder).FirstOrDefault();
            return nextRelease;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get All Release Details
        /// </summary>
        /// <returns>List of Releases</returns>
        private List<Release> GetAllReleases()
        {
            // Check in the cache
            var cachedData = (List<Release>)CacheManager.Get(CACHE_KEY);
            if (cachedData == null)
            {
                // if nothing in the cache, ask the repository, then cache it
                releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                cachedData = releaseRepo.All.ToList();

                // Check that cache is still empty
                if (CacheManager.Get(CACHE_KEY) == null)
                    CacheManager.Insert(CACHE_KEY, cachedData);
            }
            return cachedData;
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
            if (releaseToUpdate.Stage1FreezeMtgId.GetValueOrDefault() > 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage1FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage1FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage1FreezeDate = mtg.END_DATE;
                }
            }
            else
            {
                releaseToUpdate.Stage1FreezeMtgId = null;
                releaseToUpdate.Stage1FreezeDate = null;
                releaseToUpdate.Stage1FreezeMtgRef = null;
            }

            // Freeze stage 2
            releaseToUpdate.Stage2FreezeMtgId = release.Stage2FreezeMtgId;
            if (releaseToUpdate.Stage2FreezeMtgId.GetValueOrDefault() > 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage2FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage2FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage2FreezeDate = mtg.END_DATE;
                }
            }
            else
            {
                releaseToUpdate.Stage2FreezeMtgId = null;
                releaseToUpdate.Stage2FreezeDate = null;
                releaseToUpdate.Stage2FreezeMtgRef = null;
            }

            // Freeze stage 3
            releaseToUpdate.Stage3FreezeMtgId = release.Stage3FreezeMtgId;
            if (releaseToUpdate.Stage3FreezeMtgId.GetValueOrDefault() > 0)
            {
                var mtg = mtgMgr.GetMeetingById(releaseToUpdate.Stage3FreezeMtgId.Value);
                if (mtg != null)
                {
                    releaseToUpdate.Stage3FreezeMtgRef = mtg.MtgShortRef;
                    releaseToUpdate.Stage3FreezeDate = mtg.END_DATE;
                }
            }
            else
            {
                releaseToUpdate.Stage3FreezeMtgId = null;
                releaseToUpdate.Stage3FreezeDate = null;
                releaseToUpdate.Stage3FreezeMtgRef = null;
            }

            // End date ==> Logged
            if (release.EndMtgId.GetValueOrDefault() != releaseToUpdate.EndMtgId.GetValueOrDefault())
            {
                // Compute old end date.
                string oldEndDate = "";
                string oldEndMeeting = "None";
                if (releaseToUpdate.EndMtgId.GetValueOrDefault() > 0)
                {
                    oldEndDate = releaseToUpdate.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    oldEndMeeting = releaseToUpdate.EndMtgRef;
                }
                string newEndDateStr = "";
                string newEndMtgStr = "None";
                if (release.EndMtgId.GetValueOrDefault() > 0)
                {
                    var mtg = mtgMgr.GetMeetingById(release.EndMtgId.Value);
                    if (mtg != null)
                    {
                        release.EndMtgRef = mtg.MtgShortRef;
                        release.EndDate = mtg.END_DATE.GetValueOrDefault();
                        newEndDateStr = release.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        newEndMtgStr = release.EndMtgRef;
                    }
                }
                else
                    release.EndMtgId = 0;
                changes.Add(String.Format("End date: {0} ({1}) => {2} ({3})",
                    oldEndDate, oldEndMeeting, newEndDateStr, newEndMtgStr));

                releaseToUpdate.EndMtgId = release.EndMtgId;
                releaseToUpdate.EndDate = release.EndDate;
                releaseToUpdate.EndMtgRef = release.EndMtgRef;
            }

            // Closure date ==> Logged
            if (releaseToUpdate.ClosureMtgId.GetValueOrDefault() != release.ClosureMtgId.GetValueOrDefault())
            {
                string oldClosureDate = "";
                string oldClosureMtg = "None";
                if (releaseToUpdate.ClosureMtgId.GetValueOrDefault() > 0)
                {
                    oldClosureDate = releaseToUpdate.ClosureDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    oldClosureMtg = releaseToUpdate.ClosureMtgRef;
                }

                string newClosureDateStr = "";
                string newClosureMtgStr = "None";
                if (release.ClosureMtgId.GetValueOrDefault() > 0)
                {
                    var mtg = mtgMgr.GetMeetingById(release.ClosureMtgId.Value);
                    if (mtg != null)
                    {
                        release.ClosureMtgRef = mtg.MtgShortRef;
                        release.ClosureDate = mtg.END_DATE.GetValueOrDefault();
                        newClosureDateStr = release.ClosureDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        newClosureMtgStr = release.ClosureMtgRef;
                    }
                }
                else
                    release.ClosureMtgId = 0;
                changes.Add(String.Format("Closure date: {0} ({1}) => {2} ({3})",
                    oldClosureDate, oldClosureMtg, newClosureDateStr, newClosureMtgStr));

                releaseToUpdate.ClosureDate = release.ClosureDate;
                releaseToUpdate.ClosureMtgId = release.ClosureMtgId;
                releaseToUpdate.ClosureMtgRef = release.ClosureMtgRef;
            }

            // Manage remarks
            foreach (var rk in release.Remarks)
            {
                if (rk.Pk_RemarkId != default(int))
                {
                    var updRk = releaseToUpdate.Remarks.Where(r => r.Pk_RemarkId == rk.Pk_RemarkId).FirstOrDefault();
                    if (updRk != null)
                        updRk.IsPublic = rk.IsPublic;
                }
                else
                {
                    rk.Fk_PersonId = personId;
                    rk.Release = releaseToUpdate;
                    rk.Fk_ReleaseId = releaseToUpdate.Pk_ReleaseId;
                    releaseToUpdate.Remarks.Add(rk);
                }
            }

            // Admin tab
            releaseToUpdate.IturCode = release.IturCode;
            releaseToUpdate.Version2g = release.Version2g;
            releaseToUpdate.Version3g = release.Version3g;
            releaseToUpdate.WpmCode2g = release.WpmCode2g;
            releaseToUpdate.WpmCode3g = release.WpmCode3g;

            //releaseToUpdate.LAST_MOD_BY = personId;
            releaseToUpdate.LAST_MOD_TS = DateTime.Now;

            if (changes.Count > 0)
            {
                var historyEntry = new History()
                {
                    Fk_PersonId = personId,
                    Release = releaseToUpdate,
                    HistoryText = String.Format(Utils.Localization.History_Release_Updated, string.Join("<br />", changes)),
                    CreationDate = DateTime.Now.ToUniversalTime()
                };

                releaseToUpdate.Histories.Add(historyEntry);
            }
        }

        /// <summary>
        /// Manage Sort Order for all releases
        /// </summary>
        /// <param name="aReleaseToAdd">New Release</param>
        /// <param name="previousReleaseId">Previous Release ID</param>
        private void ManageSortOrder(Release aReleaseToAdd, int previousReleaseId)
        {
            List<Release> allReleases = new List<Release>();
            Release previousRelease;
            if (previousReleaseId != 0)
            {
                previousRelease = releaseRepo.Find(previousReleaseId);
                aReleaseToAdd.SortOrder = previousRelease.SortOrder + 10;
                allReleases = releaseRepo.All.Where(r => r.Pk_ReleaseId != aReleaseToAdd.Pk_ReleaseId).OrderByDescending(x => x.SortOrder).ToList();
                foreach (Release r in allReleases)
                {
                    if (r.SortOrder <= previousRelease.SortOrder)
                        break;
                    if (r.SortOrder > previousRelease.SortOrder)
                    {
                        r.SortOrder += 10;
                        releaseRepo.InsertOrUpdate(r);
                    }
                }
            }
            else
            {
                allReleases = releaseRepo.All.OrderBy(x => x.SortOrder).ToList();
                var firstSortOrder = allReleases[0].SortOrder;
                foreach (Release r in allReleases)
                {
                    r.SortOrder += 10;
                    releaseRepo.InsertOrUpdate(r);
                }
                aReleaseToAdd.SortOrder = firstSortOrder;
            }
        }

        /// <summary>
        /// Clears the cache each time an action is performed on the releases.
        /// </summary>
        private void ClearCache()
        {
            CacheManager.Clear(CACHE_KEY);
        }

        #endregion
    }   
}
