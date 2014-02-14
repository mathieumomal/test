using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport.ModuleImport
{
    interface IModuleImport
    {

        DataReport Report { get; set; }

        void CleanDatabase();

        void FillDatabase();
    }
}
