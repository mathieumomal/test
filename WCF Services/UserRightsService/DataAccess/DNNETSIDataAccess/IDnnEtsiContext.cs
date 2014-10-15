using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.UserRights.DNNETSIDataAccess
{
    public interface IDnnEtsiContext
    {
        DbSet<ProfilePropertyDefinition> ProfilePropertyDefinitions { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
    }
}
