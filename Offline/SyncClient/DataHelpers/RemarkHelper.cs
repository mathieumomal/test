using SyncInterface;
using SyncInterface.DataContract;
using System;
using System.Collections.Generic;
using System.Data;

namespace SyncClient.DataHelpers
{
    /// <summary>
    /// Data helper class for dbo.Remarks table
    /// </summary>
    public class RemarkHelper : IDataHelper
    {
        #region Constants

        private const string CONST_TABLE_NAME = "Remarks";
        private const string CONST_INSERT_FLAG = "I";
        private const string CONST_UPDATE_FLAG = "U"; 

        #endregion

        #region Properties

        /// <summary>
        /// Table Name in Database
        /// </summary>
        public string TableName
        {
            get { return CONST_TABLE_NAME; }
        }

        #endregion

        #region IDataHelper Members

        /// <summary>
        /// Get inserted data (offline)
        /// </summary>
        /// <returns>Inserted objects</returns>
        public KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>> GetInsertData()
        {
            List<KeyValuePair<int, object>> remarks = new List<KeyValuePair<int, object>>();
            DataTable dtInsertRecords = SQLHelper.GetChanges(CONST_TABLE_NAME, CONST_INSERT_FLAG);
            foreach (DataRow dr in dtInsertRecords.Rows)
            {
                Remark remark = new Remark();
                FillRemark(remark, dr, true);
                var remarkWithPrimaryKey = new KeyValuePair<int, object>(remark.Pk_RemarkId, remark);
                remarks.Add(remarkWithPrimaryKey);
            }

            var insertData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>>(EnumEntity.Remark, remarks);
            return insertData;
        }

        /// <summary>
        /// Get updated data (offline)
        /// </summary>
        /// <returns>Updated objects</returns>
        public KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>> GetUpdateData()
        {
            List<KeyValuePair<int, object>> remarks = new List<KeyValuePair<int, object>>();
            DataTable dtUpdateRecords = SQLHelper.GetChanges(CONST_TABLE_NAME, CONST_UPDATE_FLAG);
            foreach (DataRow dr in dtUpdateRecords.Rows)
            {
                Remark remark = new Remark();
                FillRemark(remark, dr, false);
                var remarkWithPrimaryKey = new KeyValuePair<int, object>((int)dr["OfflinePrimaryKey"], remark);
                remarks.Add(remarkWithPrimaryKey);
            }

            var updateData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>>(EnumEntity.Remark, remarks);
            return updateData;
        }

        /// <summary>
        /// Get deleted data (offline)
        /// </summary>
        /// <returns>Deleted objects</returns>
        public KeyValuePair<EnumEntity, List<KeyValuePair<int, int>>> GetDeleteData()
        {
            var deleteData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, int>>>(EnumEntity.Remark, SQLHelper.GetDeleteInfo(CONST_TABLE_NAME));
            return deleteData;
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Fill Remark object
        /// </summary>
        /// <param name="remark">Remark</param>
        /// <param name="dataRow">DataRow</param>
        /// <param name="isInsert">Insert/Update flag</param>
        private void FillRemark(Remark remark, DataRow dataRow, bool isInsert)
        {
            int intResult;
            DateTime dateTimeResult;
            bool booleanResult;
            //Primary Key should be Offline Key for Insert & Online Key for Update
            remark.Pk_RemarkId = isInsert ? (int)dataRow["OfflinePrimaryKey"] : (int)dataRow["OnlinePrimaryKey"];
            if ((dataRow["Fk_PersonId"] != null) && (Int32.TryParse(dataRow["Fk_PersonId"].ToString(), out intResult)))
                remark.Fk_PersonId = intResult;
            if ((dataRow["IsPublic"] != null) && (Boolean.TryParse(dataRow["IsPublic"].ToString(), out booleanResult)))
                remark.IsPublic = booleanResult;
            if ((dataRow["CreationDate"] != null) && (DateTime.TryParse(dataRow["CreationDate"].ToString(), out dateTimeResult)))
                remark.CreationDate = dateTimeResult;
            if (dataRow["RemarkText"] != null)
                remark.RemarkText = Convert.ToString(dataRow["RemarkText"]);
            if ((dataRow["Fk_VersionId"] != null) && (Int32.TryParse(dataRow["Fk_VersionId"].ToString(), out intResult)))
                remark.Fk_VersionId = intResult;
        }

        #endregion
    }
}
