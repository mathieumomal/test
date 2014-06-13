using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.DataAccess;
using domain = Etsi.Ultimate.DomainClasses;
using DatabaseImport.ModuleImport;
using Etsi.Ultimate.DomainClasses;
using System.IO;

namespace DatabaseImport
{
    class Program
    {
        private const string logPath = "../../../import.log";
        static void Main(string[] args)
        {
            var newContext = new UltimateContext();

            var oldContext = new TmpDB();
            var report = new Report();


            // CONFIGURE HERE
            var operations = new List<IModuleImport>();
            
            //---> (First to FILLDATABASE)
            
            /*operations.Add(new ReleaseImport());
            operations.Add(new WorkItemImport());
            operations.Add(new Enum_SerieImport());
            operations.Add(new Enum_TechnologyImport());
            operations.Add(new SpecificationImport());//+SpecificationTechnologies
            operations.Add(new SpecificationResponsibleGroupImport());
            operations.Add(new SpecificationsGenealogyImport());
            operations.Add(new SpecificationRapporteurImport());
            operations.Add(new SpecificationWorkitemImport());
            operations.Add(new SpecificationReleaseImport());
            operations.Add(new VersionImport());*/

            operations.Add(new Enum_CRCategoryImport());
            operations.Add(new Enum_TDocStatusImport());
            operations.Add(new Enum_CRImpactImport());
            
            //---> (First to CLEANDATABASE)

            Console.WriteLine("Setting up the different classes");

            // set up all contexts and reports
            foreach (var import in operations)
            {
                import.LegacyContext = oldContext;
                import.NewContext = newContext;
                import.Report = report;
            }

            Console.WriteLine("Cleaning database");

            // Clean up in reverse order
            operations.Reverse();
            foreach (var toClean in operations)
            {
                toClean.CleanDatabase();
            }

            Console.WriteLine("Filling in database");

            // Load in normal order
            operations.Reverse();
            foreach (var toImport in operations)
            {
                toImport.FillDatabase();
            }

            Console.WriteLine("Saving changes");
            // Save new context
            newContext.SaveChanges();

            Console.WriteLine("Changes saved, import is now finished!");

            Console.WriteLine("------------------- REPORT ---------------------");
            File.WriteAllText(logPath, report.PrintReport());
            Console.WriteLine("You could find the log file : 'import.log' in " + logPath);          
            
            Console.Read();
            Console.Read();
            Console.Read();
        }
    }
}
