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
        public string Wpid { get; set; }
        public string UID { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public int Level { get; set; }
        public string Release { get; set; }
        public string ResponsibleGroups { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Completion { get; set; }
        public string HyperLink { get; set; }
        public public string StatusReport { get; set; }
        public string WIRaporteur { get; set; }
        public string WIRaporteurEmail { get; set; }
        public string Notes { get; set; }
        public string RelatedTSs_TRs { get; set; }
        private CustomizableCellStyle NameCellStyle { get; set; }
        private CustomizableCellStyle rowStyle ;

        public WorkItemForExport(WorkItem workItem){
            Wpid = workItem.WorkplanId;
            UID = workItem.Pk_WorkItemUid; 
            Name = workItem.Name;
            Acronym = workItem.Acronym;
            Level = workItem.WiLevel;
            Release = workItem.Release.Description;
            ResponsibleGroups = workItem.WorkItems_ResponsibleGroups.Join(",", r => r.ResponsibleGroup);
            StartDate = workItem.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            EndDate = workItem.EndDate..GetValueOrDefault().ToString("yyyy-MM-dd");
            Completion = workItem.Completion;
            HyperLink = workItem.Wid;
            StatusReport = workItem.StatusReport;
            WIRaporteur = workItem.RapporteurCompany;
            WIRaporteurEmail = workItem.RapporteurStr;
            Notes =  workItem.Remarks.Join(" ", r => r.RemarkText);
            RelatedTSs_TRs = workItem.TssAndTrs;
        }

        public static List<WorkItemForExport> GetWorkItemsListForExport(List<WorkItem> exportDataSource){
            List<WorkItemForExport> formatedDataSource = new List<WorkItemForExport>();
            foreach(WorkItem wi in exportDataSource){
                formatedDataSource.Add(new WorkItemForExport(wi));
            }
            return formatedDataSource;
        }

        public CustomizableCellStyle CustomizableCellStyle GetNameCellStyle(){
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
