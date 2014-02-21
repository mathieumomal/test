using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml.Linq;
using System.Web.Caching;
using System.Web;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class Security_RightsRepository : Security_IRightsRepository
    {
        private string _docSource;

        public Security_RightsRepository()
        {
            _docSource = "";
        }


        /// <summary>
        /// Retrieve all the rights that match the roles
        /// </summary>
        /// <param name="userRoles">List of roles of the current user</param>
        /// <param name="status">status of the corresponding contribution</param>
        /// <returns>A list of rights</returns>
        public List<Enum_SecurityRights> GetRightsPerRoles(List<SecurityRoles> roles)
        {
            var rights = new List<Enum_SecurityRights>();
            var doc = XDocument.Load(_docSource);

            foreach (var item in roles)
            {
                var allRights = from role in doc.Descendants("UserRole")
                                where (string)role.Attribute("value") == item.ToString()
                                select role;
                var currentRights = from right in allRights.Descendants("Right")
                                    select (string)right.Attribute("value");

                foreach (var right in currentRights)
                {
                    var enumvalue = (Enum_SecurityRights)Enum.Parse(typeof(Enum_SecurityRights), right);

                    if (!rights.Contains(enumvalue))
                    {
                        rights.Add(enumvalue);
                    }
                }
            }

            return rights;
        }

        #region IDisposable Membres

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
