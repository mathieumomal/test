using OfflineSetup.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OfflineSetup
{
    public partial class Main : Form
    {
        #region Constants

        private const string CONST_USER_CONNECTION = "Server={0};Database={1};User Id={2};Password={3};";
        private const string CONST_TRUSTED_CONNECTION = "Server={0};Database={1};Trusted_Connection=True;";

        #endregion

        #region Constructor

        public Main()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Main_Load(object sender, EventArgs e)
        {
            this.txtServerName.Text = "(local)";
            this.txtDatabaseName.Text = "U3GPPDB";
            this.cbIntegratedSecurity.Checked = true;
            this.btnOfflineSetup.Enabled = false;
        }

        private void cbIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = ((CheckBox)sender).Checked;
            this.txtUserName.Enabled = !isChecked;
            this.txtPassword.Enabled = !isChecked;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string connectionString = cbIntegratedSecurity.Checked ? String.Format(CONST_TRUSTED_CONNECTION, txtServerName.Text, txtDatabaseName.Text) : 
                String.Format(CONST_USER_CONNECTION, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);
            if (SQLHelper.CanConnectSQL(connectionString))
            {
                MessageBox.Show("Connection Succeeded!!");

                this.txtServerName.Enabled = false;
                this.txtDatabaseName.Enabled = false;
                this.txtUserName.Enabled = false;
                this.txtPassword.Enabled = false;
                this.cbIntegratedSecurity.Enabled = false;
                this.btnConnect.Enabled = false;

                this.btnOfflineSetup.Enabled = true;
            }
            else 
            {
                MessageBox.Show("Connection failed!!");
            }

            this.Cursor = Cursors.Default;
        }

        private void btnOfflineSetup_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string connectionString = cbIntegratedSecurity.Checked ? String.Format(CONST_TRUSTED_CONNECTION, txtServerName.Text, txtDatabaseName.Text) :
                                                                         String.Format(CONST_USER_CONNECTION, txtServerName.Text, txtDatabaseName.Text, txtUserName.Text, txtPassword.Text);

                //Load schema for tracking tables
                var trackingTables = SQLHelper.LoadTrackingTables();

                //Get the tracking table names to work with Foreign Key columns as part of change tracking stored procedures
                List<string> trackingTableNames = trackingTables.Select(x => x.Schema + '.' + x.Name).ToList();
                foreach (var trackingTable in trackingTables)
                {
                    //Build Tracking Table
                    var queryTrackingTable = String.Format(Properties.Settings.Default.TrackingTable, trackingTable.Schema, trackingTable.Name);
                    SQLHelper.RunQuery(queryTrackingTable, connectionString);

                    // Build Triggers on base table for tracking
                    var queryInsertTrigger = String.Format(Properties.Settings.Default.InsertTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                    SQLHelper.RunQuery(queryInsertTrigger, connectionString);

                    var queryUpdateTrigger = String.Format(Properties.Settings.Default.UpdateTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                    SQLHelper.RunQuery(queryUpdateTrigger, connectionString);

                    var queryDeleteTrigger = String.Format(Properties.Settings.Default.DeleteTrigger, trackingTable.Schema, trackingTable.Name, trackingTable.PrimaryKey);
                    SQLHelper.RunQuery(queryDeleteTrigger, connectionString);

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
                    SQLHelper.RunQuery(queryGetChangesStoredProcedure, connectionString);
                }
                MessageBox.Show(String.Format("Offline Setup installed successfully on '{0}'!!", txtDatabaseName.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Offline Setup Failed!!\nError:" + ex.Message);
            }

            this.Cursor = Cursors.Default;
        }

        #endregion
    }
}
