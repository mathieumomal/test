using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests
{
    class FakeContext: IUltimateContext
    {

        #region IUltimateContext Members

        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Enum_ReleaseStatus> Enum_ReleaseStatus
        { get; set; }

        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Release> Releases
        { get; set; }

        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Meeting> Meetings
        { get; set; }

        public System.Data.Entity.IDbSet<Ultimate.DomainClasses.Remark> Remarks
        { get; set; }

        public void SetModified(object entity)
        {
            Releases.Add((Release)entity);
        }

        public void SetAdded(object entity)
        {
            if (entity.GetType() == typeof(Release))
                Releases.Add((Release)entity);
            else if (entity.GetType() == typeof(Meeting))
                Meetings.Add((Meeting)entity);
            else if (entity.GetType() == typeof(Remark))
                Remarks.Add((Remark)entity);
            else if (entity.GetType() == typeof(Enum_ReleaseStatus))
                Enum_ReleaseStatus.Add((Enum_ReleaseStatus)entity);

            else
                throw new NotImplementedException("Something is missing in the FakeContext");
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
