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