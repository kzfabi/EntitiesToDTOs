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
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Entity Association.
    /// </summary>
    internal class EntityAssociation
    {
        /// <summary>
        /// Association Name.
        /// </summary>
        public string AssociationName { get; private set; }

        /// <summary>
        /// Entity Name.
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// DTO Name.
        /// </summary>
        public string DTOName { get; private set; }

        /// <summary>
        /// Association Role Name.
        /// </summary>
        public string EndRoleName { get; private set; }

        /// <summary>
        /// End Multiplicity Type.
        /// </summary>
        public EntityAssociationMultiplicity EndMultiplicity { get; private set; }



        /// <summary>
        /// Creates an instance of <see cref="EntityAssociation"/>.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        public EntityAssociation(XElement associationNode, EntityAssociationEnd entityAssociationEndDesired, 
            GenerateDTOsParams genParams)
        {
            // Set Association Name
            this.AssociationName = associationNode.Attribute(EdmxNodeAttributes.Association_Name).Value;

            // Get End nodes
            XElement[] endNodes = associationNode.DescendantsCSDL(EdmxNodes.End).ToArray();

            // Get the position of the desired End node
            int endPosition = (entityAssociationEndDesired == EntityAssociationEnd.First ? 0 : 1);

            // Set DTO Name
            this.EntityName = endNodes[endPosition].Attribute(EdmxNodeAttributes.End_Type).Value;
            string[] entityNameSplitted = this.EntityName.Split(new string[] { Resources.Dot }, StringSplitOptions.RemoveEmptyEntries);
            this.EntityName = entityNameSplitted[(entityNameSplitted.Length - 1)];
            this.DTOName = Utils.ConstructDTOName(this.EntityName, genParams);

            // Set End Role Name
            this.EndRoleName = endNodes[endPosition].Attribute(EdmxNodeAttributes.End_Role).Value;

            // Set Multiplicity
            this.EndMultiplicity = EntityAssociationHelper.GetMultiplicity(endNodes[endPosition]);
        }
    }
}