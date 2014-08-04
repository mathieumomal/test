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
    }
}
