using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml.Linq;
using System.Web.Caching;
using System.Web;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{

    /// <summary>
    /// This class is the default implementation of the IUserRightsRepository. It reads an
    /// XML file (location can be provided) to fetch the association between the rights and the 
    /// roles.
    /// </summary>
    public class UserRightsRepository : IUserRightsRepository
    {
        private static readonly string CACHE_KEY = "ULT_REPO_RIGHTS_ALL";
        
        public string XmlDocumentPath { get; set; }

        public UserRightsRepository()
        {
            XmlDocumentPath = ServerTopology.GetServerRootPath()+ "./UserRights.xml";
        }

        /// <summary>
        /// Retrieve all the rights that match the roles. Seesk first for the right-roles associations from the cache, else fetches it from the XML file
        /// using the LoadXmlAndComputeRights method.
        /// 
        /// Throws a FileNotFoundException if it cannot locate the XML file.
        /// </summary>
        /// <param name="userRoles">List of roles of the current user</param>
        /// <param name="status">status of the corresponding contribution</param>
        /// <returns>A list of rights</returns>
        public List<Enum_UserRights> GetRightsForRoles(List<Enum_UserRoles> roles)
        {

            // Check in the cache if roles have already been stored.
            var rightsRolesMapping = (Dictionary<Enum_UserRoles, List<Enum_UserRights>>) CacheManager.Get(CACHE_KEY);
            if (rightsRolesMapping != null)
                return rightsRolesMapping.Where(kv => roles.Contains(kv.Key)).SelectMany(kv => kv.Value).Distinct().ToList();
           
            // Else, build the cache
            var rightsMapping = LoadXmlAndComputeRights();
                
            CacheManager.Insert(CACHE_KEY,rightsMapping);
            return rightsMapping.Where(kv => roles.Contains(kv.Key)).SelectMany(kv => kv.Value).Distinct().ToList();
               
        }

        /// <summary>
        /// Retrieves the list of rights from the file.
        /// 
        /// Throws a FileNotFoundException if it cannot locate the XML file.
        /// </summary>
        /// <returns></returns>
        private Dictionary<Enum_UserRoles, List<Enum_UserRights>> LoadXmlAndComputeRights()
        {
            var rightsMapping =  new Dictionary<Enum_UserRoles, List<Enum_UserRights>>();
            Enum_UserRoles tmpEnumRole;
            Enum_UserRights tmpEnumRight;

            var doc = XDocument.Load(XmlDocumentPath);



            var xmlRoles = doc.Descendants("UserRole");
            foreach (var xmlRole in xmlRoles)
            {
                var roleValue = xmlRole.Attribute("value").Value;
                if (!Enum.TryParse(roleValue, true, out tmpEnumRole))
                    continue;

                var rightsList = new List<Enum_UserRights>();

                foreach (var xmlRight in xmlRole.Descendants("Right"))
                {
                    if (Enum.TryParse(xmlRight.Attribute("value").Value, true, out tmpEnumRight))
                        rightsList.Add(tmpEnumRight);
                }

                // Finally add it to the Dictionnary
                rightsMapping.Add(tmpEnumRole, rightsList.Distinct().ToList() );
            }
            return rightsMapping;
        }

     
    }

    /// <summary>
    /// Any class implementation this interface is responsible for providing a mapping
    /// between the User roles and the user rights.
    /// </summary>
    public interface IUserRightsRepository 
    {
        /// <summary>
        /// Returns the list of rights associated to the roles passed in parameter.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        List<Enum_UserRights> GetRightsForRoles(List<Enum_UserRoles> roles);
    }
}
