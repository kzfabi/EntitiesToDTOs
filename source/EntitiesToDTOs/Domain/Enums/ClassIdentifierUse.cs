using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Domain.Enums
{
    /// <summary>
    /// Provides the available uses of the identifier for generated class names.
    /// </summary>
    internal enum ClassIdentifierUse
    {
        /// <summary>
        /// Do not use identifier.
        /// </summary>
        None,

        /// <summary>
        /// Use identifier as prefix in dto class name.
        /// </summary>
        Prefix,

        /// <summary>
        /// Use identifier as suffix in dto class name.
        /// </summary>
        Suffix
    }
}