using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineDBSetup
{
    /// <summary>
    /// Process Offline Setup for the given database
    /// </summary>
    public class OfflineSetup
    {
        /// <summary>
        /// Process Offline Setup
        /// </summary>
        public void ProcessOfflineSetup()
        {
            try
            {
                if (SQLHelper.CanConnectSQL(Properties.Settings.Default.ConnectionString))
                {
                    //Load schema for tracking tables
                    var trackingTables = SQLHelper.LoadTrackingTables();

                    //Get the tracking table names to work with Foreign Key columns as part of change tracking stored procedures
                    List<string> trackingTableNames = trackingTables.Select(x => x.Schema + '.' + x.Name).ToList();
                    foreach (var trackingTable in trackingTables)
                    {
                        //Build Tracking Table
                        var queryTrackingTable = String.Format(Properties.Settings.Default.TrackingTable, trackingTable.Schema, trackingTable.Name);
                        SQLHelper.RunQuery(queryTrackingTable, Properties.Settings.Default.ConnectionString);

                        // Build Triggers on base table for tracking
                        var queryInsertTrigger = String.Format(Properties.Settings.Default.InsertTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                        SQLHelper.RunQuery(queryInsertTrigger, Properties.Settings.Default.ConnectionString);

                        var queryUpdateTrigger = String.Format(Properties.Settings.Default.UpdateTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                        SQLHelper.RunQuery(queryUpdateTrigger, Properties.Settings.Default.ConnectionString);

                        var queryDeleteTrigger = String.Format(Properties.Settings.Default.DeleteTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                        SQLHelper.RunQuery(queryDeleteTrigger, Properties.Settings.Default.ConnectionString);

                        //Build Stored Procedure to retrieve offline changes
                        StringBuilder queryColumns = new StringBuilder();
                        trackingTable.Columns.ForEach(x => queryColumns.AppendLine(String.Format(Properties.Settings.Default.ColumnSQL, x)));

                        StringBuilder queryForeignKeyColumns = new StringBuilder();
                        StringBuilder queryForeignKeyTables = new StringBuilder();

                        trackingTable.ForeignKeys.ForEach(x =>
                        {
                            if (trackingTableNames.Exists(y => y == x.Schema + '.' + x.TableName))
                            {
                                queryForeignKeyColumns.AppendLine(String.Format(Properties.Settings.Default.ForeignKeyColumnSQL, x.ColumnName, x.TableName.ToLower()));
                                queryForeignKeyTables.AppendLine(String.Format(Properties.Settings.Default.ForeignKeyTableSQL, x.Schema, x.TableName, x.TableName.ToLower(), x.ColumnName));
                            }
                            else
                            {
                                queryForeignKeyColumns.AppendLine(String.Format(Properties.Settings.Default.ColumnSQL, x.ColumnName));
                            }
                        });

                        var queryGetChangesStoredProcedure = String.Format(Properties.Settings.Default.GetChangesStoredProcedure, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey, queryColumns.ToString(), queryForeignKeyColumns.ToString(), queryForeignKeyTables.ToString());
                        SQLHelper.RunQuery(queryGetChangesStoredProcedure, Properties.Settings.Default.ConnectionString);
                    }
                    Console.WriteLine("Offline Setup installed successfully!!");
                }
                else
                {
                    Console.WriteLine("Connection failed!!");                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Offline Setup Failed!!\nError:" + ex.Message);
            }
        }
    }
}
