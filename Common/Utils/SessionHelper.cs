using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Etsi.Ultimate.Utils
{
    public class SessionHelper
    {
        /// <summary>
        /// Insert value into the session using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="o">Item</param>
        /// <param name="key">Name of item</param>
        public static void Add<T>(T o, string key)
        {
            HttpContext.Current.Session[key] = o;
        }

        /// <summary>
        /// Remove item from session 
        /// </summary>
        /// <param name="key">Name of item</param>
        public static void Clear(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// Check for item in session
        /// </summary>
        /// <param name="key">Name of item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return HttpContext.Current.Session[key] != null;
        }

        /// <summary>
        /// Retrieve item from session
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="key">Name of item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return (T)HttpContext.Current.Session[key];
            }
            catch
            {
                return default(T);
            }
        }
    }
}
