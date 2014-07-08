using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public interface ICommunityManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get All Communities
        /// </summary>
        /// <returns>List of Communities</returns>
        List<Community> GetCommunities();

         /// <summary>
        /// Return short name of a community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>Short name of the community</returns>
        string GetCommmunityshortNameById(int id);


        /// <summary>
        /// Return community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>community</returns>
        Community GetCommmunityById(int id);

        /// <summary>
        /// Get a communityShortname object by community id (TbId)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Enum_CommunitiesShortName GetEnumCommunityShortNameByCommunityId(int id);
       
    }
}
