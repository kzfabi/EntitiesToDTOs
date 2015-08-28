using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;
using EnvDTE;
using EnvDTE80;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a DTO to generate for a specific class type.
    /// </summary>
    internal abstract class DTOClass
    {
        /// <summary>
        /// Source type name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// DTO Class name.
        /// </summary>
        public string NameDTO { get; private set; }

        /// <summary>
        /// DTO type attributes.
        /// </summary>
        public List<DTOAttribute> Attributes { get; private set; }

        /// <summary>
        /// The Assembler to generate for this DTO.
        /// </summary>
        public Assembler Assembler { get; private set; }

        /// <summary>
        /// DTO class access.
        /// </summary>
        public vsCMAccess DTOClassAccess { get; private set; }

        /// <summary>
        /// DTO Class Kind (used to generate partial classes).
        /// </summary>
        public vsCMClassKind DTOClassKind { get; private set; }

        /// <summary>
        /// Class properties.
        /// </summary>
        public List<DTOClassProperty> Properties { get; private set; }



        public DTOClass(XElement typeNode, GenerateDTOsParams genParams)
        {
            // Set source type name
            this.Name = typeNode.Attribute(EdmxNodeAttributes.EntityType_Name).Value;

            // Set DTO name
            this.NameDTO = Utils.ConstructDTOName(this.Name, genParams);

            #region Set Class Attributes

            this.Attributes = new List<DTOAttribute>();

            if (genParams.DTOsServiceReady)
            {
                const string parameters = null;
                this.Attributes.Add(new DTOAttribute(Resources.AttributeDataContract, parameters));
            }

            #endregion Set Class Attributes

            // Set Assembler (it does not have one by default)
            this.Assembler = null;

            // Set Class Access
            this.DTOClassAccess = vsCMAccess.vsCMAccessPublic;

            // Set the Class Kind to partial so the final user can extend it
            this.DTOClassKind = vsCMClassKind.vsCMClassKindPartialClass;

            #region Set Properties

            this.Properties = new List<DTOClassProperty>();

            // Get Key Node
            XElement keyNode = typeNode.DescendantsCSDL(EdmxNodes.Key).FirstOrDefault();

            // Get source keys
            var sourceKeys = new List<string>();
            if (keyNode != null)
            {
                sourceKeys = keyNode.DescendantsCSDL(EdmxNodes.PropertyRef)
                    .Select(n => n.Attribute(EdmxNodeAttributes.PropertyRef_Name).Value).ToList();
            }

            // Get Property nodes
            IEnumerable<XElement> entityPropertiesNodes = typeNode.DescendantsCSDL(EdmxNodes.Property);

            // Variables
            string edmxTypeValue;
            string[] edmxTypeValueSplitted;
            string edmxTypeName;
            bool addProperty;
            bool isEnum;

            foreach (XElement propertyNode in entityPropertiesNodes)
            {
                addProperty = true;
                isEnum = false;

                // Get the Type value
                edmxTypeValue = propertyNode.Attribute(EdmxNodeAttributes.Property_Type).Value;

                // Split typeValue to check if it is a Complex Type
                edmxTypeValueSplitted = edmxTypeValue.Split(new string[] { Resources.Dot }, StringSplitOptions.RemoveEmptyEntries);

                // Get EDMX type name (split to ensure that if it is from the same model we only get the name)
                edmxTypeName = edmxTypeValueSplitted[(edmxTypeValueSplitted.Length - 1)];

                // Check if property type is complex or entity type
                if (genParams.AllComplexTypesNames.Contains(edmxTypeName) == true
                    || genParams.AllEntityTypesNames.Contains(edmxTypeName) == true)
                {
                    // Check if the type is going to be generated
                    addProperty = genParams.TypesToGenerateFilter.Contains(edmxTypeName);
                }
                else if(genParams.AllEnumTypesNames.Contains(edmxTypeName) == true)
                {
                    isEnum = true;
                }

                if (addProperty == true)
                {
                    // Check if it is a complex type property
                    bool isComplex = (genParams.AllComplexTypesNames.Contains(edmxTypeName) == true);

                    this.Properties.Add(new DTOClassProperty(propertyNode, sourceKeys,
                        this.Name, genParams.DTOsServiceReady, isComplex, isEnum));
                }
            }

            #endregion Set Properties
        }



        /// <summary>
        /// Sets the Assembler for this DTO.
        /// </summary>
        /// <param name="genParams">Assemblers generation parameters.</param>
        public void SetAssembler(GenerateAssemblersParams genParams)
        {
            this.Assembler = new Assembler(this,
                Utils.ConstructAssemblerName(this.Name, genParams));
        }

    }
}