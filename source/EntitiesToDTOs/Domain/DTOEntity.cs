/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Domain.Enums;
using Microsoft.VisualStudio.Shell;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a DTO to generate for a specific Entity class type.
    /// </summary>
    internal class DTOEntity : DTOClass
    {
        /// <summary>
        /// DTO Base Class name.
        /// </summary>
        public string NameBaseDTO { get; private set; }

        /// <summary>
        /// DTO Base Class reference.
        /// </summary>
        public DTOEntity BaseDTO { get; private set; }

        /// <summary>
        /// DTOs childs of this DTO.
        /// </summary>
        public List<DTOEntity> DTOChilds { get; private set; }

        /// <summary>
        /// Indicates if this DTO is Abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        

        /// <summary>
        /// Creates an instance of <see cref="DTOEntity"/>.
        /// </summary>
        /// <param name="typeNode">Type node.</param>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <param name="navigations">Entity navigations available.</param>
        public DTOEntity(XElement typeNode, GenerateDTOsParams genParams, List<EntityNavigation> navigations)
            : base(typeNode, genParams)
        {
            // Define Class Base Type (if exists)
            string entityBaseType = EdmxHelper.GetEntityBaseType(typeNode);
            if (string.IsNullOrWhiteSpace(entityBaseType) == false)
            {
                // Check if base type is going to be generated
                if (genParams.TypesToGenerateFilter.Contains(entityBaseType) == true)
                {
                    this.NameBaseDTO = Utils.ConstructDTOName(entityBaseType, genParams);
                }
            }

            #region Set IsAbstract

            if (typeNode.Attribute(EdmxNodeAttributes.EntityType_Abstract) != null)
            {
                string abstractValue = typeNode.Attribute(EdmxNodeAttributes.EntityType_Abstract).Value.ToLower();
                this.IsAbstract = (abstractValue == Resources.XmlBoolTrue);
            }

            #endregion Set IsAbstract

            #region Set Navigation Properties

            if (navigations != null)
            {
                IEnumerable<EntityNavigation> myNavigations = navigations.Where(a => a.DTOName == this.NameDTO);

                foreach (EntityNavigation entityNav in myNavigations)
                {
                    foreach (EntityNavigationProperty navProperty in entityNav.NavigationProperties)
                    {
                        // Add property only if target type is going to be generated
                        if (genParams.TypesToGenerateFilter.Contains(navProperty.EntityTargetName) == true)
                        {
                            if (genParams.AssociationType == AssociationType.KeyProperty
                                && this.Properties.Exists(p => p.PropertyName == navProperty.Name))
                            {
                                VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                                    string.Format(Resources.Warning_PropertyNameConflict, this.Name, navProperty.Name, navProperty.EntityTargetName),
                                    genParams.TargetProject, null, null, null);
                            }
                            else
                            {
                                this.Properties.Add(new DTOClassProperty(
                                    entityNav.NavigationPropertyNameEDMX, navProperty, genParams.DTOsServiceReady));
                            }
                        }
                    }
                }
            }

            #endregion Set Association Properties
        }



        /// <summary>
        /// Sets the KnowTypes of this DTO.
        /// </summary>
        /// <param name="entities">List of all the DTOs that are going to be generated.</param>
        /// <param name="isServiceReady">Specifies if the DTOs are Service-Ready.</param>
        public void SetKnownTypes(List<DTOEntity> entities, bool isServiceReady)
        {
            if (isServiceReady)
            {
                IEnumerable<DTOEntity> knownTypes = entities.Where(e => e.NameBaseDTO == this.NameDTO);

                foreach (DTOEntity entity in knownTypes)
                {
                    this.Attributes.Add(new DTOAttribute(Resources.AttributeKnownType,
                            string.Format(Resources.CSharpCodeTypeOf, entity.NameDTO)));
                }
            }
        }

        /// <summary>
        /// Sets the childs of this DTO.
        /// </summary>
        /// <param name="entities">List of all the DTOs that are going to be generated.</param>
        public void SetChilds(List<DTOEntity> entities)
        {
            this.DTOChilds = entities.Where(e => e.NameBaseDTO == this.NameDTO).ToList();
        }

        /// <summary>
        /// Sets the reference to the DTO Base Class.
        /// </summary>
        /// <param name="entitiesDTOs">Entities DTOs to generate.</param>
        public void SetDTOBase(List<DTOEntity> entitiesDTOs)
        {
            if (string.IsNullOrWhiteSpace(this.NameBaseDTO) == false)
            {
                DTOEntity baseDTO = entitiesDTOs.FirstOrDefault(e => e.NameDTO == this.NameBaseDTO);

                this.BaseDTO = baseDTO;
            }
        }

    }
}