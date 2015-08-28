using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Generators.Parameters;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a DTO to generate for a specific Complex class type.
    /// </summary>
    internal class DTOComplex : DTOClass
    {
        /// <summary>
        /// Creates an instance of <see cref="DTOComplex"/>.
        /// </summary>
        /// <param name="typeNode">Type node.</param>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        public DTOComplex(XElement typeNode, GenerateDTOsParams genParams)
            : base(typeNode, genParams)
        {

        }

    }
}