﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// The ITranspositionManager is in charge of sending to ETSI a version of a specification for transposition.
    /// </summary>
    public interface ITranspositionManager
    {
        /// <summary>
        /// Sends the version to ETSI for transposition.
        /// </summary>
        /// <param name="spec">Specification for which transposition is required</param>
        /// <param name="version">Version to transpose.</param>
        /// <returns></returns>
        bool Transpose(Specification spec, SpecVersion version);
    }
}