using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport.ModuleImport
{
    public class ImportReport
    {

        private List<string> errorList;
        private List<string> warningList;

        string CurrentModule { get; set; }


        public ImportReport()
        {
            errorList = new List<string>();
            warningList = new List<string>();
        }

        public void LogWarning(string msg)
        {
            warningList.Add(msg);
        }
        public void LogWarning(string msg, string table)
        {
            warningList.Add("[" + table + "] " + msg);
        }

        public void LogError(string msg)
        {
            errorList.Add(msg);
        }
        public void LogError(string msg, string table)
        {
            errorList.Add("[" + table + "] " + msg);

        }


        public string PrintReport()
        {
            StringBuilder strB = new StringBuilder();
            strB.AppendLine("There are " + errorList.Count + " errors and " + warningList.Count + " warnings.");
            strB.AppendLine(" ");
            strB.AppendLine("----------------------------------------------------");
            strB.AppendLine(string.Join(Environment.NewLine, errorList));
            strB.AppendLine("----------------------------------------------------");
            strB.AppendLine(string.Join(Environment.NewLine, warningList));
            strB.AppendLine("----------------------------------------------------");

            return strB.ToString();
        }




        
    }
}
