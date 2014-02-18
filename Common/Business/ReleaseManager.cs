using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public class ReleaseManager
    {
        public IUltimateUnitOfWork UoW {get; set;}
        
        public ReleaseManager() { }


        /// <summary>
        /// Retrieves all the data for the releases.
        /// </summary>
        /// <returns></returns>
        public List<Release> GetAllReleases()
        {
            IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
            return repo.All.ToList();
        }


    }
}
