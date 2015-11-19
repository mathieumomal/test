using System;
using System.Linq;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Versions
{
    public class SpecVersionNumberValidator : ISpecVersionNumberValidator
    {
        /// <summary>
        /// Unit of work
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Check spec version number
        /// </summary>
        /// <param name="dbVersion">Version in database (could be null in create mode)</param>
        /// <param name="uiVersion">Version created or updated</param>
        /// <param name="mode">Mode</param>
        /// <param name="personId">Person id</param>
        /// <returns></returns>
        public ServiceResponse<bool> CheckSpecVersionNumber(SpecVersion dbVersion, SpecVersion uiVersion, SpecNumberValidatorMode mode,
            int personId)
        {
            var response = new ServiceResponse<bool>{ Result = true };

            try
            {
                //ALLOCATE
                if (mode == SpecNumberValidatorMode.Allocate)
                {
                    //[1] Check version doesn't already exist
                    var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                    versionRepository.UoW = UoW;
                    var versionsFoundForSpec = versionRepository.GetVersionsBySpecId(uiVersion.Fk_SpecificationId ?? 0);
                    if (versionsFoundForSpec.Any(v => v.MajorVersion == uiVersion.MajorVersion
                                                            && v.TechnicalVersion == uiVersion.TechnicalVersion
                                                                && v.EditorialVersion == uiVersion.EditorialVersion))
                    {
                        throw new Exception(Localization.Allocate_Error_Version_Not_Allowed);
                    }

                    //[2] Check major version number : 
                    // - draft : nothing to check
                    // - ucc : be sure that major version number correspond to the correct release number
                    if ((uiVersion.MajorVersion ?? 0) > 2)
                    {
                        //Get release of the current version
                        var releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                        releaseRepo.UoW = UoW;
                        var release = releaseRepo.Find(uiVersion.Fk_ReleaseId ?? 0);
                        if (release == null)
                            throw new Exception(Localization.Error_Release_Does_Not_Exist);

                        if (uiVersion.MajorVersion != release.Version3g){
                            response.Result = false;
                            response.Report.ErrorList.Add(Localization.Version_Major_Number_Allocate_Not_Valid);
                        }
                    }
                }
                else if (mode == SpecNumberValidatorMode.Upload)//UPLOAD
                {
                    //[1] Check major version number : 
                    // - draft : nothing to check
                    // - ucc : be sure that major version number correspond to the correct release number
                    if ((uiVersion.MajorVersion ?? 0) > 2)
                    {
                        //Get release of the current version
                        var releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                        releaseRepo.UoW = UoW;
                        var release = releaseRepo.Find(uiVersion.Fk_ReleaseId ?? 0);
                        if (release == null)
                            throw new Exception(Localization.Error_Release_Does_Not_Exist);

                        if (uiVersion.MajorVersion != release.Version3g)
                        {
                            response.Result = false;
                            response.Report.ErrorList.Add(Localization.Version_Major_Number_Allocate_Not_Valid);
                        }
                    }
                }
                else//EDIT
                {
                    //ONLY IF VERSION CHANGED APPLY CHECK RULES
                    if (dbVersion.MajorVersion != uiVersion.MajorVersion ||
                        dbVersion.TechnicalVersion != uiVersion.TechnicalVersion ||
                            dbVersion.EditorialVersion != uiVersion.EditorialVersion)
                    {

                        //[1] Check if version already exists in db for this spec
                        var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                        versionRepository.UoW = UoW;
                        var versionsFoundForSpec = versionRepository.GetVersionsBySpecId(dbVersion.Fk_SpecificationId ?? 0);
                        if (versionsFoundForSpec.Any(v => v.MajorVersion == uiVersion.MajorVersion
                                                                && v.TechnicalVersion == uiVersion.TechnicalVersion
                                                                    && v.EditorialVersion == uiVersion.EditorialVersion))
                        {
                            throw new Exception(string.Format(Localization.Version_Edit_Already_Exists, uiVersion.Version));
                        }

                        //[2] Check if user have the right to edit version
                        var specVersionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
                        specVersionMgr.UoW = UoW;
                        var numbersEditAllowed = specVersionMgr.CheckVersionNumbersEditAllowed(uiVersion, personId);
                        if (!numbersEditAllowed.Result || numbersEditAllowed.Report.GetNumberOfErrors() > 0 ||
                            numbersEditAllowed.Report.WarningList.Any())
                        {
                            numbersEditAllowed.Report.ErrorList.AddRange(numbersEditAllowed.Report.WarningList);
                            throw new Exception(string.Join("\n", numbersEditAllowed.Report.ErrorList));
                        }
                            

                        //[3] IF DOESN'T ALREADY EXIST : should apply other business rules on Major version number if updated
                        if (dbVersion.MajorVersion != uiVersion.MajorVersion)
                        {
                            //Business rules : allowed values for new version MAJOR number
                            // - if db version is draft -> accept ONLY 0, 1, 2 or current release number. Raise an error otherwise
                            if ((dbVersion.MajorVersion ?? 0) <= 2)
                            {
                                //Get release of the current version
                                var releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                                releaseRepo.UoW = UoW;
                                var release = releaseRepo.Find(uiVersion.Fk_ReleaseId ?? 0);
                                if (release == null)
                                    throw new Exception(Localization.Error_Release_Does_Not_Exist);

                                if ((uiVersion.MajorVersion ?? 0) <= 2 || uiVersion.MajorVersion == release.Version3g)
                                {
                                    dbVersion.MajorVersion = uiVersion.MajorVersion;
                                }
                                else
                                {
                                    response.Result = false;
                                    response.Report.ErrorList.Add(string.Format(Localization.Version_Edit_Draft_Allowed_Major_Numbers, release.Version3g));
                                }
                            }
                            // - if db version is UCC -> accept ONLY db major version number OR current release number. Raise an error otherwise
                            else
                            {
                                if (uiVersion.MajorVersion == dbVersion.MajorVersion ||
                                    uiVersion.MajorVersion == dbVersion.Release.Version3g)
                                {
                                    dbVersion.MajorVersion = uiVersion.MajorVersion;
                                }
                                else
                                {
                                    response.Result = false;
                                    response.Report.ErrorList.Add(Localization.Version_Edit_Ucc_Major_Cannot_Be_Update);
                                }
                            }
                        }

                        //For technical and editorial version number : always allow modifications
                        dbVersion.TechnicalVersion = uiVersion.TechnicalVersion;
                        dbVersion.EditorialVersion = uiVersion.EditorialVersion;
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.Error("CheckSpecVersionNumber in allocate mode - an unexpected error occured", e);
                response.Report.ErrorList.Add(e.Message);
                response.Result = false;
            }
            
            return response;
        }
    }

    /// <summary>
    /// Mode to check validity of specVersion number
    /// </summary>
    public enum SpecNumberValidatorMode
    {
        Edit,
        Allocate,
        Upload
    }

    public interface ISpecVersionNumberValidator
    {
        /// <summary>
        /// Unit of work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Check spec version number
        /// </summary>
        /// <param name="dbVersion">Version in database (could be null in create mode)</param>
        /// <param name="versionUpdated">Version created or updated</param>
        /// <param name="mode">Mode</param>
        /// <param name="personId">Person id</param>
        /// <returns></returns>
        ServiceResponse<bool> CheckSpecVersionNumber(SpecVersion dbVersion, SpecVersion versionUpdated, SpecNumberValidatorMode mode, int personId);
    }
}
