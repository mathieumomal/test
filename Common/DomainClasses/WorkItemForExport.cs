using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class WorkItemForExport
    {
        //TODO: A dictionnary that contains all possible styles (e.g. 12)
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


        public WorkItemForExport(WorkItem workItem){
            Wpid = workItem.WorkplanId;
            UID = workItem.Pk_WorkItemUid;
            Name = GetEmptyString(workItem.WiLevel.Value) + workItem.Name;
            Acronym = workItem.Acronym;
            Level = workItem.WiLevel.Value;
            Release = workItem.Release.Description;
            ResponsibleGroups = string.Join(" ", workItem.WorkItems_ResponsibleGroups.Select(r => r.ResponsibleGroup).ToArray()); 
            StartDate = workItem.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            EndDate = workItem.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            Completion = ((workItem.Completion != null) ? workItem.Completion: 0)/100;
            HyperLink = workItem.Wid;
            StatusReport = workItem.StatusReport;
            WIRaporteur = workItem.RapporteurCompany;
            WIRaporteurEmail = workItem.RapporteurStr;
            Notes = string.Join(" ", workItem.Remarks.Select(r => r.RemarkText).ToArray()); 
            RelatedTSs_TRs = workItem.TssAndTrs;
        }

        private string GetEmptyString(int level)
        {
            StringBuilder space = new StringBuilder();
            if (level > 1)
            {
                for (int i = 3; i <= level * 2; i++)
                    space.Append(" ");
            }
            return space.ToString();
        }

        public static List<WorkItemForExport> GetWorkItemsListForExport(List<WorkItem> exportDataSource){
            List<WorkItemForExport> formatedDataSource = new List<WorkItemForExport>();
            foreach(WorkItem wi in exportDataSource){
                formatedDataSource.Add(new WorkItemForExport(wi));
            }
            return formatedDataSource;
        }
       
        public System.Drawing.Color GetCellBgColor()
        {
            if(Completion.Value == 1)
                return System.Drawing.Color.FromArgb(204, 255, 204);
            else
                return System.Drawing.Color.White;
        }

        
    }
}
