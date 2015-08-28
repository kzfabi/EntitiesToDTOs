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