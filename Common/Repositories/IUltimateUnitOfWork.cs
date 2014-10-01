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
        /// Cuts of the "auto-detect" change feature that the Context might use.
        /// </summary>
        /// <param name="detect"></param>
        void SetAutoDetectChanges(bool detect);

        /// <summary>
        /// Performs a save of the modifications in context inside the database.
        /// </summary>
        void Save();

        /// <summary>
        /// Set entity state to deleted
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="Entity">Entity</param>
        void MarkDeleted<T>(T Entity);  
    }
}
