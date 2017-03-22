using System.Collections.Generic;
using Etsi.Ultimate.WCF.Interface;

namespace Etsi.Ultimate.WCF.Service.Converters
{
    public class ServiceResponseConverter
    {
        public static ServiceResponse<T> ConvertUltimateObjectToServiceObject<T>(DomainClasses.ServiceResponse<T> obj)
        {
            var serviceReport = new ServiceReport { ErrorList = new List<string>(), InfoList = new List<string>(), WarningList = new List<string>() };
            var svcResponse = new ServiceResponse<T> { Report = serviceReport };

            if (obj != null)
            {
                svcResponse.Result = obj.Result;
                svcResponse.Report.ErrorList = obj.Report.ErrorList;
                svcResponse.Report.WarningList = obj.Report.WarningList;
                svcResponse.Report.InfoList = obj.Report.InfoList;
            }

            return svcResponse;
        }
    }
}