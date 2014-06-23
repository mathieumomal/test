using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport
{
    public class Utils
    {
        /// <summary>
        /// Convert bool? to bool and delivered a warning message, if the default value is applied, which contains a specific message that we could, partialy, defined (logDescriptionCase)
        /// </summary>
        /// <param name="boo"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public static bool NullBooleanCheck(bool? boo, string logDescriptionCase, bool defaultValue, Report report)
        {
            if (!boo.HasValue)
            {
                report.LogWarning(logDescriptionCase + " not defined as true or false. By default convert to " + defaultValue.ToString() + ".");
                boo = defaultValue;
            }
            return (bool)boo;
        }

        /// <summary>
        /// Check string length + check null + Trim
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lenght"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string CheckString(string str, int lenght, string logDescriptionCase, string id, Report report)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
            }
            else if (lenght != 0 && str.Length > lenght)
            {
                report.LogWarning("String validation error : " + logDescriptionCase + " : string too long for : " + id);
                return "";
            }
            return str.Trim();
        }

        /// <summary>
        /// Check int validity
        /// </summary>
        /// <param name="val"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="id"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static int CheckInt(int? val, string logDescriptionCase, string id, Report report)
        {
            if (val == null)
            {
                report.LogWarning(logDescriptionCase + " : int null for : " + id);
                return 0;
            }
                
            return val.Value;
        }

        /// <summary>
        /// Convert an int to a string
        /// </summary>
        /// <param name="val"></param>
        /// <param name="lenght"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="id"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static string CheckIntToString(int? val, int lenght, string logDescriptionCase, string id, Report report)
        {
            if (val != null)
            {
                string str = val.ToString();

                if (String.IsNullOrEmpty(str))
                {
                    return "";
                }
                else if (lenght != 0 && str.Length > lenght)
                {
                    report.LogWarning("Convert int to string error : " + logDescriptionCase + " : string too long for : " + id);
                    return "";
                }
                return str.Trim();
            }
            report.LogWarning(logDescriptionCase + " : int null for : " + id);
            return "";
        }

        /// <summary>
        /// Convert a string to an int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="id"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static int? CheckStringToInt(string str, int? defaultValue, string logDescriptionCase, string id, Report report)
        {
            int returnValue = 0;
            string checkStr = CheckString(str, 0, " (During ChechStringToInt) ", id, report);

            bool success = int.TryParse(str, out returnValue);
            if (!success)
            {
                report.LogWarning("Convert string to int error : " + logDescriptionCase + " (return by default "+defaultValue+") - id : " + id);
                return defaultValue;
            }
            return returnValue;
        }

    }
}
