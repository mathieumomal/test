using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business.Versions
{
    public class SpecVersionUploadManager : ISpecVersionUploadManager
    {
        /// <summary>
        /// Change filename of uploaded version, in order to avoid Interop popup issues (see https://support.microsoft.com/en-us/help/257757/considerations-for-server-side-automation-of-office)
        ///     PLEASE FIND AN EXTRACT OF THIS PAGE:
        ///     "Problems using server-side Automation of Office" -> "Interactivity with the desktop" -> 
        ///     If an unexpected error occurs, or if an unspecified parameter is needed to complete a function, 
        ///     Office is designed to prompt the user with a modal dialog box that asks the user what the user wants to do. 
        ///     A modal dialog box on a non-interactive desktop cannot be dismissed. 
        ///     Therefore, that thread stops responding (hangs) indefinitely. 
        ///     Although certain coding practices can help reduce the likelihood of this issue, these practices cannot prevent the issue entirely. 
        ///     This fact alone makes running Office Applications from a server-side environment risky and unsupported.
        ///     
        /// Notes:
        /// After some of this kind of errors occured on server side, we detected that: 
        /// By trying to upload version one time -> an error could occured with a popup raised by Interop.
        /// Then by fixing on our machine, then by retrying to upload it -> it failed again (we suppose because of the name of the file which is the same than the previous one...)
        /// By fixing on our machine, then removing old file on the server, then finally by retrying to upload it -> it was a success!
        /// So, the idea is to generate a different path/filename for any try! Like that users will be able to open the file on there machine fix any unexpected problems then try to upload it again without contacting helpdesk. 
        /// </summary>
        /// <param name="path">Initial path of the version</param>
        /// <returns>new unique path for the version</returns>
        public string GetUniquePath(string path)
        {
            var guid = Guid.NewGuid().ToString().Substring(0, 6);
            var uniquePath = Path.Combine(Path.GetDirectoryName(path), guid + Path.GetExtension(path));
            File.Move(path, uniquePath);
            LogManager.InfoFormat("UPLOAD VERSION: Filename CORRESPONDENCE {0} -> {1}", Path.GetFileName(path), Path.GetFileName(uniquePath));
            return uniquePath;
        }

        public string ArchiveUploadedVersion(string path, string uniquePath)
        {
            var filename = Path.GetFileName(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var uploadedFileExtension = Path.GetExtension(path);

            //Check if archive folder exists
            if (!Directory.Exists(ConfigVariables.UploadVersionArchiveFolder))
            {
                Directory.CreateDirectory(ConfigVariables.UploadVersionArchiveFolder);
            }

            //Create folder for current version
            var currentArchiveFolder = Path.Combine(ConfigVariables.UploadVersionArchiveFolder, filenameWithoutExtension);
            if (!Directory.Exists(currentArchiveFolder))
            {
                Directory.CreateDirectory(currentArchiveFolder);
                LogManager.DebugFormat("UPLOAD VERSION: Dedicated archive folder created {0}", currentArchiveFolder);
            }
            else
            {
                DirectoryInfo alreadyExistingArchiveFolder = new DirectoryInfo(currentArchiveFolder);
                foreach (FileInfo file in alreadyExistingArchiveFolder.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo directory in alreadyExistingArchiveFolder.GetDirectories())
                {
                    directory.Delete(true);
                }
                LogManager.DebugFormat("UPLOAD VERSION: Content of archive folder removed {0}", currentArchiveFolder);
            }

            if (uploadedFileExtension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                var unzipFolder = Path.Combine(Path.GetDirectoryName(uniquePath), Path.GetFileNameWithoutExtension(uniquePath));
                if (ConfigVariables.UploadVersionSystemShouldKeepUnZipFolder)
                {
                    Directory.Move(unzipFolder, Path.Combine(currentArchiveFolder, "unzip"));
                }
                else
                {
                    if (Directory.Exists(unzipFolder))
                    {
                        Directory.Delete(unzipFolder, true);
                    }
                }
            }
            var finalPath = Path.Combine(currentArchiveFolder, filename);
            File.Move(uniquePath, finalPath);
            LogManager.DebugFormat("UPLOAD VERSION: Version archived! {0}", finalPath);
            return finalPath;
        }
    }

    public interface ISpecVersionUploadManager
    {
        string GetUniquePath(string path);
        string ArchiveUploadedVersion(string path, string uniquePath);
    }
}
