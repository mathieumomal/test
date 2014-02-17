using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.ModuleImport
{
    public class ReleaseImport : IModuleImport
    {
        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext
        {
            get; set;
        }

        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext
        {
            get; set;
        }

        public ImportReport Report
        {
            get; set;
        }

        /// <summary>
        /// Removes all the releases from the database.
        /// 
        /// Also removes all the remarks that are linked to releases.
        /// </summary>
        public void CleanDatabase()
        {
            foreach (var aRelease in NewContext.Releases.ToList())
            {
                // Remove associated remarks
                var remarks = NewContext.Remarks.Where(r => r.Fk_ReleaseId != null).ToList();
                for (int i = 0; i < remarks.Count; ++i)
                    NewContext.Remarks.Remove(remarks[i]);

                NewContext.Releases.Remove(aRelease);
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
                var newRelease = new Domain.Release();


                // Retrieve general information
                RetrieveGeneralInfo(newRelease, legacyRelease);

                // Try to fill the freeze dates
                GetFreezeMeeting(newRelease, legacyRelease);
                
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
                    var previousRelease = NewContext.Releases.Where(r => r.Code == legacyRelease.previousRelease).FirstOrDefault();
                    if (previousRelease == null)
                        Report.LogWarning("Coudln't find release " + legacyRelease.previousRelease + " as previous release of release " + newRelease.Code, "Releases");
                }

                // Last modification
                newRelease.LAST_MOD_BY = "System";
                newRelease.LAST_MOD_TS = DateTime.Now;

                NewContext.Releases.Add(newRelease);


            }
        }

        /// <summary>
        /// Retrieves the general information of the meeting (code, name, short name, start and closure dates...)
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void RetrieveGeneralInfo(Domain.Release newRelease, OldDomain.Release legacyRelease)
        {
            // Release code => to code field
            newRelease.Code = legacyRelease.Release_code.Trim();

            // Release description ==> Name field
            newRelease.Name = legacyRelease.Release_description.Trim();

            // Release short description ==> Short name field
            newRelease.ShortName = legacyRelease.Release_short_description.Trim();

            // Release start date and closure date
            newRelease.StartDate = legacyRelease.rel_proj_start;
            newRelease.ClosureDate = legacyRelease.rel_proj_end;

        }

        /// <summary>
        /// Retrieve technical information of the release (2g, 3g codes, etc...)
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void RetrieveTechnicalInfo(Domain.Release newRelease, OldDomain.Release legacyRelease)
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
        private void ParseRemarks(Domain.Release newRelease, OldDomain.Release legacyRelease)
        {
            var remarksField = legacyRelease.remarks;

            if (remarksField == null || remarksField.Length == 0)
                return;

            var remark = new Domain.Remark()
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                Release = newRelease
                
            };

            NewContext.Remarks.Add(remark);
        }

        /// <summary>
        /// Retrieves the freeze meeting, tries to parse it, and fill in the new release accordingly.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void GetFreezeMeeting(Domain.Release newRelease, OldDomain.Release legacyRelease)
        {
            if (!string.IsNullOrEmpty(legacyRelease.freeze_meeting) && legacyRelease.freeze_meeting != "-")
            {
                var fullRef =  Domain.Meeting.ToFullReference(legacyRelease.freeze_meeting);
                var mtg = NewContext.Meetings.Where(m => m.MTG_REF == fullRef).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning("Release " + newRelease.Code + ": could not find freeze meeting " + legacyRelease.freeze_meeting);
                else
                {
                    newRelease.Stage3FreezeMtgId = mtg.MTG_ID;
                    newRelease.Stage3FreezeDate = mtg.END_DATE;
                }
                newRelease.Stage3FreezeMtgRef = legacyRelease.freeze_meeting;
            }
        }

        /// <summary>
        /// Computes the release status according to the "defunct" and frozen dates.
        /// </summary>
        /// <param name="newRelease"></param>
        /// <param name="legacyRelease"></param>
        private void ComputeReleaseStatus(Domain.Release newRelease, OldDomain.Release legacyRelease)
        {
            var releaseStatuses = NewContext.Enum_ReleaseStatus.ToList();

            try
            {
                // If release is defunct, then it's closed.
                if (legacyRelease.defunct.GetValueOrDefault())
                {
                    var enumId = releaseStatuses.Where(s => s.ReleaseStatus.Contains("Closed")).FirstOrDefault();
                    newRelease.Fk_ReleaseStatus = enumId.Enum_ReleaseStatusId;
                }
                else if (newRelease.Stage3FreezeDate != null && newRelease.Stage3FreezeDate < DateTime.Now)
                {
                    newRelease.Fk_ReleaseStatus = releaseStatuses.Where(s => s.ReleaseStatus.Contains("Frozen")).FirstOrDefault().Enum_ReleaseStatusId;
                }
                else
                    newRelease.Fk_ReleaseStatus = releaseStatuses.Where(s => s.ReleaseStatus.Contains("Open")).FirstOrDefault().Enum_ReleaseStatusId;
            }
            catch (Exception e)
            {
                Report.LogError("While retrieving status for release " + newRelease.Code + ": " + e.Message, "Releases");
            }
        }

        

    }
}
