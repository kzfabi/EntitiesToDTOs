using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Domain.Enums
{
    /// <summary>
    /// Provides the types of Association Multiplicity.
    /// </summary>
    internal enum EntityAssociationMultiplicity
    {
        /// <summary>
        /// Zero or One
        /// </summary>
        ZeroOrOne,

        /// <summary>
        /// One
        /// </summary>
        One,

        /// <summary>
        /// Many
        /// </summary>
        Many
    }
}