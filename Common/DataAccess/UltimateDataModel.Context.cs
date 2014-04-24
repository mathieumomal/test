﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    using Etsi.Ultimate.DomainClasses;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class UltimateContext : DbContext, IUltimateContext
    {
        public UltimateContext()
            : base("name=UltimateContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public IDbSet<Enum_ReleaseStatus> Enum_ReleaseStatus { get; set; }
        public IDbSet<Release> Releases { get; set; }
        public IDbSet<Remark> Remarks { get; set; }
        public IDbSet<Users_Groups> Users_Groups { get; set; }
        public IDbSet<Users_AdHoc_Roles> Users_AdHoc_Roles { get; set; }
        public IDbSet<History> Histories { get; set; }
        public IDbSet<View_Persons> View_Persons { get; set; }
        public IDbSet<Community> Communities { get; set; }
        public IDbSet<WorkItem> WorkItems { get; set; }
        public IDbSet<WorkItems_ResponsibleGroups> WorkItems_ResponsibleGroups { get; set; }
        public IDbSet<Meeting> Meetings { get; set; }
        public IDbSet<ShortUrl> ShortUrls { get; set; }
        public IDbSet<View_ModulesPages> View_ModulesPages { get; set; }
        public IDbSet<WorkPlanFile> WorkPlanFiles { get; set; }
        public IDbSet<Enum_Serie> Enum_Serie { get; set; }
        public IDbSet<Enum_Technology> Enum_Technology { get; set; }
        public IDbSet<Specification> Specifications { get; set; }
        public IDbSet<Specification_Release> Specification_Release { get; set; }
        public IDbSet<Specification_WorkItem> Specification_WorkItem { get; set; }
        public IDbSet<SpecificationRapporteur> SpecificationRapporteurs { get; set; }
        public IDbSet<Enum_CommunitiesShortName> Enum_CommunitiesShortName { get; set; }
        public IDbSet<Enum_SpecificationStage> Enum_SpecificationStage { get; set; }
        public IDbSet<SpecificationResponsibleGroup> SpecificationResponsibleGroups { get; set; }
        public IDbSet<SpecificationTechnology> SpecificationTechnologies { get; set; }
    	
    	/**
    	 * This code is intended to enable testability of the different layers,
    	 * in particular the repositories.
    	 */
    	public void SetModified(object entity)
    	{
    		Entry(entity).State = EntityState.Modified;
    	}
    
    	public void SetAdded(object entity)
    	{
    		Entry(entity).State = EntityState.Added;
    	}
    
    	public void SetDeleted(object entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }
    
    	public void SetAutoDetectChanges(bool detect)
        {
            Configuration.AutoDetectChangesEnabled = detect;
        }
      
    
        public virtual int CleanAllTablesForImport()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CleanAllTablesForImport");
        }
    }
}
