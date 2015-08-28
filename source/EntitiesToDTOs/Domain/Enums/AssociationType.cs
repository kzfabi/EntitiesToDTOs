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
    /// Associations types between DTOs.
    /// </summary>
    internal enum AssociationType
    {
        /// <summary>
        /// Associate by Entity Key Property.
        /// </summary>
        KeyProperty,

        /// <summary>
        /// Associate by Entity Class Type.
        /// </summary>
        ClassType
    }
}