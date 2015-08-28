using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;
using Microsoft.VisualStudio.Shell;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Entity Navigation.
    /// </summary>
    internal class EntityNavigation
    {
        /// <summary>
        /// DTO Name (Owner of the Navigations).
        /// </summary>
        public string DTOName { get; private set; }

        /// <summary>
        /// Navigation property name found in the EDMX.
        /// </summary>
        public string NavigationPropertyNameEDMX { get; private set; }

        /// <summary>
        /// List of Navigation Properties.
        /// </summary>
        public List<EntityNavigationProperty> NavigationProperties { get; set; }



        /// <summary>
        /// Creates an instance of <see cref="EntityNavigation"/>.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        public EntityNavigation(XElement navigationNode, List<EntityAssociation> entitiesAssociations,
            List<EntityKeyProperty> entitiesKeys, GenerateDTOsParams genParams)
        {
            // Set DTO Name
            string entityName = navigationNode.Parent.Attribute(EdmxNodeAttributes.EntityType_Name).Value;
            this.DTOName = Utils.ConstructDTOName(entityName, genParams);

            // Get the To Role End name
            string toRole = navigationNode.Attribute(EdmxNodeAttributes.NavigationProperty_ToRole).Value;

            // Get the Association Name
            string associationName = EdmxHelper.GetNavigationAssociationName(navigationNode);

            this.NavigationProperties = new List<EntityNavigationProperty>();

            // Find the Association
            EntityAssociation association = (
                from a in entitiesAssociations
                where (a.AssociationName == associationName) && (a.EndRoleName == toRole)
                select a
                ).FirstOrDefault();

            // Find the DTO associated keys
            IEnumerable<EntityKeyProperty> dtoToKeys = entitiesKeys.Where(k => k.DTOName == association.DTOName);

            // Get the base Property Name
            this.NavigationPropertyNameEDMX = navigationNode.Attribute(EdmxNodeAttributes.NavigationProperty_Name).Value;
            string propertyName = PropertyHelper.GetPropertyName(this.NavigationPropertyNameEDMX, entityName);

            // Set association type desired, can change if requirements are not met
            AssociationType associationTypeDesired = genParams.AssociationType;

            if (association.EndMultiplicity == EntityAssociationMultiplicity.Many)
            {
                if ((associationTypeDesired != AssociationType.ClassType) 
                    && (dtoToKeys.Count() != 1))
                {
                    // TODO: ffernandez, indicate Project-ProjectItem-Line-Column
                    VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                        string.Format(Resources.Warning_CannotCreateNavPropManyKeyProp, this.DTOName, association.DTOName),
                        null, null, null, null);

                    associationTypeDesired = AssociationType.ClassType;
                }

                bool isList = true;

                if (associationTypeDesired == AssociationType.ClassType)
                {
                    // List<T> Type
                    string type = string.Format(Resources.CSharpTypeListT, association.DTOName);

                    this.NavigationProperties.Add(new EntityNavigationProperty(type, propertyName, isList, 
                        association.DTOName, association.EntityName, association.DTOName));
                }
                else
                {
                    // AssociationTypeEnum.KeyProperty
                    EntityKeyProperty dtoToKey = dtoToKeys.First();
                    
                    // List<T> Type
                    string type = string.Format(Resources.CSharpTypeListT, dtoToKey.Type);

                    propertyName += (Resources.NameSeparator + dtoToKey.Name);

                    this.NavigationProperties.Add(new EntityNavigationProperty(type, propertyName, isList,
                        dtoToKey.Type, association.EntityName, association.DTOName));
                }
            }
            else
            {
                // EntityAssociationMultiplicityEnum.One
                // EntityAssociationMultiplicityEnum.ZeroOrOne
                bool isList = false;
                string listOf = null;

                if (associationTypeDesired == AssociationType.ClassType)
                {
                    this.NavigationProperties.Add(new EntityNavigationProperty(association.DTOName, propertyName, isList,
                        listOf, association.EntityName, association.DTOName));
                }
                else
                {
                    // AssociationTypeEnum.KeyProperty

                    foreach (EntityKeyProperty dtoToKey in dtoToKeys)
                    {
                        string propName = (propertyName + Resources.NameSeparator + dtoToKey.Name);

                        this.NavigationProperties.Add(new EntityNavigationProperty(dtoToKey.Type, propName, isList,
                            listOf, association.EntityName, association.DTOName));
                    }
                }
            }
        }
    }
}