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

        public KeyValuePair<string, Report> CheckVersionForUpload(int personId, SpecVersion version, string path)
        {
            var report = new Report();

            report = CheckPersonRightToUploadVersion(report, personId);
            if (report.GetNumberOfErrors() == 0)
            {
                //...
            }

            var result = new KeyValuePair<string, Report>("token", report);
            return result;
        }

        public KeyValuePair<string, Report> UploadVersion(int personId, SpecVersion version, string token)
        {
            var report = new Report();

            report = CheckPersonRightToUploadVersion(report, personId);
            if (report.GetNumberOfErrors() == 0)
            {
                //...
            }

            var result = new KeyValuePair<string, Report>("token", report);
            return result;
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
