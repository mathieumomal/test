﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etsi.UserRights.DSDBDataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DSDBContext : DbContext
    {
        public DSDBContext()
            : base("name=DSDBContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<PERSON> People { get; set; }
        public DbSet<PERSON_IN_LIST> PERSON_IN_LIST { get; set; }
        public DbSet<PERSON_LIST> PERSON_LIST { get; set; }
    }
}