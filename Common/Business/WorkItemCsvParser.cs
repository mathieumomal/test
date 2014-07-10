using System;
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
        private Dictionary<int, WorkItem> lastWIForLevel;
        private WorkItem lastTreatedWi;
        private List<WorkItem> TreatedWorkItems { get; set; }
        private List<WorkItem> ModifiedWorkItems { get; set; }
        private bool IsCurrentWIModified { get; set; }

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
                        IsCurrentWIModified = false;
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

                        if (IsCurrentWIModified)
                            ModifiedWorkItems.Add(wi);
                        TreatedWorkItems.Add(wi);
                    }

                }

                // Return the list of workitems that were modified, along with the report.
                return new KeyValuePair<List<WorkItem>, Report>(ModifiedWorkItems, Report);
            }

            catch (System.IO.FileNotFoundException e)
            {
                // Log the error
                Utils.LogManager.Error("Error occured in WorkplanCsvParser: cannot find file" + fileLocation);
                var errorReport = new Report();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_FileNotFound, fileLocation));

                return new KeyValuePair<List<WorkItem>, Report>(new List<WorkItem>(), errorReport);
            }
            catch (CsvHelper.CsvBadDataException e)
            {
                var errorReport = new Report();
                errorReport.LogError(String.Format(Utils.Localization.WorkItem_Import_Bad_Format, e.Message));

                return new KeyValuePair<List<WorkItem>, Report>(new List<WorkItem>(), errorReport);
            }

            catch (Exception e)
            {
                // Log the error
                Utils.LogManager.Error("Error occured in WorkplanCsvParser:" + e.Message);
                Utils.LogManager.Error("Stacktrace:" + e.StackTrace);

                string lastSuccessfullyTreatedWi = "None";
                if (lastTreatedWi != null)
                    lastSuccessfullyTreatedWi = lastTreatedWi.Pk_WorkItemUid.ToString();
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
            lastWIForLevel = new Dictionary<int, WorkItem>();
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
                    IsCurrentWIModified = true;
                    wi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.IsDeleted = true);
                }
                return;
            }


            // Check if it has changed
            var respStr = string.Join(",", wi.WorkItems_ResponsibleGroups.Select(c => c.ResponsibleGroup).ToList());
            if (respStr != resourceStr)
            {
                IsCurrentWIModified = true;
                wi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.IsDeleted = true);

                var resources = record.Resource_Names.Split(',');
                bool isFirst = true;
                foreach (var res in resources)
                {
                    // First check resource corresponds to a community
                    var community = AllCommunities.Where(c => c.ShortName == res).FirstOrDefault();

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
                IsCurrentWIModified = true;
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
                IsCurrentWIModified = true;
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
            string approvedTSGMtgRef = record.TSG_approved;
            if (approvedTSGMtgRef == "-")
                approvedTSGMtgRef = "";

            if (wi.TsgApprovalMtgRef != approvedTSGMtgRef)
            {
                IsCurrentWIModified = true;

                if (approvedTSGMtgRef.Length > 20)
                {
                    Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_TsgApprovalMtgRef_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                    approvedTSGMtgRef = approvedTSGMtgRef.Substring(0, 20);
                }

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
                var mtg = AllMeetings.Where(m => m.MtgShortRef == mtgRef).FirstOrDefault();
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

            if (tsAndTr.Length > 50)
            {
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_TsTr_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid));
                tsAndTr = tsAndTr.Substring(0, 50);
            }

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
                    wi.Remarks.Add(new Remark() { CreationDate = DateTime.Now, RemarkText = remarkStr, IsPublic = true });
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

                var person = AllPersons.Where(p => p.Email == email).FirstOrDefault();
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
                IsCurrentWIModified = true;

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
                IsCurrentWIModified = true;
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
                IsCurrentWIModified = true;
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
            var targetRelease = AllReleases.Where(r => r.Code == record.Release).FirstOrDefault();
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

            if (acronym.Length > 50)
            {
                Report.LogWarning( String.Format(Utils.Localization.WorkItem_Import_Acronym_Too_Long, wi.WorkplanId, wi.Pk_WorkItemUid, acronym));
                acronym = acronym.Substring(0,50);
            }

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
            string name = record.Name;
            if (wi.Name != name)
            {
                IsCurrentWIModified = true;
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
        /// <param name="wi"></param>
        private WorkItem TreatWiUid(WorkItemImportClass record)
        {
            int UidToSearchFor = record.Unique_ID;

            WorkItem searchWi;
            // Exception: if UID = 0, we are looking for UID = WorkplanId
            if (UidToSearchFor == 0)
            {
                searchWi = AllWorkItems.Where(w => w.Name == record.Name).FirstOrDefault();
            }
            else
            {
                // Seek for the WI in the list
                searchWi = AllWorkItems.Where(w => w.Pk_WorkItemUid == UidToSearchFor).FirstOrDefault();
            }

            WorkItem aReturnWI;
            if (searchWi == null)
            {
                // Did not find wi => We need a new one. We can thus flag it as to be added
                aReturnWI = new WorkItem();

                if (UidToSearchFor == 0)
                {
                    int nextLevel0Uid = LowerBoundForLevel0Wi;
                    if (AllWorkItems.Count > 0)
                        nextLevel0Uid = Math.Max(nextLevel0Uid, AllWorkItems.Max(w => w.Pk_WorkItemUid));
                    if (TreatedWorkItems.Count > 0)
                        nextLevel0Uid = Math.Max(nextLevel0Uid, TreatedWorkItems.Max(w => w.Pk_WorkItemUid));

                    aReturnWI.Pk_WorkItemUid = ++nextLevel0Uid;
                }
                else
                {
                    aReturnWI.Pk_WorkItemUid = UidToSearchFor;
                }
                IsCurrentWIModified = true;
                aReturnWI.IsNew = true;
            }
            else
            {
                //Remove Reference Objects to avoid Referential Integrity Errors
                searchWi.Release = null;
                searchWi.ParentWi = null;
                searchWi.ChildWis.Clear();
                searchWi.WorkItems_ResponsibleGroups.ToList().ForEach(x => x.WorkItem = null);
                searchWi.Remarks.ToList().ForEach(x => x.WorkItem = null);

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

            // Check order is correct
            if (lastTreatedWi != null && lastTreatedWi.WorkplanId > wi.WorkplanId)
            {
                Report.LogError(String.Format(Utils.Localization.WorkItem_Import_Wrong_Order, wi.WorkplanId, lastTreatedWi.WorkplanId));
            }

            // Check that no work item already has this number.
            if (TreatedWorkItems.Where(w => w.WorkplanId == wi.WorkplanId).FirstOrDefault() != null)
                Report.LogWarning(String.Format(Utils.Localization.WorkItem_Import_DuplicateWorkplanId, wi.WorkplanId.ToString()));

        }

        #endregion

    }

    public interface IWorkItemCsvParser
    {
        KeyValuePair<List<WorkItem>, Report> ParseCsv(string fileLocation);
        IUltimateUnitOfWork UoW { get; set; }
    }
}
