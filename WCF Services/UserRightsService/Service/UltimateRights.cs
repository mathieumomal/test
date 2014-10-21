using System.Globalization;
using Etsi.Dsdb.DataAccess;
using Etsi.UserRights.DNN3GPPDataAccess;
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

        private const int ConstMccListId = 5240;
        private const string ConstPersonIdMappingKey = "ETSI_DS_ID";
        private const string ConstRoleDnnPortalAdministrator = "Administrators";
        private const string ConstRoleDnnPortalWorkPlanManager = "Work Plan Managers";
        private const string ConstRoleDnnPortalSpecificationManager = "Specification Managers";
        private const string ConstRoleDnnPortalMeetingManager = "Meeting Managers";
        private const string ConstOrgaStatus3GppmarkRep = "3GPPMARK_REP";
        private const string ConstOrgaStatus3Gppmember = "3GPPMEMBER";
        private const string ConstOrgaStatus3Gppobserv = "3GPPOBSERV";
        private const string ConstOrgaStatus3GpporgRep = "3GPPORG_REP";
        private const string ConstOrgaStatus3GppGuest = "3GPPGUEST";
        private const string ConstOrgaStatus3GppInvite = "3GPPINVITE";
        private const string ConstOrgaStatus3Gppexpelled = "3GPPEXPELLED";
        private const string ConstOrgaStatus3Gppwithdraw = "3GPPWITHDRAW";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user rights XML path.
        /// </summary>
        public string UserRightsXmlPath { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>User Rights object</returns>
        public PersonRights GetRights(int personId)
        {
            var personRights = new PersonRights();

            //Compute User Application Rights
            var applicationRoles = GetApplicationRoles(personId);
            personRights.ApplicationRights = GetRightsForRoles(applicationRoles);

            //Compute User Committee Rights
            var committeeRoles = GetCommitteeRoles(personId);
            personRights.CommitteeRights = GetCommitteeRights(committeeRoles);

            return personRights;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>List of Non Committee Roles</returns>
        public List<Enum_UserRoles> GetApplicationRoles(int personId)
        {
            var personRoles = new List<Enum_UserRoles>();

            try
            {
                //[1] Anonymous - Return without going any further
                if (personId <= 0)
                {
                    personRoles.Add(Enum_UserRoles.Anonymous);
                    return personRoles;
                }

                //Check the rest of roles for logged in user
                //[2] EolAccountOwner - All logged in users are 'EOL Account Owners'
                personRoles.Add(Enum_UserRoles.EolAccountOwner);

                //************************************************
                //************ DNN BASED ROLES  ******************
                //************************************************
                using (var context = DatabaseFactory.Resolve<IDnn3gppContext>())
                {
                    var rolesInDnnPortal = new[] { ConstRoleDnnPortalAdministrator, 
                                                      ConstRoleDnnPortalWorkPlanManager,
                                                      ConstRoleDnnPortalSpecificationManager,
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

                    //[3] Administrators - Check 'Administrators' role in DNN
                    if (userRolesInDnn.Contains(ConstRoleDnnPortalAdministrator))
                        personRoles.Add(Enum_UserRoles.Administrator);

                    //[4] SuperUser - Check 'Specification Managers' role in DNN
                    if (userRolesInDnn.Contains(ConstRoleDnnPortalSpecificationManager))
                        personRoles.Add(Enum_UserRoles.SuperUser);

                    //[5] WorkPlanManager - Check 'Work Plan Managers' role in DNN
                    if (userRolesInDnn.Contains(ConstRoleDnnPortalWorkPlanManager))
                        personRoles.Add(Enum_UserRoles.WorkPlanManager);

                    //[6] Meeting Manager - Check 'Meeting Managers' role in DNN
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

                    //[7] StaffMember - Check 'MCC Member' role in DSDB
                    if (userMccRecord)
                        personRoles.Add(Enum_UserRoles.StaffMember);

                    var allowedOrganisationStatus = new[] { ConstOrgaStatus3GppmarkRep, 
                                                                   ConstOrgaStatus3Gppmember,
                                                                   ConstOrgaStatus3Gppobserv,
                                                                   ConstOrgaStatus3GpporgRep, 
                                                                   ConstOrgaStatus3GppGuest, 
                                                                   ConstOrgaStatus3GppInvite };
                    var prohibitedOrganisationStatus = new string[] { ConstOrgaStatus3Gppexpelled, ConstOrgaStatus3Gppwithdraw };

                    var organisationStatues = (from people in context.People
                                               join organisation in context.Organisations on people.ORGA_ID equals organisation.ORGA_ID
                                               join organisationStatus in context.OrganisationStatus on organisation.ORGA_ID equals organisationStatus.ORGA_ID
                                               where people.PERSON_ID == personId
                                               select organisationStatus.ORGS_CODE).ToList();

                    if (!organisationStatues.Any(x => prohibitedOrganisationStatus.Contains(x))) //Organisation status should not be in prohibited list
                    {
                        //[8] U3GPPMember - Check 'Organisation Status' in DSDB
                        if (organisationStatues.Any(x => allowedOrganisationStatus.Contains(x)))
                            personRoles.Add(Enum_UserRoles.U3GPPMember);
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
        /// <param name="personId">Person ID</param>
        /// <returns>List of Non Committee Roles</returns>
        public Dictionary<Enum_UserRoles, List<int>> GetCommitteeRoles(int personId)
        {
            var committeeRoles = new Dictionary<Enum_UserRoles, List<int>>();

            try
            {
                //**********************************************************
                //************ COMMITTEE (TB) BASED ROLES  *****************
                //**********************************************************

                using (var context = DatabaseFactory.Resolve<IDSDBContext>())
                {
                    var committeeOfficialRoles = new[] { "chairman", "vicechairman", "convenor", "secretary" };
                    var personListTypes = new[] { "MASTER", "OTHER" };

                    //Get the Committee List which the given person has Committee Official (Chairman / ViceChairman / Convenor / Secretary) Right
                    var tbList = (from personList in context.PersonLists
                                  join personInList in context.PersonInLists on personList.PLIST_ID equals personInList.PLIST_ID
                                  join person in context.People on personInList.PERSON_ID equals person.PERSON_ID
                                  where person.DELETED_FLG == "N"
                                  && committeeOfficialRoles.Contains(personInList.PERS_ROLE_CODE.ToLower())
                                  && personListTypes.Contains(personList.PLIST_TYPE)
                                  && person.PERSON_ID == personId
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
            var committeeRights = new Dictionary<int, List<string>>();
            foreach (var committeeRole in committeeRoles)
            {
                var committeeRoleRights = GetRightsForRoles(new List<Enum_UserRoles> { committeeRole.Key });
                foreach (var committeeId in committeeRole.Value)
                {
                    if (committeeRoleRights.Count > 0)
                    {
                        if (!committeeRights.ContainsKey(committeeId))
                            committeeRights.Add(committeeId, new List<string>());

                        foreach (var committeeRight in committeeRoleRights)
                        {
                            committeeRights[committeeId].Add(committeeRight);
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

            var doc = XDocument.Load(UserRightsXmlPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserRights.xml"));

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
