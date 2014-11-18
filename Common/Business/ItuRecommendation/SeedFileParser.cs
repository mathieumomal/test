using System;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Standard implementation of the ISeedFileParser
    /// </summary>
    public class SeedFileParser: ISeedFileParser
    {
        /// <summary>
        /// Checks that file is a docx, then loops through all headers to find TS/TR numbers.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ServiceResponse<List<KeyValuePair<string,string>>> ExtractSpecificationNumbersFromSeeFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Interface to isolate the algorithm to parse the seed file.
    /// </summary>
    public interface ISeedFileParser
    {

        /// <summary>
        /// Opens the file and parses it to extract all the specifications numbers from the headers.
        /// </summary>
        /// <param name="filePath">Path where the file can be found.</param>
        /// <returns>A list of keyvalue pairs (clause#, spec#) strings representing all the specification numbers extracted if the parsing went well.
        /// Else, the Report should contain errors.</returns>
        ServiceResponse<List<KeyValuePair<string, string>>> ExtractSpecificationNumbersFromSeeFile(string filePath);
    }
}
