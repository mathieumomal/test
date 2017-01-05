using System;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Versions
{
    public class VersionFilenameManager : IVersionFilenameManager
    {
        public string GenerateValidFilename(string specNumber, int? major, int? technical, int? editorial)
        {
            return string.Format("{0}-{1}",
                specNumber.Replace(".", ""),
                GenerateVersionString(major, technical, editorial));
        }

        public string GenerateVersionString(int? major, int? technical, int? editorial)
        {
            if (major == null || technical == null || editorial == null)
            {
                LogManager.Error("VersionFilenameManager.GenerateVersionString: One version number was null");
                throw new Exception(Localization.GenericError);
            }

            string versionString;
            if (major >= 36 || technical >= 36 || editorial >= 36)
            {
                versionString = major.Value.ToString("00") + technical.Value.ToString("00") + editorial.Value.ToString("00");
            }
            else
            {
                versionString = UtilsManager.EncodeVersionToBase36(major, technical, editorial);
            }
            return versionString;
        }
    }

    public interface IVersionFilenameManager
    {
        /// <summary>
        /// Business rules for version numbers: (logic inside method GenerateVersionString)
        /// - All version numbers lower than 36 : Convert version numbers in base36
        /// - At lease one version number is greater than 36 : Convert version numbers in two digits sequences
        /// Concerning spec number -> dot removed
        /// </summary>
        /// <param name="specNumber"></param> 
        /// <param name="major"></param>
        /// <param name="technical"></param>
        /// <param name="editorial"></param>
        /// <returns></returns>
        string GenerateValidFilename(string specNumber, int? major, int? technical, int? editorial);

        /// <summary>
        /// Business rules:
        /// - All version numbers lower than 36 : Convert version numbers in base36
        /// - At lease one version number is greater than 36 : Convert version numbers in two digits sequences
        /// </summary>
        /// <param name="major"></param>
        /// <param name="technical"></param>
        /// <param name="editorial"></param>
        /// <returns></returns>
        string GenerateVersionString(int? major, int? technical, int? editorial);
    }
}
