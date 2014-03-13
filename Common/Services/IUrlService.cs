using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface IUrlService
    {
        
        /// <summary>
        /// Returns the main page for the provided module.
        /// </summary>
        /// <param name="moduleId">The Id of the module in which the link should be built</param>
        /// <param name="baseAddress"> The base address to use to create the address</param>
        /// <param name="urlParams"> The list of params in the GET request.</param>
        /// <returns> A Key value association containing: 
        /// - the Page Id
        /// - the full Url.
        /// </returns>
        KeyValuePair<int, string> GetPageIdAndFullAddressForModule(int moduleId, string baseAddress, Dictionary<string, string> urlParams);

        /// <summary>
        /// Generates a shortUrl and returns it to the module.
        /// </summary>
        /// <param name="moduleId">The Id of the module in which the link should be built</param>
        /// <param name="baseAddress">The base address to use to create the address</param>
        /// <param name="urlParams">The list of GET parameters inside the address</param>
        /// <returns>The short URL link.</returns>
        string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> urlParams);

        /// <summary>
        /// Returns the full URL corresponding to the token passed in parameter.
        /// </summary>
        /// <param name="token">The ShortUrl token</param>
        /// <returns>The full URL to redirect to.</returns>
        string GetFullUrlForToken(string token);
    }
}
