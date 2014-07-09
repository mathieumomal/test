using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    [Serializable]
    public class Report
    {
        public List<string> ErrorList { get; set;}
        public List<string> WarningList { get; set; }
        public List<string> InfoList { get; set; }

        string CurrentModule { get; set; }

        public Report()
        {
            ErrorList = new List<string>();
            WarningList = new List<string>();
            InfoList = new List<string>();
        }

        public void LogError(string msg)
        {
            ErrorList.Add(msg);
        }
        public void LogError(string msg, string table)
        {
            ErrorList.Add("[" + table + "] " + msg);

        }

        public void LogWarning(string msg)
        {
            WarningList.Add(msg);
        }
        public void LogWarning(string msg, string table)
        {
            WarningList.Add("[" + table + "] " + msg);
        }

        public void LogInfo(string msg)
        {
            InfoList.Add(msg);
        }
        public void LogInfo(string msg, string table)
        {
            InfoList.Add("[" + table + "] " + msg);
        }

        public int GetNumberOfIssues()
        {
            return ErrorList.Count + WarningList.Count;
        }

        public int GetNumberOfErrors()
        {
            return ErrorList.Count;
        }

        public int GetNumberOfWarnings()
        {
            return WarningList.Count;
        }

        public int GetNumberOfInfos()
        {
            return InfoList.Count;
        }

        public string PrintReport()
        {
            StringBuilder strB = new StringBuilder();
            strB.AppendLine("There are " + ErrorList.Count + " errors and " + WarningList.Count + " warnings and " + this.GetNumberOfInfos() + " infos.");
            strB.AppendLine(" ");
            strB.AppendLine("----------------------------------------------------");
            strB.AppendLine(string.Join(Environment.NewLine, ErrorList));
            strB.AppendLine("----------------------------------------------------");
            strB.AppendLine(string.Join(Environment.NewLine, WarningList));
            strB.AppendLine("----------------------------------------------------");
            strB.AppendLine(string.Join(Environment.NewLine, InfoList));
            strB.AppendLine("----------------------------------------------------");

            return strB.ToString();
        }




        
    }
}
