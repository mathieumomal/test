using Etsi.Dsdb.DataAccess;
using Etsi.Dsdb.DomainClasses;
using Etsi.UserRights.Service;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace UserRightsService.Tests
{
    [TestFixture]
    class DelegateRoleFor3gppTests : BaseEffortTest
    {
        #region Constants

        private const int _3GPP_MARK_REP_ORGAID = 1;
        private const int _3GPP_MEMBER_ORGAID = 2;
        private const int _3GPP_OBSERV_ORGAID = 3;
        private const int _3GPP_ORG_REP_ORGAID = 4;
        private const int _3GPP_GUEST_ORGAID = 5;
        private const int _3GPP_INVITE_ORGAID = 6;
        private const int _3GPP_EXPELLED_ORGAID = 7;
        private const int _3GPP_WITHDRAW_ORGAID = 8;
        private const int _PERSONID = 1; 

        #endregion

        #region Tests

        //     -8 Months                                           Today                                          +8Months
        //         |                                                  |                                                |
        //        m|3                                                m|9                                               |
        //       m2|                                                m8|                                               m|13
        // m1      |       m4        m5         m6         m7         |         m10        m11          m12            |m14   m15
        //---------|--------------------------------------------------|------------------------------------------------|-------------
        //--------- -------------------------------------------------- ------------------------------------------------ -------------

        // Current Organisation Status     : Valid 
        // Represented Organisation Status : Valid/InValid 
        // Attended                        : Yes/No           ==> Is Delegate  ==> YES
        //=============================================================================
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 1, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 1, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 2, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 2, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 8, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 8, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 1, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 1, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 2, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 2, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 8, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 8, false, true)]
        // Current Organisation Status     : Valid 
        // Represented Organisation Status : Valid/InValid 
        // Registered                      : Yes/No           ==> Is Delegate  ==> YES
        //=============================================================================
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 9, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 9, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 14, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 14, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 15, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_MEMBER_ORGAID, 15, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 9, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 9, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 14, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 14, false, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 15, true, true)]
        [TestCase(_PERSONID, _3GPP_MARK_REP_ORGAID, _3GPP_EXPELLED_ORGAID, 15, false, true)]
        // Current Organisation Status     : InValid 
        // Represented Organisation Status : Valid
        // Attended                        : Yes       ==> Is Delegate  ==> YES
        // Attended                        : No        ==> Is Delegate  ==> NO
        //======================================================================
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 1, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 1, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 2, true, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 2, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 7, true, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 7, false, false)]
        // Current Organisation Status     : InValid 
        // Represented Organisation Status : Valid
        // Registered                      : Yes/No     ==> Is Delegate  ==> YES
        //======================================================================
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 8, true, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 8, false, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 9, true, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 9, false, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 14, true, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 14, false, true)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 15, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_GUEST_ORGAID, 15, false, false)]
        // Current Organisation Status     : InValid 
        // Represented Organisation Status : InValid
        // Attended                        : Yes/No    ==> Is Delegate  ==> NO
        //======================================================================
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 1, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 1, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 2, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 2, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 8, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 8, false, false)]
        // Current Organisation Status     : InValid 
        // Represented Organisation Status : InValid
        // Registered                      : Yes/No     ==> Is Delegate  ==> NO
        //======================================================================
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 9, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 9, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 14, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 14, false, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 15, true, false)]
        [TestCase(_PERSONID, _3GPP_EXPELLED_ORGAID, _3GPP_WITHDRAW_ORGAID, 15, false, false)]
        public void Check_Delegate_Role(int personId, int currentOrgaId, int repOrgaId, int meetingId, bool isRepresented, bool isDelegate)
        {
            var person = new Person { PERSON_ID = personId, ORGA_ID = currentOrgaId };
            var peoples = new List<Person> { person };

            var personList = new PersonList { PLIST_ID = 1, MTG_ID = meetingId };
            var personLists = new List<PersonList> { personList };

            var personInList = new PersonInList { PLIST_ID = 1, PERSON_ID = personId, REPRESENT_ORGA_ID = repOrgaId, REPRESENT_FLG = (isRepresented) ? "Y" : "N" };
            var personInLists = new List<PersonInList> { personInList };

            var mockDataContext = MockRepository.GenerateMock<IDSDBContext>();
            mockDataContext.Stub(x => x.People).Return(GetDbSet<Person>(peoples));
            mockDataContext.Stub(x => x.Meetings).Return(GetDbSet<Meeting>(GetMeetings()));
            mockDataContext.Stub(x => x.Organisations).Return(GetDbSet<Organisation>(GetOrganisations()));
            mockDataContext.Stub(x => x.OrganisationStatus).Return(GetDbSet<OrganisationStatus>(GetOrganisationStatus()));
            mockDataContext.Stub(x => x.PersonLists).Return(GetDbSet<PersonList>(personLists));
            mockDataContext.Stub(x => x.PersonInLists).Return(GetDbSet<PersonInList>(personInLists));
            DatabaseFactory.Container.RegisterInstance(typeof(IDSDBContext), mockDataContext);

            var ultimateRights = new UltimateRights();
            var appRoles = ultimateRights.GetApplicationRoles(personId);
            Assert.AreEqual(isDelegate, appRoles.Contains(Enum_UserRoles.U3GPPMember));
        } 

        #endregion

        #region Data

        /// <summary>
        /// Gets the meetings.
        /// </summary>
        /// <returns>List of meetings</returns>
        private List<Meeting> GetMeetings()
        {
            //     -8 Months                                           Today                                          +8Months
            //         |                                                  |                                                |
            //        m|3                                                m|9                                               |
            //       m2|                                                m8|                                               m|13
            // m1      |       m4        m5         m6         m7         |         m10        m11          m12            |m14   m15
            //---------|--------------------------------------------------|------------------------------------------------|-------------
            //--------- -------------------------------------------------- ------------------------------------------------ -------------

            var currentDate = DateTime.UtcNow.Date;
            var m1 = new Meeting { MTG_ID = 1, START_DATE = currentDate.AddMonths(-8).AddDays(-2), END_DATE = currentDate.AddMonths(-8).AddDays(-1) };
            var m2 = new Meeting { MTG_ID = 2, START_DATE = currentDate.AddMonths(-8).AddDays(-2), END_DATE = currentDate.AddMonths(-8) };
            var m3 = new Meeting { MTG_ID = 3, START_DATE = currentDate.AddMonths(-8).AddDays(-1), END_DATE = currentDate.AddMonths(-8).AddDays(1) };
            var m4 = new Meeting { MTG_ID = 4, START_DATE = currentDate.AddMonths(-7), END_DATE = currentDate.AddMonths(-7).AddDays(1) };
            var m5 = new Meeting { MTG_ID = 5, START_DATE = currentDate.AddMonths(-6), END_DATE = currentDate.AddMonths(-6).AddDays(1) };
            var m6 = new Meeting { MTG_ID = 6, START_DATE = currentDate.AddMonths(-5), END_DATE = currentDate.AddMonths(-5).AddDays(1) };
            var m7 = new Meeting { MTG_ID = 7, START_DATE = currentDate.AddMonths(-4), END_DATE = currentDate.AddMonths(-4).AddDays(1) };
            var m8 = new Meeting { MTG_ID = 8, START_DATE = currentDate.AddDays(-1), END_DATE = currentDate };
            var m9 = new Meeting { MTG_ID = 9, START_DATE = currentDate.AddDays(-1), END_DATE = currentDate.AddDays(1) };
            var m10 = new Meeting { MTG_ID = 10, START_DATE = currentDate.AddMonths(1), END_DATE = currentDate.AddMonths(1).AddDays(1) };
            var m11 = new Meeting { MTG_ID = 11, START_DATE = currentDate.AddMonths(2), END_DATE = currentDate.AddMonths(2).AddDays(1) };
            var m12 = new Meeting { MTG_ID = 12, START_DATE = currentDate.AddMonths(3), END_DATE = currentDate.AddMonths(3).AddDays(1) };
            var m13 = new Meeting { MTG_ID = 13, START_DATE = currentDate.AddMonths(8).AddDays(-1), END_DATE = currentDate.AddMonths(8).AddDays(1) };
            var m14 = new Meeting { MTG_ID = 14, START_DATE = currentDate.AddMonths(8), END_DATE = currentDate.AddMonths(8).AddDays(1) };
            var m15 = new Meeting { MTG_ID = 15, START_DATE = currentDate.AddMonths(8).AddDays(1), END_DATE = currentDate.AddMonths(8).AddDays(2) };
            var meetings = new List<Meeting> { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m14, m15 };
            return meetings;
        }

        /// <summary>
        /// Gets the organisations.
        /// </summary>
        /// <returns>List of organisations</returns>
        private List<Organisation> GetOrganisations()
        {
            var o1 = new Organisation { ORGA_ID = 1 }; //Valid Organisations
            var o2 = new Organisation { ORGA_ID = 2 };
            var o3 = new Organisation { ORGA_ID = 3 };
            var o4 = new Organisation { ORGA_ID = 4 };
            var o5 = new Organisation { ORGA_ID = 5 };
            var o6 = new Organisation { ORGA_ID = 6 };
            var o7 = new Organisation { ORGA_ID = 7 }; //Invalid Organisations
            var o8 = new Organisation { ORGA_ID = 8 };

            var organisations = new List<Organisation> { o1, o2, o3, o4, o5, o6, o7, o8 };
            return organisations;
        }

        /// <summary>
        /// Gets the organisation status.
        /// </summary>
        /// <returns>List of organisation status</returns>
        private List<OrganisationStatus> GetOrganisationStatus()
        {
            var os1 = new OrganisationStatus { ORGA_ID = 1, ORGS_CODE = "3GPPMARK_REP" }; //Valid Organisations
            var os2 = new OrganisationStatus { ORGA_ID = 2, ORGS_CODE = "3GPPMEMBER" };
            var os3 = new OrganisationStatus { ORGA_ID = 3, ORGS_CODE = "3GPPOBSERV" };
            var os4 = new OrganisationStatus { ORGA_ID = 4, ORGS_CODE = "3GPPORG_REP" };
            var os5 = new OrganisationStatus { ORGA_ID = 5, ORGS_CODE = "3GPPGUEST" };
            var os6 = new OrganisationStatus { ORGA_ID = 6, ORGS_CODE = "3GPPINVITE" };
            var os7 = new OrganisationStatus { ORGA_ID = 7, ORGS_CODE = "3GPPEXPELLED" }; //Invalid Organisations
            var os8 = new OrganisationStatus { ORGA_ID = 8, ORGS_CODE = "3GPPWITHDRAW" };

            var organisationStatus = new List<OrganisationStatus> { os1, os2, os3, os4, os5, os6, os7, os8 };
            return organisationStatus;
        } 

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert list to IDbSet
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>IDbSet for the given List</returns>
        private static IDbSet<T> GetDbSet<T>(IList<T> data) where T : class
        {
            IQueryable<T> queryable = data.AsQueryable();

            IDbSet<T> dbSet = MockRepository.GenerateMock<IDbSet<T>, IQueryable>();

            dbSet.Stub(m => m.Provider).Return(queryable.Provider);
            dbSet.Stub(m => m.Expression).Return(queryable.Expression);
            dbSet.Stub(m => m.ElementType).Return(queryable.ElementType);
            dbSet.Stub(m => m.GetEnumerator()).Return(queryable.GetEnumerator());

            return dbSet;
        } 

        #endregion
    }
}
