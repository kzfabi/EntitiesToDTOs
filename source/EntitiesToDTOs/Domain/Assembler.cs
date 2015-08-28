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
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Assembler to generate.
    /// </summary>
    internal class Assembler
    {
        /// <summary>
        /// The name of this Assembler.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// DTO class reference.
        /// </summary>
        public DTOClass DTO { get; set; }


        public Assembler(DTOClass dtoClass, string name)
        {
            this.DTO = dtoClass;
            this.Name = name;
        }


        /// <summary>
        /// Gets the Assembler code.
        /// </summary>
        /// <param name="toDTOInstanceCode">ToDTO method instance creation code.</param>
        /// <param name="toEntityInstanceCode">ToEntity method instance creation code.</param>
        /// <param name="toDTOAssignmentsCode">ToDTO method assignments code.</param>
        /// <param name="toEntityAssignmentsCode">ToEntity method assignments code.</param>
        public string GetAssemblerCode(string toDTOInstanceCode, string toEntityInstanceCode, 
            string toDTOAssignmentsCode, string toEntityAssignmentsCode)
        {
            return Resources.Assembler_cs_template
                .Replace(Resources.AssemblerName_Wildcard, this.Name)
                .Replace(Resources.AssemblerEntityName_Wildcard, this.DTO.Name)
                .Replace(Resources.AssemblerDTO_Wildcard, this.DTO.NameDTO)
                .Replace(Resources.AssemblerAccess_Wildcard, Resources.AssemblerAccess)
                .Replace(Resources.AssemblerBaseName_Wildcard, Resources.AssemblerBaseName)
                .Replace(Resources.AssemblerToDTOInstance_Wildcard, toDTOInstanceCode)
                .Replace(Resources.AssemblerToDTOAssignments_Wildcard, toDTOAssignmentsCode)
                .Replace(Resources.AssemblerToEntityInstance_Wildcard, toEntityInstanceCode)
                .Replace(Resources.AssemblerToEntityAssignments_Wildcard, toEntityAssignmentsCode);
        }
    }
}