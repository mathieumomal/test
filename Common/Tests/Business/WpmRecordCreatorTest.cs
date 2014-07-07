using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
    public class WpmRecordCreatorTest
    {
        [Test]
        public void AddWPMRecord_Nominal()
        {            
            var communityMock = MockRepository.GenerateMock<ICommunityRepository>();
            //Set WG number to 4
            communityMock.Stub(x => x.Find(1)).Return(new Community() { TbId = 1, ParentTbId = 2, ShortName = "SP1", TbType ="WG"});
            communityMock.Stub(x => x.GetWgNumber(1, 2)).Return(4);
            RepositoryFactory.Container.RegisterInstance<ICommunityRepository>(communityMock);

            var secretaryMock = MockRepository.GenerateMock<IResponsibleGroupSecretaryRepository>();
            //Set SecretaryId to to 2
            secretaryMock.Stub(x => x.FindAllByCommiteeId(1)).Return(new List<ResponsibleGroup_Secretary>(){ new ResponsibleGroup_Secretary(){TbId=1, PersonId=2}}.AsQueryable());
            RepositoryFactory.Container.RegisterInstance<IResponsibleGroupSecretaryRepository>(secretaryMock);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)GetSpecVersions());
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)GetSpecs());
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)GetReleases());

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var WorkProgramRepoMock = MockRepository.GenerateMock<IWorkProgramRepository>();
            //Set WKI_ID to 1
            WorkProgramRepoMock.Expect(wp => wp.InsertEtsiWorkITem(Arg<EtsiWorkItemImport>.Is.Anything)).Return(1);
            WorkProgramRepoMock.Expect(wp => wp.InsertWIScheduleEntry(Arg<int>.Is.Equal(1), Arg<int>.Is.Equal(10), Arg<int>.Is.Equal(2), Arg<int>.Is.Equal(1)));
            WorkProgramRepoMock.Expect(wp => wp.InsertWIKeyword(Arg<int>.Is.Equal(1), Arg<string>.Is.Anything));
            WorkProgramRepoMock.Expect(wp => wp.InsertWIProject(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything));
            WorkProgramRepoMock.Expect(wp => wp.InsertWIRemeark(Arg<int>.Is.Equal(1), Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            
            RepositoryFactory.Container.RegisterInstance<IWorkProgramRepository>(WorkProgramRepoMock);


            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new WpmRecordCreator(uow);
            //manager._uoW = uow;

            var versionManager = ManagerFactory.Resolve<ISpecVersionManager>();
            versionManager._uoW = uow;

            manager.AddWpmRecords(versionManager.GetVersionsForASpecRelease(1, 1).FirstOrDefault());

            WorkProgramRepoMock.AssertWasCalled(wp => wp.InsertEtsiWorkITem(Arg<EtsiWorkItemImport>.Matches(c => c.EtsiNumber.Equals("126 124") && c.EtsiDocNumber == 26124 && c.SerialNumber.Equals("26124vA21") && c.Reference.Equals("DTS/TSGS-0426124vA21"))));
            WorkProgramRepoMock.VerifyAllExpectations();

           //TODO
        }


        // <summary>
        /// Get Fake SpecVersion Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecVersionFakeDBSet GetSpecVersions()
        {
            var versionDbSet = new SpecVersionFakeDBSet();

            var version = new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "http://www.3gpp.org/ftp/Specs/archive/06_series/06.22/0622-500.zip",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 1, Fk_VersionId = 1, RemarkText = "R1" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1

            };
            var version2 = new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = null,
                MajorVersion = 20,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 10, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };            
            versionDbSet.Add(version);
            versionDbSet.Add(version2);

            return versionDbSet;
        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { 
                Pk_SpecificationId = 1, Number = "26.124", 
                IsUnderChangeControl = true , 
                IsTS = true,
                SpecificationResponsibleGroups = new List<SpecificationResponsibleGroup>(){
                    new SpecificationResponsibleGroup(){
                        Pk_SpecificationResponsibleGroupId = 1,
                        IsPrime = true,
                        Fk_commityId = 1,
                        Fk_SpecificationId = 1
                    }
                }
            });
            return list;
        }

        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1, Name = "Release 1" });            
            return list;
        }
    }
}
