using System.Collections.Generic;
using DatabaseImport.ModuleImport;

namespace DatabaseImport.PartialImport
{
    public class BaseU3GppdbData
    {
        public readonly List<IModuleImport> Operations;

        public BaseU3GppdbData()
        {
            Operations = new List<IModuleImport>
            {
                //new ReleaseImport(),
                //new WorkItemImport(),
                //new Enum_SerieImport(),
                //new Enum_TechnologyImport(),
                //new SpecificationImport(),
                //new SpecificationResponsibleGroupImport(),
                //new SpecificationsGenealogyImport(),
                //new SpecificationRapporteurImport(),
                //new SpecificationWorkitemImport(),
                //new SpecificationReleaseImport(),
                //new VersionImport(),
            };
        }
    }
}
