using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
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
    }
}
