using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.UserRights.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using UltimateEntities = Etsi.Ultimate.DomainClasses;
using UltimateServiceEntities = Etsi.Ultimate.WCF.Interface.Entities;

namespace Etsi.Ultimate.WCF.Service
{
    /// <summary>
    /// Helper class to provide necessary information to service
    /// </summary>
    public class ServiceHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <returns>List of releases</returns>
        public List<UltimateServiceEntities.Release> GetReleases(int personID)
        {
            List<UltimateServiceEntities.Release> releases = new List<UltimateServiceEntities.Release>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObjects = svc.GetAllReleases(personID);
                releaseRightsObjects.Key.ForEach(x => releases.Add(ConvertUltimateReleaseToServiceRelease(x)));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format("Ultimate Service Error [GetReleases]: {0}", ex.Message));
            }

            return releases;
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the ultimate release to service release.
        /// </summary>
        /// <param name="ultimateRelease">The ultimate release.</param>
        /// <returns>Service release entity</returns>
        private UltimateServiceEntities.Release ConvertUltimateReleaseToServiceRelease(UltimateEntities.Release ultimateRelease)
        {
            UltimateServiceEntities.Release serviceRelease = new UltimateServiceEntities.Release();
            if (ultimateRelease != null)
            {
                serviceRelease.Pk_ReleaseId = ultimateRelease.Pk_ReleaseId;
                serviceRelease.Name = ultimateRelease.Name;
                serviceRelease.ShortName = ultimateRelease.ShortName;
                serviceRelease.Status = ultimateRelease.Enum_ReleaseStatus.Description;
            }
            return serviceRelease;
        } 

        #endregion
    }
}