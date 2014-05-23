using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{

    /// <summary>
    /// This repository encompasses: 
    /// - The retrieval of the ETSI based roles (i.e Roles fetched inside DSDB)
    /// - The retrieval of the Ad hoc defined roles (i.e. Roles defined inside DNN)
    /// </summary>
    public class UserRolesRepository : IUserRolesRepository
    {

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        /// <summary>
        /// Returns the association between the roles defined outside of ETSI system 
        /// (i.e. in DNN : Administrator, WorkPlanManager, Specification Manager) and the users.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Users_AdHoc_Roles> GetAllAdHocRoles()
        {
            return UoW.Context.Users_AdHoc_Roles;
        }

        /// <summary>
        /// Returns the associations between the ETSI roles (located in the "LISTS") and the users.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Users_Groups> GetAllEtsiBasedRoles()
        {
            return UoW.Context.Users_Groups;
        }



        #region IUserRolesRepository Membres

        public List<int> GetSpecMgr()
        {
            throw new NotImplementedException();
        }

        public List<int> GetWpMgr()
        {
            throw new NotImplementedException();
        }

        public List<int> GetSecretaryForComittee(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// This interface enables to establish a link between the user and the roles he has been associated to in external systems.
    /// </summary>
    public interface IUserRolesRepository
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Return roles associated outside ETSI (for example in DNN)
        /// </summary>
        /// <returns></returns>
        IQueryable<Users_AdHoc_Roles> GetAllAdHocRoles();
        
        /// <summary>
        /// Return roles associated inside ETSI.
        /// </summary>
        /// <returns></returns>
        IQueryable<Users_Groups> GetAllEtsiBasedRoles();

        /// <summary>
        /// Return Specification managers' id
        /// </summary>
        /// <returns></returns>
        List<int> GetSpecMgr();

        /// <summary>
        /// Return Workplan managers' id
        /// </summary>
        /// <returns></returns>
        List<int> GetWpMgr();

        /// <summary>
        /// Return secretary's id from a comittee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<int> GetSecretaryForComittee(int id);
    }
}
