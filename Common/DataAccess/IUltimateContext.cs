using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.DataAccess
{
    public interface IUltimateContext : IDisposable
    {

        IDbSet<Enum_ReleaseStatus> Enum_ReleaseStatus { get; }
        IDbSet<Release> Releases { get; }
        IDbSet<Meeting> Meetings { get; }
        IDbSet<Remark> Remarks { get; }
        IDbSet<Users_Groups> Users_Groups { get; }
        IDbSet<Users_AdHoc_Roles> Users_AdHoc_Roles { get; }

        void SetModified(object entity);
        void SetAdded(object entity);
        int SaveChanges();
    }
}
