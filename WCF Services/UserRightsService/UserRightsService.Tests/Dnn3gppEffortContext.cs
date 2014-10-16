using Effort.DataLoaders;
using Etsi.UserRights.DNN3GPPDataAccess;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Text;

namespace UserRightsService.Tests
{
    /// <summary>
    /// Effort context for Dnn3gpp
    /// </summary>
    public class Dnn3gppEffortContext : DNN3GPPContext
    {
        #region Constants

        const string CsvPathFor3gpp = "\\Database\\DNN3GPP";
        const string Dnn3gppContextName = "DNN3GPPContext"; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Dnn3gppEffortContext class.
        /// </summary>
        public Dnn3gppEffortContext()
            : base(GetEntityConnection())
        {
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the entity connection of effort ngppdb.
        /// </summary>
        /// <returns></returns>
        private static EntityConnection GetEntityConnection()
        {
            //CSV files are stored in a specific folder : Data_For_EffortUnitTests
            string Dnn3gppDir = Environment.CurrentDirectory + CsvPathFor3gpp;

            //Load DS related data
            IDataLoader Dnn3gppLoader = new CsvDataLoader(Dnn3gppDir);
            try
            {
                EntityConnection ngppdbConnection = Effort.EntityConnectionFactory.CreateTransient(
                    new StringBuilder().Append("name=").Append(Dnn3gppContextName).ToString(),
                    Dnn3gppLoader);
                return ngppdbConnection;
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
