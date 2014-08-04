using Etsi.UserRights.DNN3GPPDataAccess;
using Etsi.UserRights.DSDBDataAccess;
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
    /// Fetch user rights for Ultimate portal
    /// </summary>
    public class UltimateRights : IRights
    {
        #region Constants

        private const int CONST_MCC_LIST_ID = 5240;
        private const string CONST_PERSON_ID_MAPPING_KEY = "ETSI_DS_ID";
        private const string CONST_ROLE_DNN_PORTAL_ADMINISTRATOR = "Administrators";
        private const string CONST_ROLE_DNN_PORTAL_WORK_PLAN_MANAGER = "Work Plan Managers";
        private const string CONST_ROLE_DNN_PORTAL_SPECIFICATION_MANAGER = "Specification Managers";
        private const string CONST_ROLE_DNN_PORTAL_MEETING_MANAGER = "Meeting Managers";

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>User Rights object</returns>
        public PersonRights GetRights(int personID)
        {
            PersonRights personRights = new PersonRights();

            //Compute User Application Rights
            var applicationRoles = GetApplicationRoles(personID);
            personRights.ApplicationRights = GetRightsForRoles(applicationRoles);

            //Compute User Committee Rights
            var committeeRoles = GetCommitteeRoles(personID);
            personRights.CommitteeRights = GetCommitteeRights(committeeRoles);

            return personRights;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>List of Non Committee Roles</returns>
        private List<Enum_UserRoles> GetApplicationRoles(int personID)
        {
            var personRoles = new List<Enum_UserRoles>();

            try
            {
                //[1] Anonymous - All users should have Anonymous access irrespective of logged in status
                personRoles.Add(Enum_UserRoles.Anonymous);

                //Check the rest of roles for logged in user
                if (personID > 0)
                {
                    //[2] EolAccountOwner - All logged in users are 'EOL Account Owners'
                    personRoles.Add(Enum_UserRoles.EolAccountOwner);

                    //************************************************
                    //************ DNN BASED ROLES  ******************
                    //************************************************
                    using (var context = new DNN3GPPContext())
                    {
                        var rolesInDNNPortal = new string[] { CONST_ROLE_DNN_PORTAL_ADMINISTRATOR, 
                                                      CONST_ROLE_DNN_PORTAL_WORK_PLAN_MANAGER,
                                                      CONST_ROLE_DNN_PORTAL_SPECIFICATION_MANAGER,
                                                      CONST_ROLE_DNN_PORTAL_MEETING_MANAGER };

                        string strPersonID = personID.ToString();

                        var userRolesInDNN = (from userProfile in context.UserProfiles
                                              join propertyDefiniton in context.ProfilePropertyDefinitions on userProfile.PropertyDefinitionID equals propertyDefiniton.PropertyDefinitionID
                                              join userRole in context.UserRoles on userProfile.UserID equals userRole.UserID
                                              join role in context.Roles on userRole.RoleID equals role.RoleID
                                              where propertyDefiniton.PropertyName == CONST_PERSON_ID_MAPPING_KEY
                                                    && rolesInDNNPortal.Contains(role.RoleName)
                                                    && userProfile.PropertyValue == strPersonID
                                              select role.RoleName).ToList();

                        //[3] Administrators - Check 'Administrators' role in DNN
                        if (userRolesInDNN.Contains(CONST_ROLE_DNN_PORTAL_ADMINISTRATOR))
                            personRoles.Add(Enum_UserRoles.Administrator);

                        //[4] SuperUser - Check 'Specification Managers' role in DNN
                        if (userRolesInDNN.Contains(CONST_ROLE_DNN_PORTAL_SPECIFICATION_MANAGER))
                            personRoles.Add(Enum_UserRoles.SuperUser);

                        //[5] WorkPlanManager - Check 'Work Plan Managers' role in DNN
                        if (userRolesInDNN.Contains(CONST_ROLE_DNN_PORTAL_WORK_PLAN_MANAGER))
                            personRoles.Add(Enum_UserRoles.WorkPlanManager);

                        //[6] Meeting Manager - Check 'Meeting Managers' role in DNN
                        if (userRolesInDNN.Contains(CONST_ROLE_DNN_PORTAL_MEETING_MANAGER))
                            personRoles.Add(Enum_UserRoles.MeetingManager);
                    }

                    //************************************************
                    //************ ETSI BASED ROLES  *****************
                    //************************************************
                    using (var context = new DSDBContext())
                    {

                        var userMCCRecord = (from personInList in context.PERSON_IN_LIST
                                             where personInList.PLIST_ID == CONST_MCC_LIST_ID
                                             && personInList.PERSON_ID == personID
                                             select personInList).Any();

                        //[7] StaffMember - Check 'MCC Member' role in DSDB
                        if (userMCCRecord)
                            personRoles.Add(Enum_UserRoles.StaffMember);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.UserRightsLogger.Error(String.Format("User Rights Error [Ultimate - GetApplicationRoles]: {0}", ex.Message));
            }

            return personRoles;
        }

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>List of Non Committee Roles</returns>
        private Dictionary<Enum_UserRoles, List<int>> GetCommitteeRoles(int personID)
        {
            Dictionary<Enum_UserRoles, List<int>> committeeRoles = new Dictionary<Enum_UserRoles, List<int>>();

            try
            {
                //**********************************************************
                //************ COMMITTEE (TB) BASED ROLES  *****************
                //**********************************************************

                using (var context = new DSDBContext())
                {
                    var committeeOfficialRoles = new string[] { "Chairman", "ViceChairman", "Convenor", "Secretary" };
                    var personListTypes = new string[] { "MASTER", "OTHER" };

                    //Get the Committee List which the given person has Committee Official (Chairman / ViceChairman / Convenor / Secretary) Right
                    var tbList = (from personList in context.PERSON_LIST
                                         join personInList in context.PERSON_IN_LIST on personList.PLIST_ID equals personInList.PLIST_ID
                                         join person in context.People on personInList.PERSON_ID equals person.PERSON_ID
                                         where person.DELETED_FLG == "N"
                                         && committeeOfficialRoles.Contains(personInList.PERS_ROLE_CODE)
                                         && personListTypes.Contains(personList.PLIST_TYPE)
                                         && person.PERSON_ID == personID
                                         select personList.TB_ID ?? 0).ToList();
                    tbList.RemoveAll(x => x == 0); //Avoid NULL TB_ID records

                    //[1] CommitteeOfficial - Check for Committee Official role in DSDB
                    if (tbList.Count > 0)
                        committeeRoles.Add(Enum_UserRoles.CommitteeOfficial, tbList);
                }
            }
            catch (Exception ex)
            {
                LogManager.UserRightsLogger.Error(String.Format("User Rights Error [Ultimate - GetCommitteeRoles]: {0}", ex.Message));
            }

            return committeeRoles;
        }

        /// <summary>
        /// Get rights for committee roles
        /// </summary>
        /// <param name="committeeRoles">Committee Roles</param>
        /// <returns>Committee Rights</returns>
        private Dictionary<int, List<string>> GetCommitteeRights(Dictionary<Enum_UserRoles, List<int>> committeeRoles)
        {
            Dictionary<int, List<string>> committeeRights = new Dictionary<int, List<string>>();
            foreach (var committeeRole in committeeRoles)
            {
                var committeeRoleRights = GetRightsForRoles(new List<Enum_UserRoles> { committeeRole.Key });
                foreach (var committeeID in committeeRole.Value)
                {
                    if (committeeRoleRights.Count > 0)
                    {
                        if (!committeeRights.ContainsKey(committeeID))
                            committeeRights.Add(committeeID, new List<string>());

                        foreach (var committeeRight in committeeRoleRights)
                        {
                            committeeRights[committeeID].Add(committeeRight);
                        }
                    }
                }
            }
            return committeeRights;
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
                var rightsElements = doc.XPathSelectElements(String.Format("Portals//Portal[@value='Ultimate']//UserRoles//UserRole[@value='{0}']//Right", personRole)).ToList();
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
