using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class UserRolesFakeRepository : IUserRolesRepository
    {
        static public readonly int MCC_MEMBER_ID = 27900;
        static public readonly int ADMINISTRATOR_ID = 53388;
        static public readonly int WORKPLANMGR_ID = 27904;
        static public readonly int SPECMGR_ID = 637;
        static public readonly int CHAIRMAN_ID = 12;
        static public readonly int VICECHAIRMAN_ID = 13;
        static public readonly int SECRETARY_ID = 15;
        static public readonly int OTHER_RIGHTS_ID = 16;

        static public readonly int TB_ID1 = 15;
        static public readonly int TB_ID2 = 16;


        #region IEntityRepository<Users_Groups> Members

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

       

        public IQueryable<Users_AdHoc_Roles> GetAllAdHocRoles()
        {
            
            var dbSet = new UsersAdHocRolesFakeDBSet();
            dbSet.Add(new Users_AdHoc_Roles() { PERSON_ID=""+WORKPLANMGR_ID, UserID=9, RoleName="Work Plan Managers" });
            dbSet.Add(new Users_AdHoc_Roles() { PERSON_ID = "" + ADMINISTRATOR_ID, UserID = 10, RoleName = "Administrators" });
            dbSet.Add(new Users_AdHoc_Roles() { PERSON_ID = "" + SPECMGR_ID, UserID = 39, RoleName = "Specification Managers" });

            return dbSet.AsQueryable();
        }

      
        public IQueryable<Users_Groups> GetAllEtsiBasedRoles()
        {
            // User 27904 is MCC
            // User 27903 is Chairman for TSG 15.
            // User 27906 is Support (hence no additional right) for TSG 15.
            var dbSet = new UserGroupsFakeDBSet();
            dbSet.Add(new Users_Groups() { PLIST_ID = 5240, PERSON_ID = MCC_MEMBER_ID, TB_ID = 0, PERS_ROLE_CODE = null });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5240, PERSON_ID = 27905, TB_ID = 0, PERS_ROLE_CODE = null });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = CHAIRMAN_ID, TB_ID = TB_ID1, PERS_ROLE_CODE = "Chairman" });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = VICECHAIRMAN_ID, TB_ID = TB_ID1, PERS_ROLE_CODE = "ViceChairman" });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = CHAIRMAN_ID, TB_ID = TB_ID2, PERS_ROLE_CODE = "Convenor" });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = SECRETARY_ID, TB_ID = TB_ID1, PERS_ROLE_CODE = "Secretary" });
            dbSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = OTHER_RIGHTS_ID, TB_ID = TB_ID1, PERS_ROLE_CODE = "Support" });

            return dbSet;
        }

        #endregion
    }
}
