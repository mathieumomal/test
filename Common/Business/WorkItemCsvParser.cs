using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Represents the CSV record, as provided by the CSV parser.
    /// </summary>
    internal class WorkItemImportClass
    {
        public int ID { get; set; }
        public int Unique_ID { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string Outline_Level { get; set; }
        public string Release { get; set; }
        public string Resource_Names { get; set; }
        public string Start_Date { get; set; }
        public string Finish_Date { get; set; }
        public string Percent_Complete { get; set; }
        public string Hyperlink { get; set; }
        public string Status_Report { get; set; }
        public string WI_rapporteur_name { get; set; }
        public string WI_rapporteur_e_mail { get; set; }
        public string Notes { get; set; }
        public string Impacted_TSs_and_TRs { get; set; }
        public string TSG_approved { get; set; }
        public string PCG_approved { get; set; }
        public string TSG_stopped { get; set; }
        public string PCG_stopped { get; set; }
        public string Created { get; set; }
        public string last_modif { get; set; }
    }

    /// <summary>
    /// This class is in charge of parsing the CSV file and returns the list of all work items that are modified.
    /// The system is using the CSVHelper parsing library.
    /// </summary>
    public class WorkItemCsvParser : IWorkItemCsvParser
    {
        // Any level 0 workitem will be affected a Pk over 100 000 000
        private const int LowerBoundForLevel0Wi = 100000000;

        public IUltimateUnitOfWork UoW { get; set; }

        // Data retrieved to be able to establish references to the different objects.
        private List<WorkItem> AllWorkItems { get; set; }
        private List<Release> AllReleases { get; set; }
        private List<View_Persons> AllPersons { get; set; }
        private List<Meeting> AllMeetings { get; set; }
        private List<Community> AllCommunities { get; set; }

        // Report that is returned after parsing.
        private Report Report { get; set; }

        // List of treated data, and references used during the parsing.
        private Dictionary<int, WorkItem> _lastWiForLevel;
        private WorkItem _lastTreatedWi;
        private List<WorkItem> TreatedWorkItems { get; set; }
        private List<WorkItem> ModifiedWorkItems { get; set; }
        private bool IsCurrentWiModified { get; set; }

        /// <summary>
        /// Parses the CSV file.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public KeyValuePair<List<WorkItem>, Report> ParseCsv(string fileLocation)
        {
            try
            {
                if (UoW == null)
                    throw new InvalidOperationException("Cannot process with UoW defined");
                // Initialize all fields
                InitializeCommonData();

                // Check the file extension
                if (!fileLocation.EndsWith("csv"))
                {
                    Report.LogError(Utils.Localization.WorkItem_Import_Invalid_File_Format);
                    return new KeyValuePair<List<WorkItem>, Report>(ModifiedWorkItems, Report);
                }

                // Open the file.
                using (StreamReader reader = new StreamReader(fileLocation, Encoding.Default))
                {

                    var csv = new CsvReader(reader);

                    // Configure the reader
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.DetectColumnCountChanges = true;
                    //csv.Configuration.Encoding = Encoding.UTF8;
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.TrimFields = false;

                    // For each line:
                    // - Create a new record (that might be discarded if WI already exist)
                    // - Treat the fields one by one.
                    // - Depending on whether or not the WI has been updated, store it in the list of WIs to be updated.
                    while (csv.Read())
                    {
                        IsCurrentWiModified = false;
                        var record = csv.GetRecord<WorkItemImportClass>();

                        var wi = TreatWiUid(record);
                        TreatWpId(record, wi);
                        TreatLevel(record, wi);
                        TreatName(record, wi);
                        TreatResource(record, wi);
                        TreatAcronym(record, wi);
                        TreatRelease(record, wi);
                        TreatStartDate(record, wi);
                        TreatEndDate(record, wi);
                        TreatCompletion(record, wi);
                        TreatWid(record, wi);
                        TreatStatusReport(record, wi);
                        TreatRapporteurCompany(record, wi);
                        TreatRapporteur(record, wi);
                        TreatRemarks(record, wi);
                        TreatTssAndTrs(record, wi);
                        TreatApprovedTsgMeeting(record, wi);
                        TreatApprovedPcgMeeting(record, wi);
                        TreatStoppedTsgMeeting(record, wi);
                        TreatStoppedPcgMeeting(record, wi);
                        TreatCreationDate(record, wi);
                        TreatLastModifiedDate(record, wi);

                        if (IsCurrentWiModified)
                            ModifiedWorkItems.Add(wi);
                        TreatedWorkItems.Add(wi);
                    }

                }

                // Return the list of workitems that were modified, along with the report.
                return new KeyValuePair<List<WorkItem>, Report>(ModifiedWorkItems, Report);
            }

            catch (FileNotFoundException)
            {
                // Log the error
                LogManager.Error("Error occured in WorkplanCsvParser: cannot find file" + fileLocation);
                var errorReport = new Report();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_FileNotFound, fileLocation));

                return new KeyValuePair<List<WorkItem>, Report>(new List<WorkItem>(), errorReport);
            }
            catch (CsvBadDataException e)
            {
                var errorReport = new Report();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_Bad_Format, e.Message));

                return new KeyValuePair<List<WorkItem>, Report>(new List<WorkItem>(), errorReport);
            }

            catch (Exception e)
            {
                // Log the error
                LogManager.Error("Error occured in WorkplanCsvParser:" + e.Message);
                LogManager.Error("Stacktrace:" + e.StackTrace);

                string lastSuccessfullyTreatedWi = "None";
                if (_lastTreatedWi != null)
                    lastSuccessfullyTreatedWi = _lastTreatedWi.Pk_WorkItemUid.ToString();
                var errorReport = new Report();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_Unknown_Exception, lastSuccessfullyTreatedWi));

                return new KeyValuePair<List<WorkItem>, Report>(new List<WorkItem>(), errorReport);

            }
        }


        /// <summary>
        /// Sets up all the repositories
        /// </summary>
        private void InitializeCommonData()
        {
            // Retrieve all the existing work items.
            var wiRepo = RepositoryFactory.Resolve<IWorkItemRepository>();
            wiRepo.UoW = UoW;
            AllWorkItems = wiRepo.AllIncluding(w => w.WorkItems_ResponsibleGroups, w => w.Remarks).ToList();

            // Retrieve all the persons.
            var personRepo = RepositoryFactory.Resolve<IPersonRepository>();
            personRepo.UoW = UoW;
            AllPersons = personRepo.All.ToList();

            // Retrieve all the meetings.
            var meetingRepo = RepositoryFactory.Resolve<IMeetingRepository>();
            meetingRepo.UoW = UoW;
            AllMeetings = meetingRepo.All.ToList();

            // Retrieve all the communities
            var communityRepo = RepositoryFactory.Resolve<ICommunityRepository>();
            communityRepo.UoW = UoW;
            AllCommunities = communityRepo.All.ToList();

            // Setup all the other empty structures
            _lastWiForLevel = new Dictionary<int, WorkItem>();
            Report = new Report();
            ModifiedWorkItems = new List<WorkItem>();
            TreatedWorkItems = new List<WorkItem>();
        }

        #region Field Parsing

        /// <summary>
        /// Manages the resource. System parses the field against "," field, and looks for each group.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatResource(WorkItemImportClass record, WorkItem wi)
        {
            string resourceStr = record.Resource_Names;
            if (resourceStr == "-")
                resourceStr = "";

            // Treat the empty case.
            if (string.IsNullOrEmpty(resourceStr))
            {
                if (wi.WorkItems_ResponsibleGroups.Count > 0)
                {
                    IsCurrentWiModified = true;
                    wi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.IsDeleted = true);
                }
                return;
            }


            // Check if it has changed
            var respStr = string.Join(",", wi.WorkItems_ResponsibleGroups.Select(c => c.ResponsibleGroup).ToList());
            if (respStr != resourceStr)
            {
                IsCurrentWiModified = true;
                wi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.IsDeleted = true);

                var resources = record.Resource_Names.Split(',');
                var isFirst = true;
                foreach (var res in resources)
                {
                    // First check resource corresponds to a community
                    var community = AllCommunities.FirstOrDefault(c => c.ShortName == res);

                    if (community == null)
                    {
                        Report.LogWarning(
                            String.Format(Utils.Localization.WorkItem_Import_Invalid_Resource, wi.WorkplanId, wi.Pk_WorkItemUid, res));
                    }
                    else
                    {
                        wi.WorkItems_ResponsibleGroups.Add(
                            new WorkItems_ResponsibleGroups()
                            {
                                Fk_TbId = community.TbId,
                                ResponsibleGroup = community.ShortName,
                                IsPrimeResponsible = isFirst
                            });
                        isFirst = false;
                    }

                }
            }
        }


        /// <summary>
        /// Manages the Last modified date. Might throw an error if not well formed.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatLastModifiedDate(WorkItemImportClass record, WorkItem wi)
        {
            DateTime? lastModifiedDate = null;
            if (!string.IsNullOrEmpty(record.last_modif) && record.last_modif != "-")
            {
                DateTime tmpDate;
                if (DateTime.TryParseExact(record.last_modif, "d/M/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out tmpDate))
                {
                    lastModifiedDate = tmpDate;
                }
                else
                {
                    Report.LogWarning(
                        String.Format(Utils.Localization.WorkItem_Import_Incorrect_LastModifiedDate,
                        wi.WorkplanId, wi.Pk_WorkItemUid, record.last_modif));
                }

            }

            if (wi.LastModification != lastModifiedDate)
            {
                IsCurrentWiModified = true;
                wi.LastModification = lastModifiedDate;
            }
        }

        /// <summary>
        /// Manages the creation date. Might throw an error if not well formed.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatCreationDate(WorkItemImportClass record, WorkItem wi)
        {
            DateTime? creationDate = null;
            if (!string.IsNullOrEmpty(record.Created) && record.Created != "-")
            {
                DateTime tmpDate;
                if (DateTime.TryParseExact(record.Created, "d/M/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out tmpDate))
                {
                    creationDate = tmpDate;
                }
                else
                {
                    Report.LogWarning(
                        String.Format(Utils.Localization.WorkItem_Import_Incorrect_CreationDate,
                        wi.WorkplanId, wi.Pk_WorkItemUid, record.Created));
                }

            }

            if (wi.CreationDate != creationDate)
            {
                IsCurrentWiModified = true;
                wi.CreationDate = creationDate;
            }
        }

        /// <summary>
        /// Handles the Stopped PCG meeting.
        /// Foreach meeting, system searches in the MeetingRepository if the meeting exists.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatStoppedPcgMeeting(WorkItemImportClass record, WorkItem wi)
        {
            string stoppedPcgMtgRef = record.PCG_stopped;
            if (stoppedPcgMtgRef == "-")
                stoppedPcgMtgRef = "";

            if (wi.PcgStoppedMtgRef != stoppedPcgMtgRef)
            {
                IsCurrentWiModified = true;

                if (stoppedPcgMtgRef.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_PcgStoppedMtgRef_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    stoppedPcgMtgRef = stoppedPcgMtgRef.Substring(0, 20);
                }

                wi.PcgStoppedMtgRef = stoppedPcgMtgRef;

                // Search for corresponding meeting, but only if ref is not empty
                if (string.IsNullOrEmpty(stoppedPcgMtgRef))
                    return;

                int mtgId = GetMeetingForRef(stoppedPcgMtgRef);
                if (mtgId != -1)
                {
                    wi.PcgStoppedMtgId = mtgId;
                }
                else
                {
                    Report.LogWarning(String.Format(
                        Utils.Localization.WorkItem_Import_Invalid_PcgStoppedMtg, wi.WorkplanId, wi.Pk_WorkItemUid, stoppedPcgMtgRef));
                }
            }
        }


        /// <summary>
        /// Handles the Stopped TSG meeting.
        /// Foreach meeting, system searches in the MeetingRepository if the meeting exists.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatStoppedTsgMeeting(WorkItemImportClass record, WorkItem wi)
        {
            string stoppedTsgMtgRef = record.TSG_stopped;
            if (stoppedTsgMtgRef == "-")
                stoppedTsgMtgRef = "";

            if (wi.TsgStoppedMtgRef != stoppedTsgMtgRef)
            {
                IsCurrentWiModified = true;
                if (stoppedTsgMtgRef.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_TsgStoppedMtgRef_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    stoppedTsgMtgRef = stoppedTsgMtgRef.Substring(0, 20);
                }
                wi.TsgStoppedMtgRef = stoppedTsgMtgRef;

                // Search for corresponding meeting, but only if ref is not empty
                if (string.IsNullOrEmpty(stoppedTsgMtgRef))
                    return;

                int mtgId = GetMeetingForRef(stoppedTsgMtgRef);
                if (mtgId != -1)
                {
                    wi.TsgStoppedMtgId = mtgId;
                }
                else
                {
                    Report.LogWarning(String.Format(
                        Utils.Localization.WorkItem_Import_Invalid_TsgStoppedMtg, wi.WorkplanId, wi.Pk_WorkItemUid, stoppedTsgMtgRef));
                }
            }
        }


        /// <summary>
        /// Handles the Approved PCG meeting.
        /// Foreach meeting, system searches in the MeetingRepository if the meeting exists.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatApprovedPcgMeeting(WorkItemImportClass record, WorkItem wi)
        {
            string approvedPcgMtgRef = record.PCG_approved;
            if (approvedPcgMtgRef == "-")
                approvedPcgMtgRef = "";

            if (wi.PcgApprovalMtgRef != approvedPcgMtgRef)
            {
                IsCurrentWiModified = true;
                if (approvedPcgMtgRef.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_PcgApprovalMtgRef_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    approvedPcgMtgRef = approvedPcgMtgRef.Substring(0, 20);
                }
                wi.PcgApprovalMtgRef = approvedPcgMtgRef;

                // Search for corresponding meeting, but only if ref is not empty
                if (string.IsNullOrEmpty(approvedPcgMtgRef))
                    return;

                int mtgId = GetMeetingForRef(approvedPcgMtgRef);
                if (mtgId != -1)
                {
                    wi.PcgApprovalMtgId = mtgId;
                }
                else
                {
                    Report.LogWarning(String.Format(
                        Utils.Localization.WorkItem_Import_Invalid_PcgApprovedMtg, wi.WorkplanId, wi.Pk_WorkItemUid, approvedPcgMtgRef));
                }

            }
        }

        /// <summary>
        /// Handles the Approved TSG meeting. 
        /// Foreach meeting, system searches in the MeetingRepository if the meeting exists.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatApprovedTsgMeeting(WorkItemImportClass record, WorkItem wi)
        {
            var approvedTsgMtgRef = record.TSG_approved;
            if (approvedTsgMtgRef == "-")
                approvedTsgMtgRef = "";

            if (wi.TsgApprovalMtgRef != approvedTsgMtgRef)
            {
                IsCurrentWiModified = true;

                if (approvedTsgMtgRef.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_TsgApprovalMtgRef_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    approvedTsgMtgRef = approvedTsgMtgRef.Substring(0, 20);
                }

                wi.TsgApprovalMtgRef = approvedTsgMtgRef;

                // Search for corresponding meeting, but only if ref is not empty
                if (string.IsNullOrEmpty(approvedTsgMtgRef))
                    return;

                int mtgId = GetMeetingForRef(approvedTsgMtgRef);
                if (mtgId != -1)
                {
                    wi.TsgApprovalMtgId = mtgId;
                }
                else
                {
                    Report.LogWarning(String.Format(
                        Utils.Localization.WorkItem_Import_Invalid_TsgApprovedMtg, wi.WorkplanId, wi.Pk_WorkItemUid, approvedTsgMtgRef));
                }
            }
        }

        /// <summary>
        /// Seeks for the meeting ID corresponding to the meetingRef.
        /// 
        /// Returns -1 if meeting cannot be found or there is a formatting error.
        /// </summary>
        /// <param name="mtgRef"></param>
        /// <returns></returns>
        private int GetMeetingForRef(string mtgRef)
        {
            // Try to fetch the meeting ID
            try
            {
                var mtg = AllMeetings.FirstOrDefault(m => m.MtgShortRef == mtgRef);
                if (mtg == null)
                {
                    return -1;
                }
                else
                {
                    return mtg.MTG_ID;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Handle the TSsAndTRs field. just fetching the data.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatTssAndTrs(WorkItemImportClass record, WorkItem wi)
        {
            string tsAndTr = record.Impacted_TSs_and_TRs;

            if (tsAndTr == "-")
                tsAndTr = "";

            if (tsAndTr.Length > 50)
            {
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_TsTr_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                tsAndTr = tsAndTr.Substring(0, 50);
            }

            if (wi.TssAndTrs != tsAndTr)
            {
                IsCurrentWiModified = true;
                wi.TssAndTrs = tsAndTr;
            }
        }


        /// <summary>
        /// Handles the remark. Principle is the following: 
        /// - if there is no remark current and remark field is empty, add a remark.
        /// - if there are remarks already, clear those remarks from the remark string, and check if the Notes field still contains something readable.
        ///     -- if no, do nothing
        ///     -- else, create a new remark.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatRemarks(WorkItemImportClass record, WorkItem wi)
        {
            string remarkStr = record.Notes;
            if (remarkStr == "-")
                remarkStr = "";

            if (!string.IsNullOrEmpty(remarkStr))
            {
                // Are there any remark in the wi
                if (wi.Remarks.Count == 0)
                {
                    IsCurrentWiModified = true;
                    wi.Remarks.Add(new Remark() { CreationDate = DateTime.Now, RemarkText = remarkStr, IsPublic = true });
                }
                else
                {
                    foreach (var remark in wi.Remarks)
                    {
                        var pos = remarkStr.IndexOf(remark.RemarkText);
                        if (pos >= 0)
                            remarkStr = remarkStr.Remove(pos, remark.RemarkText.Length);
                    }

                    // Clean a little bit the remaining string.
                    const string cleanRemarkPattern = "[A-Za-z0-9].*$";
                    var match = Regex.Match(remarkStr, cleanRemarkPattern);

                    if (match.Success && match.Length > 1)
                    {
                        IsCurrentWiModified = true;
                        wi.Remarks.Add(new Remark() { CreationDate = DateTime.Now, RemarkText = match.Value, IsPublic = true });
                    }

                }
            }

        }

        /// <summary>
        /// Handles the rapporteur. Seek in database for the list of emails.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatRapporteur(WorkItemImportClass record, WorkItem wi)
        {
            string rapporteurStr = record.WI_rapporteur_e_mail;
            if (rapporteurStr == "-")
                rapporteurStr = "";

            int? rapporteurId = null;

            // Seek for the rapporteur by email.
            if (!string.IsNullOrEmpty(rapporteurStr))
            {
                // try to refine to get the email
                string email = rapporteurStr;
                email = Regex.Match(email, @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)").Value;

                var person = AllPersons.FirstOrDefault(p => p.Email == email);
                if (!string.IsNullOrEmpty(email) && person != null)
                {
                    rapporteurId = person.PERSON_ID;
                }
                else
                {
                    Report.LogWarning(
                        String.Format(Utils.Localization.WorkItem_Import_Invalid_Rapporteur, wi.WorkplanId, wi.Pk_WorkItemUid, rapporteurStr));
                }
            }


            // Update the rapporteur str
            if (wi.RapporteurStr != rapporteurStr)
            {
                IsCurrentWiModified = true;

                if (rapporteurStr.Length > 100)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_RapporteurStr_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    rapporteurStr = rapporteurStr.Substring(0, 100);
                }

                wi.RapporteurStr = rapporteurStr;
            }

            // Update the rapporteur ID
            if (wi.RapporteurId != rapporteurId)
            {
                IsCurrentWiModified = true;
                wi.RapporteurId = rapporteurId;
            }

        }

        /// <summary>
        /// Manages the rapporteur's company, as a simple text field.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatRapporteurCompany(WorkItemImportClass record, WorkItem wi)
        {
            string rapporteurCompanyName = record.WI_rapporteur_name;
            if (rapporteurCompanyName == "-")
                rapporteurCompanyName = "";

            if (wi.RapporteurCompany != rapporteurCompanyName)
            {
                IsCurrentWiModified = true;
                if (rapporteurCompanyName.Length > 100)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_RapporteurCompany_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    rapporteurCompanyName = rapporteurCompanyName.Substring(0, 100);
                }
                wi.RapporteurCompany = rapporteurCompanyName;
            }
        }

        private void TreatStatusReport(WorkItemImportClass record, WorkItem wi)
        {
            string statusReport = record.Status_Report;
            if (statusReport == "-")
                statusReport = "";

            // Check that reference looks good
            if (!string.IsNullOrEmpty(statusReport))
            {
                if (!Regex.IsMatch(statusReport, "^[A-Za-z0-9]{2}-[0-9]{5,6}$", RegexOptions.IgnoreCase))
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_StatusReport, wi.WorkplanId, wi.Pk_WorkItemUid, statusReport));
                    statusReport = "";
                }
            }

            if (statusReport != wi.StatusReport)
            {
                IsCurrentWiModified = true;
                if (statusReport.Length > 50)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_StatusReport_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    statusReport = statusReport.Substring(0, 50);
                }
                wi.StatusReport = statusReport;
            }
        }

        /// <summary>
        /// Handles the Work Item Description document.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatWid(WorkItemImportClass record, WorkItem wi)
        {
            string wid = record.Hyperlink;
            if (wid == "-")
                wid = "";

            // Check that reference looks good
            if (!string.IsNullOrEmpty(wid))
            {
                if (!Regex.IsMatch(wid, "^[A-Za-z0-9]{2}-[0-9]{5,6}$", RegexOptions.IgnoreCase))
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_WiD, wi.WorkplanId, wi.Pk_WorkItemUid, wid));
                    wid = "";
                }
            }

            if (wid != wi.Wid)
            {
                IsCurrentWiModified = true;
                if (wid.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Wid_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    wid = wid.Substring(0, 20);
                }
                wi.Wid = wid;
            }
        }

        /// <summary>
        /// Handles the completion rate. Ensures that completion is well formed.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatCompletion(WorkItemImportClass record, WorkItem wi)
        {
            string completion = record.Percent_Complete;
            int? compl = null;
            bool warning = false;
            if (completion != "-")
            {
                try
                {
                    var split = completion.Split('%');
                    if (split.Count() != 2 || !string.IsNullOrEmpty(split[1]))
                        warning = true;
                    else
                        compl = Int32.Parse(completion.Split('%')[0]);
                }
                catch (Exception)
                {
                    warning = true;
                }

                if (compl.HasValue && compl.Value > 100)
                    warning = true;

                if (warning)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Incorrect_Completion, wi.WorkplanId, wi.Pk_WorkItemUid, record.Percent_Complete));
                }
            }

            if (wi.Completion != compl)
            {
                IsCurrentWiModified = true;
                wi.Completion = compl;
            }

        }

        /// <summary>
        /// Handles the End date. Ignores empty dates or date as "-". Logs 
        /// a warning whenever date cannot be properly decoded.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatEndDate(WorkItemImportClass record, WorkItem wi)
        {
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(record.Finish_Date) && record.Finish_Date != "-")
            {
                DateTime tmpDate;
                if (DateTime.TryParseExact(record.Finish_Date, "d/M/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out tmpDate))
                {
                    endDate = tmpDate;
                }
                else
                {
                    Report.LogWarning(
                        String.Format(Utils.Localization.WorkItem_Import_Incorrect_EndDate,
                        wi.WorkplanId, wi.Pk_WorkItemUid, record.Finish_Date));
                }

            }

            if (wi.EndDate != endDate)
            {
                IsCurrentWiModified = true;
                wi.EndDate = endDate;
            }
        }

        /// <summary>
        /// Handles the Start date. Ignores empty dates or date as "-". Logs 
        /// a warning whenever date cannot be properly decoded.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatStartDate(WorkItemImportClass record, WorkItem wi)
        {
            DateTime? startDate = null;
            if (!string.IsNullOrEmpty(record.Start_Date) && record.Start_Date != "-")
            {
                DateTime tmpDate;
                if (DateTime.TryParseExact(record.Start_Date, "d/M/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out tmpDate))
                {
                    startDate = tmpDate;
                }
                else
                {
                    Report.LogWarning(
                        String.Format(Utils.Localization.WorkItem_Import_Incorrect_StartDate,
                        wi.WorkplanId, wi.Pk_WorkItemUid, record.Start_Date));
                }

            }

            if (wi.StartDate != startDate)
            {
                IsCurrentWiModified = true;
                wi.StartDate = startDate;
            }

        }

        /// <summary>
        /// Fills in the release. Check that the release exist, and bind it.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatRelease(WorkItemImportClass record, WorkItem wi)
        {
            int? releaseFk = null;

            if (record.Release == "-")
                return;

            // Get all the releases if we don't have them yet.
            if (AllReleases == null)
            {
                var releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
                releaseRepo.UoW = UoW;
                AllReleases = releaseRepo.All.ToList();
            }

            // Check against the release shortname
            var targetRelease = AllReleases.FirstOrDefault(r => r.Code == record.Release);
            if (targetRelease != null)
            {
                releaseFk = targetRelease.Pk_ReleaseId;                
            }
            else
            {
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_Release, wi.WorkplanId, wi.Pk_WorkItemUid, record.Release));
            }

            if (wi.Fk_ReleaseId != releaseFk)
            {
                IsCurrentWiModified = true;
                wi.Fk_ReleaseId = releaseFk;
            }

            if (wi.Fk_ParentWiId != null)
            {
                var parentWi = TreatedWorkItems.FirstOrDefault(x => x.Pk_WorkItemUid == wi.Fk_ParentWiId);
                if ((parentWi != null) && (parentWi.Fk_ReleaseId != null))
                {
                    var parentRelease = AllReleases.FirstOrDefault(x => x.Pk_ReleaseId == parentWi.Fk_ReleaseId);
                    string parentReleaseCode = (parentRelease != null) ? parentRelease.Code : String.Empty;

                    if (!record.Release.Equals(parentReleaseCode, StringComparison.InvariantCultureIgnoreCase))
                        Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Parent_Release_Different_With_Child, wi.WorkplanId, wi.Pk_WorkItemUid, record.Release, parentReleaseCode));
                }
            }
        }

        private void TreatAcronym(WorkItemImportClass record, WorkItem wi)
        {
            var acronym = record.Acronym;
            
            if (acronym == "-")
                acronym = "";

            if (acronym.Length > 50)
            {
                Report.LogWarning( String.Format(Utils.Localization.WorkItem_Import_Acronym_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid, acronym));
                acronym = acronym.Substring(0,50);
            }

            if (!string.IsNullOrEmpty(acronym))
            {
                // Check for a WI with same acronym. If found, assess that it's a parent of the WI.
                var wiWithSameAcronym = TreatedWorkItems.FirstOrDefault(w => w.Acronym == acronym);
                if (wiWithSameAcronym != null)
                {
                    var isParent = false;
                    var level = wi.WiLevel.GetValueOrDefault();

                    var parentWi = wi;

                    while (level > 0 && parentWi.Fk_ParentWiId.HasValue)
                    {
                        // Get parent
                        parentWi = TreatedWorkItems.FirstOrDefault(w => w.Pk_WorkItemUid == parentWi.Fk_ParentWiId.Value);
                        if (parentWi == null)
                            break;

                        if (parentWi.Pk_WorkItemUid == wiWithSameAcronym.Pk_WorkItemUid)
                        {
                            isParent = true;
                            break;
                        }

                        level = parentWi.WiLevel.GetValueOrDefault();
                    }

                    // If is parent, log a warning
                    if (isParent)
                    {
                        Report.LogWarning(
                            string.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSubLevel,
                            wi.WorkplanId, wi.Pk_WorkItemUid, acronym));
                        acronym = "";
                    }
                    else
                    {
                        Report.LogError(
                            string.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSameLevel,
                            wi.WorkplanId, wi.Pk_WorkItemUid, acronym, wiWithSameAcronym.Pk_WorkItemUid));
                    }
                }
            }

            var effectiveAcronym = record.Acronym == "-" ? string.Empty : record.Acronym;
            if (wi.Acronym != acronym || wi.Effective_Acronym != effectiveAcronym)
            {
                wi.Acronym = acronym;
                wi.Effective_Acronym = effectiveAcronym;
                IsCurrentWiModified = true;
            }
        }


        /// <summary>
        /// Decode the level.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatLevel(WorkItemImportClass record, WorkItem wi)
        {
            // Check that the level is indeed an int. Else default it to 0.
            int level;
            if (String.IsNullOrEmpty(record.Outline_Level) || record.Outline_Level == "-")
            {
                level = 0;
            }
            else if (!Int32.TryParse(record.Outline_Level, out level))
            {
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_Level, wi.WorkplanId, wi.Pk_WorkItemUid, record.Outline_Level));
                level = 0;
            }

            // Level 0 work items are the ones that correspond to milestones.
            if (record.Unique_ID == 0)
                level = 0;

            if (!wi.WiLevel.HasValue || wi.WiLevel.GetValueOrDefault() != level)
            {
                IsCurrentWiModified = true;
                wi.WiLevel = level;
            }

            // Now define the parent
            if (_lastTreatedWi != null && wi.WiLevel.Value > 0)
            {
                // The previous WI must have a lower value, or be the parent. If the gap is greater, there is an issue.
                if (wi.WiLevel.Value > _lastTreatedWi.WiLevel.Value + 1)
                    Report.LogError(String.Format(Utils.Localization.WorkItem_Import_Invalid_Hierarchy, wi.WorkplanId, wi.Pk_WorkItemUid, wi.WiLevel.Value, _lastTreatedWi.WiLevel.Value));
                else
                {
                    // Get the last level -1 parent
                    if (_lastWiForLevel.ContainsKey(wi.WiLevel.Value - 1))
                        wi.Fk_ParentWiId = (_lastWiForLevel[wi.WiLevel.Value - 1]).Pk_WorkItemUid;
                }
            }

            // Finally replace the record with the last treated one
            _lastWiForLevel[wi.WiLevel.Value] = wi;
            _lastTreatedWi = wi;
        }

        /// <summary>
        /// Retrieves the name of the WI. 
        /// 
        /// Check if the WI name has changed.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatName(WorkItemImportClass record, WorkItem wi)
        {
            string name = record.Name;
            if (wi.Name != name)
            {
                IsCurrentWiModified = true;
                if (name.Length > 255)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Name_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    name = name.Substring(0, 255);
                }
                wi.Name = name;
            }
        }

        /// <summary>
        /// Handles the work item unique ID. The ID must not have already been parsed, 
        /// else it is an error.
        /// Returns the workitem to work on, if ever it was encountered in the data source
        /// </summary>
        /// <param name="record"></param>
        private WorkItem TreatWiUid(WorkItemImportClass record)
        {
            var uidToSearchFor = record.Unique_ID;

            WorkItem searchWi;
            // Exception: if UID = 0, we are looking for UID = WorkplanId
            if (uidToSearchFor == 0)
            {
                searchWi = AllWorkItems.FirstOrDefault(w => w.Name == record.Name);
            }
            else
            {
                // Seek for the WI in the list
                searchWi = AllWorkItems.FirstOrDefault(w => w.Pk_WorkItemUid == uidToSearchFor);
            }

            WorkItem aReturnWi;
            if (searchWi == null)
            {
                // Did not find wi => We need a new one. We can thus flag it as to be added
                aReturnWi = new WorkItem();

                if (uidToSearchFor == 0)
                {
                    int nextLevel0Uid = LowerBoundForLevel0Wi;
                    if (AllWorkItems.Count > 0)
                        nextLevel0Uid = Math.Max(nextLevel0Uid, AllWorkItems.Max(w => w.Pk_WorkItemUid));
                    if (TreatedWorkItems.Count > 0)
                        nextLevel0Uid = Math.Max(nextLevel0Uid, TreatedWorkItems.Max(w => w.Pk_WorkItemUid));

                    aReturnWi.Pk_WorkItemUid = ++nextLevel0Uid;
                }
                else
                {
                    aReturnWi.Pk_WorkItemUid = uidToSearchFor;
                }
                IsCurrentWiModified = true;
                aReturnWi.IsNew = true;
            }
            else
            {
                //Remove Reference Objects to avoid Referential Integrity Errors
                searchWi.Release = null;
                searchWi.ParentWi = null;
                searchWi.ChildWis.Clear();
                searchWi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.WorkItem = null);
                searchWi.Remarks.ToList().ForEach(x => x.WorkItem = null);

                aReturnWi = searchWi;
            }

            // Ensure that not two WI exist.
            var sameUidWi = TreatedWorkItems.FirstOrDefault(w => w.Pk_WorkItemUid == aReturnWi.Pk_WorkItemUid);
            if (sameUidWi != null)
            {
                Report.LogError(String.Format(Utils.Localization.WorkItem_Import_DuplicateUID, aReturnWi.Pk_WorkItemUid, sameUidWi.WorkplanId, record.ID));
            }

            return aReturnWi;

        }


        /// <summary>
        /// Treats the WorkPlan ID field.
        /// 
        /// Logs a warning if such a WPId has already been treated.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatWpId(WorkItemImportClass record, WorkItem wi)
        {
            // Check if the field is not null, it means we need to check against 
            if (wi.WorkplanId.GetValueOrDefault() != record.ID)
            {
                IsCurrentWiModified = true;
            }
            wi.WorkplanId = record.ID;

            // Check order is correct
            if (_lastTreatedWi != null && _lastTreatedWi.WorkplanId > wi.WorkplanId)
            {
                Report.LogError(String.Format(Utils.Localization.WorkItem_Import_Wrong_Order, wi.WorkplanId, _lastTreatedWi.WorkplanId));
            }

            // Check that no work item already has this number.
            if (TreatedWorkItems.FirstOrDefault(w => w.WorkplanId == wi.WorkplanId) != null)
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_DuplicateWorkplanId, wi.WorkplanId));

        }

        #endregion

    }

    public interface IWorkItemCsvParser
    {
        KeyValuePair<List<WorkItem>, Report> ParseCsv(string fileLocation);
        IUltimateUnitOfWork UoW { get; set; }
    }
}
