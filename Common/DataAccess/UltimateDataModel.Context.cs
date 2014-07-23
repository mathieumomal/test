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
    using System.Data.Entity.Core.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Data.Entity.Core.EntityClient;
    
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
        public IDbSet<SpecificationResponsibleGroup> SpecificationResponsibleGroups { get; set; }
        public IDbSet<SpecificationTechnology> SpecificationTechnologies { get; set; }
        public IDbSet<ETSI_WorkItem> ETSI_WorkItem { get; set; }
        public IDbSet<SpecVersion> SpecVersions { get; set; }
        public IDbSet<ResponsibleGroup_Secretary> ResponsibleGroupSecretaries { get; set; }
        public IDbSet<Enum_CRCategory> Enum_CRCategory { get; set; }
        public IDbSet<Enum_TDocStatus> Enum_TDocStatus { get; set; }
        public IDbSet<Enum_CRImpact> Enum_CRImpact { get; set; }
        public IDbSet<ChangeRequest> ChangeRequests { get; set; }
        public IDbSet<CR_WorkItems> CR_WorkItems { get; set; }
        public IDbSet<Community> Communities { get; set; }
        public IDbSet<SyncInfo> SyncInfoes { get; set; }
    	
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
    
        public void SetDetached(object entity)
        {
            Entry(entity).State = EntityState.Detached;
        }
    
    	public void SetAutoDetectChanges(bool detect)
        {
            Configuration.AutoDetectChangesEnabled = detect;
        }
    
        public void SetValidateOnSave(bool detect)
        {
            Configuration.ValidateOnSaveEnabled = detect;
        }
      
    
        public virtual int Specifications_CleanAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Specifications_CleanAll");
        }
    
        public virtual int WorkItems_CleanAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("WorkItems_CleanAll");
        }
    
        public virtual int Versions_CleanAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Versions_CleanAll");
        }
    
        public virtual int CR_CleanAll()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CR_CleanAll");
        }
    
        public virtual int Transposition_CreateEtsiWorkItem(ObjectParameter nEW_WKI_ID, string eTSI_NUMBER, string sTANDARD_TYPE, Nullable<int> eTSI_DOC_NUMBER, string rEFERENCE, string sERIAL_NUMBER, string vERSION, Nullable<int> cOMMUNITY_ID, string tITLE_PART1, string tITLE_PART2, string tITLE_PART3, Nullable<int> rAPPORTEUR_ID, Nullable<int> sECRETARY_ID, string wORKING_TITLE)
        {
            var eTSI_NUMBERParameter = eTSI_NUMBER != null ?
                new ObjectParameter("ETSI_NUMBER", eTSI_NUMBER) :
                new ObjectParameter("ETSI_NUMBER", typeof(string));
    
            var sTANDARD_TYPEParameter = sTANDARD_TYPE != null ?
                new ObjectParameter("STANDARD_TYPE", sTANDARD_TYPE) :
                new ObjectParameter("STANDARD_TYPE", typeof(string));
    
            var eTSI_DOC_NUMBERParameter = eTSI_DOC_NUMBER.HasValue ?
                new ObjectParameter("ETSI_DOC_NUMBER", eTSI_DOC_NUMBER) :
                new ObjectParameter("ETSI_DOC_NUMBER", typeof(int));
    
            var rEFERENCEParameter = rEFERENCE != null ?
                new ObjectParameter("REFERENCE", rEFERENCE) :
                new ObjectParameter("REFERENCE", typeof(string));
    
            var sERIAL_NUMBERParameter = sERIAL_NUMBER != null ?
                new ObjectParameter("SERIAL_NUMBER", sERIAL_NUMBER) :
                new ObjectParameter("SERIAL_NUMBER", typeof(string));
    
            var vERSIONParameter = vERSION != null ?
                new ObjectParameter("VERSION", vERSION) :
                new ObjectParameter("VERSION", typeof(string));
    
            var cOMMUNITY_IDParameter = cOMMUNITY_ID.HasValue ?
                new ObjectParameter("COMMUNITY_ID", cOMMUNITY_ID) :
                new ObjectParameter("COMMUNITY_ID", typeof(int));
    
            var tITLE_PART1Parameter = tITLE_PART1 != null ?
                new ObjectParameter("TITLE_PART1", tITLE_PART1) :
                new ObjectParameter("TITLE_PART1", typeof(string));
    
            var tITLE_PART2Parameter = tITLE_PART2 != null ?
                new ObjectParameter("TITLE_PART2", tITLE_PART2) :
                new ObjectParameter("TITLE_PART2", typeof(string));
    
            var tITLE_PART3Parameter = tITLE_PART3 != null ?
                new ObjectParameter("TITLE_PART3", tITLE_PART3) :
                new ObjectParameter("TITLE_PART3", typeof(string));
    
            var rAPPORTEUR_IDParameter = rAPPORTEUR_ID.HasValue ?
                new ObjectParameter("RAPPORTEUR_ID", rAPPORTEUR_ID) :
                new ObjectParameter("RAPPORTEUR_ID", typeof(int));
    
            var sECRETARY_IDParameter = sECRETARY_ID.HasValue ?
                new ObjectParameter("SECRETARY_ID", sECRETARY_ID) :
                new ObjectParameter("SECRETARY_ID", typeof(int));
    
            var wORKING_TITLEParameter = wORKING_TITLE != null ?
                new ObjectParameter("WORKING_TITLE", wORKING_TITLE) :
                new ObjectParameter("WORKING_TITLE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Transposition_CreateEtsiWorkItem", nEW_WKI_ID, eTSI_NUMBERParameter, sTANDARD_TYPEParameter, eTSI_DOC_NUMBERParameter, rEFERENCEParameter, sERIAL_NUMBERParameter, vERSIONParameter, cOMMUNITY_IDParameter, tITLE_PART1Parameter, tITLE_PART2Parameter, tITLE_PART3Parameter, rAPPORTEUR_IDParameter, sECRETARY_IDParameter, wORKING_TITLEParameter);
        }
    
        public virtual int Transposition_CreateWiKeywordEntry(Nullable<int> wKI_ID, string kEYWORD_CODE)
        {
            var wKI_IDParameter = wKI_ID.HasValue ?
                new ObjectParameter("WKI_ID", wKI_ID) :
                new ObjectParameter("WKI_ID", typeof(int));
    
            var kEYWORD_CODEParameter = kEYWORD_CODE != null ?
                new ObjectParameter("KEYWORD_CODE", kEYWORD_CODE) :
                new ObjectParameter("KEYWORD_CODE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Transposition_CreateWiKeywordEntry", wKI_IDParameter, kEYWORD_CODEParameter);
        }
    
        public virtual int Transposition_CreateWiProjectEntry(Nullable<int> wKI_ID, Nullable<int> pROJECT_ID)
        {
            var wKI_IDParameter = wKI_ID.HasValue ?
                new ObjectParameter("WKI_ID", wKI_ID) :
                new ObjectParameter("WKI_ID", typeof(int));
    
            var pROJECT_IDParameter = pROJECT_ID.HasValue ?
                new ObjectParameter("PROJECT_ID", pROJECT_ID) :
                new ObjectParameter("PROJECT_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Transposition_CreateWiProjectEntry", wKI_IDParameter, pROJECT_IDParameter);
        }
    
        public virtual int Transposition_CreateWiRemarkEntry(Nullable<int> wKI_ID, Nullable<int> sEQ_NO, string rEMARK_TEXT)
        {
            var wKI_IDParameter = wKI_ID.HasValue ?
                new ObjectParameter("WKI_ID", wKI_ID) :
                new ObjectParameter("WKI_ID", typeof(int));
    
            var sEQ_NOParameter = sEQ_NO.HasValue ?
                new ObjectParameter("SEQ_NO", sEQ_NO) :
                new ObjectParameter("SEQ_NO", typeof(int));
    
            var rEMARK_TEXTParameter = rEMARK_TEXT != null ?
                new ObjectParameter("REMARK_TEXT", rEMARK_TEXT) :
                new ObjectParameter("REMARK_TEXT", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Transposition_CreateWiRemarkEntry", wKI_IDParameter, sEQ_NOParameter, rEMARK_TEXTParameter);
        }
    
        public virtual int Transposition_CreateWiScheduleEntries(Nullable<int> wKI_ID, Nullable<int> mAJOR_VERSION, Nullable<int> tECHNICAL_VERSION, Nullable<int> eDITORIAL_VERSION)
        {
            var wKI_IDParameter = wKI_ID.HasValue ?
                new ObjectParameter("WKI_ID", wKI_ID) :
                new ObjectParameter("WKI_ID", typeof(int));
    
            var mAJOR_VERSIONParameter = mAJOR_VERSION.HasValue ?
                new ObjectParameter("MAJOR_VERSION", mAJOR_VERSION) :
                new ObjectParameter("MAJOR_VERSION", typeof(int));
    
            var tECHNICAL_VERSIONParameter = tECHNICAL_VERSION.HasValue ?
                new ObjectParameter("TECHNICAL_VERSION", tECHNICAL_VERSION) :
                new ObjectParameter("TECHNICAL_VERSION", typeof(int));
    
            var eDITORIAL_VERSIONParameter = eDITORIAL_VERSION.HasValue ?
                new ObjectParameter("EDITORIAL_VERSION", eDITORIAL_VERSION) :
                new ObjectParameter("EDITORIAL_VERSION", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Transposition_CreateWiScheduleEntries", wKI_IDParameter, mAJOR_VERSIONParameter, tECHNICAL_VERSIONParameter, eDITORIAL_VERSIONParameter);
        }
    }
}
