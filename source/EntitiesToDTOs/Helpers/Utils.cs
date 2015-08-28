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
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Domain.Enums;
using EntitiesToDTOs.Generators.Parameters;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Utilities.
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// Sets the first letter of the received text to lowercase.
        /// </summary>
        /// <param name="text">Text to format.</param>
        /// <returns></returns>
        public static string SetFirstLetterLowercase(string text)
        {
            return (Char.ToLower(text[0]) + text.Substring(1));
        }

        /// <summary>
        /// Constructs a dto name using the entity name and configuration received.
        /// </summary>
        /// <param name="entityName">Entity name.</param>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        public static string ConstructDTOName(string entityName, GenerateDTOsParams genParams)
        {
            if (string.IsNullOrWhiteSpace(entityName) == true)
            {
                return null;
            }

            string dtoName = entityName;

            if (genParams.ClassIdentifierUse == ClassIdentifierUse.Prefix)
            {
                dtoName = (genParams.ClassIdentifierWord + dtoName);
            }
            else if (genParams.ClassIdentifierUse == ClassIdentifierUse.Suffix)
            {
                dtoName = (dtoName + genParams.ClassIdentifierWord);
            }

            return dtoName;
        }

        /// <summary>
        /// Constructs an assembler name using the entity name and configuration received.
        /// </summary>
        /// <param name="entityName">Entity name.</param>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        public static string ConstructAssemblerName(string entityName, GenerateAssemblersParams genParams)
        {
            if (string.IsNullOrWhiteSpace(entityName) == true)
            {
                return null;
            }

            string assemblerName = entityName;

            if (genParams.ClassIdentifierUse == ClassIdentifierUse.Prefix)
            {
                assemblerName = (genParams.ClassIdentifierWord + assemblerName);
            }
            else if (genParams.ClassIdentifierUse == ClassIdentifierUse.Suffix)
            {
                assemblerName = (assemblerName + genParams.ClassIdentifierWord);
            }

            return assemblerName;
        }

    }
}