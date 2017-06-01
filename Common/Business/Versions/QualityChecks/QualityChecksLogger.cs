using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business.Versions.QualityChecks
{
    public class QualityChecksLogger
    {
        public const string QualityChecksLogActionName = "#QC: ";

        public string QualityCheckLogUid { get; set; }
        public string Param { get; set; }
        public bool ExpectedResult { get; set; }

        public QualityChecksLogger(string qualityCheckName, object obj, bool expectedResult)
        {
            QualityCheckLogUid = QualityChecksLogActionName + "[" + qualityCheckName + "]    ";
            Param = obj == null ? "NO PARAM" : obj.ToString();
            ExpectedResult = expectedResult;

            LogManager.Debug(QualityCheckLogUid + "entry parameter -> " + Param);
        }

        public bool QcStopping(bool result)
        {
            if (ExpectedResult == result)
            {
                LogManager.Info(" " + QualityCheckLogUid + "SUCCESS RESULT ==> is a success for the current version.");
            }
            else
            {
                LogManager.Warn(" " + QualityCheckLogUid + "ERROR RESULT ==> failed for the current version.");
            }
            return result;
        }

        public QualityChecksLogger Log(string message)
        {
            LogManager.Debug(QualityCheckLogUid + "MESSAGE: " + message);
            return this;
        }
    }
}
