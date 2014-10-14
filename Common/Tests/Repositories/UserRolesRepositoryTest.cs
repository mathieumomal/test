using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class UserRolesRepositoryTest : BaseTest
    {

        [Test]
        public void getAll()
        {
            var repo = new UserRolesRepository() { UoW = GetUnitOfWork() };
            var results = repo.GetAllEtsiBasedRoles().ToList();

            Assert.AreEqual(7, results.Count);
        }

        [Test]
        public void GetWpMgrIds()
        {
            var repo = new UserRolesRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual(1, repo.GetWpMgr().ToList().Count);
        }

        [Test]
        public void GetSpecMgrIds()
        {
            var repo = new UserRolesRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual(1, repo.GetSpecMgr().ToList().Count);
        }

        [Test]
        public void GetChairmanIdByCommitteeId()
        {
            var repo = new UserRolesRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual(27904, repo.GetChairmanIdByCommitteeId(15));//One person with the role chairman and with an null end_date : result expected 
            Assert.AreEqual(0, repo.GetChairmanIdByCommitteeId(2));//Two persons with no end_date : no result expected 
            Assert.AreEqual(0, repo.GetChairmanIdByCommitteeId(16));//Two persons with end_date : no result expected
        }

        
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();

            var userGroupsDBSet = new UserGroupsFakeDBSet();
            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5204, PERSON_ID = 27904, TB_ID = 2, PERS_ROLE_CODE = "Chairman" });
            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5204, PERSON_ID = 27905, TB_ID = 2, PERS_ROLE_CODE = "Chairman" });

            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = 59862, TB_ID = 15, PERS_ROLE_CODE = "Chairman", END_DATE = DateTime.Now.AddMonths(-2) });
            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = 27904, TB_ID = 15, PERS_ROLE_CODE = "Chairman", END_DATE = null });
            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = 27906, TB_ID = 15, PERS_ROLE_CODE = "Support" });

            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = 27904, TB_ID = 16, PERS_ROLE_CODE = "Chairman", END_DATE = DateTime.Now.AddMonths(-4) });
            userGroupsDBSet.Add(new Users_Groups() { PLIST_ID = 5322, PERSON_ID = 27904, TB_ID = 16, PERS_ROLE_CODE = "Chairman", END_DATE = DateTime.Now.AddMonths(-4) });


            var userDnnRolesGroupsDBSet = new UsersAdHocRolesFakeDBSet();
            userDnnRolesGroupsDBSet.Add(new Users_AdHoc_Roles() { UserID = 9, PERSON_ID = "27904", RoleName = "Work Plan Managers" });
            userDnnRolesGroupsDBSet.Add(new Users_AdHoc_Roles() { UserID = 10, PERSON_ID = "27906", RoleName = "Specification Managers" });

            iUltimateContext.Users_Groups = userGroupsDBSet;
            iUltimateContext.Users_AdHoc_Roles = userDnnRolesGroupsDBSet;
            
            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }
    }
}
