using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Etsi.Ultimate.Utils
{

    /// <summary>
    /// The cache manager is in charge of putting values in and out of the cache when requested.
    /// 
    /// It provides a (very) simple interface to the HttpRuntime Cache, which is the cache that is going 
    /// to be used as a first approach.
    /// </summary>
    public class CacheManager
    {

        private CacheManager() { }

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
        /// Inserts data into the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value);
        }


        /// <summary>
        /// Clears a cache entry.
        /// </summary>
        /// <param name="key"></param>
        public static void Clear(string key)
        {
            if (HttpRuntime.Cache[key] == null)
                return;

            HttpRuntime.Cache.Remove(key);
        }
    }
}
