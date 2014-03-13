﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.Ultimate.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    using Etsi.Ultimate.DomainClasses;
    
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
      
    }
}
