using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class WpmRecordCreator
    {
        private const string REMARK_TEXT = "Record created automatically from 3GPP Specifications Status database.";
        private const int SEQ_NO = 1;

        public IUltimateUnitOfWork UoW { get; set; }
        
        public WpmRecordCreator(IUltimateUnitOfWork UoW)
        {
           this.UoW = UoW;
        }

        /// <summary>
        /// Add all needed records at WPMDB
        /// </summary>
        /// <param name="version">The transposed version</param>
        /// <returns>ETSI work item identifier</returns>
        public bool AddWpmRecords(SpecVersion version) 
        {
            if (version == null)
                return false; 
            try
            {
                IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                int releaseID = version.Fk_ReleaseId.GetValueOrDefault();
                Release release = releaseRepo.Find(releaseID);

                ISpecificationRepository specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
                specRepo.UoW = UoW;
                int specID = version.Fk_SpecificationId.GetValueOrDefault();
                Specification spec = specRepo.Find(specID);

                ICommunityRepository communityRepo = RepositoryFactory.Resolve<ICommunityRepository>();
                communityRepo.UoW = UoW;
                Community c = communityRepo.Find(spec.PrimeResponsibleGroup.Fk_commityId);
                int wgNumber = communityRepo.GetWgNumber(c.TbId, c.ParentTbId.GetValueOrDefault());

                IResponsibleGroupSecretaryRepository rgSecretaryRepo = RepositoryFactory.Resolve<IResponsibleGroupSecretaryRepository>();
                rgSecretaryRepo.UoW = UoW;
                int secretaryID = rgSecretaryRepo.FindAllByCommiteeId(c.TbId).FirstOrDefault().PersonId;

                bool isAlreadyTransposed = false;
                ISpecVersionsRepository versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                versionRepo.UoW = UoW;
                isAlreadyTransposed = (versionRepo.GetVersionsForSpecRelease(version.Fk_SpecificationId.GetValueOrDefault(), version.Fk_ReleaseId.GetValueOrDefault()).FirstOrDefault(v => v.ETSI_WKI_ID != null) != null);

                EtsiWorkItemImport importData = new EtsiWorkItemImport(version, spec, c, wgNumber, secretaryID, release.Name, isAlreadyTransposed);
                UtilsManager utilsMgr = new UtilsManager();
                importData.SetSerialNumber(utilsMgr.EncodeToBase36Digits2(version.MajorVersion.GetValueOrDefault()), utilsMgr.EncodeToBase36Digits2(version.TechnicalVersion.GetValueOrDefault()), utilsMgr.EncodeToBase36Digits2(version.EditorialVersion.GetValueOrDefault()));
                IWorkProgramRepository wpRepo = RepositoryFactory.Resolve<IWorkProgramRepository>();
                wpRepo.UoW = UoW;
                //Import Work Item to WPMDB
                int WKI_ID = wpRepo.InsertEtsiWorkITem(importData);

                //Import Schedule to WPMDB
                wpRepo.InsertWIScheduleEntry(WKI_ID, version.MajorVersion.GetValueOrDefault(), version.TechnicalVersion.GetValueOrDefault(), version.EditorialVersion.GetValueOrDefault());

                //Import Remark to WPMDB
                wpRepo.InsertWIRemeark(WKI_ID, SEQ_NO, REMARK_TEXT);

                //Import Keyword to WPMDB ==> TODO
                wpRepo.InsertWIKeyword(WKI_ID, "LTE");

                //Import project to WPMDB
                ImportProjectsToWPMDB(version, WKI_ID, wpRepo);
                
                //Add ETSI_WKI_ID field in version TABLE IF(WKI_ID != -1)                    
                version.ETSI_WKI_ID = WKI_ID;
                
                return true;
            }
            catch (Exception e)
            {
                Utils.LogManager.Error("WPM record creation error: " + e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// Import projects to the WPMDB. Three types of projects :
        /// <para>- global (3GPP)</para>
        /// <para>- associated to the release of the version</para>
        /// <para>- associated to technologies of the specification of the version</para>
        /// <para>- associated to the TSG</para>
        /// </summary>
        /// <param name="version"></param>
        /// <param name="WKI_ID"></param>
        /// <param name="wpRepo"></param>
        public void ImportProjectsToWPMDB(SpecVersion version, int WKI_ID, IWorkProgramRepository wpRepo)
        {
            ISpecificationTechnologiesManager specTechnosMgr = ManagerFactory.Resolve<ISpecificationTechnologiesManager>();
            specTechnosMgr.UoW = UoW;

            IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;
            int releaseID = version.Fk_ReleaseId.GetValueOrDefault();
            Release release = releaseRepo.Find(releaseID);

            ISpecificationRepository specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            int specID = version.Fk_SpecificationId.GetValueOrDefault();
            Specification spec = specRepo.Find(specID);

            ICommunityManager comMgr = ManagerFactory.Resolve<ICommunityManager>();
            comMgr.UoW = UoW;

            //Import related projects to WPMDB :
            //- global (value found in the web.config)
            var global3GPPProjectId = ConfigVariables.Global3GPPProjetId;
            if (global3GPPProjectId != 0)
                wpRepo.InsertWIProject(WKI_ID, global3GPPProjectId);
            //- release (value found in the release table, column WpmProjectId)
            var releaseWpmProjectId = release.WpmProjectId.GetValueOrDefault();
            if (releaseWpmProjectId != 0)
                wpRepo.InsertWIProject(WKI_ID, releaseWpmProjectId);
            //- technos (value found by the technologies associated to the version specification)
            var specId = version.Fk_SpecificationId ?? 0;
            var relatedTechnos = specTechnosMgr.GetASpecificationTechnologiesBySpecId(specId);
            if(relatedTechnos != null && relatedTechnos.Count != 0)
            {
                foreach (var techno in relatedTechnos)
                {
                    if(techno.WpmProjectId != null)
                        wpRepo.InsertWIProject(WKI_ID, techno.WpmProjectId ?? 0);
                }
            }
            //- tsg (value found by the TSG associated to the specification of the version).
            if(spec.PrimeResponsibleGroup != null){
                var community = comMgr.GetEnumCommunityShortNameByCommunityId(spec.PrimeResponsibleGroup.Fk_commityId);
                if(community != null){
                    if (community.WpmProjectId != null)
                    wpRepo.InsertWIProject(WKI_ID, community.WpmProjectId ?? 0);
                    else
                    {
                        //If we don't have any project ID for a TSG : we find its parent and we try to get this project ID
                        var parentCommunity = comMgr.GetParentCommunityByCommunityId(community.Fk_TbId ?? 0);
                        var parentCommunityShortName = comMgr.GetEnumCommunityShortNameByCommunityId(parentCommunity.TbId);
                        if (parentCommunityShortName != null && parentCommunityShortName.WpmProjectId != null)
                        {
                            wpRepo.InsertWIProject(WKI_ID, parentCommunityShortName.WpmProjectId ?? 0);
                        }
                    }
                }
            }
        }

    }    
}
