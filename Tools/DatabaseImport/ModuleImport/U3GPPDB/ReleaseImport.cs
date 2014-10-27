using System;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB
{
    public class ReleaseImport : IModuleImport
    {
        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext
        {
            get;
            set;
        }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext
        {
            get;
            set;
        }

        public Report Report
        {
            get;
            set;
        }

        /// <summary>
        /// Removes all the releases from the database.
        /// 
        /// Also removes all the remarks that are linked to releases.
        /// </summary>
        public void CleanDatabase()
        {
            foreach (var aRelease in UltimateContext.Releases.ToList())
            {
                // Remove associated remarks
                var remarks = UltimateContext.Remarks.Where(r => r.Fk_ReleaseId == aRelease.Pk_ReleaseId).ToList();
                for (int i = 0; i < remarks.Count; ++i)
                    UltimateContext.Remarks.Remove(remarks[i]);
                var histories = UltimateContext.Histories.Where(h => h.Fk_ReleaseId == aRelease.Pk_ReleaseId).ToList();
                for (int i = 0; i < histories.Count; ++i)
                    UltimateContext.Histories.Remove(histories[i]);                

                UltimateContext.Releases.Remove(aRelease);
            }
        }

        /// <summary>
        /// Puts all the releases into database.
        /// </summary>
        public void FillDatabase()
        {
            foreach (var legacyRelease in LegacyContext.Releases)
            {
                if (legacyRelease.Release_code.Contains("?"))
                    continue;

                // Create the release
                var newRelease = new Etsi.Ultimate.DomainClasses.Release();


                // Retrieve general information
                RetrieveGeneralInfo(newRelease, legacyRelease);

                // Try to fill the freeze, closure and end dates
                GetFreezeMeeting(newRelease, legacyRelease);
                GetCloureMeeting(newRelease, legacyRelease);
                GetEndMeeting(newRelease, legacyRelease);

                // Release status
                ComputeReleaseStatus(newRelease, legacyRelease);


                // Retrieve wpm technical info
                RetrieveTechnicalInfo(newRelease, legacyRelease);

                // Parse the remarks
                ParseRemarks(newRelease, legacyRelease);

                // Sort order
                newRelease.SortOrder = legacyRelease.sort_order;

                // previous release
                if (legacyRelease.previousRelease != null)
                {
                    var previousRelease = UltimateContext.Releases.Where(r => r.Code == legacyRelease.previousRelease).FirstOrDefault();
                    if (previousRelease == null)
                        Report.LogWarning("Coudln't find release " + legacyRelease.previousRelease + " as previous release of release " + newRelease.Code, "Releases");
                }

                // Last modification
                newRelease.LAST_MOD_BY = "System";
                newRelease.LAST_MOD_TS = DateTime.Now;

                UltimateContext.Releases.Add(newRelease);


            }
        }

        /// <summary>
        /// Retrieves the general information of the meeting (code, name, short name, start and closure dates...)
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void RetrieveGeneralInfo(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            // Release code => to code field
            newRelease.Code = legacyRelease.Release_code.Trim();

            // Release description ==> Name field
            newRelease.Name = legacyRelease.Release_description.Trim();

            // Release short description ==> Short name field
            newRelease.ShortName = legacyRelease.Release_short_description.Trim();

            // Release start date and closure date
            newRelease.StartDate = legacyRelease.rel_proj_start;
            //newRelease.ClosureDate = legacyRelease.rel_proj_end;

        }

        /// <summary>
        /// Retrieve technical information of the release (2g, 3g codes, etc...)
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void RetrieveTechnicalInfo(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            // Version 2g (hexadecimal) => Nowhere
            // Version 2g (decimal) => Version2g
            // Same for 3g
            try
            {
                newRelease.Version2g = Int32.Parse(legacyRelease.version_2g_dec);
            }
            catch (Exception e)
            {
                Report.LogError("While parsing 2gversion for release " + newRelease.Code + ": " + e.Message, "Releases");
            }

            if (legacyRelease.version_3g_dec != "-")
            {
                try
                {
                    newRelease.Version3g = Int32.Parse(legacyRelease.version_3g_dec);
                }
                catch (Exception e)
                {
                    Report.LogError("While parsing 3gversion for release " + newRelease.Code + ": " + e.Message, "Releases");
                }
            }

            // WPM codes
            if (legacyRelease.wpm_code_2g != null)
                newRelease.WpmCode2g = legacyRelease.wpm_code_2g.Trim();
            if (legacyRelease.wpm_code_3g != null)
                newRelease.WpmCode3g = legacyRelease.wpm_code_3g.Trim();

            // WPM project ID
            newRelease.WpmProjectId = legacyRelease.PROJECT_ID;

            // ITUR code
            if (legacyRelease.ITUR_code != null)
                newRelease.IturCode = legacyRelease.ITUR_code.Trim();
        }

        /// <summary>
        /// Parses the remarks linked to the meeting.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void ParseRemarks(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            var remarksField = legacyRelease.remarks;

            if (remarksField == null || remarksField.Length == 0)
                return;

            var remark = new Etsi.Ultimate.DomainClasses.Remark()
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                Release = newRelease

            };

            UltimateContext.Remarks.Add(remark);
        }

        /// <summary>
        /// Retrieves the freeze meeting, tries to parse it, and fill in the new release accordingly.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void GetFreezeMeeting(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            if (!string.IsNullOrEmpty(legacyRelease.Stage1_freeze) && legacyRelease.Stage1_freeze != "-")
            {                
                var mtg = UltimateContext.Meetings.Where(m => m.MtgShortRef == legacyRelease.Stage1_freeze).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find freeze meeting " + legacyRelease.Stage1_freeze);
                else
                {
                    newRelease.Stage1FreezeMtgId = mtg.MTG_ID;
                    newRelease.Stage1FreezeDate = mtg.END_DATE;
                }
                newRelease.Stage1FreezeMtgRef = legacyRelease.Stage1_freeze;
            }

            if (!string.IsNullOrEmpty(legacyRelease.Stage2_freeze) && legacyRelease.Stage2_freeze != "-")
            {                
                var mtg = UltimateContext.Meetings.Where(m => m.MtgShortRef == legacyRelease.Stage2_freeze).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find freeze meeting " + legacyRelease.Stage2_freeze);
                else
                {
                    newRelease.Stage2FreezeMtgId = mtg.MTG_ID;
                    newRelease.Stage2FreezeDate = mtg.END_DATE;
                }
                newRelease.Stage2FreezeMtgRef = legacyRelease.Stage2_freeze;
            }

            if (!string.IsNullOrEmpty(legacyRelease.Stage3_freeze) && legacyRelease.Stage3_freeze != "-")
            {                
                var mtg = UltimateContext.Meetings.Where(m => m.MtgShortRef == legacyRelease.Stage3_freeze).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find freeze meeting " + legacyRelease.Stage3_freeze);
                else
                {
                    newRelease.Stage3FreezeMtgId = mtg.MTG_ID;
                    newRelease.Stage3FreezeDate = mtg.END_DATE;
                }
                newRelease.Stage3FreezeMtgRef = legacyRelease.Stage3_freeze;
            }
        }

        /// <summary>
        /// Retrieves the closure meeting, tries to parse it, and fill in the new release accordingly.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void GetCloureMeeting(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            if (!string.IsNullOrEmpty(legacyRelease.Closed) && legacyRelease.Closed != "-")
            {                
                var mtg = UltimateContext.Meetings.Where(m => m.MtgShortRef == legacyRelease.Closed).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find Closure meeting " + legacyRelease.Closed);
                else
                {
                    newRelease.ClosureMtgId = mtg.MTG_ID;
                    newRelease.ClosureDate = mtg.END_DATE;
                }
                newRelease.ClosureMtgRef = legacyRelease.Closed;
            }
        }

        /// <summary>
        /// Retrieves the end meeting, tries to parse it, and fill in the new release accordingly.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void GetEndMeeting(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            if (!string.IsNullOrEmpty(legacyRelease.Protocols_freeze) && legacyRelease.Protocols_freeze != "-")
            {                
                var mtg = UltimateContext.Meetings.Where(m => m.MtgShortRef == legacyRelease.Protocols_freeze).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find End meeting " + legacyRelease.Protocols_freeze);
                else
                {
                    newRelease.EndMtgId = mtg.MTG_ID;
                    newRelease.EndDate = mtg.END_DATE;
                }
                newRelease.EndMtgRef = legacyRelease.Protocols_freeze;
            }
        }

        /// <summary>
        /// Computes the release status according to the "defunct" and frozen dates.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void ComputeReleaseStatus(Etsi.Ultimate.DomainClasses.Release newRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Release legacyRelease)
        {
            var releaseStatuses = UltimateContext.Enum_ReleaseStatus.ToList();

            try
            {
                // If release is defunct, then it's closed.
                if (legacyRelease.defunct.GetValueOrDefault())
                {
                    var enumId = releaseStatuses.Where(s => s.Code.Contains("Closed")).FirstOrDefault();
                    newRelease.Fk_ReleaseStatus = enumId.Enum_ReleaseStatusId;
                }
                else if ((newRelease.Stage3FreezeDate != null && newRelease.Stage3FreezeDate < DateTime.Now)
                    || (newRelease.Stage3FreezeDate == null && newRelease.Stage3FreezeMtgRef != null))
                {
                    newRelease.Fk_ReleaseStatus = releaseStatuses.Where(s => s.Code.Contains("Frozen")).FirstOrDefault().Enum_ReleaseStatusId;
                }
                else
                    newRelease.Fk_ReleaseStatus = releaseStatuses.Where(s => s.Code.Contains("Open")).FirstOrDefault().Enum_ReleaseStatusId;
            }
            catch (Exception e)
            {
                Report.LogError("While retrieving status for release " + newRelease.Code + ": " + e.Message, "Releases");
            }
        }
    }
}
