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

        private const string CONST_TABLE_NAME = "SpecVersion";
        private const string CONST_INSERT_FLAG = "I";
        private const string CONST_UPDATE_FLAG = "U"; 

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
            //Primary Key should be Offline Key for Insert & Online Key for Update
            specVersion.Pk_VersionId = isInsert ? (int)dataRow["OfflinePrimaryKey"] : (int)dataRow["OnlinePrimaryKey"];
            specVersion.MajorVersion = Convert.ToInt32(dataRow["MajorVersion"]);
            specVersion.TechnicalVersion = Convert.ToInt32(dataRow["TechnicalVersion"]);
            specVersion.EditorialVersion = Convert.ToInt32(dataRow["EditorialVersion"]);
            specVersion.AchievedDate = Convert.ToDateTime(dataRow["AchievedDate"]);
            specVersion.ExpertProvided = Convert.ToDateTime(dataRow["ExpertProvided"]);
            specVersion.Location = Convert.ToString(dataRow["Location"]);
            specVersion.SupressFromSDO_Pub = Convert.ToBoolean(dataRow["SupressFromSDO_Pub"]);
            specVersion.ForcePublication = Convert.ToBoolean(dataRow["ForcePublication"]);
            specVersion.DocumentUploaded = Convert.ToDateTime(dataRow["DocumentUploaded"]);
            specVersion.DocumentPassedToPub = Convert.ToDateTime(dataRow["DocumentPassedToPub"]);
            specVersion.Multifile = Convert.ToBoolean(dataRow["Multifile"]);
            specVersion.Source = Convert.ToInt32(dataRow["Source"]);
            specVersion.ETSI_WKI_ID = Convert.ToInt32(dataRow["ETSI_WKI_ID"]);
            specVersion.ProvidedBy = Convert.ToInt32(dataRow["ProvidedBy"]);
            specVersion.Fk_SpecificationId = Convert.ToInt32(dataRow["Fk_SpecificationId"]);
            specVersion.Fk_ReleaseId = Convert.ToInt32(dataRow["Fk_ReleaseId"]);
            specVersion.ETSI_WKI_Ref = Convert.ToString(dataRow["ETSI_WKI_Ref"]);
        } 

        #endregion
    }
}
