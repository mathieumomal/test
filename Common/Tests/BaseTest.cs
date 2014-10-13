using CsvHelper;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Business.UserRightsService;

namespace Etsi.Ultimate.Tests
{
    public class BaseTest
    {
        [TestFixtureSetUp]
        public virtual void RegisterEffort()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
        }

        [SetUp]
        public virtual void Setup()
        {
            ManagerFactory.SetDefaultDependencies();
            ServicesFactory.SetDefaultDependencies();
            RepositoryFactory.SetDefaultDependencies();
            UtilsFactory.SetDefaultDependencies();
        }

        /// <summary>
        /// Provide List of Objects by reading csv file
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="csvFileLocation">Csv File Location</param>
        /// <returns>List of Objects</returns>
        protected List<T> GetAllTestRecords<T>(string csvFileLocation)
        {
            List<T> dataCollection = new List<T>();
           
            try
            {
                if (!String.IsNullOrEmpty(csvFileLocation))
                {
                    using (StreamReader reader = new StreamReader(csvFileLocation, Encoding.Default))
                    {
                        var csv = new CsvReader(reader);

                        csv.Configuration.Delimiter = ",";
                        csv.Configuration.DetectColumnCountChanges = true;
                        //csv.Configuration.Encoding = Encoding.UTF8;
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.TrimFields = false;
                        csv.Configuration.WillThrowOnMissingField = false;

                        while (csv.Read())
                        {
                            var record = csv.GetRecord<T>();
                            dataCollection.Add(record);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return dataCollection;
        }

        /// <summary>
        /// Initialize mock for user rights
        /// </summary>
        protected void InitializeUserRightsMock()
        {
            var mockUserRightsService = MockRepository.GenerateMock<IUserRightsService>();

            var anonymous_Applicatin_Rights = new List<string>() { "Release_ViewLimitedDetails" };
            var eolAccountOwner_Application_Rights = new List<string>() { "Release_ViewLimitedDetails", "Release_ViewDetails" };
            var superUser_Application_Rights = new List<string>() { "Release_ViewLimitedDetails", "Release_ViewDetails", "Release_ViewCompleteDetails" };
            var committeeOfficial_Application_Rights = new List<string>() { "Release_ViewLimitedDetails", "Release_ViewDetails", "Release_Close" };
            var committeeOfficial_Committee_Rights = new Dictionary<int, string[]>();
            committeeOfficial_Committee_Rights.Add(UserRolesFakeRepository.TB_ID1, new string[] { "Versions_Allocate" });
            committeeOfficial_Committee_Rights.Add(UserRolesFakeRepository.TB_ID2, new string[] { "Versions_Modify_MajorVersion" });

            var anonymousRights = new PersonRights() { ApplicationRights = anonymous_Applicatin_Rights.ToArray() };
            var specManagerRights = new PersonRights() { ApplicationRights = superUser_Application_Rights.ToArray() };
            var administratorRights = new PersonRights() { ApplicationRights = eolAccountOwner_Application_Rights.ToArray() };
            var chairmanRights = new PersonRights() { ApplicationRights = committeeOfficial_Application_Rights.ToArray(), CommitteeRights = committeeOfficial_Committee_Rights };

            mockUserRightsService.Stub(x => x.GetRights(UserRolesFakeRepository.SPECMGR_ID, ConfigVariables.PortalName)).Return(specManagerRights);
            mockUserRightsService.Stub(x => x.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID, ConfigVariables.PortalName)).Return(administratorRights);
            mockUserRightsService.Stub(x => x.GetRights(UserRolesFakeRepository.ANONYMOUS_ID, ConfigVariables.PortalName)).Return(anonymousRights);
            mockUserRightsService.Stub(x => x.GetRights(UserRolesFakeRepository.CHAIRMAN_ID, ConfigVariables.PortalName)).Return(chairmanRights);

            ManagerFactory.Container.RegisterInstance(typeof(IUserRightsService), mockUserRightsService);
        }
    }
}
