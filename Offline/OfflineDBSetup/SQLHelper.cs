using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace OfflineDBSetup
{
    /// <summary>
    /// Helper class to execute sql queries
    /// </summary>
    public class SQLHelper
    {
        #region Constants

        private const string CONST_SCHEMA_FILE_NAME = "TrackingTables.xml";
        private const string CONST_TABLE_SCHEMA = "TableSchema";
        private const string CONST_TABLE_Name = "TableName";
        private const string CONST_PRIMARY_KEY = "PrimaryKey";
        private const string CONST_FOREIGN_KEYS = "ForeignKeys";
        private const string CONST_COLUMNS = "Columns";

        #endregion

        #region Internal Helper Methods

        /// <summary>
        /// Check whether we can connect sql or not
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns>True/False</returns>
        internal static bool CanConnectSQL(string connectionString)
        {
            SqlConnection conn = new SqlConnection();

            try
            {
                conn.ConnectionString = connectionString;
                conn.Open();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return true;
        }

        /// <summary>
        /// Load Tracking Tables from xml
        /// </summary>
        /// <returns>Tracking Tables list</returns>
        internal static List<TrackingTable> LoadTrackingTables()
        {
            List<TrackingTable> trackingTables = new List<TrackingTable>();

            var doc = XDocument.Load(CONST_SCHEMA_FILE_NAME);
            doc.Root.Elements().ToList().ForEach(x =>
            {
                TrackingTable trackingTable = new TrackingTable();
                trackingTable.Schema = x.Attribute(CONST_TABLE_SCHEMA).Value;
                trackingTable.Name = x.Attribute(CONST_TABLE_Name).Value;
                trackingTable.PrimaryKey = x.Element(CONST_PRIMARY_KEY).Value;

                var foreignKeys = x.Element(CONST_FOREIGN_KEYS);
                if (foreignKeys != null)
                {
                    foreignKeys.Elements().ToList().ForEach(y =>
                    {
                        TrackingForeignKey foreignKey = new TrackingForeignKey();
                        foreignKey.Schema = y.Attribute(CONST_TABLE_SCHEMA).Value;
                        foreignKey.TableName = y.Attribute(CONST_TABLE_Name).Value;
                        foreignKey.ColumnName = y.Value;
                        trackingTable.ForeignKeys.Add(foreignKey);
                    });
                }

                var columns = x.Element(CONST_COLUMNS);
                if (columns != null)
                {
                    columns.Elements().ToList().ForEach(z =>
                    {
                        trackingTable.Columns.Add(z.Value);
                    });
                }
                trackingTables.Add(trackingTable);
            });
            return trackingTables;
        }

        /// <summary>
        /// Run query on database
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="connectionString">Connection String</param>
        internal static void RunQuery(string query, string connectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                ServerConnection serverConnection = new ServerConnection(sqlConnection);
                Server server = new Server(serverConnection);
                server.ConnectionContext.ExecuteNonQuery(query);
            }
        }

        #endregion
    }
}
