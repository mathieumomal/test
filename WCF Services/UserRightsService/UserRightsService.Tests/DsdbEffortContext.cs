using Effort.DataLoaders;
using Etsi.Dsdb.DataAccess;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Text;

namespace UserRightsService.Tests
{
    /// <summary>
    /// Effort context for dsdb
    /// </summary>
    public class DsdbEffortContext : DSDBContext
    {
        #region Constants

        const string CsvPathForDsdb = "\\Database\\DSDB";
        const string DsdbContextName = "DSDBContext"; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DsdbEffortContext class.
        /// </summary>
        public DsdbEffortContext()
            : base(GetEntityConnection())
        {
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the entity connection of effort dsdb.
        /// </summary>
        /// <returns></returns>
        private static EntityConnection GetEntityConnection()
        {
            //CSV files are stored in a specific folder : Data_For_EffortUnitTests
            string DsdbDir = Environment.CurrentDirectory + CsvPathForDsdb;

            //Load DS related data
            IDataLoader DsLoader = new CsvDataLoader(DsdbDir);
            try
            {
                EntityConnection dsdbConnection = Effort.EntityConnectionFactory.CreateTransient(
                    new StringBuilder().Append("name=").Append(DsdbContextName).ToString(),
                    DsLoader);
                return dsdbConnection;
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
        } 

        #endregion
    }
}
