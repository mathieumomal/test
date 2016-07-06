using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Etsi.Ultimate.Business.Versions
{
    public class FtpFoldersManager : IFtpFoldersManager
    {
        private const string ConstFtpArchivePath = "{0}\\Specs\\archive\\{1}_series\\{2}\\";
        private const string ConstFtpCustomPath = "{0}\\Specs\\{1}\\{2}\\{3}_series\\";
        private const string ConstFtpLatestPath = "{0}\\Specs\\latest\\{1}\\{2}_series\\";
        private const string ConstFtpLatestPathBase = "{0}\\Specs\\latest";
        private const string ConstValidFilename = "{0}-{1}";
        private const string ConstFtpFoldersManagerStatus = "FtpFoldersManagerStatus";

        private FtpFoldersManagerStatus ftpFoldersManagerStatus
        {
            get
            {
                return CacheManager.Get<FtpFoldersManagerStatus>(ConstFtpFoldersManagerStatus);
            }
            set
            {
                CacheManager.InsertForLimitedTimeWithSlidingExpiration(ConstFtpFoldersManagerStatus, value, 2);
            }
        }

        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Checks before create latest folder
        /// </summary>
        /// <returns>Service response bool</returns>
        public ServiceResponse<bool> CheckLatestFolder(string folderName, int personId)
        {
            var response = new ServiceResponse<bool>() { Result = true };

            var latestFolderRepository = RepositoryFactory.Resolve<ILatestFolderRepository>();
            latestFolderRepository.UoW = UoW;

            /* Check User rights */
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);
            if (!personRights.HasRight(Enum_UserRights.Specification_SyncHardLinksOnLatestFolder))
            {
                response.Report.LogError(Localization.RightError);
                response.Result = false;
                return response;
            }

            /* Check if folderName is not empty */
            if (string.IsNullOrWhiteSpace(folderName))
            {
                response.Report.LogError(Localization.LatestFolder_FolderName_IsMandatory);
                response.Result = false;
                return response;
            }

            /* Check if another lastest folder task is running */
            if (ftpFoldersManagerStatus != null && !ftpFoldersManagerStatus.Finished)
            {
                response.Report.LogError(Localization.LatestFolder_AnotherTask_IsRunning);
                response.Result = false;
                return response;
            }

            /* Check if folder name exists */
            if (latestFolderRepository.Exists(folderName))
            {
                response.Report.LogError(Localization.LatestFolder_FolderName_Already_Exist);
                response.Result = false;
                return response;
            }

            return response;
        }

        /// <summary>
        /// Create & Fill version Lastest Folder
        /// </summary>
        public void CreateAndFillVersionLatestFolder(string folderName, int personId)
        {            
            var thread = new Thread(CreateAndFillVersionLatestFolderThreadMethod);
            thread.Start(new ThreadFacade { PersonId = personId, FolderName = folderName, Identity = WindowsIdentity.GetCurrent() });
        }

        private void CreateAndFillVersionLatestFolderThreadMethod(object threadFacade)
        {
            var tf = threadFacade as ThreadFacade;
            WindowsIdentity identity = tf.Identity;
            WindowsImpersonationContext impersonateContext = null;
            if (identity != null)
                impersonateContext = identity.Impersonate();

            try
            {
                HttpContext ctx = HttpContext.Current;
                HttpContext.Current = ctx;
                CreateAndFillVersionLatestFolderThread(tf.FolderName, tf.PersonId);
            }
            catch (Exception e)
            {
                LogManager.Error("Error occured inside CreateAndFillVersionLatestFolderThreadMethod", e);
            }
            finally
            {
                if (impersonateContext != null)
                    impersonateContext.Undo();
            }
        }

        /// <summary>
        /// Create & Fill version Lastest Folder (thread)
        /// </summary>
        public void CreateAndFillVersionLatestFolderThread(string folderName, int personId)
        {
            using (var UoWThread = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    /* Init ftp Folders Manager Status */
                    ftpFoldersManagerStatus = new FtpFoldersManagerStatus();

                    var latestFolderRepository = RepositoryFactory.Resolve<ILatestFolderRepository>();
                    latestFolderRepository.UoW = UoWThread;

                    /* Get latest version of each SpecRelease open or frozen */
                    var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                    specVersionRepo.UoW = UoWThread;
                    var SpecVersions = specVersionRepo.GetLatestVersionGroupedBySpecRelease();
                    SpecVersions = SpecVersions.Where(x => x.DocumentUploaded.HasValue).ToList();

                    if (SpecVersions != null && SpecVersions.Count > 0)
                    {
                        /* Create FTP paths */
                        var ftpBasePath = ConfigVariables.FtpBasePhysicalPath;

                        /* Clear current latest folder */
                        var ftpLatestFolder = string.Format(ConstFtpLatestPathBase, ftpBasePath);
                        clearFolder(ftpLatestFolder);

                        /* init total files into cache */
                        ftpFoldersManagerStatus.TotalFiles = SpecVersions.Count;

                        for (int i = 0; i < SpecVersions.Count; i++)
                        {
                            var version = SpecVersions.ElementAt(i);
                            ftpFoldersManagerStatus.CurrentNumberFile = i;
                            var splittedSpecNumber = version.Specification.Number.Split('.')[0];
                            /* Ftp "Archive" folder path */
                            var ftpArchivePath = string.Format(ConstFtpArchivePath, ftpBasePath, splittedSpecNumber, version.Specification.Number);
                            /* Ftp "Latest" folder path */
                            var ftpTargetPathLatest = string.Format(ConstFtpLatestPath, ftpBasePath, version.Release.Code, splittedSpecNumber);
                            /* Ftp <xxxx> custom folder path */
                            var ftpTargetPathCustom = string.Format(ConstFtpCustomPath, ftpBasePath, folderName, version.Release.Code, splittedSpecNumber);
                            
                            /* Create latest target folder if not exists */
                            DirectoryInfo di = new DirectoryInfo(ftpTargetPathLatest);
                            if (!di.Exists)
                                di.Create();

                            /* Create custom target folder if not exists */
                            DirectoryInfo diCust = new DirectoryInfo(ftpTargetPathCustom);
                            if (!diCust.Exists)
                                diCust.Create();

                            /* Generate archive full path */
                            var zipFile = GetValidFileName(version) + ".zip";
                            var versionPathToCopy = Path.Combine(ftpArchivePath, zipFile);
                            /* Generate custom full path destination */
                            var versionCustomPathToSave = Path.Combine(ftpTargetPathCustom, zipFile);
                            /* Generate latest full path destination */
                            var versionLatestPathToSave = Path.Combine(ftpTargetPathLatest, zipFile);
                            
                            try
                            {
                                /* try to copy archive file to custom folder */
                                File.Copy(versionPathToCopy, versionCustomPathToSave);
                                /* try to copy archive file to latest folder */
                                File.Copy(versionPathToCopy, versionLatestPathToSave);
                            }
                            catch (DirectoryNotFoundException e)
                            {
                                LogManager.Error(string.Format("An error occured while copy {0} to latest folder", versionPathToCopy), e);
                            }
                            catch (FileNotFoundException e)
                            {
                                LogManager.Error(string.Format("An error occured while copy {0} to latest folder", versionPathToCopy), e);
                            }
                            catch (Exception e)
                            {
                                LogManager.Error(string.Format("An error occured while copy {0} to latest folder", versionPathToCopy), e);
                                ftpFoldersManagerStatus.ErrorMessages.Add(
                                    string.Format(Localization.LatestFolder_CopyFileError, versionPathToCopy));
                            }
                        }

                        /* Get user Infos */
                        var personMgr = ManagerFactory.Resolve<IPersonManager>();
                        personMgr.UoW = UoWThread;
                        var person = personMgr.FindPerson(personId);
                        var userName = string.Empty;
                        if (person != null)
                        {
                            userName = person.Username;
                        }

                        /* Adding Lastest Folder name into DB */
                        latestFolderRepository.Add(folderName, userName);
                        UoWThread.Save();
                    }

                }
                catch (Exception e)
                {
                    LogManager.Error("An error occured while create latest folder ", e);
                    ftpFoldersManagerStatus.ErrorMessages.Add(Localization.GenericErrorManageFolder);
                }
                finally
                {
                    ftpFoldersManagerStatus.Finished = true;
                }
            }
        }

        /// <summary>
        /// Get valide file name
        /// </summary>
        /// <returns>valid file name</returns>
        private string GetValidFileName(SpecVersion specVersion)
        {
            var specNumber = specVersion.Specification.Number;
            var validFileName = String.Format(ConstValidFilename,
                specNumber.Replace(".", ""),
                UtilsManager.EncodeVersionToBase36(specVersion.MajorVersion, specVersion.TechnicalVersion, specVersion.EditorialVersion));
            return validFileName;
        }

        /// <summary>
        /// Get Ftp Folders Manager Status (from cache)
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        public FtpFoldersManagerStatus GetStatus()
        {
            return ftpFoldersManagerStatus;
        }

        /// <summary>
        /// Clear cache of Ftp Folders Manager Status
        /// </summary>
        public void ClearStatus()
        {
            CacheManager.Clear(ConstFtpFoldersManagerStatus);
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    File.Delete(fi.FullName);
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    clearFolder(di.FullName);
                    /* Directory.Delete(di.FullName); doesn't work on dev env */ 
                }
            }
        }

        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        public string GetFTPLatestFolderName()
        {
            var latestFolderRepository = RepositoryFactory.Resolve<ILatestFolderRepository>();
            latestFolderRepository.UoW = UoW;
            return latestFolderRepository.GetLatestFolderName();
        }
    }

    public interface IFtpFoldersManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Create & Fill version Lastest Folder
        /// </summary>
        void CreateAndFillVersionLatestFolder(string folderName, int personId);

        /// <summary>
        /// Create & Fill version Lastest Folder (new thread)
        /// </summary>
        void CreateAndFillVersionLatestFolderThread(string folderName, int personId);

        /// <summary>
        /// Checks before create latest folder
        /// </summary>
        /// <returns>Service response bool</returns>
        ServiceResponse<bool> CheckLatestFolder(string folderName, int personId);

        /// <summary>
        /// Get Ftp Folders Manager Status (from cache)
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        FtpFoldersManagerStatus GetStatus();

        /// <summary>
        /// Clear cache of Ftp Folders Manager Status
        /// </summary>
        void ClearStatus();

        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        string GetFTPLatestFolderName();
    }

    public class FtpFoldersManagerStatus
    {
        public FtpFoldersManagerStatus()
        {
            CurrentNumberFile = 0;
            Finished = false;
            TotalFiles = 0;
            ErrorMessages = new List<string>();
        }

        public int Percent
        {
            get
            {
                if (TotalFiles > 0)
                {
                    return Convert.ToInt32((double)CurrentNumberFile / (double)TotalFiles * 100);
                }
                else
                {
                    return 0;
                }
                
            }
        }
        public int TotalFiles { get; set; }
        public int CurrentNumberFile { get; set; }
        public bool Finished { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    public class ThreadFacade
    {
        public string Guid { get; set; }
        public WindowsIdentity Identity { get; set; }
        public string FolderName { get; set; }
        public int PersonId { get; set; }
    }

}
