using System.Linq;
using Etsi.Ngppdb.DomainClasses;

namespace DatabaseImportTests.FakeDBSets
{
    public class ContributionFakeDbSet : FakeDBSet<Contribution>
    {
        public override Contribution Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(r => r.pk_Contribution == keyValue);
        }
    }
}
