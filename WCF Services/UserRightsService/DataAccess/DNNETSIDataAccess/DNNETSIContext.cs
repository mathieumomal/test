using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.UserRights.DNNETSIDataAccess
{
    public partial class DNNETSIContext : DbContext, IDnnEtsiContext
    {
        public DNNETSIContext(EntityConnection connection)
            : base(connection, true)
        {

        }
    }
}
