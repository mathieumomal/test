using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications.Interfaces
{
    /// <summary>
    /// Interface for the technologies related to specifications
    /// </summary>
    public interface ISpecificationTechnologiesManager
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Returns all specification technologies
        /// </summary>
        /// <returns></returns>
        List<Enum_Technology> GetAllSpecificationTechnologies();

        /// <summary>
        /// Return a technology based on its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Enum_Technology> GetASpecificationTechnologiesBySpecId(int id);
    }
}
