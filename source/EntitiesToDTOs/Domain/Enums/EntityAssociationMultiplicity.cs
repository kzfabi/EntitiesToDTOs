/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
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