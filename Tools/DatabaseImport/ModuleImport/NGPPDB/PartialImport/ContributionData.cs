using System.Collections.Generic;
using DatabaseImport.ModuleImport.NGPPDB.Contribution;

namespace DatabaseImport.ModuleImport.NGPPDB.PartialImport
{
    public class ContributionData
    {
        public readonly List<IModuleImport> Operations;

        public ContributionData()
        {
            Operations = new List<IModuleImport>
            {
                new ContributionImport()
            };
        }
    }
}
