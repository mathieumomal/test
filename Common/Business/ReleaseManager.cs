﻿using System;
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
        private static string FREEZE_REL_STATUS = "Frozen";

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
            if (release.Enum_ReleaseStatus.ReleaseStatus == "Frozen")
            {
                personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
            }
            else if (release.Enum_ReleaseStatus.ReleaseStatus == "Closed")
            {
                personRights.RemoveRight(Enum_UserRights.Release_Freeze, true);
                personRights.RemoveRight(Enum_UserRights.Release_Close, true);
            }

            return new KeyValuePair<Release, UserRightsContainer>(repo.Find(id), personRights);
        }

        public void FreezeRelease(int releaseId, DateTime endDate)
        {
            IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
            repo.UoW = UoW;

            //** THIS CODE NEEDS SOME UPDATE : START **//
            //IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            //relStatusRepo.UoW = UoW;
            //var frozen = relStatusRepo.All.Where(x => x.ReleaseStatus == FREEZE_REL_STATUS).FirstOrDefault();
            //** THIS CODE NEEDS SOME UPDATE : END **//

            Enum_ReleaseStatus er = new Enum_ReleaseStatus { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" };
            var updatedObj = repo.Find(releaseId);
            updatedObj.Fk_ReleaseStatus = er.Enum_ReleaseStatusId;
            updatedObj.Enum_ReleaseStatus = er;
            
            //** THIS CODE NEEDS SOME UPDATE : START **//
            //updatedObj.Fk_ReleaseStatus = frozen.Enum_ReleaseStatusId;
            //updatedObj.Enum_ReleaseStatus = frozen;
            //** THIS CODE NEEDS SOME UPDATE : END **//

            updatedObj.EndDate = endDate;

            repo.InsertOrUpdate(updatedObj);
            repo.UoW.Save();
        }
    }
}
