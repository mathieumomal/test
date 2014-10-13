using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.UserRights.DNN3GPPDataAccess;
using Etsi.UserRights.DNNETSIDataAccess;
using Effort.DataLoaders;
using Etsi.UserRights.Service;
using Microsoft.Practices.Unity;
using Etsi.Dsdb.DataAccess;
using System.Data.Entity.Core.EntityClient;

namespace UserRightsService.Tests
{
    public class BaseEffortTest
    {

        /// <summary>
        /// Context for DSDB
        /// </summary>
        public DNN3GPPContext Dnn3gppContext { get; set; }

        /// <summary>
        /// Context for Ngppdb
        /// </summary>
        public DSDBContext DsdbContext { get; set; }

        /// <summary>
        /// CSVs folder path
        /// </summary>
        public string CsvPathFor3gpp = "\\Database\\DNN3GPP";
        public string CsvPathForDsdb = "\\Database\\DSDB";
        /// <summary>
        /// Context names (present in the App.config)
        /// </summary>
        public string Dnn3gppContextName = "DNN3GPPContext";
        public string DsdbContextName = "DSDBContext";

        /// <summary>
        /// Initialization of the context by a set of csv files
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            //CSV files are stored in a specific folder : Data_For_EffortUnitTests
            string Dnn3gppDir = Environment.CurrentDirectory + CsvPathFor3gpp;
            string DsdbDir = Environment.CurrentDirectory + CsvPathForDsdb;

            //Load DS related data
            IDataLoader Dnn3gppLoader = new CsvDataLoader(Dnn3gppDir);
            IDataLoader DsLoader = new CsvDataLoader(DsdbDir);
            try
            {
                EntityConnection dsdbConnection = Effort.EntityConnectionFactory.CreateTransient(
                    new StringBuilder().Append("name=").Append(Dnn3gppContextName).ToString(),
                    Dnn3gppLoader);
                EntityConnection ngppdbConnection = Effort.EntityConnectionFactory.CreateTransient(
                    new StringBuilder().Append("name=").Append(DsdbContextName).ToString(),
                    DsLoader);
                //Context creation
                Dnn3gppContext = new DNN3GPPContext(dsdbConnection);
                DsdbContext= new DSDBContext(ngppdbConnection);
            }
            catch (Exception e)
            {
                var error = new StringBuilder()
                    .Append("An error occured when effort try to import CSVs files and/or load the context... Please check the next logs :")
                    .Append(" MESSAGE : ")
                    .Append(e.Message)
                    .Append(" INNEREXCEPTION : ")
                    .Append(e.InnerException)
                    .ToString();
                throw new Exception(error);
            }

            DatabaseFactory.Container.RegisterInstance(typeof(IDnn3gppContext), Dnn3gppContext);
            DatabaseFactory.Container.RegisterInstance(typeof(DSDBContext), DsdbContext);
        }

        [TearDown]
        public void TearDown()
        {
            Dnn3gppContext.Dispose();
            DsdbContext.Dispose();
        }

    }
}
