using System;
using System.Web;
using System.Web.Caching;

namespace Etsi.Ultimate.Utils.Core
{
    /// <summary>
    /// The cache manager is in charge of putting values in and out of the cache when requested.
    /// 
    /// It provides a (very) simple interface to the HttpRuntime Cache, which is the cache that is going 
    /// to be used as a first approach.
    /// </summary>
    public class CacheManager
    {
        #region Constructor

        /// <summary>
        /// Private constructor to implement singleton pattern
        /// </summary>
        private CacheManager() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves data from the cache.
        /// </summary>
        /// <param name="key">Key to retrieve</param>
        /// <returns>The cached value as an object. Null if no key corresponds.</returns>
        public static object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }

        /// <summary>
        /// Retrieve item from Cache
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="key">Name of item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return (T)HttpRuntime.Cache.Get(key);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Inserts data into the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value);
        }

        /// <summary>
        /// Insert data into the cache for a limited number of time.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="minutes">Time span in minutes</param>
        public static void InsertForLimitedTime(string key, object value, int minutes)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// Insert data into the cache for a limited number of time with Sliding Expiration.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="minutes">Time span in minutes</param>
        public static void InsertForLimitedTimeWithSlidingExpiration(string key, object value, int minutes)
        {
            HttpRuntime.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Clears a cache entry.
        /// </summary>
        /// <param name="key">Key</param>
        public static void Clear(string key)
        {
            if (HttpRuntime.Cache[key] == null)
                return;

            HttpRuntime.Cache.Remove(key);
        }

        #endregion
    }
}
