using System.Collections.Generic;
using DatabaseImport.ModuleImport;
using DatabaseImport.ModuleImport.CR;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.PartialImport
{
    public class ContributionData
    {
        public readonly List<IModuleImport> Operations;

        public ContributionData()
        {
            Operations = new List<IModuleImport>
            {
                //new EnumChangeRequestStatusImport() !!! Not imported (need manual import) !!!
                new EnumCrCategoryImport(),
                //new Enum_CRImpactImport(), not taking in consideration for the moment
                new CrImport(),
                //new TDocImport(),

            };
        }
    }
}
