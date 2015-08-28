/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using System;
using System.Text;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a type of the C# Language.
    /// </summary>
    internal class CSharpType
    {
        /// <summary>
        /// Name of the type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates if the type can be declared as nullable.
        /// </summary>
        public bool AllowsNullable { get; private set; }


        public CSharpType(string name, bool allowsNullable)
        {
            this.Name = name;
            this.AllowsNullable = allowsNullable;
        }
    }
}