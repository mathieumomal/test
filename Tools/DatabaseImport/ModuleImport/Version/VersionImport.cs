using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service = Etsi.Ultimate.Services;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;


namespace DatabaseImport.ModuleImport
{
    /// <summary>
    /// Generate Version table datas
    /// 
    /// NOTE ABOUT DELETED COLUMN :
    /// - DocumentChecked : datetime NULL
    /// - SupressTransposition : bit non NULL
    /// - Milestone (rempace by source) : varchar(45) NULL (source : int NULL)
    /// </summary>
    public class VersionImport : IModuleImport
    {

        public const string RefImportForLog = "[Version]";
        public List<Meeting> mtgs = null;
        public int MeetingsNotFound = 0;
        /// <summary>
        /// Old table(s) : 
        /// 2001-04-25_schedule
        /// </summary>
        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase()
        {
            NewContext.Versions_CleanAll();
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            try
            {
                NewContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var test = ex;
                Console.WriteLine(ex.InnerException);
                Console.ReadLine();
            }
        }

        #region migration methods
        private void CreateDatas()
        {
            //To improve performance we already find all the meetings
            mtgs = NewContext.Meetings.ToList();

            var total = LegacyContext.C2001_04_25_schedule.Count();
            var count = 0;
            foreach (var legacyVersion in LegacyContext.C2001_04_25_schedule)
            {
                //Id for a specific entry in the schedule table
                var idVersion = new StringBuilder()
                    .Append((legacyVersion.MAJOR_VERSION_NB != null) ? "-" + legacyVersion.MAJOR_VERSION_NB.ToString() + "-" : "")
                    .Append((legacyVersion.TECHNICAL_VERSION_NB != null) ? "-" + legacyVersion.MAJOR_VERSION_NB.ToString() + "-" : "")
                    .Append((legacyVersion.EDITORIAL_VERSION_NB != null) ? "-" + legacyVersion.MAJOR_VERSION_NB.ToString() + "-" : "")
                    .Append((legacyVersion.spec != null) ? "-" + legacyVersion.MAJOR_VERSION_NB.ToString() + "-" : "")
                    .ToString();

                var newVersion = new Domain.SpecVersion();

                newVersion.MajorVersion = Utils.CheckInt(legacyVersion.MAJOR_VERSION_NB, RefImportForLog + " MajorVersion", idVersion, Report);

                newVersion.TechnicalVersion = Utils.CheckInt(legacyVersion.TECHNICAL_VERSION_NB, RefImportForLog + " TechnicalVersion", idVersion, Report);

                newVersion.EditorialVersion = Utils.CheckInt(legacyVersion.EDITORIAL_VERSION_NB, RefImportForLog + " EditorialVersion", idVersion, Report);

                newVersion.AchievedDate = legacyVersion.ACHIEVED_DATE;

                newVersion.ExpertProvided = legacyVersion.expert_provided;

                newVersion.SupressFromSDO_Pub = Utils.NullBooleanCheck(legacyVersion.suppress_SDO_publication, RefImportForLog + " SupressFromSDO_Pub", false, Report);

                newVersion.ForcePublication = Utils.NullBooleanCheck(legacyVersion.force_publication, RefImportForLog + " ForcePublication", false, Report);

                newVersion.DocumentUploaded = legacyVersion.uploaded;

                newVersion.DocumentPassedToPub = legacyVersion.toETSI;

                newVersion.Multifile = Utils.NullBooleanCheck(legacyVersion.multifile, RefImportForLog + " ForcePublication", false, Report);

                newVersion.ETSI_WKI_ID = legacyVersion.WKI_ID;

                LocationCase(newVersion, legacyVersion, idVersion);

                SpecificationCase(newVersion, legacyVersion, idVersion);

                ReleaseCase(newVersion, legacyVersion, idVersion);

                MeetingCase(newVersion, legacyVersion, idVersion);

                // NO DATAS TO IMPORT
                //newVersion.ETSI_WKI_Ref = ;
                //newVersion.ProvidedBy = ; 

                NewContext.SpecVersions.Add(newVersion);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
            Report.LogWarning(RefImportForLog + " " + MeetingsNotFound + " meetings not found...");
        }

        private void LocationCase(Domain.SpecVersion newVersion, OldDomain.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var legacyLocation = Utils.CheckString(legacyVersion.location, 150, RefImportForLog + " Location", idVersion, Report);
            Match match = Regex.Match(legacyLocation, "(#).*(#)");
            if (!String.IsNullOrEmpty(legacyLocation) && match.Success)
            {
                var splitLocation = legacyLocation.Split('#');
                newVersion.Location = splitLocation[1];
            }
            else if(!String.IsNullOrEmpty(legacyLocation))
            {
                newVersion.Location = legacyLocation;
            }
        }

        /// <summary>
        /// Associate a version to a release
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="idVersion"></param>
        private void ReleaseCase(Domain.SpecVersion newVersion, OldDomain.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionRelease = Utils.CheckString(legacyVersion.release, 0, RefImportForLog + " Release", idVersion, Report);
            var release = NewContext.Releases.Where(x => x.Code.Equals(versionRelease)).FirstOrDefault();
            if (release != null)
            {
                newVersion.Fk_ReleaseId = release.Pk_ReleaseId;
            }
            else
            {
                Report.LogWarning(RefImportForLog + " release not found (" + legacyVersion.release + ")");
            }
        }

        /// <summary>
        /// Associate a version to a specification
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="idVersion"></param>
        private void SpecificationCase(Domain.SpecVersion newVersion, OldDomain.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionSpec = Utils.CheckString(legacyVersion.spec, 0, RefImportForLog + " Specification", idVersion, Report);
            var spec = NewContext.Specifications.Where(x => x.Number.Equals(versionSpec)).FirstOrDefault();
            if (spec != null)
            {
                newVersion.Fk_SpecificationId = spec.Pk_SpecificationId;
            }
            else
            {
                Report.LogWarning(RefImportForLog + " specification not found (" + legacyVersion.spec + ")");
            }
        }

        /// <summary>
        /// Associate a version to a meeting "source"
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="idVersion"></param>
        private void MeetingCase(Domain.SpecVersion newVersion, OldDomain.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionMeeting = Utils.CheckString(legacyVersion.meeting, 0, RefImportForLog + " Meeting", idVersion, Report);
            var mtg = mtgs.Where(m => m.MtgShortRef == versionMeeting).FirstOrDefault();
            if (mtg != null)
            {
                newVersion.Source = mtg.MTG_ID;
            }
            else
            {
                MeetingsNotFound++;
                //Report.LogError(RefImportForLog + " meeting not found (" + legacyVersion.spec + ")");
            }
        }

        #endregion
    }
}
