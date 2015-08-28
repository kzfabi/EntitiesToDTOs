/* EntitiesToDTOs. Copyright (c) 2011. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using System.CodeDom;
using System.CodeDom.Compiler;
using EntitiesToDTOs.Helpers;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a DTO Class Property.
    /// </summary>
    internal class DTOClassProperty
    {
        /// <summary>
        /// Attributes of the property.
        /// </summary>
        public List<DTOAttribute> PropertyAttributes { get; private set; }

        /// <summary>
        /// Property access.
        /// </summary>
        public vsCMAccess PropertyAccess { get; private set; }

        /// <summary>
        /// Property Type.
        /// </summary>
        public string PropertyType { get; private set; }

        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Property name found in the EDMX.
        /// </summary>
        public string PropertyNameEDMX { get; private set; }

        /// <summary>
        /// Indicates if this property is Key of the Entity.
        /// </summary>
        public bool IsKey { get; private set; }

        /// <summary>
        /// Indicates if this property is a navigation property.
        /// </summary>
        public bool IsNavigation { get; private set; }

        /// <summary>
        /// If this is a Navigation, this is the DTO target. Otherwise = null.
        /// </summary>
        public DTOEntity NavigatesToDTO { get; private set; }

        /// <summary>
        /// If this is a Navigation, this is the DTO target name. Otherwise = null.
        /// </summary>
        public string NavigatesToDTOName { get; private set; }

        /// <summary>
        /// Indicates if this property is a List.
        /// </summary>
        public bool IsList { get; private set; }

        /// <summary>
        /// Indicates the type of objects contained if it is a List. Same value as PropertyType if not.
        /// </summary>
        public string ListOf { get; private set; }

        /// <summary>
        /// Indicates if it is a complex type property.
        /// </summary>
        public bool IsComplex { get; set; }

        /// <summary>
        /// Indicates if it is an enum type property.
        /// </summary>
        public bool IsEnum { get; set; }



        private DTOClassProperty(bool isServiceReady)
        {
            #region Set Property Attributes
            this.PropertyAttributes = new List<DTOAttribute>();

            if (isServiceReady)
            {
                this.PropertyAttributes.Add(new DTOAttribute(Resources.AttributeDataMember, null));
            }
            #endregion Set Property Attributes

            this.PropertyAccess = vsCMAccess.vsCMAccessPublic;

            // Set defaults
            this.IsKey = false;
            this.IsNavigation = false;
            this.IsList = false;
            this.IsComplex = false;
            this.IsEnum = false;
        }

        public DTOClassProperty(XElement propertyNode, List<string> entityKeys, 
            string entityOwnerName, bool isServiceReady, bool isComplex, bool isEnum) 
            : this(isServiceReady)
        {
            // Set PropertyType
            this.PropertyType = PropertyHelper.GetTypeFromEDMXProperty(propertyNode, entityOwnerName);

            // Set PropertyName
            this.PropertyNameEDMX = propertyNode.Attribute(EdmxNodeAttributes.Property_Name).Value;
            this.PropertyName = PropertyHelper.GetPropertyName(this.PropertyNameEDMX, entityOwnerName);

            // Set remaining properties
            this.IsKey = entityKeys.Contains(this.PropertyName);
            this.ListOf = this.PropertyType;
            this.IsComplex = isComplex;
            this.IsEnum = IsEnum;
        }

        public DTOClassProperty(string navigationPropertyNameEDMX, 
            EntityNavigationProperty navProperty, bool isServiceReady)
            : this(isServiceReady)
        {
            // Set PropertyType
            this.PropertyType = navProperty.Type;

            // Set PropertyName
            this.PropertyNameEDMX = navigationPropertyNameEDMX;
            this.PropertyName = navProperty.Name;

            // Set IsNavigation
            this.IsNavigation = true;

            // Set NavigatesToDTOName
            this.NavigatesToDTOName = navProperty.DTOTargetName;

            // Set IsList
            this.IsList = navProperty.IsList;

            // Set ListOf
            if (this.IsList)
            {
                this.ListOf = navProperty.ListOf;
            }
            else
            {
                this.ListOf = this.PropertyType;
            }
        }



        public void SetNavigatesToDTOReference(DTOEntity dtoTarget)
        {
            this.NavigatesToDTO = dtoTarget;
        }

    }
}