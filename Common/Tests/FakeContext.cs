﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests
{
    class FakeContext: IUltimateContext
    {
        #region IUltimateContext Members

        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Enum_ReleaseStatus> Enum_ReleaseStatus { get; set; }
        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Release> Releases{ get; set; }
        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Meeting> Meetings{ get; set; }
        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Remark> Remarks{ get; set; }
        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.History> Histories{ get; set; }
        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Users_Groups> Users_Groups{ get; set; }
        public System.Data.Entity.IDbSet<Users_AdHoc_Roles> Users_AdHoc_Roles{ get; set; }
        public System.Data.Entity.IDbSet<WorkItem> WorkItems{ get; set; }
        public System.Data.Entity.IDbSet<WorkItems_ResponsibleGroups> WorkItems_ResponsibleGroups{ get; set; }
        public System.Data.Entity.IDbSet<View_Persons> View_Persons{ get; set;}
        public System.Data.Entity.IDbSet<Community> Communities{ get; set; }
        public System.Data.Entity.IDbSet<View_ModulesPages> View_ModulesPages{ get; set;}
        public System.Data.Entity.IDbSet<ShortUrl> ShortUrls{get;set;}
        public System.Data.Entity.IDbSet<WorkPlanFile> WorkPlanFiles{get;set;}
        public System.Data.Entity.IDbSet<Enum_Serie> Enum_Serie{ get;set;}
        public System.Data.Entity.IDbSet<Enum_Technology> Enum_Technology{get; set;}
        public System.Data.Entity.IDbSet<Specification> Specifications{get;set;}
        public System.Data.Entity.IDbSet<Specification_Release> Specification_Release{get;set;}
        public System.Data.Entity.IDbSet<Specification_WorkItem> Specification_WorkItem{get;set;}
        public System.Data.Entity.IDbSet<SpecificationRapporteur> SpecificationRapporteurs{get;set;}
        public System.Data.Entity.IDbSet<SpecificationTechnology> SpecificationTechnologies{get;set;}
        public System.Data.Entity.IDbSet<SpecificationResponsibleGroup> SpecificationResponsibleGroups {get;set; }
        public System.Data.Entity.IDbSet<ResponsibleGroup_Secretary> ResponsibleGroupSecretaries{get;set;}
        public System.Data.Entity.IDbSet<SpecVersion> SpecVersions{get;set;}
        public System.Data.Entity.IDbSet<Enum_CRCategory> Enum_CRCategory{get;set;}
        public System.Data.Entity.IDbSet<Enum_ChangeRequestStatus> Enum_ChangeRequestStatus{ get; set;}
        public System.Data.Entity.IDbSet<Enum_CRImpact> Enum_CRImpact { get; set; }
        public System.Data.Entity.IDbSet<ChangeRequest> ChangeRequests { get; set; }
        public System.Data.Entity.IDbSet<CR_WorkItems> CR_WorkItems { get; set; }
        public System.Data.Entity.IDbSet<SyncInfo> SyncInfoes { get; set; }
        public System.Data.Entity.IDbSet<Enum_CommunitiesShortName> Enum_CommunitiesShortName { get; set; }
        public System.Data.Entity.IDbSet<ChangeRequestTsgData> ChangeRequestTsgDatas { get; set; }
        public System.Data.Entity.IDbSet<View_CrPacks> View_CrPacks { get; set; }
        public System.Data.Entity.IDbSet<LatestFolder> LatestFolders { get; set; }

        public void SetModified(object entity)
        {
            if (entity.GetType() == typeof(Release))
                Releases.Add((Release)entity);
            else if (entity.GetType() == typeof(WorkItem))
                WorkItems.Add((WorkItem)entity);
            else if (entity.GetType() == typeof(SpecVersion))
                SpecVersions.Add((SpecVersion)entity);
        }

        public void SetAdded(object entity)
        {
            if (entity.GetType() == typeof(Release))
                Releases.Add((Release)entity);
            else if (entity.GetType() == typeof(Meeting))
                Meetings.Add((Meeting)entity);
            else if (entity.GetType() == typeof(Remark))
                Remarks.Add((Remark)entity);
            else if (entity.GetType() == typeof(History))
                Histories.Add((History)entity);
            else if (entity.GetType() == typeof(Enum_ReleaseStatus))
                Enum_ReleaseStatus.Add((Enum_ReleaseStatus)entity);
            else if (entity.GetType() == typeof(WorkItem))
                WorkItems.Add((WorkItem)entity);
            else if (entity.GetType() == typeof(ShortUrl))
                ShortUrls.Add((ShortUrl)entity);
            else if (entity.GetType() == typeof(WorkPlanFile))
                WorkPlanFiles.Add((WorkPlanFile)entity);
            else if (entity.GetType() == typeof(Specification))
                Specifications.Add((Specification)entity);
            else if (entity.GetType() == typeof(SpecVersion))
                SpecVersions.Add((SpecVersion)entity);
            else
                throw new NotImplementedException("Something is missing in the FakeContext");
        }

        public void SetDeleted(object entity)
        {
            throw new NotImplementedException();
        }

        public void SetAutoDetectChanges(bool detect)
        {
            throw new NotImplementedException();
        }

        public void SetDetached(object entity)
        {
            throw new NotImplementedException();
        }

        public void SetValidateOnSave(bool detect)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUltimateContext Members


        public int Specifications_CleanAll()
        {
            throw new NotImplementedException();
        }

        public int WorkItems_CleanAll()
        {
            throw new NotImplementedException();
        }

        public int Versions_CleanAll()
        {
            throw new NotImplementedException();
        }

        public int CR_CleanAll()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUltimateContext Members


        public virtual int Transposition_CreateEtsiWorkItem(ObjectParameter nEW_WKI_ID, string eTSI_NUMBER, string sTANDARD_TYPE, Nullable<int> eTSI_DOC_NUMBER, string rEFERENCE, string sERIAL_NUMBER, string vERSION, Nullable<int> cOMMUNITY_ID, string tITLE_PART1, string tITLE_PART2, string tITLE_PART3, Nullable<int> rAPPORTEUR_ID, Nullable<int> sECRETARY_ID, string wORKING_TITLE)
        {
            throw new NotImplementedException();
        }

        public int Transposition_CreateWiKeywordEntry(int? wKI_ID, string kEYWORD_CODE)
        {
            throw new NotImplementedException();
        }

        public int Transposition_CreateWiProjectEntry(int? wKI_ID, int? pROJECT_ID)
        {
            throw new NotImplementedException();
        }

        public int Transposition_CreateWiRemarkEntry(int? wKI_ID, int? sEQ_NO, string rEMARK_TEXT)
        {
            throw new NotImplementedException();
        }

        public virtual int Transposition_CreateWiScheduleEntries(Nullable<int> wKI_ID, Nullable<int> mAJOR_VERSION, Nullable<int> tECHNICAL_VERSION, Nullable<int> eDITORIAL_VERSION, Nullable<int> mEETING_ID)
        {
            throw new NotImplementedException();
        }

        public virtual int Transposition_CreateWiMemoEntry(Nullable<int> wKI_ID, string wKI_SCOPE)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectStateEntry> GetEntities<T>(System.Data.Entity.EntityState entityState)
        {
            throw new NotImplementedException();
        }

        #endregion


        public System.Data.Entity.IDbSet<ETSI_WorkItem> ETSI_WorkItem
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public System.Data.Entity.IDbSet<View_ContributionsWithAditionnalData> View_ContributionsWithAditionnalData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public int Transposition_CreateEtsiWorkItem(ObjectParameter nEW_WKI_ID, string eTSI_NUMBER, string sTANDARD_TYPE, int? eTSI_DOC_NUMBER, int? eTSI_PART_NUMBER, string rEFERENCE, string sERIAL_NUMBER, string vERSION, int? cOMMUNITY_ID, string tITLE_PART1, string tITLE_PART2, string tITLE_PART3, int? rAPPORTEUR_ID, int? sECRETARY_ID, string wORKING_TITLE)
        {
            throw new NotImplementedException();
        }
    }
}
