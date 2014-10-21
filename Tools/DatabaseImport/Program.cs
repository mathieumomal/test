using System;
using System.Collections.Generic;
using DatabaseImport.ModuleImport.CR;
using DatabaseImport.PartialImport;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.DataAccess;
using domain = Etsi.Ultimate.DomainClasses;
using DatabaseImport.ModuleImport;
using Etsi.Ultimate.DomainClasses;
using System.IO;

namespace DatabaseImport
{
    static class Program
    {
        private const string LogPath = "../../../import.log";
        static void Main()
        {
            var newContext = new UltimateContext();
            var oldContext = new TmpDB();
            var report = new Report();
            List<IModuleImport> operations;

            Console.WriteLine("Which data would you import ?");
            Console.WriteLine("Enter 'A' for All next data");
            Console.WriteLine("Enter 'B' for Base U3GPPDB data (releases, specs, ...)");
            Console.WriteLine("Enter 'C' for Contribution data (CR, TDoc,...)");
            var response = Console.Read();
            switch (response)
            {
                case 'A':
                    Console.WriteLine("Import ALL data...");
                    operations = new List<IModuleImport>();
                    operations.AddRange(new BaseU3GppdbData().Operations);
                    operations.AddRange(new ContributionData().Operations);
                    break;
                case 'B':
                    Console.WriteLine("Import base U3GPPDB data...");
                    operations = new BaseU3GppdbData().Operations;
                    break;
                case 'C':
                    Console.WriteLine("Import contribution data...");
                    operations = new ContributionData().Operations;
                    break;
                default:
                    return;
            }

            // SET UP all contexts and reports
            foreach (var import in operations)
            {
                import.LegacyContext = oldContext;
                import.NewContext = newContext;
                import.Report = report;
            }

            Console.WriteLine("Cleaning database...");

            // CLEAN UP in reverse order
            operations.Reverse();
            foreach (var toClean in operations)
            {
                toClean.CleanDatabase();
            }

            Console.WriteLine("Filling in database...");

            // Load in normal order
            operations.Reverse();
            foreach (var toImport in operations)
            {
                toImport.FillDatabase();
            }


            Console.WriteLine("Saving changes...");
            // Save new context
            newContext.SaveChanges();

            Console.WriteLine("Changes saved, import is now finished!");

            Console.WriteLine("------------------- REPORT ---------------------");
            File.WriteAllText(LogPath, report.PrintReport());
            Console.WriteLine("You could find the log file : 'import.log' in " + LogPath);

            Console.Read();
            Console.Read();
            Console.Read();

        }
    }
}
