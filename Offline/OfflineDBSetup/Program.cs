
namespace OfflineDBSetup
{
    /// <summary>
    /// Entry point of applicatin
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point of applicatin
        /// </summary>
        /// <param name="args">Arguments list</param>
        static void Main(string[] args)
        {
            OfflineSetup offlineSetup = new OfflineSetup();
            offlineSetup.ProcessOfflineSetup();
        }
    }
}
