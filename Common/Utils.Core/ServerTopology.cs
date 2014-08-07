using System.Web;

namespace Etsi.Ultimate.Utils.Core
{
    /// <summary>
    /// This class is an entry point for the different variables that the environment might require.
    /// </summary>
    public class ServerTopology
    {
        #region Public Methods

        /// <summary>
        /// Returns the path of the server. If no httpcontext exist, returns "".
        /// </summary>
        /// <returns></returns>
        public static string GetServerRootPath()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Request.PhysicalApplicationPath;
            return "";
        } 

        #endregion
    }
}
