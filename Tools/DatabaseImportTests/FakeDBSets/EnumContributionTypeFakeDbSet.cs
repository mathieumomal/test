using System.Linq;
using Etsi.Ngppdb.DomainClasses;

namespace DatabaseImportTests.FakeDBSets
{
    public class EnumContributionTypeFakeDbSet : FakeDBSet<Enum_ContributionType>
    {
        public override Enum_ContributionType Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(p => p.pk_Enum_ContributionType == keyValue);
        }
    }
}
