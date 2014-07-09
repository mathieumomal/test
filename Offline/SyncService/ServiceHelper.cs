using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using SyncInterface;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SyncService
{
    /// <summary>
    /// Helper class to connect service layer of ultimate solution for offline data synchronization
    /// </summary>
    public class ServiceHelper
    {
        #region Public Methods

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Primary Key of inserted entity</returns>
        public static int InsertEntity(object entity, EnumEntity entityType, string terminalName)
        {
            int primaryKeyID = 0;

            switch (entityType)
            {
                case EnumEntity.SpecVersion:
                    primaryKeyID = InsertEntity<SpecVersion>(entity, terminalName);
                    break;
                case EnumEntity.Remark:
                    primaryKeyID = InsertEntity<Remark>(entity, terminalName);
                    break;
            }

            return primaryKeyID;
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public static bool UpdateEntity(object entity, EnumEntity entityType)
        {
            bool isSuccess = false;

            switch (entityType)
            {
                case EnumEntity.SpecVersion:
                    isSuccess = UpdateEntity<SpecVersion>(entity);
                    break;
                case EnumEntity.Remark:
                    isSuccess = UpdateEntity<Remark>(entity);
                    break;
            }

            return isSuccess;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="primaryKeyID">Primary Key</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public static bool DeleteEntity(int primaryKeyID, EnumEntity entityType)
        {
            bool isSuccess = false;

            switch (entityType)
            {
                case EnumEntity.SpecVersion:
                    isSuccess = DeleteEntity<SpecVersion>(primaryKeyID);
                    break;
                case EnumEntity.Remark:
                    isSuccess = DeleteEntity<Remark>(primaryKeyID);
                    break;
            }

            return isSuccess;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Insert entity in online server
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Primary Key of inserted entity</returns>
        private static int InsertEntity<T>(object entity, string terminalName)
        {
            IOfflineService<T> specVersionService = ServicesFactory.Resolve<IOfflineService<T>>();
            return specVersionService.InsertEntity(ConvertObject<T>(entity), terminalName);
        }

        /// <summary>
        /// Update entity in online server
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Success/Failure</returns>
        private static bool UpdateEntity<T>(object entity)
        {
            IOfflineService<T> specVersionService = ServicesFactory.Resolve<IOfflineService<T>>();
            return specVersionService.UpdateEntity(ConvertObject<T>(entity));
        }

        /// <summary>
        /// Delete entity in online server
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="primaryKey">Primary Key</param>
        /// <returns>Success/Failure</returns>
        private static bool DeleteEntity<T>(int primaryKey)
        {
            IOfflineService<T> specVersionService = ServicesFactory.Resolve<IOfflineService<T>>();
            return specVersionService.DeleteEntity(primaryKey);
        }

        /// <summary>
        /// Convert source object to target (SyncInterface.DataContract => Etsi.Ultimate.DomainClasses)
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="objSource">Source object</param>
        /// <returns>Target object</returns>
        private static T ConvertObject<T>(object objSource)
        {

            //step : 1 Get the type of source object and create a new instance of that type
            Type typeSource = objSource.GetType();
            object objTarget = Activator.CreateInstance(typeof(T));

            //Step2 : Get all the properties of source object type
            PropertyInfo[] sourcePropertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<PropertyInfo> targetPropertyList = new List<PropertyInfo>(objTarget.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            //Step : 3 Assign all source property to taget object 's properties
            foreach (PropertyInfo sourceProperty in sourcePropertyInfo)
            {
                //Check whether property can be written to
                if (sourceProperty.CanWrite)
                {
                    //Step : 4 check whether property type is value type, enum or string type
                    if (sourceProperty.PropertyType.IsValueType || sourceProperty.PropertyType.IsEnum || sourceProperty.PropertyType.Equals(typeof(System.String)))
                    {
                        PropertyInfo targetProperty = targetPropertyList.Find(x => x.Name == sourceProperty.Name);
                        if (targetProperty != null)
                            targetProperty.SetValue(objTarget, sourceProperty.GetValue(objSource, null));
                    }
                }
            }
            return (T)objTarget;
        }

        #endregion
    }
}