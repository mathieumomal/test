
using System.Linq;
using Etsi.Ngppdb.DomainClasses;
namespace DatabaseImportTests.FakeDBSets
{
    public class EnumContributionStatusFakeDbSet : FakeDBSet<Enum_ContributionStatus>
    {
        public override Enum_ContributionStatus Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.pk_Enum_ContributionStatus == keyValue);
        }
    }
}
