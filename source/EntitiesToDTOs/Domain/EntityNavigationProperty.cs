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

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Entity Navigation Property.
    /// </summary>
    internal class EntityNavigationProperty
    {
        /// <summary>
        /// Property Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Property Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if this property is a List.
        /// </summary>
        public bool IsList { get; set; }

        /// <summary>
        /// Indicates the type of objects contained if it is a List. Same as Type if not.
        /// </summary>
        public string ListOf { get; set; }

        /// <summary>
        /// Entity Target Name.
        /// </summary>
        public string EntityTargetName { get; private set; }

        /// <summary>
        /// DTO Target Name.
        /// </summary>
        public string DTOTargetName { get; private set; }


        public EntityNavigationProperty(string type, string name, bool isList, string listOf, 
            string entityTargetName, string dtoTargetName)
        {
            this.Type = type;
            this.Name = name;
            this.IsList = isList;

            this.ListOf = this.Type;

            if (this.IsList)
            {
                this.ListOf = listOf;
            }

            this.EntityTargetName = entityTargetName;
            this.DTOTargetName = dtoTargetName;
        }
    }
}