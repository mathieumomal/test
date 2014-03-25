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
        private CustomizableCellStyle NameCellStyle { get; set; }
        private CustomizableCellStyle rowStyle ;

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

        public CustomizableCellStyle GetNameCellStyle(){
            throw new NotImplementedException();
            // Depenging on the propoerties of the current object return one of the pre-defined styles
        }

        /*public CustomizableCellStyle GetRowStyle(){
            if(Completion.Length>1 && Completion.Substring(0,Completion.Length-2).Equals("100")){
                rowStyle = new CustomizableCellStyle()
                rowStyle.FillForegroundColor= new NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index; 
            }
            else{
                rowStyle = null
            }
            return rowStyle; 
        }*/
    }
}
