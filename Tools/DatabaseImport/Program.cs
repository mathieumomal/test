using System;
using System.Collections.Generic;
using DatabaseImport.ModuleImport.NGPPDB.PartialImport;
using DatabaseImport.ModuleImport.U3GPPDB.PartialImport;
using Etsi.Ngppdb.DataAccess;
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
            var u3GppContext = new UltimateContext();
            var ngppContext = new NGPPDBContext();
            var oldContext = new TmpDB();
            var report = new Report();
            List<IModuleImport> operations;

            Console.WriteLine("Which data would you import ?");
            Console.WriteLine("Enter 'A' for All next data (inside U3GPPDB & NGPPDB)");
            Console.WriteLine("Enter 'B' for Base U3GPPDB data (releases, specs, ...)");
            Console.WriteLine("Enter 'C' for CR data (CR,... inside U3GPPDB)");
            Console.WriteLine("Enter 'N' for Ngppdb data (Contribution inside NGPPDB)");
            var responsePartialData = Console.Read();
            switch (responsePartialData)
            {
                case 'A':
                    Console.WriteLine("Import ALL data (U3GPPDB & NGPPDB)...");
                    operations = new List<IModuleImport>();
                    operations.AddRange(new BaseU3GppdbData().Operations);
                    operations.AddRange(new ChangeRequestData().Operations);
                    operations.AddRange(new ContributionData().Operations);
                    break;
                case 'B':
                    Console.WriteLine("Import base U3GPPDB data (U3GPPDB)...");
                    operations = new BaseU3GppdbData().Operations;
                    break;
                case 'C':
                    Console.WriteLine("Import CR data (U3GPPDB)...");
                    operations = new ChangeRequestData().Operations;
                    break;
                case 'N':
                    Console.WriteLine("Import Contribution data (NGPPDB)...");
                    operations = new ContributionData().Operations;
                    break;
                default:
                    return;
            }

            // SET UP all contexts and reports
            var meetingHelper = new MeetingHelper(oldContext, u3GppContext);
            foreach (var import in operations)
            {
                import.LegacyContext = oldContext;
                import.UltimateContext = u3GppContext;
                import.NgppdbContext = ngppContext;
                import.MtgHelper = meetingHelper;
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
            u3GppContext.SaveChanges();
            ngppContext.SaveChanges();

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
