using System.Globalization;
using Etsi.Dsdb.DataAccess;
using Etsi.UserRights.DNNETSIDataAccess;
using Etsi.UserRights.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Etsi.UserRights.Service
{
    /// <summary>
    /// Fetch user rights for ETSI portal
    /// </summary>
    public class ETSIRights : IRights
    {
        #region Constants

        private const int ConstMccListId = 5240;
        private const string ConstPersonIdMappingKey = "ETSI_DS_ID";
        private const string ConstRoleDnnPortalAdministrator = "Administrators";
        private const string ConstRoleDnnPortalMeetingManager = "Meeting Managers";

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>User Rights object</returns>
        public PersonRights GetRights(int personId)
        {
            PersonRights personRights = new PersonRights();

            //Compute User Application Rights
            var applicationRoles = GetApplicationRoles(personId);
            personRights.ApplicationRights = GetRightsForRoles(applicationRoles);

            return personRights;
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>List of Roles</returns>
        private List<Enum_UserRoles> GetApplicationRoles(int personId)
        {
            var personRoles = new List<Enum_UserRoles>();

            try
            {
                //[1] Anonymous - All users should have Anonymous access irrespective of logged in status
                personRoles.Add(Enum_UserRoles.Anonymous);

                //Check the rest of roles for logged in user
                if (personId > 0)
                {
                    //************************************************
                    //************ DNN BASED ROLES  ******************
                    //************************************************

                    using (var context = new DNNETSIContext())
                    {
                        var rolesInDnnPortal = new[] { ConstRoleDnnPortalAdministrator, 
                                                              ConstRoleDnnPortalMeetingManager };

                        var strPersonId = personId.ToString(CultureInfo.InvariantCulture);

                        var userRolesInDnn = (from userProfile in context.UserProfiles
                                              join propertyDefiniton in context.ProfilePropertyDefinitions on userProfile.PropertyDefinitionID equals propertyDefiniton.PropertyDefinitionID
                                              join userRole in context.UserRoles on userProfile.UserID equals userRole.UserID
                                              join role in context.Roles on userRole.RoleID equals role.RoleID
                                              where propertyDefiniton.PropertyName == ConstPersonIdMappingKey
                                                    && rolesInDnnPortal.Contains(role.RoleName)
                                                    && userProfile.PropertyValue == strPersonId
                                              select role.RoleName).ToList();

                        //[2] Administrators - Check 'Administrators' role in DNN
                        if (userRolesInDnn.Contains(ConstRoleDnnPortalAdministrator))
                            personRoles.Add(Enum_UserRoles.Administrator);

                        //[3] Meeting Manager - Check 'Meeting Managers' role in DNN
                        if (userRolesInDnn.Contains(ConstRoleDnnPortalMeetingManager))
                            personRoles.Add(Enum_UserRoles.MeetingManager);
                    }

                    //************************************************
                    //************ ETSI BASED ROLES  *****************
                    //************************************************
                    using (var context = DatabaseFactory.Resolve<IDSDBContext>())
                    {

                        var userMccRecord = (from personInList in context.PersonInLists
                                             where personInList.PLIST_ID == ConstMccListId
                                             && personInList.PERSON_ID == personId
                                             select personInList).Any();

                        //[4] StaffMember - Check 'MCC Member' role in DSDB
                        if (userMccRecord)
                            personRoles.Add(Enum_UserRoles.StaffMember);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.UserRightsLogger.Error(String.Format("User Rights Error [ETSI - GetApplicationRoles]: {0}", ex.Message));
            }

            return personRoles;
        }

        /// <summary>
        /// Get rights for given roles
        /// </summary>
        /// <param name="personRoles">Person Roles</param>
        /// <returns>User Rights</returns>
        private List<string> GetRightsForRoles(List<Enum_UserRoles> personRoles)
        {
            var personRights = new List<string>();

            var doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserRights.xml"));

            foreach (var personRole in personRoles)
            {
                var rightsElements = doc.XPathSelectElements(String.Format("Portals//Portal[@value='ETSI']//UserRoles//UserRole[@value='{0}']//Right", personRole)).ToList();
                rightsElements.ForEach(x =>
                {
                    if (x.Attribute("value") != null)
                        personRights.Add(x.Attribute("value").Value);
                });
            }

            return personRights.Distinct().ToList();
        } 

        #endregion
    }
}
