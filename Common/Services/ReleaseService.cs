using System;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class ReleaseService : IReleaseService
    {

        #region IReleaseService Membres

        public List<DomainClasses.Release> GetAllReleases()
        {
            //Need to initialize the context

            //Need to close the context
            Enum_ReleaseStatus status = new Enum_ReleaseStatus();
            status.ReleaseStatus = "FREEZE";
            List<DomainClasses.Release> releases = new List<DomainClasses.Release>();
            releases.Add(new DomainClasses.Release() { Pk_ReleaseId = 1, Name = "First release", Fk_ReleaseStatus = 1, StartDate = new DateTime(2010, 1, 18), Enum_ReleaseStatus = status });
            return releases;
        }

        #endregion
    }
}
