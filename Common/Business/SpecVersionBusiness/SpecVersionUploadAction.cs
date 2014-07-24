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

        public KeyValuePair<int, Report> UploadVersion(int personId, SpecVersion version)
        {
            var report = new Report();

            IRightsManager rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);

            if (rights.HasRight(Enum_UserRights.Versions_Upload))
            {

            }
            else
            {
                report.LogError(Utils.Localization.RightError);
            }

            var result = new KeyValuePair<int, Report>(1, report);
            return result;
        }
    }
}
