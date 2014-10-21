using System.Collections.Generic;
using DatabaseImport.ModuleImport.U3GPPDB.CR;

namespace DatabaseImport.ModuleImport.U3GPPDB.PartialImport
{
    public class ChangeRequestData
    {
        public readonly List<IModuleImport> Operations;

        public ChangeRequestData()
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
