using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DataAccess
{
    public partial class UltimateContext : DbContext, IUltimateContext
    {
        public UltimateContext(EntityConnection connection)
            : base(connection, true)
        {
        }

        /// <summary>
        /// Gets the entities based on the given entity state.
        /// </summary>
        /// <param name="entityState">State of the entity.</param>
        /// <returns>Entities</returns>
        public IEnumerable<System.Data.Entity.Core.Objects.ObjectStateEntry> GetEntities<T>(EntityState entityState)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntries(entityState).Where(x => x.Entity.GetType() == typeof(T));
        }
    }
}
