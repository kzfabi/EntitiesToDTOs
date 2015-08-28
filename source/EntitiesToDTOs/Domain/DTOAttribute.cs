using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Attribute of a DTO.
    /// </summary>
    internal class DTOAttribute
    {
        /// <summary>
        /// Attribute Name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Attribute parameters.
        /// </summary>
        public string Parameters { get; private set; }


        public DTOAttribute(string name, string parameters)
        {
            this.Name = name;
            this.Parameters = parameters;
        }
    }
}