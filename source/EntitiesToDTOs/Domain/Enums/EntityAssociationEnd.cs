using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Domain.Enums
{
    /// <summary>
    /// Provides the types of Association End desired.
    /// </summary>
    internal enum EntityAssociationEnd
    {
        /// <summary>
        /// Get the First Association End.
        /// </summary>
        First,

        /// <summary>
        /// Get the Second Association End.
        /// </summary>
        Second
    }
}