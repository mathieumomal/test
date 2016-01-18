using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Etsi.Ultimate.Utils
{
    public class CacheHelper
    {
        /// <summary>
        /// Insert value into the Cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="o">Item</param>
        /// <param name="key">Name of item</param>
        /// <param name="Minutes">Cache timeout</param>
        public static void Add<T>(T o, string key, int minutes)
        {
            HttpRuntime.Cache.Add(key, o, null,
              DateTime.UtcNow.AddMinutes(minutes),
              System.Web.Caching.Cache.NoSlidingExpiration,
              System.Web.Caching.CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// Remove item from Cache 
        /// </summary>
        /// <param name="key">Name of item</param>
        public static void Clear(string key)
        {
            HttpRuntime.Cache.Remove(key);
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
    }
}
