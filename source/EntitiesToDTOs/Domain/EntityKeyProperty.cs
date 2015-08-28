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
using System.Xml.Linq;
using EnvDTE;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Generators.Parameters;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Entity Key Property.
    /// </summary>
    internal class EntityKeyProperty
    {
        /// <summary>
        /// DTO Name Owner of the property.
        /// </summary>
        public string DTOName { get; set; }

        /// <summary>
        /// Property Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Property Name.
        /// </summary>
        public string Name { get; set; }



        /// <summary>
        /// Creates an instance of <see cref="EntityKeyProperty"/>.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        public EntityKeyProperty(XElement entityNode, XElement propertyRefNode, 
            IEnumerable<XElement> propertyNodeElements, GenerateDTOsParams genParams)
        {
            // Get the Entity name
            string entityName = entityNode.Attribute(EdmxNodeAttributes.EntityType_Name).Value;

            // Set the DTO name
            this.DTOName = Utils.ConstructDTOName(entityName, genParams);

            // Set the Property name
            string propertyNameDesired = propertyRefNode.Attribute(EdmxNodeAttributes.PropertyRef_Name).Value;
            this.Name = PropertyHelper.GetPropertyName(propertyNameDesired, entityName);

            // Find the Property node
            XElement propertyNode = propertyNodeElements.FirstOrDefault(p =>
                p.Attribute(EdmxNodeAttributes.Property_Name).Value == propertyNameDesired);

            // Check Property node exists
            if (propertyNode == null)
            {
                throw new ApplicationException(string.Format(Resources.Error_PropertyKeyMissing, 
                    entityName, propertyNameDesired));
            }

            this.Type = PropertyHelper.GetTypeFromEDMXProperty(propertyNode, entityName);
        }
    }
}