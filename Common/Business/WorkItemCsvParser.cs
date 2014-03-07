﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

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
        public int Outline_Level { get; set; }
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
    public class WorkItemCsvParser
    {
        public IUltimateUnitOfWork UoW { get; set; }

        // Data retrieved to be able to establish references to the different objects.
        protected List<WorkItem> AllWorkItems { get; set; }
        protected List<Release> AllReleases { get; set; }
        protected List<View_Persons> AllPersons { get; set; }
        protected List<Meeting> AllMeetings { get; set; }
        protected List<Community> AllCommunities { get; set; }

        // Report that is returned after parsing.
        protected ImportReport Report { get; set; }
        
        // List of treated data, and references used during the parsing.
        protected Dictionary<int, WorkItem> lastWIForLevel;
        protected WorkItem lastTreatedWi;
        protected List<WorkItem> TreatedWorkItems { get; set; }
        protected List<WorkItem> ModifiedWorkItems { get; set; }
        protected bool IsCurrentWIModified { get; set; }

        /// <summary>
        /// Parses the CSV file.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public KeyValuePair<List<WorkItem>, ImportReport> ParseCsv(string fileLocation)
        {
            try
            {

                var path = fileLocation;
                // Treat the file
                if (path.EndsWith("zip"))
                {
                    var files = Zip.Extract(path, false);
                    if (files.Count != 1 || !files.First().EndsWith("csv"))
                    {

                    }
                    else
                    {
                        path = files.First();
                    }
                }

                if (UoW == null)
                    throw new InvalidOperationException("Cannot process with UoW defined");

                // Initialize all fields
                InitializeCommonData();

                // Open the file.
                using (StreamReader reader = new StreamReader(path))
                {

                    var csv = new CsvReader(reader);

                    // Configure the reader
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.DetectColumnCountChanges = true;
                    csv.Configuration.Encoding = Encoding.UTF8;
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.TrimFields = false;

                    // For each line:
                    // - Create a new record (that might be discarded if WI already exist)
                    // - Treat the fields one by one.
                    // - Depending on whether or not the WI has been updated, store it in the list of WIs to be updated.
                    while (csv.Read())
                    {
                        IsCurrentWIModified = false;
                        var record = csv.GetRecord<WorkItemImportClass>();
                        var wi = new WorkItem();

                        wi = TreatWiUid(record, wi);
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

                        if (IsCurrentWIModified)
                            ModifiedWorkItems.Add(wi);
                        TreatedWorkItems.Add(wi);
                    }

                }

                // Return the list of workitems that were modified, along with the report.
                return new KeyValuePair<List<WorkItem>, ImportReport>(ModifiedWorkItems, Report);
            }

            catch (System.IO.FileNotFoundException e)
            {
                // Log the error
                Utils.LogManager.Error("Error occured in WorkplanCsvParser: cannot find file" + fileLocation);
                var errorReport = new ImportReport();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_FileNotFound, fileLocation));

                return new KeyValuePair<List<WorkItem>, ImportReport>(new List<WorkItem>(), errorReport);
            }

            catch (Exception e)
            {
                // Log the error
                Utils.LogManager.Error("Error occured in WorkplanCsvParser:" + e.Message);
                Utils.LogManager.Error("Stacktrace:" + e.StackTrace);

                string lastSuccessfullyTreatedWi = "None";
                if (lastTreatedWi != null)
                    lastSuccessfullyTreatedWi = lastTreatedWi.Pk_WorkItemUid.ToString();
                var errorReport = new ImportReport();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_Unknown_Exception, lastSuccessfullyTreatedWi));

                return new KeyValuePair<List<WorkItem>, ImportReport>(new List<WorkItem>(), errorReport);

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
            AllWorkItems = wiRepo.AllIncluding(w=> w.WorkItems_ResponsibleGroups, w => w.Remarks).ToList();

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
            lastWIForLevel = new Dictionary<int, WorkItem>();
            Report = new ImportReport();
            ModifiedWorkItems = new List<WorkItem>();
            TreatedWorkItems = new List<WorkItem>();
        }

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
                    IsCurrentWIModified=true;
                    wi.WorkItems_ResponsibleGroups.Clear();
                }
                return;
            }


            // Check if it has changed
            var respStr = string.Join(",",wi.WorkItems_ResponsibleGroups.Select(c => c.ResponsibleGroup).ToList());
            if (respStr != resourceStr)
            {
                IsCurrentWIModified = true;
                wi.WorkItems_ResponsibleGroups.Clear();

                var resources = record.Resource_Names.Split(',');
                bool isFirst = true;
                foreach (var res in resources)
                {
                    // First check resource corresponds to a community
                    var community = AllCommunities.Where(c => c.ShortName==res).FirstOrDefault();
                        
                    if (community == null)
                    {
                        Report.LogWarning(
                            String.Format(Utils.Localization.WorkItem_Import_Invalid_Resource, wi.WorkplanId, wi.Pk_WorkItemUid, res));
                    }
                    else
                    {
                        wi.WorkItems_ResponsibleGroups.Add(
                            new WorkItems_ResponsibleGroups() { Fk_TbId = community.TbId, ResponsibleGroup = community.ShortName,
                                IsPrimeResponsible = isFirst });
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
                if (DateTime.TryParseExact(record.last_modif, "dd/MM/yyyy", CultureInfo.InvariantCulture,
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
                IsCurrentWIModified = true;
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
                if (DateTime.TryParseExact(record.Created, "dd/MM/yyyy", CultureInfo.InvariantCulture,
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
                IsCurrentWIModified = true;
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
                IsCurrentWIModified = true;
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

            if (wi.TsgStoppedMtgRef!= stoppedTsgMtgRef)
            {
                IsCurrentWIModified = true;
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
                IsCurrentWIModified = true;
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
            string approvedTSGMtgRef = record.TSG_approved;
            if (approvedTSGMtgRef == "-")
                approvedTSGMtgRef = "";

            if (wi.TsgApprovalMtgRef != approvedTSGMtgRef)
            {
                IsCurrentWIModified = true;
                wi.TsgApprovalMtgRef = approvedTSGMtgRef;

                // Search for corresponding meeting, but only if ref is not empty
                if (string.IsNullOrEmpty(approvedTSGMtgRef))
                    return;

                int mtgId = GetMeetingForRef(approvedTSGMtgRef);
                if (mtgId != -1)
                {
                    wi.TsgApprovalMtgId = mtgId;
                }
                else
                {
                    Report.LogWarning(String.Format(
                        Utils.Localization.WorkItem_Import_Invalid_TsgApprovedMtg, wi.WorkplanId, wi.Pk_WorkItemUid, approvedTSGMtgRef));
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
            string fullMtgRef;
            // Try to fetch the meeting ID
            try
            {
                fullMtgRef = Meeting.ToFullReference(mtgRef);
                var mtg = AllMeetings.Where(m => m.MTG_REF == fullMtgRef).FirstOrDefault();
                if (mtg == null)
                {
                    return -1;
                }
                else
                {
                    return mtg.MTG_ID;
                }
            }
            catch (Exception e)
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

            if (wi.TssAndTrs != tsAndTr)
            {
                IsCurrentWIModified = true;
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
                    IsCurrentWIModified = true;
                    wi.Remarks.Add(new Remark() { Fk_PersonId = 0, CreationDate = DateTime.Now, RemarkText = remarkStr });
                }
                else
                {
                    foreach (var remark in wi.Remarks)
                    {
                        int pos = remarkStr.IndexOf(remark.RemarkText);
                        if (pos >= 0)
                            remarkStr = remarkStr.Remove(pos, remark.RemarkText.Length);
                    }

                    // Clean a little bit the remaining string.
                    string cleanRemarkPattern = "[A-Za-z0-9].*$";
                    var match = Regex.Match(remarkStr, cleanRemarkPattern);

                    if (match.Success && match.Length > 1)
                    {
                        IsCurrentWIModified = true;
                        wi.Remarks.Add(new Remark() { Fk_PersonId = 0, CreationDate = DateTime.Now, RemarkText = match.Value });
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
                var person = AllPersons.Where(p => p.Email == rapporteurStr).FirstOrDefault();
                if (person != null)
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
                IsCurrentWIModified = true;
                wi.RapporteurStr = rapporteurStr;
            }

            // Update the rapporteur ID
            if (wi.RapporteurId != rapporteurId)
            {
                IsCurrentWIModified = true;
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
                IsCurrentWIModified = true;
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
                if (!Regex.IsMatch(statusReport, "^[A-Za-z0-9]{2}-[0-9]{6}$", RegexOptions.IgnoreCase))
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_StatusReport, wi.WorkplanId, wi.Pk_WorkItemUid, statusReport));
                    statusReport = "";
                }
            }

            if (statusReport != wi.StatusReport)
            {
                IsCurrentWIModified = true;
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
                if (!Regex.IsMatch(wid, "[A-Za-z0-9]{2}-[0-9]{6}", RegexOptions.IgnoreCase))
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_Invalid_WiD, wi.WorkplanId, wi.Pk_WorkItemUid, wid));
                    wid = "";
                }
            }

            if (wid != wi.Wid)
            {
                IsCurrentWIModified = true;
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
                catch (Exception e)
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
                IsCurrentWIModified = true;
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
                if (DateTime.TryParseExact(record.Finish_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture,
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
                IsCurrentWIModified = true;
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
            if ( !string.IsNullOrEmpty(record.Start_Date) && record.Start_Date != "-")
            {
                DateTime tmpDate;
                if (DateTime.TryParseExact(record.Start_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture,
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
                IsCurrentWIModified = true;
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
            int releaseFk = 0;

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
            var targetRelease = AllReleases.Where(r => r.ShortName == record.Release).FirstOrDefault();
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
                IsCurrentWIModified = true;
                wi.Fk_ReleaseId = releaseFk;
            }


        }

        private void TreatAcronym(WorkItemImportClass record, WorkItem wi)
        {
            string acronym = record.Acronym;
            if (acronym == "-")
                acronym = "";

            if (!string.IsNullOrEmpty(acronym))
            {
                // Check for a WI with same acronym. If found, assess that it's a parent of the WI.
                var wiWithSameAcronym = TreatedWorkItems.Where(w => w.Acronym == acronym).FirstOrDefault();
                if (wiWithSameAcronym != null)
                {
                    bool isParent = false;
                    int level = wi.WiLevel.GetValueOrDefault();

                    var parentWi = wi;

                    while (level > 0 && parentWi.Fk_ParentWiId.HasValue)
                    {
                        // Get parent
                        parentWi = TreatedWorkItems.Where(w => w.Pk_WorkItemUid == parentWi.Fk_ParentWiId.Value).FirstOrDefault();
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
                            String.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSubLevel,
                            wi.WorkplanId, wi.Pk_WorkItemUid, acronym));
                        acronym = "";
                    }
                    else
                    {
                        Report.LogError(
                            String.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSameLevel,
                            wi.WorkplanId, wi.Pk_WorkItemUid, acronym, wiWithSameAcronym.Pk_WorkItemUid));
                    }
                }
            }

            if (wi.Acronym != acronym)
            {
                wi.Acronym = acronym;
                IsCurrentWIModified = true;
            }
        }


        /// <summary>
        /// Decode the level.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private void TreatLevel(WorkItemImportClass record, WorkItem wi)
        {
            // Level 0 work items are the ones that correspond to milestones.
            var level = record.Outline_Level;
            if (record.Unique_ID == 0)
                level = 0;

            if (!wi.WiLevel.HasValue || wi.WiLevel.GetValueOrDefault() != level)
            {
                IsCurrentWIModified = true;
                wi.WiLevel = level;
            }

            // Now define the parent
            if (lastTreatedWi != null && wi.WiLevel.Value > 0)
            {
                // The previous WI must have a lower value, or be the parent. If the gap is greater, there is an issue.
                if (wi.WiLevel.Value > lastTreatedWi.WiLevel.Value + 1)
                    Report.LogError(String.Format(Utils.Localization.WorkItem_Import_Invalid_Hierarchy, wi.WorkplanId, wi.Pk_WorkItemUid, wi.WiLevel.Value, lastTreatedWi.WiLevel.Value));
                else
                {
                    // Get the last level -1 parent
                    if (lastWIForLevel.ContainsKey(wi.WiLevel.Value - 1))
                        wi.Fk_ParentWiId = (lastWIForLevel[wi.WiLevel.Value - 1]).Pk_WorkItemUid;
                }
            }

            // Finally replace the record with the last treated one
            lastWIForLevel[wi.WiLevel.Value] = wi;
            lastTreatedWi = wi;
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
            if (wi.Name != record.Name)
            {
                IsCurrentWIModified = true;
                wi.Name = record.Name;
            }
        }

        /// <summary>
        /// Handles the work item unique ID. The ID must not have already been parsed, 
        /// else it is an error.
        /// Returns the workitem to work on, if ever it was encountered in the data source
        /// </summary>
        /// <param name="record"></param>
        /// <param name="wi"></param>
        private WorkItem TreatWiUid(WorkItemImportClass record, WorkItem wi)
        {
            int UidToSearchFor = record.Unique_ID;
            // Exception: if UID = 0, we are looking for UID = WorkplanId
            if (UidToSearchFor == 0)
                UidToSearchFor = record.ID;

            // Seek for the WI in the list
            var searchWi = AllWorkItems.Where(w => w.Pk_WorkItemUid == UidToSearchFor).FirstOrDefault();

            WorkItem aReturnWI;
            if (searchWi == null) // Did not find wi => We need a new one. We can thus flag it as to be added
            {
                aReturnWI = wi;
                wi.Pk_WorkItemUid = UidToSearchFor;
                IsCurrentWIModified = true;
                wi.IsNew = true;
            }
            else
            {
                aReturnWI = searchWi;
            }

            // Ensure that not two WI exist.
            var sameUidWi = TreatedWorkItems.Where(w => w.Pk_WorkItemUid == aReturnWI.Pk_WorkItemUid).FirstOrDefault();
            if (sameUidWi != null)
            {
                Report.LogError(String.Format(Utils.Localization.WorkItem_Import_DuplicateUID, aReturnWI.Pk_WorkItemUid, sameUidWi.WorkplanId, record.ID));
            }

            return aReturnWI;

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
                IsCurrentWIModified = true;
            }
            wi.WorkplanId = record.ID;

            // Check that no work item already has this number.
            if (TreatedWorkItems.Where(w => w.WorkplanId == wi.WorkplanId).FirstOrDefault() != null)
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_DuplicateWorkplanId, wi.WorkplanId.ToString()));



        }


        
    }
}