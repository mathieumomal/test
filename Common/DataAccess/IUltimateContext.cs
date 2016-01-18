using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.DataAccess
{
    public interface IUltimateContext : IDisposable
    {

        IDbSet<Enum_ReleaseStatus> Enum_ReleaseStatus { get; }
        IDbSet<Release> Releases { get; }
        IDbSet<Meeting> Meetings { get; }
        IDbSet<Remark> Remarks { get; }
        IDbSet<History> Histories { get; }
        IDbSet<Users_Groups> Users_Groups { get; }
        IDbSet<Users_AdHoc_Roles> Users_AdHoc_Roles { get; }
        IDbSet<WorkItem> WorkItems { get; }
        IDbSet<WorkItems_ResponsibleGroups> WorkItems_ResponsibleGroups { get; }
        IDbSet<View_Persons> View_Persons { get; }
        IDbSet<Community> Communities { get; }
        IDbSet<View_ModulesPages> View_ModulesPages { get; }
        IDbSet<ShortUrl> ShortUrls { get; }
        IDbSet<WorkPlanFile> WorkPlanFiles { get; }
        IDbSet<Enum_Serie> Enum_Serie { get; set; }
        IDbSet<Enum_Technology> Enum_Technology { get; set; }
        IDbSet<Specification> Specifications { get; set; }
        IDbSet<Specification_Release> Specification_Release { get; set; }
        IDbSet<Specification_WorkItem> Specification_WorkItem { get; set; }
        IDbSet<SpecificationRapporteur> SpecificationRapporteurs { get; set; }
        IDbSet<SpecificationTechnology> SpecificationTechnologies { get; set; }
        IDbSet<SpecificationResponsibleGroup> SpecificationResponsibleGroups { get; set; }
        IDbSet<ResponsibleGroup_Secretary> ResponsibleGroupSecretaries { get; set; }
        IDbSet<ETSI_WorkItem> ETSI_WorkItem { get; set; }
        IDbSet<SpecVersion> SpecVersions { get; set; }
        IDbSet<Enum_CRCategory> Enum_CRCategory { get; set; }
        IDbSet<Enum_CRImpact> Enum_CRImpact { get; set; }
        IDbSet<ChangeRequest> ChangeRequests { get; set; }
        IDbSet<CR_WorkItems> CR_WorkItems { get; set; }
        IDbSet<SyncInfo> SyncInfoes { get; set; }
        IDbSet<Enum_CommunitiesShortName> Enum_CommunitiesShortName { get; set; }
        IDbSet<Enum_ChangeRequestStatus> Enum_ChangeRequestStatus{ get; set; }
        IDbSet<ChangeRequestTsgData> ChangeRequestTsgDatas { get; set; }
        IDbSet<View_CrPacks> View_CrPacks { get; }
        IDbSet<View_ContributionsWithAditionnalData> View_ContributionsWithAditionnalData { get; set; }
        IDbSet<LatestFolder> LatestFolders { get; set; }

        void SetModified(object entity);
        void SetAdded(object entity);
        void SetDeleted(object entity);
        void SetAutoDetectChanges(bool detect);
        void SetValidateOnSave(bool detect);
        void SetDetached(object entity);
        int SaveChanges();
        IEnumerable<System.Data.Entity.Core.Objects.ObjectStateEntry> GetEntities<T>(System.Data.Entity.EntityState entityState);

        // ---- STORED PROCEDURES calls -----------
        int Specifications_CleanAll();
        int WorkItems_CleanAll();
        int Versions_CleanAll();
        int CR_CleanAll();

        int Transposition_CreateEtsiWorkItem(ObjectParameter nEW_WKI_ID, string eTSI_NUMBER, string sTANDARD_TYPE, Nullable<int> eTSI_DOC_NUMBER, string rEFERENCE, string sERIAL_NUMBER, string vERSION, Nullable<int> cOMMUNITY_ID, string tITLE_PART1, string tITLE_PART2, string tITLE_PART3, Nullable<int> rAPPORTEUR_ID, Nullable<int> sECRETARY_ID, string wORKING_TITLE);
        int Transposition_CreateWiKeywordEntry(Nullable<int> wKI_ID, string kEYWORD_CODE);
        int Transposition_CreateWiProjectEntry(Nullable<int> wKI_ID, Nullable<int> pROJECT_ID);
        int Transposition_CreateWiRemarkEntry(Nullable<int> wKI_ID, Nullable<int> sEQ_NO, string rEMARK_TEXT);
        int Transposition_CreateWiScheduleEntries(Nullable<int> wKI_ID, Nullable<int> mAJOR_VERSION, Nullable<int> tECHNICAL_VERSION, Nullable<int> eDITORIAL_VERSION);
    }
}
