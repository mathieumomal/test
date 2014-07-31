using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

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
        private readonly int MCC_LIST_ID = 5240;
        private readonly string CACHE_BASE_STR = "BIZ_USER_RIGHTS_";

        private readonly string ADMIN_GROUP = "Administrators";
        private readonly string WORKPLAN_MANAGER_GROUP = "Work Plan Managers";
        private readonly string SUPERUSER_GROUP = "Specification Managers";


        #region IRightsManager Membres

        /// <summary>
        /// Returns the rights of the user given his ID.
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public UserRightsContainer GetRights(int personID)
        {
            // Check that we can do the query
            if (UoW == null)
                throw new InvalidOperationException("You must define UoW before calling GetRights");


            var cacheData = (UserRightsContainer)CacheManager.Get(CACHE_BASE_STR+personID.ToString());
            if (cacheData != null)
            {
                return cacheData.Copy();
            }

            cacheData = new UserRightsContainer();

            // Computes the rights that are independent on Committe
            var roles = GetNonCommitteeRelatedRoles(personID);
            IUserRightsRepository rightsRepo = RepositoryFactory.Resolve<IUserRightsRepository>();
            foreach (var right in rightsRepo.GetRightsForRoles(roles))
            {
                cacheData.AddRight(right);
            }

            // Computes the rights that are depending on the Committee
            ComputeCommitteeRights(cacheData, personID);

            // Update the cache
            CacheManager.InsertForLimitedTime(CACHE_BASE_STR + personID.ToString(), cacheData, 10);
            return cacheData.Copy();

        }


        /// <summary>
        /// Computes the roles of the user given his ID. Based on a IUserRolesRepository.
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public List<Enum_UserRoles> GetNonCommitteeRelatedRoles(int personID)
        {
            var roles = new List<Enum_UserRoles>();

            // Anonymous users have no additional rights.
            if (personID <= 0)
            {
                roles.Add(Enum_UserRoles.Anonymous);
                return roles;
            }

            // Else, person is logged, so at least it is EOL
            roles.Add(Enum_UserRoles.EolAccountOwner);

            // now we can check whether the user is MCC
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW ;
            var records = repo.GetAllEtsiBasedRoles().Where(p => p.PERSON_ID == personID && p.PLIST_ID == MCC_LIST_ID).FirstOrDefault();
            if (records != null)
            {
                roles.Add(Enum_UserRoles.StaffMember);
            }

            // Let's check for the DNN Rights
            string strPersonID = personID.ToString();
            var otherRoles = repo.GetAllAdHocRoles().Where(p => p.PERSON_ID == strPersonID).Select(r => r.RoleName).ToList();
            if (otherRoles.Contains(ADMIN_GROUP))
                roles.Add(Enum_UserRoles.Administrator);
            if (otherRoles.Contains(WORKPLAN_MANAGER_GROUP))
                roles.Add(Enum_UserRoles.WorkPlanManager);
            if (otherRoles.Contains(SUPERUSER_GROUP))
                roles.Add(Enum_UserRoles.SuperUser);

            return roles;
        }

        /// <summary>
        /// Return the list of all roles that may be linked to committee.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Dictionary<int, List<Enum_UserRoles>> GetCommitteeRelatedRoles (int personId)
        {
            var roles = new Dictionary<int, List<Enum_UserRoles>>();
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;

            // Check official roles.
            string [] rolesStr = { "Chairman", "ViceChairman", "Convenor", "Secretary" };
            var officialRoles = repo.GetAllEtsiBasedRoles().Where(p => p.PERSON_ID == personId && rolesStr.Contains(p.PERS_ROLE_CODE)).ToList();
            foreach (var aRole in officialRoles)
            {
                if (!aRole.TB_ID.HasValue)
                    continue;
                
                int tbId = aRole.TB_ID.Value;

                if (!roles.ContainsKey(tbId))
                {
                    roles[tbId] = new List<Enum_UserRoles>();
                }

                if (!roles[tbId].Contains(Enum_UserRoles.CommitteeOfficial))
                    roles[tbId].Add(Enum_UserRoles.CommitteeOfficial);
            }

            return roles;
        }

        /// <summary>
        /// Gets the list of rights per committee.
        /// 
        /// To save time, this function computes a list of distinct roles for which the system needs the 
        /// rights and store these rights temporarily.
        /// It then fills in the UserContainer
        /// </summary>
        /// <param name="container">In out variable hosting the rights</param>
        /// <param name="personID">the person ID</param>
        private void ComputeCommitteeRights(UserRightsContainer container, int personID )
        {
            IUserRightsRepository rightsRepo = RepositoryFactory.Resolve<IUserRightsRepository>();

            // Computes the rights that are dependant in Committee
            var comRoles = GetCommitteeRelatedRoles(personID);
            var distinctRoles = comRoles.Values.ToList().SelectMany ( x=>x ).Distinct().ToList();

            // From that, computes the rights on a per role base.
            var rightsDict = new Dictionary<Enum_UserRoles, List<Enum_UserRights>>();
            foreach (var distinctRole in distinctRoles)
            {
                var tmpList = new List<Enum_UserRoles>();
                tmpList.Add(distinctRole);
                var rightsAssociatedToRole = rightsRepo.GetRightsForRoles( tmpList );
                rightsDict.Add(distinctRole, rightsAssociatedToRole);
            }

            // Now loops through the committees, and computes the rights.
            foreach (var committeeId in comRoles.Keys)
            {
                foreach (var committeeRole in comRoles[committeeId])
                {
                    var correspondingRights = rightsDict[committeeRole];
                    foreach (var aRight in correspondingRights)
                        container.AddRight(aRight, committeeId);
                }
            }
        }

        /// <summary>
        /// Check whether the user is MCC member or not
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>true/false</returns>
        public bool IsUserMCCMember(int personID)
        {
            // Check whether the user is MCC
            IUserRolesRepository repo = RepositoryFactory.Resolve<IUserRolesRepository>();
            repo.UoW = UoW;
            var records = repo.GetAllEtsiBasedRoles().Where(p => p.PERSON_ID == personID && p.PLIST_ID == MCC_LIST_ID).FirstOrDefault();
            return (records != null);
        }

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        #endregion
    }
}
