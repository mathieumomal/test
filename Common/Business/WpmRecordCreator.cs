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
            IWorkProgramRepository wpRepo = RepositoryFactory.Resolve<IWorkProgramRepository>();
            wpRepo.UoW = _uoW;

            EtsiWorkItemImport importData = new EtsiWorkItemImport(version);

            //Import Work Item to WPMDB
            int WKI_ID = wpRepo.InsertEtsiWorkITem(importData);

            //Import Schedule to WPMDB
            wpRepo.InsertWIScheduleEntry(WKI_ID,0);

            //Import Keyword to WPMDB
            wpRepo.InsertWIKeyword(WKI_ID,"");

            
            //Get WPM project code 
            IReleaseRepository releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = _uoW;
            int releaseID = version.Fk_ReleaseId.GetValueOrDefault();
            //Import project to WPMDB
            wpRepo.InsertWIProject(WKI_ID, releaseRepo.Find(releaseID).WpmProjectId.GetValueOrDefault());

            //Import Remark to WPMDB
            wpRepo.InsertWIRemeark(WKI_ID,0,"");
            

            return 0;
        }

    }    
}
