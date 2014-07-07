using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class WpmRecordCreator
    {
        public IUltimateUnitOfWork _uoW { get; set; }
        
        public WpmRecordCreator(IUltimateUnitOfWork Uow)
        {
            _uoW = Uow;
        }

        /// <summary>
        /// Add all needed records at WPMDB
        /// </summary>
        /// <param name="version">The transposed version</param>
        /// <returns>ETSI work item identifier</returns>
        public int AddWpmRecords(SpecVersion version) 
        {
            if (version == null)
                return -1;
            try
            {
                IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = _uoW;
                int releaseID = version.Fk_ReleaseId.GetValueOrDefault();
                string releaseName = releaseRepo.Find(releaseID).Name;

                ISpecificationRepository specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
                specRepo.UoW = _uoW;
                int specID = version.Fk_SpecificationId.GetValueOrDefault();
                Specification spec = specRepo.Find(specID);

                ICommunityRepository communityRepo = RepositoryFactory.Resolve<ICommunityRepository>();
                communityRepo.UoW = _uoW;
                Community c = communityRepo.Find(spec.PrimeResponsibleGroup.Fk_commityId);
                int wgNumber = communityRepo.GetWgNumber(c.TbId, c.ParentTbId.GetValueOrDefault());

                IResponsibleGroupSecretaryRepository rgSecretaryRepo = RepositoryFactory.Resolve<IResponsibleGroupSecretaryRepository>();
                rgSecretaryRepo.UoW = _uoW;
                int secretaryID = rgSecretaryRepo.FindAllByCommiteeId(c.TbId).FirstOrDefault().PersonId;

                bool isAlreadyTransposed = false;
                ISpecVersionsRepository versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                versionRepo.UoW = _uoW;
                isAlreadyTransposed = (versionRepo.GetVersionsForSpecRelease(version.Fk_SpecificationId.GetValueOrDefault(), version.Fk_ReleaseId.GetValueOrDefault()).FirstOrDefault(v => v.ETSI_WKI_ID != null) != null);


                EtsiWorkItemImport importData = new EtsiWorkItemImport(version, spec, c, wgNumber, secretaryID, releaseName, isAlreadyTransposed);
                IWorkProgramRepository wpRepo = RepositoryFactory.Resolve<IWorkProgramRepository>();
                wpRepo.UoW = _uoW;
                //Import Work Item to WPMDB
                int WKI_ID = wpRepo.InsertEtsiWorkITem(importData);

                //Import Schedule to WPMDB
                wpRepo.InsertWIScheduleEntry(WKI_ID, version.MajorVersion.GetValueOrDefault(), version.TechnicalVersion.GetValueOrDefault(), version.EditorialVersion.GetValueOrDefault());

                //Import Keyword to WPMDB
                wpRepo.InsertWIKeyword(WKI_ID, "");


                //Get WPM project code 

                //Import project to WPMDB
                wpRepo.InsertWIProject(WKI_ID, 0);

                //Import Remark to WPMDB
                wpRepo.InsertWIRemeark(WKI_ID, 0, "");

                return WKI_ID;
            }
            catch (Exception e)
            {
                Utils.LogManager.Error("WPM record creation error: " + e.Message);
                return -1;
            }
            
        }

    }    
}
