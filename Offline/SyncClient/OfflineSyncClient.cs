using SyncClient.DataHelpers;
using SyncClient.Properties;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Timers;

namespace SyncClient
{
    /// <summary>
    /// Windows Service to upload offline changes to online server
    /// </summary>
    public partial class OfflineSyncClient : ServiceBase
    {
        #region Constants
        
        private const string CONST_OFFLINE_TABLE_NAME = "dbo.Offline_{0}";

        #endregion

        #region Variables

        Timer timer = new Timer();
        bool isCurrentScheduleRunning = false;
        List<IDataHelper> insertHelpers = new List<IDataHelper>();
        List<IDataHelper> updateHelpers = new List<IDataHelper>();
        List<IDataHelper> deleteHelpers = new List<IDataHelper>();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize all data helpers
        /// </summary>
        public OfflineSyncClient()
        {
            InitializeComponent();
            LoadInsertHelperInfo();
            LoadUpdateHelperInfo();
            LoadDeleteHelperInfo();
        }

        #endregion

        #region Overridable Methods

        /// <summary>
        /// Service Start - Apply offline changes to online server
        /// </summary>
        /// <param name="args">Event arguments</param>
        protected override void OnStart(string[] args)
        {
            OnElapsedTime(null, null);
            timer.Elapsed += OnElapsedTime;
            timer.Interval = Settings.Default.ScheduleInterval;
            timer.Enabled = true;  
        }

        /// <summary>
        /// Service End - Stop timer & remove timer events
        /// </summary>
        protected override void OnStop()
        {
            timer.Elapsed -= OnElapsedTime;
            timer.Enabled = false; 
        }

        #endregion

        #region Debug Methods

        /// <summary>
        /// Debug Start
        /// </summary>
        public void DebugStart()
        {
            string[] args = new string[1];
            this.OnStart(args);
        }

        /// <summary>
        /// Debug End
        /// </summary>
        public void DebugEnd()
        {
            this.OnStop();
        }

        #endregion   

        #region Private Methods

        /// <summary>
        /// Load Insert Helper Information
        /// </summary>
        private void LoadInsertHelperInfo()
        {
            insertHelpers.Add(new SpecVersionHelper());
        }

        /// <summary>
        /// Load Update Helper Information
        /// </summary>
        private void LoadUpdateHelperInfo()
        {
            updateHelpers.Add(new SpecVersionHelper());
        }

        /// <summary>
        /// Load Delete Helper Information
        /// </summary>
        private void LoadDeleteHelperInfo()
        {
            deleteHelpers.Add(new SpecVersionHelper());
        }

        /// <summary>
        /// Elapsed Time Event (Occurs according to the given interval)
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void OnElapsedTime(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isCurrentScheduleRunning)
            {
                isCurrentScheduleRunning = true;
                try
                {
                    using (SyncServiceProxy proxy = new SyncServiceProxy(Settings.Default.ServiceURL))
                    {
                        //[1] Insert Data Online & update Offline references
                        foreach (var insertHelper in insertHelpers)
                        {
                            var insertData = insertHelper.GetInsertData();
                            foreach (var insertObject in insertData.Value)
                            {
                                int pk_Id = proxy.InsertRecord(insertObject.Value, insertData.Key, System.Environment.MachineName);
                                if (pk_Id > 0)
                                    SQLHelper.UpdatePrimaryKeyAndStatus(String.Format(CONST_OFFLINE_TABLE_NAME, insertHelper.TableName), insertObject.Key, pk_Id);
                            }
                        }

                        //[2] Update Data Online & update Offline references
                        foreach (var updateHelper in updateHelpers)
                        {
                            var updateData = updateHelper.GetUpdateData();
                            foreach (var updateObject in updateData.Value)
                            {
                                bool isSuccess = proxy.UpdateRecord(updateObject.Value, updateData.Key);
                                if (isSuccess)
                                    SQLHelper.UpdateStatus(String.Format(CONST_OFFLINE_TABLE_NAME, updateHelper.TableName), updateObject.Key);
                            }
                        }

                        //[3] Delete Data Online & update Offline references
                        foreach (var deleteHelper in deleteHelpers)
                        {
                            var deleteData = deleteHelper.GetDeleteData();
                            foreach (var deleteObject in deleteData.Value)
                            {
                                bool isSuccess = proxy.DeleteRecord(deleteObject.Value, deleteData.Key);
                                if (isSuccess)
                                    SQLHelper.UpdateStatus(String.Format(CONST_OFFLINE_TABLE_NAME, deleteHelper.TableName), deleteObject.Key);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    isCurrentScheduleRunning = false;
                }
            }
        }

        #endregion
    }
}
