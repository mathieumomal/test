using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.DataAccess;
using domain = Etsi.Ultimate.DomainClasses;
using DatabaseImport.ModuleImport;

namespace DatabaseImport
{
    class Program
    {
        static void Main(string[] args)
        {
            var newContext = new UltimateContext();
            var oldContext = new TmpDB();
            var report = new ImportReport();


            // CONFIGURE HERE
            var operations = new List<IModuleImport>();
            operations.Add(new ReleaseImport());

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
            Console.WriteLine(report.PrintReport());

            Console.Read();
        }
    }
}
