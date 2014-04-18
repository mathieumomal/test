using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface ISpecificationService
    {
        /// <summary>
        /// Returns A specification details including related remarks, history, Parent/Child specification, and releases
        /// </summary>
        /// <param name="specificationId">The identifier of the requested specification</param>
        KeyValuePair<Specification, UserRightsContainer>  GetSpecificationDetailsById(int personId, int specificationId);
    }
}
