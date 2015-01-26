using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Version
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
        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            UltimateContext.Versions_CleanAll();
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            try
            {
                UltimateContext.SaveChanges();
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
            mtgs = UltimateContext.Meetings.ToList();

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

                var newVersion = new Etsi.Ultimate.DomainClasses.SpecVersion();

                newVersion.MajorVersion = Utils.CheckInt(legacyVersion.MAJOR_VERSION_NB, RefImportForLog + " MajorVersion", idVersion);

                newVersion.TechnicalVersion = Utils.CheckInt(legacyVersion.TECHNICAL_VERSION_NB, RefImportForLog + " TechnicalVersion", idVersion);

                newVersion.EditorialVersion = Utils.CheckInt(legacyVersion.EDITORIAL_VERSION_NB, RefImportForLog + " EditorialVersion", idVersion);

                newVersion.AchievedDate = legacyVersion.ACHIEVED_DATE;

                newVersion.ExpertProvided = legacyVersion.expert_provided;

                newVersion.SupressFromSDO_Pub = Utils.NullBooleanCheck(legacyVersion.suppress_SDO_publication, RefImportForLog + " SupressFromSDO_Pub", false);

                newVersion.ForcePublication = Utils.NullBooleanCheck(legacyVersion.force_publication, RefImportForLog + " ForcePublication", false);

                newVersion.DocumentUploaded = legacyVersion.uploaded;

                newVersion.DocumentPassedToPub = legacyVersion.toETSI;

                newVersion.Multifile = Utils.NullBooleanCheck(legacyVersion.multifile, RefImportForLog + " ForcePublication", false);

                newVersion.ETSI_WKI_ID = legacyVersion.WKI_ID;

                LocationCase(newVersion, legacyVersion, idVersion);

                SpecificationCase(newVersion, legacyVersion, idVersion);

                ReleaseCase(newVersion, legacyVersion, idVersion);

                MeetingCase(newVersion, legacyVersion, idVersion);

                RemarksCase(newVersion, legacyVersion, idVersion);

                // NO DATAS TO IMPORT
                //newVersion.ETSI_WKI_Ref = ;
                //newVersion.ProvidedBy = ; 

                UltimateContext.SpecVersions.Add(newVersion);
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
            LogManager.LogWarning(RefImportForLog + " " + MeetingsNotFound + " meetings not found...");
        }

        private void LocationCase(Etsi.Ultimate.DomainClasses.SpecVersion newVersion, Etsi.Ultimate.Tools.TmpDbDataAccess.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var legacyLocation = Utils.CheckString(legacyVersion.location, 150, RefImportForLog + " Location", idVersion);
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
        private void ReleaseCase(Etsi.Ultimate.DomainClasses.SpecVersion newVersion, Etsi.Ultimate.Tools.TmpDbDataAccess.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionRelease = Utils.CheckString(legacyVersion.release, 0, RefImportForLog + " Release", idVersion);
            var release = UltimateContext.Releases.Where(x => x.Code.Equals(versionRelease)).FirstOrDefault();
            if (release != null)
            {
                newVersion.Fk_ReleaseId = release.Pk_ReleaseId;
            }
            else
            {
                LogManager.LogWarning(RefImportForLog + " release not found (" + legacyVersion.release + ")");
            }
        }

        /// <summary>
        /// Associate a version to a specification
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="idVersion"></param>
        private void SpecificationCase(Etsi.Ultimate.DomainClasses.SpecVersion newVersion, Etsi.Ultimate.Tools.TmpDbDataAccess.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionSpec = Utils.CheckString(legacyVersion.spec, 0, RefImportForLog + " Specification", idVersion);
            var spec = UltimateContext.Specifications.Where(x => x.Number.Equals(versionSpec)).FirstOrDefault();
            if (spec != null)
            {
                newVersion.Fk_SpecificationId = spec.Pk_SpecificationId;
            }
            else
            {
                LogManager.LogWarning(RefImportForLog + " specification not found (" + legacyVersion.spec + ")");
            }
        }

        /// <summary>
        /// Associate a version to a meeting "source"
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="idVersion"></param>
        private void MeetingCase(Etsi.Ultimate.DomainClasses.SpecVersion newVersion, Etsi.Ultimate.Tools.TmpDbDataAccess.C2001_04_25_schedule legacyVersion, String idVersion)
        {
            var versionMeeting = Utils.CheckString(legacyVersion.meeting, 0, RefImportForLog + " Meeting", idVersion);
            var mtg = MtgHelper.FindMeetingId(versionMeeting);
            if (mtg.HasValue)
            {
                newVersion.Source = mtg.Value;
            }
            else
            {
                MeetingsNotFound++;
                //Report.LogError(RefImportForLog + " meeting not found (" + legacyVersion.spec + ")");
            }
        }

        /// <summary>
        /// Insert remark associated 
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="legacyVersion"></param>
        /// <param name="logId"></param>
        private void RemarksCase(SpecVersion newVersion, Etsi.Ultimate.Tools.TmpDbDataAccess.C2001_04_25_schedule legacyVersion, string logId)
        {
            var remarksField = Utils.CheckString(legacyVersion.comment, 1000, RefImportForLog + " Remarks ", logId);

            if (string.IsNullOrEmpty(remarksField))
                return;

            var remark = new Remark
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                Version = newVersion
            };
            newVersion.Remarks.Add(remark);
        }

        #endregion
    }
}
