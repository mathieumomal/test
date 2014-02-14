using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.DataAccess;
using domain = Etsi.Ultimate.DomainClasses;

namespace DatabaseImport
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var context = new UltimateContext())
            {
                // Delete all releases
                foreach (var rel in context.Release.ToList())
                {
                    context.Release.Remove(rel);
                    Console.WriteLine("Removed " + rel.Name);
                }


                // Let's just create one release for each existing release in the tmp database
                var tmpContext = new TmpDB();
                var allTmpRelease = tmpContext.Releases;

                foreach (var aRelease in allTmpRelease)
                {
                    var aNewRelease = new domain.Release()
                    {
                        Name = aRelease.Release_description,
                        ShortName = aRelease.Release_short_description
                    };

                    Console.WriteLine("Added " + aNewRelease.Name);
                    context.Release.Add(aNewRelease);
                }

                context.SaveChanges();
                Console.WriteLine("Export is finished!");
                Console.Read();
            }

        }
    }
}
