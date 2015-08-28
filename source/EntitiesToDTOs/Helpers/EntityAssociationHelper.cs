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
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Domain.Enums;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to Entity Associations of the EDMX
    /// </summary>
    internal class EntityAssociationHelper
    {
        /// <summary>
        /// Gets the Multiplicity Type of an Association End node of the EDMX Document.
        /// </summary>
        /// <param name="associationEndNode">Association End node of the EDMX Document from where to take the Multiplicity Type.</param>
        /// <returns></returns>
        public static EntityAssociationMultiplicity GetMultiplicity(XElement associationEndNode)
        {
            string multiplicityValue =
                associationEndNode.Attribute(EdmxNodeAttributes.End_Multiplicity).Value;

            switch (multiplicityValue)
            {
                case AppConstants.EDMX_ASSOCIATION_MULTIPLICITY_ZERO_OR_ONE: 
                    return EntityAssociationMultiplicity.ZeroOrOne;
                
                case AppConstants.EDMX_ASSOCIATION_MULTIPLICITY_ONE: 
                    return EntityAssociationMultiplicity.One;
                
                case AppConstants.EDMX_ASSOCIATION_MULTIPLICITY_MANY: 
                    return EntityAssociationMultiplicity.Many;

                default:
                    string type = associationEndNode.Attribute(EdmxNodeAttributes.End_Type).Value;
                    string role = associationEndNode.Attribute(EdmxNodeAttributes.End_Role).Value;

                    throw new ApplicationException(string.Format(Resources.Error_NotRecognizedMultiplicity, 
                        multiplicityValue, type, role));
            }
        }
    }
}