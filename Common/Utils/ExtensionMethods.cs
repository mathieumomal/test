using System.Collections.Generic;

namespace Etsi.Ultimate.Utils
{
    public static class ExtensionMethods
    {

        public static string Remove3GppAtTheBeginningOfAString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Trim().ToUpper() == "3GPP")
                return str.Trim();

            var checkStr = str.Trim().ToUpper();
            if (checkStr.StartsWith("3GPP"))
            {
                str = str.Trim().Remove(0, 4).Trim();
            }
            return str;
        }

        public static string Remove3GppInsideListOfElementsComaSeparated(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            var arrayOfsecondaryResponsibleGroups = str.Split(',');
            var finalList = new List<string>();
            for (var i = 0; i < arrayOfsecondaryResponsibleGroups.Length; i++)
            {
                if (string.IsNullOrEmpty(arrayOfsecondaryResponsibleGroups[i]))
                {
                    continue;
                }
                finalList.Add(arrayOfsecondaryResponsibleGroups[i].Remove3GppAtTheBeginningOfAString().Trim());
            }
            return string.Join(", ", finalList);
        }
    }
}
