using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport.ModuleImport
{
    class DataReport
    {

        private List<string> errorList;
        private List<string> warningList;

        string CurrentModule { get; set; }


        public DataReport()
        {
            errorList = new List<string>();
            warningList = new List<string>();
        }

        void LogWarning(string msg)
        {
            warningList.Add(msg);
        }
        void LogWarning(string msg, string table)
        {
            warningList.Add("[" + table + "] " + msg);
        }

        void LogError(string msg)
        {
            errorList.Add(msg);
        }
        void LogError(string msg, string table)
        {
            errorList.Add("[" + table + "] " + msg);

        }


        string PrintReport()
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
