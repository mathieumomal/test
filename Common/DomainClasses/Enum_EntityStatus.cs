
namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// Status flags to identify the object state
    /// New - New Entity
    /// Modified - Entity Modified
    /// Deleted - Entity Deleted
    /// Unchanged - Entity Unchanged
    /// </summary>
    public enum Enum_EntityStatus
    {
        Unchanged = 0, //Default Status
        New,
        Modified,
        Deleted
    }
}
