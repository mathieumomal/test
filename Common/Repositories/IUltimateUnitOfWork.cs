using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// This interface represents a shared context that can be used by several repositories,
    /// in order to have common data persistence.
    /// </summary>
    public interface IUltimateUnitOfWork : IDisposable
    {
        /// <summary>
        /// The Context that is commonly shared.
        /// </summary>
        IUltimateContext Context { get; }

        /// <summary>
        /// Performs a save of the modifications in context inside the database.
        /// </summary>
        void Save();
    }
}
