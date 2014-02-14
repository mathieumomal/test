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
        IDbSet<Release> Release { get; }
        IDbSet<View_Meetings> View_Meetings { get; }

        void SetModified(object entity);
        void SetAdded(object entity);
        int SaveChanges();
    }
}
