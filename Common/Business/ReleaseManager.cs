using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    class ReleaseManager
    {
        public List<Release> GetAllReleases()
        {
            ReleaseRepository repo = new ReleaseRepository();
            return repo.AllIncluding(t => t.Enum_ReleaseStatus).ToList();
        }
    }
}
