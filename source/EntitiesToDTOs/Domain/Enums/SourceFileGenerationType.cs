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
    /// Source file generation types. 
    /// </summary>
    internal enum SourceFileGenerationType
    {
        /// <summary>
        /// Indicates to generate one Source File per Class.
        /// </summary>
        SourceFilePerClass,
        
        /// <summary>
        /// Indicates to generate only one Source File.
        /// </summary>
        OneSourceFile
    }
}