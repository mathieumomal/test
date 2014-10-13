using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.UserRights.DNN3GPPDataAccess
{
    public partial class DNN3GPPContext : DbContext, IDnn3gppContext
    {
        public DNN3GPPContext(EntityConnection connection)
            : base(connection, true)
        {

        }
    }
}
