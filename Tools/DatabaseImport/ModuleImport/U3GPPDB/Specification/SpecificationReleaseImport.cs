using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
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

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            UltimateContext.SaveChanges();
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            tmpMeetings = UltimateContext.Meetings.ToList();
            var total = LegacyContext.Specs_GSM_3G_release_info.Count();
            var count = 0;
            foreach (var legacySpecInfo in LegacyContext.Specs_GSM_3G_release_info)
            {
                var new_specRelease = new Specification_Release();

                var spec = UltimateContext.Specifications.Where(x => x.Number.Equals(legacySpecInfo.Spec.Trim())).FirstOrDefault();
                var release = UltimateContext.Releases.Where(x => x.Code.Equals(legacySpecInfo.Release.Trim())).FirstOrDefault();

                if (spec != null && release != null)
                {
                    new_specRelease.Fk_ReleaseId = release.Pk_ReleaseId;
                    new_specRelease.Fk_SpecificationId = spec.Pk_SpecificationId;

                    new_specRelease.isWithdrawn = legacySpecInfo.withdrawn;
                    new_specRelease.CreationDate = legacySpecInfo.creation_date;
                    new_specRelease.UpdateDate = legacySpecInfo.update_date;

                    WithdrawnMeetingCase(new_specRelease, legacySpecInfo);

                    RemarksCase(new_specRelease, legacySpecInfo, spec.Number+ " "+ release.Description);

                    UltimateContext.Specification_Release.Add(new_specRelease);
                }
                else
                {
                    if (spec == null)
                    {
                        LogManager.LogWarning(RefImportForLog + " Specification not found (Spec : " + legacySpecInfo.Spec + ", for Release : " + legacySpecInfo.Release + ")");
                    }
                    if (release == null)
                    {
                        LogManager.LogWarning(RefImportForLog + " Release not found (Release : " + legacySpecInfo.Release + ", for Spec : " + legacySpecInfo.Spec + ")");
                    }
                }
                
                count++;
                if(count%100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}         ", count, total));
            }

        }

        private void WithdrawnMeetingCase(Specification_Release newSpecRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G_release_info legacySpecInfo)
        {
            var withdrawnMeetingId = Utils.CheckString(legacySpecInfo.stopped_at_meeting, 0, RefImportForLog + " WithdrawnMeeting", legacySpecInfo.Release);
            if (withdrawnMeetingId != "" && withdrawnMeetingId != "-")
            {
                var mtg = MtgHelper.FindMeetingId(withdrawnMeetingId);

                if (!mtg.HasValue)
                    LogManager.LogWarning(RefImportForLog + "Release " + legacySpecInfo.Release + ": could not find withdraw meeting " + withdrawnMeetingId);
                else
                {
                    newSpecRelease.WithdrawMeetingId = mtg.Value;
                }
            }
        }

        /// <summary>
        /// Insert remark associated 
        /// </summary>
        /// <param name="newSpecRelease"></param>
        /// <param name="legacySpecRelease"></param>
        /// <param name="logId"></param>
        private void RemarksCase(Specification_Release newSpecRelease, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G_release_info legacySpecRelease, string logId)
        {
            var remarksField = Utils.CheckString(legacySpecRelease.remarks, 1000, RefImportForLog + " Remarks ", logId);

            if (string.IsNullOrEmpty(remarksField))
                return;

            var remark = new Remark
            {
                CreationDate = DateTime.Now,
                IsPublic = true,
                RemarkText = remarksField,
                Specification_Release = newSpecRelease
            };
            newSpecRelease.Remarks.Add(remark);
        }

        #endregion
    }
}
