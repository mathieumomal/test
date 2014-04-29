
namespace Etsi.Ultimate.DomainClasses
{
    public partial class Community
    {
        public int ParentCommunityId { get { return ParentTbId ?? 0; } }
        public int Order { get; set; }
    }
}
