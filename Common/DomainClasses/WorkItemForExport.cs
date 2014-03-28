using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class WorkItemForExport
    {
        private const string BLANK_CELL = "  -  ";
        public Nullable<int> Wpid { get; set; }
        public int UID { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public int Level { get; set; }
        public string Release { get; set; }
        public string ResponsibleGroups { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Nullable<int> Completion { get; set; }
        public string HyperLink { get; set; }
        public string StatusReport { get; set; }
        public string WIRaporteur { get; set; }
        public string WIRaporteurEmail { get; set; }
        public string Notes { get; set; }
        public string RelatedTSs_TRs { get; set; }
        public bool StoppedMeeting { get; set; }


        public WorkItemForExport(WorkItem workItem){
            Wpid = workItem.WorkplanId;
            UID = (workItem.Pk_WorkItemUid >= Math.Pow(10,8)) ? 0 :workItem.Pk_WorkItemUid;
            Name = GetEmptyString(workItem.WiLevel.Value) + workItem.Name;
            Acronym = (workItem.Acronym.Equals(string.Empty)) ? BLANK_CELL : workItem.Acronym;
            Level = workItem.WiLevel.Value;
            Release = (workItem.Release != null) ? (((workItem.Release.Description != null) && (workItem.Release.Description.Equals(string.Empty))) ? workItem.Release.Description : BLANK_CELL) : BLANK_CELL;
            ResponsibleGroups = (workItem.WorkItems_ResponsibleGroups.Count>0) ? string.Join(" ", workItem.WorkItems_ResponsibleGroups.Select(r => r.ResponsibleGroup).ToArray()).Trim() : BLANK_CELL; 
            StartDate = (workItem.StartDate != null) ? workItem.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd") : BLANK_CELL;
            EndDate = (workItem.EndDate != null) ? workItem.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd") : BLANK_CELL;
            Completion = ((workItem.Completion != null) ? workItem.Completion: 0)/100;
            HyperLink = (workItem.Wid.Equals(string.Empty)) ? workItem.Wid : BLANK_CELL;
            StatusReport = (!workItem.StatusReport.Equals(string.Empty)) ? workItem.StatusReport: BLANK_CELL;
            WIRaporteur = (!workItem.RapporteurCompany.Equals(string.Empty)) ? workItem.RapporteurCompany: BLANK_CELL;
            WIRaporteurEmail = (!workItem.RapporteurStr.Equals(string.Empty)) ? workItem.RapporteurStr: BLANK_CELL;
            Notes = ((workItem.Remarks != null) && (workItem.Remarks.Count > 0)) ? string.Join(" ", workItem.Remarks.Select(r => r.RemarkText).ToArray()).Trim() : BLANK_CELL; 
            RelatedTSs_TRs = (!workItem.TssAndTrs.Equals(string.Empty)) ? workItem.TssAndTrs: BLANK_CELL; 
            StoppedMeeting = (workItem.TsgStoppedMtgId != null || !String.IsNullOrEmpty(workItem.TsgStoppedMtgRef) 
                                                               || workItem.PcgStoppedMtgId != null 
                                                               || !String.IsNullOrEmpty(workItem.PcgStoppedMtgRef));
        }

        private string GetEmptyString(int level)
        {
            StringBuilder space = new StringBuilder();
            if (level > 1)
            {
                for (int i = 1; i <= (level * 3) - 3; i++)
                    space.Append(" ");
            }
            return space.ToString();
        }

        public static List<WorkItemForExport> GetWorkItemsListForExport(List<WorkItem> exportDataSource){
            List<WorkItemForExport> formatedDataSource = new List<WorkItemForExport>();
            foreach(WorkItem wi in exportDataSource){
                formatedDataSource.Add(new WorkItemForExport(wi));
            }
            return formatedDataSource.OrderBy(w => w.Wpid).ToList();
        }
       
        public int GetCellStyle(string colName)
        {
            int index = 0;

            if (colName.Equals("Name"))
            {
                switch (Level)
                {
                    case 0:
                        index += 9;
                        break;
                    case 1:
                        index += 6;
                        break;
                    case 2:
                        index += 3;
                        break;
                    default:
                        break;
                }
            }

            if (StoppedMeeting)
            {
                index += 3;
            }
            else
            {
                if (Completion.Value == 1)
                    index += 2;
                else
                    index += 1;
            }
            return index;
        }

        
    }
}
