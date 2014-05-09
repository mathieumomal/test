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

namespace DatabaseImport.ModuleImport
{
    public class SpecificationReleaseImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/Release]";
        public List<Meeting> tmpMeetings = new List<Meeting>();
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G_release-info
        /// </summary>
        /// 
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            NewContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            tmpMeetings = NewContext.Meetings.ToList();
            var total = LegacyContext.Specs_GSM_3G_release_info.Count();
            var count = 0;
            foreach (var legacySpecInfo in LegacyContext.Specs_GSM_3G_release_info)
            {
                var new_specRelease = new Specification_Release();

                var spec = NewContext.Specifications.Where(x => x.Number.Equals(legacySpecInfo.Spec.Trim())).FirstOrDefault();
                var release = NewContext.Releases.Where(x => x.Code.Equals(legacySpecInfo.Release.Trim())).FirstOrDefault();

                if (spec != null && release != null)
                {
                    new_specRelease.Fk_ReleaseId = release.Pk_ReleaseId;
                    new_specRelease.Fk_SpecificationId = spec.Pk_SpecificationId;

                    new_specRelease.isWithdrawn = legacySpecInfo.withdrawn;
                    new_specRelease.CreationDate = legacySpecInfo.creation_date;
                    new_specRelease.UpdateDate = legacySpecInfo.update_date;

                    WithdrawnMeetingCase(new_specRelease, legacySpecInfo);

                    NewContext.Specification_Release.Add(new_specRelease);
                }
                else
                {
                    if (spec == null)
                    {
                        Report.LogWarning(RefImportForLog + " Specification not found (Spec : " + legacySpecInfo.Spec + ", for Release : " + legacySpecInfo.Release + ")");
                    }
                    if (release == null)
                    {
                        Report.LogWarning(RefImportForLog + " Release not found (Release : " + legacySpecInfo.Release + ", for Spec : " + legacySpecInfo.Spec + ")");
                    }
                }
                
                count++;
                if(count%100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}         ", count, total));
            }

        }

        private void WithdrawnMeetingCase(Domain.Specification_Release newSpecRelease, OldDomain.Specs_GSM_3G_release_info legacySpecInfo)
        {
            var withdrawnMeetingId = Utils.CheckString(legacySpecInfo.stopped_at_meeting, 0, RefImportForLog + " WithdrawnMeeting", legacySpecInfo.Release, Report);
            if (withdrawnMeetingId != "" && withdrawnMeetingId != "-")
            {
                var mtg = tmpMeetings.Where(m => m.MtgShortRef.Equals(withdrawnMeetingId)).FirstOrDefault();

                if (mtg == null)
                    Report.LogWarning(RefImportForLog + "Release " + legacySpecInfo.Release + ": could not find withdraw meeting " + withdrawnMeetingId);
                else
                {
                    newSpecRelease.WithdrawMeetingId = mtg.MTG_ID;
                }
            }
        }
        #endregion
    }
}
