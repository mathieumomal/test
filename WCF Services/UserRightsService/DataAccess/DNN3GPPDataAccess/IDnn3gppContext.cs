using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Etsi.UserRights.DNN3GPPDataAccess
{
    public interface IDnn3gppContext: IDisposable
    {
        DbSet<ProfilePropertyDefinition> ProfilePropertyDefinitions { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
    }
}
