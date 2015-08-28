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
using EntitiesToDTOs.Properties;
using System.Xml.Linq;
using EntitiesToDTOs.Domain;
using Microsoft.VisualStudio.Shell;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to Properties of the EDMX
    /// </summary>
    internal class PropertyHelper
    {
        /// <summary>
        /// List of reserved property names
        /// </summary>
        private static List<string> ReservedNames
        {
            get
            {
                if (_reservedNames == null)
                {
                    _reservedNames = new List<string>();
                    _reservedNames.Add(Resources.ReservedName01);
                    _reservedNames.Add(Resources.ReservedName02);
                    _reservedNames.Add(Resources.ReservedName03);
                    _reservedNames.Add(Resources.ReservedName04);
                    _reservedNames.Add(Resources.ReservedName05);
                    _reservedNames.Add(Resources.ReservedName06);
                    _reservedNames.Add(Resources.ReservedName07);
                    _reservedNames.Add(Resources.ReservedName08);
                    _reservedNames.Add(Resources.ReservedName09);
                    _reservedNames.Add(Resources.ReservedName10);
                    _reservedNames.Add(Resources.ReservedName11);
                    _reservedNames.Add(Resources.ReservedName12);
                    _reservedNames.Add(Resources.ReservedName13);
                    _reservedNames.Add(Resources.ReservedName14);
                    _reservedNames.Add(Resources.ReservedName15);
                    _reservedNames.Add(Resources.ReservedName16);
                    _reservedNames.Add(Resources.ReservedName17);
                    _reservedNames.Add(Resources.ReservedName18);
                    _reservedNames.Add(Resources.ReservedName19);
                    _reservedNames.Add(Resources.ReservedName20);
                    _reservedNames.Add(Resources.ReservedName21);
                    _reservedNames.Add(Resources.ReservedName22);
                    _reservedNames.Add(Resources.ReservedName23);
                    _reservedNames.Add(Resources.ReservedName24);
                    _reservedNames.Add(Resources.ReservedName25);
                    _reservedNames.Add(Resources.ReservedName26);
                    _reservedNames.Add(Resources.ReservedName27);
                    _reservedNames.Add(Resources.ReservedName28);
                    _reservedNames.Add(Resources.ReservedName29);
                    _reservedNames.Add(Resources.ReservedName30);
                    _reservedNames.Add(Resources.ReservedName31);
                    _reservedNames.Add(Resources.ReservedName32);
                    _reservedNames.Add(Resources.ReservedName33);
                    _reservedNames.Add(Resources.ReservedName34);
                    _reservedNames.Add(Resources.ReservedName35);
                    _reservedNames.Add(Resources.ReservedName36);
                    _reservedNames.Add(Resources.ReservedName37);
                    _reservedNames.Add(Resources.ReservedName38);
                    _reservedNames.Add(Resources.ReservedName39);
                    _reservedNames.Add(Resources.ReservedName40);
                    _reservedNames.Add(Resources.ReservedName41);
                    _reservedNames.Add(Resources.ReservedName42);
                    _reservedNames.Add(Resources.ReservedName43);
                    _reservedNames.Add(Resources.ReservedName44);
                    _reservedNames.Add(Resources.ReservedName45);
                    _reservedNames.Add(Resources.ReservedName46);
                    _reservedNames.Add(Resources.ReservedName47);
                    _reservedNames.Add(Resources.ReservedName48);
                    _reservedNames.Add(Resources.ReservedName49);
                    _reservedNames.Add(Resources.ReservedName50);
                    _reservedNames.Add(Resources.ReservedName51);
                    _reservedNames.Add(Resources.ReservedName52);
                    _reservedNames.Add(Resources.ReservedName53);
                    _reservedNames.Add(Resources.ReservedName54);
                    _reservedNames.Add(Resources.ReservedName55);
                    _reservedNames.Add(Resources.ReservedName56);
                    _reservedNames.Add(Resources.ReservedName57);
                    _reservedNames.Add(Resources.ReservedName58);
                    _reservedNames.Add(Resources.ReservedName59);
                    _reservedNames.Add(Resources.ReservedName60);
                    _reservedNames.Add(Resources.ReservedName61);
                    _reservedNames.Add(Resources.ReservedName62);
                    _reservedNames.Add(Resources.ReservedName63);
                    _reservedNames.Add(Resources.ReservedName64);
                    _reservedNames.Add(Resources.ReservedName65);
                    _reservedNames.Add(Resources.ReservedName66);
                    _reservedNames.Add(Resources.ReservedName67);
                    _reservedNames.Add(Resources.ReservedName68);
                    _reservedNames.Add(Resources.ReservedName69);
                    _reservedNames.Add(Resources.ReservedName70);
                    _reservedNames.Add(Resources.ReservedName71);
                    _reservedNames.Add(Resources.ReservedName72);
                    _reservedNames.Add(Resources.ReservedName73);
                    _reservedNames.Add(Resources.ReservedName74);
                    _reservedNames.Add(Resources.ReservedName75);
                    _reservedNames.Add(Resources.ReservedName76);
                    _reservedNames.Add(Resources.ReservedName77);
                }

                return _reservedNames;
            }
        }
        private static List<string> _reservedNames = null;

        /// <summary>
        /// Enum Types available.
        /// </summary>
        private static List<EnumType> EnumTypes { get; set; }

        /// <summary>
        /// ComplexType DTOs available
        /// </summary>
        private static List<DTOEntity> ComplexTypes { get; set; }

        /// <summary>
        /// EdmxType-CSharpType mapping.
        /// </summary>
        private static Dictionary<string, CSharpType> EdmxCSharpMapping
        {
            get
            {
                if (_edmxCSharpMapping == null)
                {
                    _edmxCSharpMapping = new Dictionary<string, CSharpType>();

                    _edmxCSharpMapping.Add(Resources.EdmxTypeBinary, new CSharpType(Resources.CSharpTypeBinary, false));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeBoolean, new CSharpType(Resources.CSharpTypeBoolean, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeByte, new CSharpType(Resources.CSharpTypeByte, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeDateTime, new CSharpType(Resources.CSharpTypeDateTime, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeDateTimeOffset, new CSharpType(Resources.CSharpTypeDateTimeOffset, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeDecimal, new CSharpType(Resources.CSharpTypeDecimal, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeDouble, new CSharpType(Resources.CSharpTypeDouble, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeGuid, new CSharpType(Resources.CSharpTypeGuid, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeInt16, new CSharpType(Resources.CSharpTypeInt16, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeInt32, new CSharpType(Resources.CSharpTypeInt32, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeInt64, new CSharpType(Resources.CSharpTypeInt64, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeSByte, new CSharpType(Resources.CSharpTypeSByte, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeSingle, new CSharpType(Resources.CSharpTypeSingle, true));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeString, new CSharpType(Resources.CSharpTypeString, false));
                    _edmxCSharpMapping.Add(Resources.EdmxTypeTime, new CSharpType(Resources.CSharpTypeTime, true));
                }

                return _edmxCSharpMapping;
            }
        }
        private static Dictionary<string, CSharpType> _edmxCSharpMapping = null;

        

        /// <summary>
        /// Set the enum types available
        /// </summary>
        /// <param name="enumTypes">Enum types available.</param>
        public static void SetEnumTypes(List<EnumType> enumTypes)
        {
            PropertyHelper.EnumTypes = enumTypes;
        }

        /// <summary>
        /// Set the Complex Types available
        /// </summary>
        /// <param name="complexTypes"></param>
        public static void SetComplexTypes(List<DTOEntity> complexTypes)
        {
            PropertyHelper.ComplexTypes = complexTypes;
        }

        /// <summary>
        /// Gets the CSharp Type from a EDMX Property.
        /// </summary>
        /// <param name="propertyNode">Type attribute.</param>
        /// <param name="entityOwnerName">Entity owner name.</param>
        /// <returns></returns>
        public static string GetTypeFromEDMXProperty(XElement propertyNode, string entityOwnerName)
        {
            // Get the Type attribute
            XAttribute typeAttribute = propertyNode.Attribute(EdmxNodeAttributes.Property_Type);

            // Check Type attribute exists
            if (typeAttribute == null)
            {
                string propertyName = propertyNode.Attribute(EdmxNodeAttributes.Property_Name).Value;

                throw new ApplicationException(string.Format(Resources.Error_PropertyTypeAttributeMissing, 
                    entityOwnerName, propertyName));
            }

            // Get the Type value
            string edmxTypeValue = propertyNode.Attribute(EdmxNodeAttributes.Property_Type).Value;

            // Check Type value is not empty
            if (string.IsNullOrWhiteSpace(edmxTypeValue))
            {
                string propertyName = propertyNode.Attribute(EdmxNodeAttributes.Property_Name).Value;

                throw new ApplicationException(string.Format(Resources.Error_PropertyTypeAttributeMissing,
                    entityOwnerName, propertyName));
            }

            // Check if it is Nullable
            bool isNullable = true;
            XAttribute nullableAttribute = propertyNode.Attribute(EdmxNodeAttributes.Property_Nullable);
            if (nullableAttribute != null)
            {
                isNullable = (nullableAttribute.Value == Resources.XmlBoolTrue);
            }

            // Variables
            string outputType = null;
            bool outputTypeAdmitsNullable = false;

            // Check if it is a supported type and we got the mapping for it
            if (PropertyHelper.EdmxCSharpMapping.ContainsKey(edmxTypeValue))
            {
                // Do the mapping between the EDMX type and the C# type
                outputType = PropertyHelper.EdmxCSharpMapping[edmxTypeValue].Name;
                outputTypeAdmitsNullable = PropertyHelper.EdmxCSharpMapping[edmxTypeValue].AllowsNullable;
            }
            else
            {
                // Get type name without namespce to check if it is a type defined in the EDMX (ComplexType, EnumType)
                string edmxTypeName = EdmxHelper.GetNameWithoutNamespace(edmxTypeValue);

                // Check if it is a complex type
                DTOEntity complexTypeDTO = PropertyHelper.ComplexTypes.FirstOrDefault(ct => ct.Name == edmxTypeName);

                if (complexTypeDTO != null)
                {
                    // It is a ComplexType
                    outputType = complexTypeDTO.NameDTO;
                    outputTypeAdmitsNullable = true;
                }
                else if (PropertyHelper.EnumTypes.Exists(e => e.Name == edmxTypeName))
                {
                    // It is an EnumType
                    outputType = edmxTypeName;
                    outputTypeAdmitsNullable = true;
                }
                else
                {
                    // Not a supported Type neither a Type defined in the EDMX
                    // Use object type and notify the user
                    outputType = Resources.CSharpTypeObject;
                    outputTypeAdmitsNullable = true;

                    string propertyName = propertyNode.Attribute(EdmxNodeAttributes.Property_Name).Value;

                    // TODO: ffernandez, indicate Project-ProjectItem-Line-Column
                    VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                        string.Format(Resources.Warning_NotSupportedEDMXPropertyType, entityOwnerName, propertyName, edmxTypeValue),
                        null, null, null, null);
                }
            }

            // Check if it is Nullable and the Type admits Nullable Types
            if (isNullable && outputTypeAdmitsNullable)
            {
                outputType = string.Format(Resources.CSharpTypeNullableT, outputType);
            }

            return outputType;
        }

        /// <summary>
        /// Gets a valid Property name
        /// </summary>
        /// <param name="propertyNameDesired">Property name desired</param>
        /// <param name="entityOwnerName">Entity owner name</param>
        /// <returns></returns>
        public static string GetPropertyName(string propertyNameDesired, string entityOwnerName)
        {
            if (string.IsNullOrWhiteSpace(propertyNameDesired))
            {
                throw new ApplicationException(string.Format(Resources.Error_PropertyNameCannotBeEmpty, entityOwnerName));
            }

            if (PropertyHelper.ReservedNames.Contains(propertyNameDesired.ToLower()))
            {
                string newPropertyName = (propertyNameDesired + Resources.NameSeparator);

                // TODO: ffernandez, indicate Project-ProjectItem-Line-Column
                VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning, 
                    string.Format(Resources.Warning_InvalidPropertyName, entityOwnerName, propertyNameDesired, newPropertyName),
                    null, null, null, null);

                return newPropertyName;
            }
            else
            {
                return propertyNameDesired;
            }
        }

    }
}