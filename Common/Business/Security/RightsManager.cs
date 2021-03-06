﻿using Etsi.Ultimate.Business.UserRightsService;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Linq;

namespace Etsi.Ultimate.Business.Security
{
    /// <summary>
    /// Default implementation of the RightsManager. The system is using:
    /// - a UserRolesRepository to fetch the Roles of the user
    /// - a UserRightRepository that retrieves the rights from the roles previously computed.
    /// </summary>
    public class RightsManager : IRightsManager
    {
        /// <summary>
        /// ID of the "MCC" list of users in the DSDB database.
        /// </summary>
        private const int MccListId = 5240;
        private const string CacheBaseStr = "ULTIMATE_BIZ_USER_RIGHTS_";

        #region IRightsManager Membres

        /// <summary>
        /// Returns the rights of the user given his ID.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public UserRightsContainer GetRights(int personId)
        {
            var cacheData = (UserRightsContainer)CacheManager.Get(CacheBaseStr + personId);
            if (cacheData != null)
                return cacheData.Copy();

            cacheData = new UserRightsContainer();

            var userRightsServiceClient = ManagerFactory.Resolve<IUserRightsService>();
            var personRights = userRightsServiceClient.GetRights(personId, ConfigVariables.PortalName);

            //Convert Generic Application rights
            if (personRights.ApplicationRights != null)
            {
                Enum_UserRights tmpRight;
                foreach (var applicationRight in personRights.ApplicationRights)
                {
                    if (Enum.TryParse(applicationRight, true, out tmpRight))
                        cacheData.AddRight(tmpRight);
                }
            }

            //Convert committe based rights
            if (personRights.CommitteeRights != null)
            {
                Enum_UserRights tmpRight;
                foreach (var committeRights in personRights.CommitteeRights) //Get rights for all committees
                {
                    foreach (var committeeRight in committeRights.Value) //Get rights for each committee
                    {
                        if (Enum.TryParse(committeeRight, true, out tmpRight))
                            cacheData.AddRight(tmpRight, committeRights.Key);
                    }
                }
            }

            CacheManager.InsertForLimitedTime(CacheBaseStr + personId, cacheData, 10);
            return cacheData.Copy();
        }

        /// <summary>
        /// Check whether the user is MCC member or not
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>true/false</returns>
        public bool IsUserMCCMember(int personId)
        {
            // Check whether the user is MCC
            var repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            var records = repo.GetAllEtsiBasedRoles().FirstOrDefault(p => p.PERSON_ID == personId && p.PLIST_ID == MccListId);
            return (records != null);
        }

        /// <summary>
        /// Context to be provided in case the database needs to be targeted.
        /// </summary>
        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        #endregion
    }
}
