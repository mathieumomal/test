using SyncClient.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SyncClient.DataHelpers
{
    /// <summary>
    /// Helper class to perform the following actions
    ///   - Get Offline changes (Inserted / Updated / Deleted)
    ///   - Deleted Objects information along with offline / online primary keys
    ///   - Update Online data Primary Key reference (in case of insert) in offline table
    ///   - Update Synchronization status done or not
    /// </summary>
    public class SQLHelper
    {
        #region Constants

        private const string CONST_PROC_GETSTATUS = "dbo.Offline_{0}_GetChanges"; 
        private const string CONST_DELETE_FLAG = "D";

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Offline Changes
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="operationFlag">Operation Flag(I/U/D)</param>
        /// <returns>Changes Data Table</returns>
        public static DataTable GetChanges(string tableName, string operationFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(Settings.Default.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(String.Format(CONST_PROC_GETSTATUS, tableName), con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Operation", operationFlag));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, tableName);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// Get Offline Deleted data information
        /// </summary>
        /// <param name="tableName">Table</param>
        /// <returns>Offline/Online primary keys of deleted records</returns>
        public static List<KeyValuePair<int, int>> GetDeleteInfo(string tableName)
        {
            List<KeyValuePair<int, int>> deleteInfo = new List<KeyValuePair<int, int>>();
            DataTable dtDeleteRecords = SQLHelper.GetChanges(tableName, CONST_DELETE_FLAG);
            foreach (DataRow dr in dtDeleteRecords.Rows)
            {
                var deleteRecordInfo = new KeyValuePair<int, int>((int)dr["OfflinePrimaryKey"], (int)dr["OnlinePrimaryKey"]);
                deleteInfo.Add(deleteRecordInfo);
            }
            return deleteInfo;
        }

        /// <summary>
        /// Update Offline Reference table with synchronization status done & update the new primary key which is generated in online server.
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="offinePrimaryKey">Offline Primary Key</param>
        /// <param name="onlinePrimaryKey">Online Primary Key</param>
        public static void UpdatePrimaryKeyAndStatus(string tableName, int offinePrimaryKey, int onlinePrimaryKey)
        {
            using (SqlConnection con = new SqlConnection(Settings.Default.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(String.Format("Update {0} Set Online_Pk_Id={1}, SyncStatus='Y' Where Offline_Pk_Id={2}", tableName, onlinePrimaryKey, offinePrimaryKey), con);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Update Offline Reference table with synchronization status done.
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="offinePrimaryKey">Offline Primary Key</param>
        public static void UpdateStatus(string tableName, int offinePrimaryKey)
        {
            using (SqlConnection con = new SqlConnection(Settings.Default.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(String.Format("Update {0} Set SyncStatus='Y' Where Offline_Pk_Id={1}", tableName, offinePrimaryKey), con);
                cmd.ExecuteNonQuery();
            }
        } 

        #endregion
    }
}
