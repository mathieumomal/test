using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
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
        IDbSet<SpecVersion> SpecVersions { get; set; }
        IDbSet<Enum_CRCategory> Enum_CRCategory { get; set; }
        IDbSet<Enum_TDocStatus> Enum_TDocStatus { get; set; }
        IDbSet<Enum_CRImpact> Enum_CRImpact { get; set; }
        IDbSet<ChangeRequest> ChangeRequests { get; set; }
        IDbSet<CR_Version> CR_Versions { get; set; }
        IDbSet<TDoc> TDocs { get; set; }

        void SetModified(object entity);
        void SetAdded(object entity);
        void SetDeleted(object entity);
        void SetAutoDetectChanges(bool detect);
        int SaveChanges();

        // ---- STORED PROCEDURES calls -----------
        int Specifications_CleanAll();
        int WorkItems_CleanAll();
        int Versions_CleanAll();
        int CR_CleanAll();
    }
}
