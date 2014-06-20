using SyncInterface;
using System;
using System.ServiceModel;

namespace SyncClient
{
    /// <summary>
    /// Proxy class to access SyncService members
    /// </summary>
    public class SyncServiceProxy : ISyncService, IDisposable
    {
        #region Variables

        private bool disposed = false;
        ISyncService proxy;
        protected string serviceURL;

        #endregion

        #region Constructor

        /// <summary>
        /// Create service proxy based on the given service url
        /// </summary>
        /// <param name="serviceURL">Service URL</param>
        public SyncServiceProxy(string serviceURL)
        {
            this.serviceURL = serviceURL;
            this.CreateProxy();
        } 

        #endregion

        #region Destructor

        /// <summary>
        /// Dispose the proxy object
        /// </summary>
        ~SyncServiceProxy()
        {
            Dispose(false);
        } 

        #endregion

        #region ISyncService Members

        /// <summary>
        /// Send insert record request to Sync service
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Primary Key of Inserted Record</returns>
        public int InsertRecord(object entity, EnumEntity entityType)
        {
            return proxy.InsertRecord(entity, entityType);
        }

        /// <summary>
        /// Send update record request to Sync service
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateRecord(object entity, EnumEntity entityType)
        {
            return proxy.UpdateRecord(entity, entityType);
        }

        /// <summary>
        /// Send delete record request to Sync service
        /// </summary>
        /// <param name="primaryKeyID">Primary Key of Record</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public bool DeleteRecord(int primaryKeyID, EnumEntity entityType)
        {
            return proxy.DeleteRecord(primaryKeyID, entityType);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose proxy object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create SyncService proxy object
        /// </summary>
        private void CreateProxy()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<ISyncService> factory = new ChannelFactory<ISyncService>(binding, new EndpointAddress(serviceURL));
            this.proxy = factory.CreateChannel();
        }

        /// <summary>
        /// Close proxy object of SyncService
        /// </summary>
        private void CloseProxy()
        {
            if (proxy != null)
            {
                ICommunicationObject channel = proxy as ICommunicationObject;
                if (channel != null)
                {
                    try
                    {
                        channel.Close();
                    }
                    catch (Exception)
                    {
                        channel.Abort();
                    }
                }
                proxy = null;
            }
        }

        /// <summary>
        /// Dispose the proxy object
        /// </summary>
        /// <param name="disposing">If true, proxy will be closed. Otherwise proxy will not be closed.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (proxy != null)
                    {
                        CloseProxy();
                    }
                }
                disposed = true;
            }
        }         

        #endregion
    }
}
