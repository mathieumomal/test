using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace Etsi.Ultimate.WCF.Service.UserRights
{
    /// <summary>
    /// TODO:: Needs to remove once ultimate solution integrated with UserRights service
    /// </summary>
    public class UserRights : IUserRightsRepository
    {
        UserRightsRepository _userRightsRepository = new UserRightsRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRights"/> class.
        /// </summary>
        public UserRights()
        {
            _userRightsRepository.XmlDocumentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserRights\\UserRights.xml");
        }

        /// <summary>
        /// Gets the rights for roles.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <returns>List of rights</returns>
        public List<Enum_UserRights> GetRightsForRoles(List<Enum_UserRoles> roles)
        {
            return _userRightsRepository.GetRightsForRoles(roles);
        }
    }
}