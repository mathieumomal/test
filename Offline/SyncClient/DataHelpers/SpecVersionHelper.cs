using SyncInterface;
using SyncInterface.DataContract;
using System;
using System.Collections.Generic;
using System.Data;

namespace SyncClient.DataHelpers
{
    /// <summary>
    /// Data helper class for dbo.SpecVersion table
    /// </summary>
    public class SpecVersionHelper : IDataHelper
    {
        #region Constants

        private const string CONST_TABLE_NAME = "Version";
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
            List<KeyValuePair<int, object>> specVersions = new List<KeyValuePair<int, object>>();
            DataTable dtInsertRecords = SQLHelper.GetChanges(CONST_TABLE_NAME, CONST_INSERT_FLAG);
            foreach (DataRow dr in dtInsertRecords.Rows)
            {
                SpecVersion specVersion = new SpecVersion();
                FillSpecVersion(specVersion, dr, true);
                var specVersionWithPrimaryKey = new KeyValuePair<int, object>(specVersion.Pk_VersionId, specVersion);
                specVersions.Add(specVersionWithPrimaryKey);
            }

            var insertData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>>(EnumEntity.SpecVersion, specVersions);
            return insertData;
        }

        /// <summary>
        /// Get updated data (offline)
        /// </summary>
        /// <returns>Updated objects</returns>
        public KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>> GetUpdateData()
        {
            List<KeyValuePair<int, object>> specVersions = new List<KeyValuePair<int, object>>();
            DataTable dtUpdateRecords = SQLHelper.GetChanges(CONST_TABLE_NAME, CONST_UPDATE_FLAG);
            foreach (DataRow dr in dtUpdateRecords.Rows)
            {
                SpecVersion specVersion = new SpecVersion();
                FillSpecVersion(specVersion, dr, false);
                var specVersionWithPrimaryKey = new KeyValuePair<int, object>((int)dr["OfflinePrimaryKey"], specVersion);
                specVersions.Add(specVersionWithPrimaryKey);
            }

            var updateData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>>(EnumEntity.SpecVersion, specVersions);
            return updateData;
        }

        /// <summary>
        /// Get deleted data (offline)
        /// </summary>
        /// <returns>Deleted objects</returns>
        public KeyValuePair<EnumEntity, List<KeyValuePair<int, int>>> GetDeleteData()
        {
            var deleteData = new KeyValuePair<EnumEntity, List<KeyValuePair<int, int>>>(EnumEntity.SpecVersion, SQLHelper.GetDeleteInfo(CONST_TABLE_NAME));
            return deleteData;
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Fill SpecVersion object
        /// </summary>
        /// <param name="specVersion">SpecVersion</param>
        /// <param name="dataRow">DataRow</param>
        /// <param name="isInsert">Insert/Update flag</param>
        private void FillSpecVersion(SpecVersion specVersion, DataRow dataRow, bool isInsert)
        {
            int intResult;
            DateTime dateTimeResult;
            bool booleanResult;
            //Primary Key should be Offline Key for Insert & Online Key for Update
            specVersion.Pk_VersionId = isInsert ? (int)dataRow["OfflinePrimaryKey"] : (int)dataRow["OnlinePrimaryKey"];
            if ((dataRow["MajorVersion"] != null) && (Int32.TryParse(dataRow["MajorVersion"].ToString(), out intResult)))
                specVersion.MajorVersion = intResult;
            if ((dataRow["TechnicalVersion"] != null) && (Int32.TryParse(dataRow["TechnicalVersion"].ToString(), out intResult)))
                specVersion.TechnicalVersion = intResult;
            if ((dataRow["EditorialVersion"] != null) && (Int32.TryParse(dataRow["EditorialVersion"].ToString(), out intResult)))
                specVersion.EditorialVersion = intResult;
            if ((dataRow["AchievedDate"] != null) && (DateTime.TryParse(dataRow["AchievedDate"].ToString(), out dateTimeResult)))
                specVersion.AchievedDate = dateTimeResult;
            if ((dataRow["ExpertProvided"] != null) && (DateTime.TryParse(dataRow["ExpertProvided"].ToString(), out dateTimeResult)))
                specVersion.ExpertProvided = dateTimeResult;
            if (dataRow["Location"] != null)
                specVersion.Location = Convert.ToString(dataRow["Location"]);
            if ((dataRow["SupressFromSDO_Pub"] != null) && (Boolean.TryParse(dataRow["SupressFromSDO_Pub"].ToString(), out booleanResult)))
                specVersion.SupressFromSDO_Pub = booleanResult;
            if ((dataRow["ForcePublication"] != null) && (Boolean.TryParse(dataRow["ForcePublication"].ToString(), out booleanResult)))
                specVersion.ForcePublication = booleanResult;
            if ((dataRow["DocumentUploaded"] != null) && (DateTime.TryParse(dataRow["DocumentUploaded"].ToString(), out dateTimeResult)))
                specVersion.DocumentUploaded = dateTimeResult;
            if ((dataRow["DocumentPassedToPub"] != null) && (DateTime.TryParse(dataRow["DocumentPassedToPub"].ToString(), out dateTimeResult)))
                specVersion.DocumentPassedToPub = dateTimeResult;
            if ((dataRow["Multifile"] != null) && (Boolean.TryParse(dataRow["Multifile"].ToString(), out booleanResult)))
                specVersion.Multifile = booleanResult;
            if ((dataRow["Source"] != null) && (Int32.TryParse(dataRow["Source"].ToString(), out intResult)))
                specVersion.Source = intResult;
            if ((dataRow["ETSI_WKI_ID"] != null) && (Int32.TryParse(dataRow["ETSI_WKI_ID"].ToString(), out intResult)))
                specVersion.ETSI_WKI_ID = intResult;
            if ((dataRow["ProvidedBy"] != null) && (Int32.TryParse(dataRow["ProvidedBy"].ToString(), out intResult)))
                specVersion.ProvidedBy = intResult;
            if ((dataRow["Fk_SpecificationId"] != null) && (Int32.TryParse(dataRow["Fk_SpecificationId"].ToString(), out intResult)))
                specVersion.Fk_SpecificationId = intResult;
            if ((dataRow["Fk_ReleaseId"] != null) && (Int32.TryParse(dataRow["Fk_ReleaseId"].ToString(), out intResult)))
                specVersion.Fk_ReleaseId = intResult;
            if (dataRow["ETSI_WKI_Ref"] != null)
                specVersion.ETSI_WKI_Ref = Convert.ToString(dataRow["ETSI_WKI_Ref"]);
        }

        #endregion
    }
}
