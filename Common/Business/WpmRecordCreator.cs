using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class WpmRecordCreator
    {
        private const string RemarkText = "Record created automatically from 3GPP Specifications Status database.";
        private const int SeqNo = 1;
        private const string SecurityGroupKeyword = "SECURITY";

        public IUltimateUnitOfWork UoW { get; set; }
        
        public WpmRecordCreator(IUltimateUnitOfWork uoW)
        {
           UoW = uoW;
        }

        /// <summary>
        /// Add all needed records at WPMDB
        /// </summary>
        /// <param name="version">The transposed version</param>
        /// <returns>ETSI work item identifier</returns>
        public bool AddWpmRecords(SpecVersion version) 
        {
            LogManager.Debug("Start AddWpmRecords...");
            if (version == null)
                return false; 
            try
            {
                IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                int releaseId = version.Fk_ReleaseId.GetValueOrDefault();
                Release release = releaseRepo.Find(releaseId);

                ISpecificationRepository specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
                specRepo.UoW = UoW;
                int specId = version.Fk_SpecificationId.GetValueOrDefault();
                Specification spec = specRepo.Find(specId);

                //Get prime responsible group of specification
                var communityRepo = RepositoryFactory.Resolve<ICommunityRepository>();
                communityRepo.UoW = UoW;
                var c = communityRepo.Find(spec.PrimeResponsibleGroup.Fk_commityId);
                var wgNumber = communityRepo.GetWgNumber(c.TbId, c.ParentTbId.GetValueOrDefault());

                //Get ResponsibleGroup_Secretary id (if there are only expired secretary : take the secretary with null expiration date)
                var rgSecretaryRepo = RepositoryFactory.Resolve<IResponsibleGroupSecretaryRepository>();
                rgSecretaryRepo.UoW = UoW;
                var secretaryFounds = rgSecretaryRepo.FindAllByCommiteeId(c.TbId).Where(x => x.roleExpirationDate >= DateTime.Now || x.roleExpirationDate == null).ToList();
                var secretaryId = 0;
                if (secretaryFounds.Any(x => x.roleExpirationDate != null))
                {
                    var secretary = secretaryFounds.First(x => x.roleExpirationDate >= DateTime.Now);
                    secretaryId = secretary.PersonId;
                }
                else
                {
                    //Will failed if no secretary found even with roleExpirationDate null (BUG on ETSI side if this is the case : 20/05/2016 Mathieu Mangion)
                    secretaryId = secretaryFounds.First().PersonId;
                }

                //Get version meeting's short reference (version.Source <=> meeting id)
                var mtgShortRef = string.Empty;
                if ((version.Source ?? 0) != 0)
                {
                    var mtgRepo = RepositoryFactory.Resolve<IMeetingRepository>();
                    mtgRepo.UoW = UoW;
                    var mtg = mtgRepo.Find(version.Source ?? 0);
                    if (mtg != null)
                    {
                        mtgShortRef = mtg.MtgShortRef;
                    }
                }

                ISpecVersionsRepository versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                versionRepo.UoW = UoW;
                var isAlreadyTransposed = (versionRepo.GetVersionsForSpecRelease(version.Fk_SpecificationId.GetValueOrDefault(), version.Fk_ReleaseId.GetValueOrDefault()).FirstOrDefault(v => v.ETSI_WKI_ID != null) != null);

                EtsiWorkItemImport importData = new EtsiWorkItemImport(version, spec, c, wgNumber, secretaryId, release.Name, isAlreadyTransposed);
                importData.SetSerialNumber(UtilsManager.EncodeVersionToBase36(version.MajorVersion, version.TechnicalVersion, version.EditorialVersion));
                IWorkProgramRepository wpRepo = RepositoryFactory.Resolve<IWorkProgramRepository>();
                wpRepo.UoW = UoW;

                LogManager.Debug("-> InsertEtsiWorkITem");
                //Import Work Item to WPMDB
                int wkiId = wpRepo.InsertEtsiWorkITem(importData);

                LogManager.Debug("-> InsertWIScheduleEntry");
                //Import Schedule to WPMDB
                wpRepo.InsertWIScheduleEntry(wkiId, version.MajorVersion.GetValueOrDefault(), version.TechnicalVersion.GetValueOrDefault(), version.EditorialVersion.GetValueOrDefault());

                LogManager.Debug("-> InsertWIRemeark");
                //Import Remark to WPMDB
                wpRepo.InsertWIRemeark(wkiId, SeqNo, RemarkText);

                LogManager.Debug("-> InsertWkiKeywords");
                //Import Keywords to WPMDB
                InsertWkiKeywords(wkiId, spec.Pk_SpecificationId, c.TbId);

                LogManager.Debug("-> InsertWIMemo");
                //Import memo to WPMDB
                wpRepo.InsertWIMemo(wkiId, mtgShortRef);

                LogManager.Debug("-> ImportProjectsToWpmdb");
                //Import project to WPMDB
                ImportProjectsToWpmdb(version, wkiId, wpRepo);

                
                
                //Add ETSI_WKI_ID field in version TABLE IF(WKI_ID != -1)                    
                version.ETSI_WKI_ID = wkiId;

                LogManager.Debug("End of AddWpmRecords.");
                return true;
            }
            catch (Exception e)
            {
                LogManager.Error("WPM record creation error",e);
                return false;
            }
        }

        /// <summary>
        /// Insert WKI keywords. Please find below business rules : 
        /// If spec has 2G technology => then add keyword "GSM"
        /// If spec has 3G technology => then add keyword "UMTS"
        /// If spec has LTE technology => then add keyword "LTE"
        /// If Primary responsible group of spec is SA3, then add keyword "SECURITY"
        /// </summary>
        /// <param name="wkiId">Wki ID</param>
        /// <param name="specId">Specification ID</param>
        /// <param name="primeResponsibleGroupCommunityTbId">Primary Responsible group community TbId</param>
        public void InsertWkiKeywords(int wkiId, int specId, int primeResponsibleGroupCommunityTbId)
        {
            var wpRepo = RepositoryFactory.Resolve<IWorkProgramRepository>();
            wpRepo.UoW = UoW;

            var specTechnosMgr = ManagerFactory.Resolve<ISpecificationTechnologiesManager>();
            specTechnosMgr.UoW = UoW;

            //1) Technologies rules
            var relatedTechnos = specTechnosMgr.GetASpecificationTechnologiesBySpecId(specId);
            if (relatedTechnos != null && relatedTechnos.Count != 0)
            {
                foreach (var techno in relatedTechnos)
                {
                    if (!string.IsNullOrEmpty(techno.WpmKeywordId))
                    {
                        wpRepo.InsertWIKeyword(wkiId, techno.WpmKeywordId);
                    }
                }
            }            

            //2) Security rule
            if (primeResponsibleGroupCommunityTbId == ConfigVariables.Global3GPPSecurityGroupTbId)
            {
                wpRepo.InsertWIKeyword(wkiId, SecurityGroupKeyword);
            }
        }

        /// <summary>
        /// Import projects to the WPMDB. Three types of projects :
        /// <para>- For all 3GPP transposed specs, add project id 703 ( id is configurable on web.config)</para>
        /// <para>- Add project ID corresponding to the release of the specification to transpose </para>
        /// <para>- Add project ID of the related technologies</para>
        /// <para>- If primary responsible group has WpmProjectId in table Enum_CommunitiesShortName, then this project ID should as well be added 
        /// (If we don't have any project ID for a WG : we find its parent and we try to get this project ID)</para>
        /// </summary>
        /// <param name="version"></param>
        /// <param name="wkiId"></param>
        /// <param name="wpRepo"></param>
        public void ImportProjectsToWpmdb(SpecVersion version, int wkiId, IWorkProgramRepository wpRepo)
        {
            ISpecificationTechnologiesManager specTechnosMgr = ManagerFactory.Resolve<ISpecificationTechnologiesManager>();
            specTechnosMgr.UoW = UoW;

            IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;
            int releaseId = version.Fk_ReleaseId.GetValueOrDefault();
            Release release = releaseRepo.Find(releaseId);

            ISpecificationRepository specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            int specId = version.Fk_SpecificationId.GetValueOrDefault();
            Specification spec = specRepo.Find(specId);

            ICommunityManager comMgr = ManagerFactory.Resolve<ICommunityManager>();
            comMgr.UoW = UoW;

            //Import related projects to WPMDB :
            //- global (value found in the web.config)
            var global3GppProjectId = ConfigVariables.Global3GPPProjetId;
            if (global3GppProjectId != 0)
                wpRepo.InsertWIProject(wkiId, global3GppProjectId);

            //- release (value found in the release table, column WpmProjectId)
            var releaseWpmProjectId = release.WpmProjectId.GetValueOrDefault();
            if (releaseWpmProjectId != 0)
                wpRepo.InsertWIProject(wkiId, releaseWpmProjectId);

            //- technos (value found by the technologies associated to the version specification)
            var versionSpecId = version.Fk_SpecificationId ?? 0;
            var relatedTechnos = specTechnosMgr.GetASpecificationTechnologiesBySpecId(versionSpecId);
            if(relatedTechnos != null && relatedTechnos.Count != 0)
            {
                foreach (var techno in relatedTechnos)
                {
                    if(techno.WpmProjectId != null)
                        wpRepo.InsertWIProject(wkiId, techno.WpmProjectId ?? 0);
                }
            }

            //- tsg (value found by the TSG associated to the specification of the version).
            if(spec.PrimeResponsibleGroup != null){
                var community = comMgr.GetEnumCommunityShortNameByCommunityId(spec.PrimeResponsibleGroup.Fk_commityId);
                if(community != null){
                    if (community.WpmProjectId != null)
                        wpRepo.InsertWIProject(wkiId, community.WpmProjectId ?? 0);
                    else
                    {
                        //If we don't have any project ID for a WG : we find its parent and we try to get this project ID
                        var parentCommunity = comMgr.GetParentCommunityByCommunityId(community.Fk_TbId ?? 0);
                        var parentCommunityShortName = comMgr.GetEnumCommunityShortNameByCommunityId(parentCommunity.TbId);
                        if (parentCommunityShortName != null && parentCommunityShortName.WpmProjectId != null)
                        {
                            wpRepo.InsertWIProject(wkiId, parentCommunityShortName.WpmProjectId ?? 0);
                        }
                    }
                }
            }
        }

    }    
}
