using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Versions.Interfaces
{
    /// <summary>
    /// The ITranspositionManager is in charge of sending to ETSI a version of a specification for transposition.
    /// </summary>
    public interface ITranspositionManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Sends the version to ETSI for transposition.
        /// </summary>
        /// <param name="spec">Specification for which transposition is required</param>
        /// <param name="version">Version to transpose.</param>
        /// <returns></returns>
        bool Transpose(Specification spec, SpecVersion version);


        /// <summary>
        /// Check if the transposition is allowed
        /// </summary>
        /// <returns></returns>
        bool TransposeAllowed(SpecVersion specVersion);
    }
}
