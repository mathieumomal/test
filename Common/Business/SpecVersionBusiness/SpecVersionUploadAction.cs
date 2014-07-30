using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionUploadAction
    {
        public IUltimateUnitOfWork UoW;

        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path)
        {
            var svcResponse = new ServiceResponse<string>();

            svcResponse.Report = CheckPersonRightToUploadVersion(svcResponse.Report, personId);
            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                //...
            }
            return svcResponse;
        }

        public ServiceResponse<string> UploadVersion(int personId, SpecVersion version, string token)
        {
            var svcResponse = new ServiceResponse<string>();

            svcResponse.Report = CheckPersonRightToUploadVersion(svcResponse.Report, personId);
            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                //...
            }
            return svcResponse;
        }

        private Report CheckPersonRightToUploadVersion(Report report, int personId)
        {
            IRightsManager rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);

            if (!rights.HasRight(Enum_UserRights.Versions_Upload))
                report.LogError(Utils.Localization.RightError);
            return report;
        }
    }
}
