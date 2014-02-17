using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tools.TmpDbDataAccess;

namespace DatabaseImport.ModuleImport
{
    interface IModuleImport
    {
        IUltimateContext NewContext { get; set; }
        ITmpDb LegacyContext { get; set; }

        ImportReport Report { get; set; }

        void CleanDatabase();

        void FillDatabase();
    }
}
