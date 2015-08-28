using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Domain.Enums;
using EntitiesToDTOs.Events;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace EntitiesToDTOs.Generators
{
    /// <summary>
    /// Manages all operations related to the generation of DTOs.
    /// </summary>
    internal class DTOGenerator
    {
        /// <summary>
        /// Generates the DTOs of the EDMX Document provided using the parameters received.
        /// </summary>
        /// <param name="parameters">Parameters for the generation of DTOs.</param>
        /// <param name="worker">BackgroundWorker reference.</param>
        public static List<DTOEntity> GenerateDTOs(GenerateDTOsParams parameters, BackgroundWorker worker)
        {
            LogManager.LogMethodStart();

            if (parameters.DTOsServiceReady)
            {
                VisualStudioHelper.AddReferenceToProject(parameters.TargetProject,
                    Resources.AssemblySystemRuntimeSerialization, Resources.AssemblySystemRuntimeSerialization);
            }

            if (GeneratorManager.CheckCancellationPending()) return null;

            EditPoint objEditPoint; // EditPoint to reuse
            CodeParameter objCodeParameter; // CodeParameter to reuse
            CodeNamespace objNamespace = null; // Namespace item to add Classes
            ProjectItem sourceFileItem = null; // Source File Item to save
            int dtosGenerated = 0;

            List<EnumType> enums = DTOGenerator.GetEnumTypes(parameters);
            PropertyHelper.SetEnumTypes(enums);

            List<DTOEntity> entitiesDTOs = DTOGenerator.GetEntityDTOs(parameters);

            if (GeneratorManager.CheckCancellationPending()) return null;

            worker.ReportProgress(0, new GeneratorOnProgressEventArgs(0, 
                string.Format(Resources.Text_DTOsGenerated, dtosGenerated, entitiesDTOs.Count)));

            TemplateClass.CreateFile();

            // Imports to add to the Source File
            var importList = new List<SourceCodeImport>();

            // EnumTypes defined in the EDMX ?
            if (enums.Exists(e => e.IsExternal == false))
            {
                importList.Add(new SourceCodeImport(parameters.EntitiesNamespace));
                VisualStudioHelper.AddReferenceToProject(parameters.TargetProject, parameters.EDMXProject);
            }

            // Include imports of external enums.
            foreach (string externalEnumNamespace in enums.Where(e => e.IsExternal).Select(e => e.Namespace).Distinct())
            {
                importList.Add(new SourceCodeImport(externalEnumNamespace));
            }

            // Generate Source File if type is One Source File
            if (parameters.SourceFileGenerationType == SourceFileGenerationType.OneSourceFile)
            {
                sourceFileItem = null;

                // Generate Source and Get the Namespace item
                objNamespace = VisualStudioHelper.GenerateSourceAndGetNamespace(
                    parameters.TargetProject, parameters.TargetProjectFolder, 
                    parameters.SourceFileName, parameters.SourceFileHeaderComment,
                    parameters.SourceNamespace, parameters.DTOsServiceReady, out sourceFileItem);

                // Add Imports to Source File
                VisualStudioHelper.AddImportsToSourceCode(ref sourceFileItem, importList);
            }

            // Check Cancellation Pending
            if (GeneratorManager.CheckCancellationPending()) return null;

            // Loop through Entities DTOs
            foreach (DTOEntity entityDTO in entitiesDTOs)
            {
                // Generate Source File if type is Source File per Class
                if (parameters.SourceFileGenerationType == SourceFileGenerationType.SourceFilePerClass)
                {
                    sourceFileItem = null;

                    // Generate Source and Get the Namespace item
                    objNamespace = VisualStudioHelper.GenerateSourceAndGetNamespace(
                        parameters.TargetProject, parameters.TargetProjectFolder, 
                        entityDTO.NameDTO, parameters.SourceFileHeaderComment, 
                        parameters.SourceNamespace, parameters.DTOsServiceReady, out sourceFileItem);

                    // Add Imports to Source File
                    VisualStudioHelper.AddImportsToSourceCode(ref sourceFileItem, importList);
                }

                // Add Class
                CodeClass objCodeClass = objNamespace.AddClassWithPartialSupport(entityDTO.NameDTO, entityDTO.NameBaseDTO,
                    entityDTO.DTOClassAccess, entityDTO.DTOClassKind);

                // Set IsAbstract
                objCodeClass.IsAbstract = entityDTO.IsAbstract;

                // Set Class Attributes
                foreach (DTOAttribute classAttr in entityDTO.Attributes)
                {
                    objCodeClass.AddAttribute(classAttr.Name, classAttr.Parameters, AppConstants.PLACE_AT_THE_END);
                }

                // Set Class Properties
                foreach (DTOClassProperty entityProperty in entityDTO.Properties)
                {
                    // Add Property
                    CodeProperty objCodeProperty = objCodeClass.AddProperty(entityProperty.PropertyName,
                        entityProperty.PropertyName, entityProperty.PropertyType, AppConstants.PLACE_AT_THE_END,
                        entityProperty.PropertyAccess, null);

                    // Get end of accessors auto-generated code
                    objEditPoint = objCodeProperty.Setter.EndPoint.CreateEditPoint();
                    objEditPoint.LineDown();
                    objEditPoint.EndOfLine();
                    var getSetEndPoint = objEditPoint.CreateEditPoint();

                    // Move to the start of accessors auto-generated code
                    objEditPoint = objCodeProperty.Getter.StartPoint.CreateEditPoint();
                    objEditPoint.LineUp();
                    objEditPoint.LineUp();
                    objEditPoint.EndOfLine();

                    // Replace accessors auto-generated code with a more cleaner one
                    objEditPoint.ReplaceText(getSetEndPoint, Resources.CSharpCodeGetSetWithBrackets,
                        Convert.ToInt32(vsEPReplaceTextOptions.vsEPReplaceTextAutoformat));

                    // Set Property Attributes
                    foreach (DTOAttribute propAttr in entityProperty.PropertyAttributes)
                    {
                        objCodeProperty.AddAttribute(propAttr.Name,
                            propAttr.Parameters, AppConstants.PLACE_AT_THE_END);
                    }

                    objEditPoint = objCodeProperty.StartPoint.CreateEditPoint();
                    objEditPoint.SmartFormat(objEditPoint);
                }

                if (parameters.GenerateDTOConstructors)
                {
                    // Add empty Constructor
                    CodeFunction emptyConstructor = objCodeClass.AddFunction(objCodeClass.Name,
                        vsCMFunction.vsCMFunctionConstructor, null, AppConstants.PLACE_AT_THE_END,
                        vsCMAccess.vsCMAccessPublic, null);

                    // Does this DTO have a Base Class ?
                    if (entityDTO.BaseDTO != null)
                    {
                        // Add call to empty Base Constructor
                        objEditPoint = emptyConstructor.StartPoint.CreateEditPoint();
                        objEditPoint.EndOfLine();
                        objEditPoint.Insert(Resources.Space + Resources.CSharpCodeBaseConstructor);
                    }

                    // Does this DTO have properties ?
                    if (entityDTO.Properties.Count > 0)
                    {
                        // Add Constructor with all properties as parameters
                        CodeFunction constructorWithParams = objCodeClass.AddFunction(objCodeClass.Name,
                            vsCMFunction.vsCMFunctionConstructor, null, AppConstants.PLACE_AT_THE_END,
                            vsCMAccess.vsCMAccessPublic, null);

                        foreach (DTOClassProperty entityProperty in entityDTO.Properties)
                        {
                            // Add Constructor parameter
                            objCodeParameter = constructorWithParams.AddParameter(
                                Utils.SetFirstLetterLowercase(entityProperty.PropertyName),
                                entityProperty.PropertyType, AppConstants.PLACE_AT_THE_END);

                            // Add assignment
                            objEditPoint = constructorWithParams.EndPoint.CreateEditPoint();
                            objEditPoint.LineUp();
                            objEditPoint.EndOfLine();
                            objEditPoint.Insert(Environment.NewLine + AppConstants.TAB + AppConstants.TAB + AppConstants.TAB);
                            objEditPoint.Insert(string.Format(Resources.CSharpCodeAssignmentThis,
                                entityProperty.PropertyName, objCodeParameter.Name));
                        }

                        // Does this DTO have a Base Class ?
                        if (entityDTO.BaseDTO != null)
                        {
                            // Get the Base Class properties (includes the properties of the base recursively)
                            List<DTOClassProperty> baseProperties =
                                DTOGenerator.GetPropertiesForConstructor(entityDTO.BaseDTO);

                            // Base Constructor parameters
                            var sbBaseParameters = new StringBuilder();

                            foreach (DTOClassProperty entityProperty in baseProperties)
                            {
                                // Add Constructor parameter
                                objCodeParameter = constructorWithParams.AddParameter(
                                    Utils.SetFirstLetterLowercase(entityProperty.PropertyName),
                                    entityProperty.PropertyType, AppConstants.PLACE_AT_THE_END);

                                // Add parameter separation if other parameters exists
                                if (sbBaseParameters.Length > 0)
                                {
                                    sbBaseParameters.Append(Resources.CommaSpace);
                                }

                                // Add to Base Constructor parameters
                                sbBaseParameters.Append(objCodeParameter.Name);
                            }

                            // Add call to Base Constructor with parameters
                            objEditPoint = constructorWithParams.StartPoint.CreateEditPoint();
                            objEditPoint.EndOfLine();
                            objEditPoint.Insert(
                                Environment.NewLine + AppConstants.TAB + AppConstants.TAB + AppConstants.TAB);
                            objEditPoint.Insert(
                                string.Format(Resources.CSharpCodeBaseConstructorWithParams, sbBaseParameters.ToString()));
                        
                        } // END if DTO has a Base Class

                    } // END if DTO has properties

                } // END if Generate DTO Constructor methods

                // Save changes to Source File Item
                sourceFileItem.Save();

                // Count DTO generated
                dtosGenerated++;

                // Report Progress
                int progress = ((dtosGenerated * 100) / entitiesDTOs.Count);
                if (progress < 100)
                {
                    worker.ReportProgress(progress, new GeneratorOnProgressEventArgs(progress, 
                        string.Format(Resources.Text_DTOsGenerated, dtosGenerated, entitiesDTOs.Count)));
                }

                // Check Cancellation Pending
                if (GeneratorManager.CheckCancellationPending()) return null;

            } // END Loop through Entities DTOs

            // Save Target Project
            parameters.TargetProject.Save();

            // Delete Template Class File
            TemplateClass.Delete();

            // Report Progress
            worker.ReportProgress(100, new GeneratorOnProgressEventArgs(100, 
                string.Format(Resources.Text_DTOsGenerated, dtosGenerated, entitiesDTOs.Count)));

            LogManager.LogMethodEnd();

            // Return the DTOs generated
            return entitiesDTOs;
        }

        /// <summary>
        /// Gets the enum types available.
        /// </summary>
        /// <param name="parameters">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<EnumType> GetEnumTypes(GenerateDTOsParams parameters)
        {
            var enums = new List<EnumType>();

            foreach (XElement xEnum in EdmxHelper.GetEnumTypeNodes(parameters.EDMXDocument))
            {
                var enumType = new EnumType();
                enumType.Name = xEnum.Attribute(EdmxNodeAttributes.EntityType_Name).Value;

                XAttribute attrExternalType = xEnum.Attribute(EdmxNodeAttributes.EnumType_ExternalTypeName);
                if (attrExternalType != null)
                {
                    enumType.ExternalTypeName = attrExternalType.Value;

                    VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                        string.Format(Resources.Warning_ManuallyAddReferenceForEnum, enumType.ExternalTypeName),
                        parameters.TargetProject, null, null, null);
                }

                enums.Add(enumType);
            }

            return enums;
        }

        /// <summary>
        /// Gets the objects that represents the DTOs that needs to be generated from the received EDMX Document.
        /// </summary>
        /// <param name="parameters">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<DTOEntity> GetEntityDTOs(GenerateDTOsParams parameters)
        {
            LogManager.LogMethodStart();

            // Variables
            string typeName = null;
            bool generateType = false;
            var complexTypeDTOs = new List<DTOEntity>();
            var entityTypeDTOs = new List<DTOEntity>();
            
            // Get the DTOs for the ComplexType nodes
            foreach (XElement complexType in EdmxHelper.GetComplexTypeNodes(parameters.EDMXDocument))
            {
                typeName = complexType.Attribute(EdmxNodeAttributes.EntityType_Name).Value;
                
                generateType = parameters.TypesToGenerateFilter.Contains(typeName);

                if (generateType == true)
                {
                    List<EntityNavigation> entityNavigations = null;

                    complexTypeDTOs.Add(new DTOEntity(complexType, parameters, entityNavigations));
                }
            }

            // Set the Complex Types available
            PropertyHelper.SetComplexTypes(complexTypeDTOs);

            // Get Navigation Properties
            List<EntityNavigation> entitiesNavigations = DTOGenerator.GetEntitiesNavigations(parameters);

            // Get the DTOs for the EntityType nodes
            foreach (XElement entityTypeNode in EdmxHelper.GetEntityTypeNodes(parameters.EDMXDocument))
            {
                typeName = entityTypeNode.Attribute(EdmxNodeAttributes.EntityType_Name).Value;
                
                generateType = parameters.TypesToGenerateFilter.Contains(typeName);

                if (generateType == true)
                {
                    entityTypeDTOs.Add(new DTOEntity(entityTypeNode, parameters, entitiesNavigations));
                }
            }

            foreach (DTOEntity dto in entityTypeDTOs)
            {
                // Set Know Types of DTO
                dto.SetKnownTypes(entityTypeDTOs, parameters.DTOsServiceReady);

                // Set DTOs childs of DTO
                dto.SetChilds(entityTypeDTOs);

                // Set reference to DTO Base Class
                dto.SetDTOBase(entityTypeDTOs);

                // Set Navigation Target DTO references
                foreach (DTOClassProperty dtoProperty in dto.Properties.Where(p => p.IsNavigation == true))
                {
                    DTOEntity dtoTarget = entityTypeDTOs.FirstOrDefault(e => e.NameDTO == dtoProperty.NavigatesToDTOName);

                    if (dtoTarget != null)
                    {
                        dtoProperty.SetNavigatesToDTOReference(dtoTarget);
                    }
                }
            }

            // Get the final result
            var result = new List<DTOEntity>();
            result.AddRange(complexTypeDTOs);
            result.AddRange(entityTypeDTOs);

            LogManager.LogMethodEnd();

            return result;
        }

        /// <summary>
        /// Gets the Associations of the provided EDMX Document.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<EntityAssociation> GetEntitiesAssociations(GenerateDTOsParams genParams)
        {
            LogManager.LogMethodStart();

            IEnumerable<XElement> associationNodeElements = 
                EdmxHelper.GetAssociationNodes(genParams.EDMXDocument);
            
            var result = new List<EntityAssociation>();

            foreach (XElement associationNodeElement in associationNodeElements)
            {
                result.Add(new EntityAssociation(associationNodeElement, 
                    EntityAssociationEnd.First, genParams));

                result.Add(new EntityAssociation(associationNodeElement, 
                    EntityAssociationEnd.Second, genParams));
            }

            LogManager.LogMethodEnd();

            return result;
        }

        /// <summary>
        /// Gets the Entity Keys of the provided EDMX Document.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<EntityKeyProperty> GetEntitiesKeyProperties(GenerateDTOsParams genParams)
        {
            LogManager.LogMethodStart();

            IEnumerable<XElement> entityNodeElements =
                genParams.EDMXDocument.DescendantsCSDL(EdmxNodes.EntityType);
            
            var result = new List<EntityKeyProperty>();

            foreach (XElement entityNode in entityNodeElements)
            {
                result.AddRange(DTOGenerator.GetEntityKeys(entityNode, entityNode, 
                    entityNodeElements, genParams));
            }

            LogManager.LogMethodEnd();

            return result;
        }

        /// <summary>
        /// Gets the Keys of an Entity.
        /// </summary>
        /// <param name="entityNodeKeysOwner">EntityType node to get the keys from.</param>
        /// <param name="entityNodeToSetKeys">EntityType node to set the keys.</param>
        /// <param name="entityNodeElements">EntityType nodes.</param>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<EntityKeyProperty> GetEntityKeys(XElement entityNodeKeysOwner, XElement entityNodeToSetKeys,
            IEnumerable<XElement> entityNodeElements, GenerateDTOsParams genParams)
        {
            LogManager.LogMethodStart();

            var result = new List<EntityKeyProperty>();

            IEnumerable<XElement> propertyRefNodeElements = entityNodeKeysOwner.DescendantsCSDL(EdmxNodes.PropertyRef);

            IEnumerable<XElement> propertyNodeElements = entityNodeKeysOwner.DescendantsCSDL(EdmxNodes.Property);

            foreach (var propertyRefNode in propertyRefNodeElements)
            {
                result.Add(new EntityKeyProperty(entityNodeToSetKeys, propertyRefNode,
                    propertyNodeElements, genParams));
            }

            // Does this Entity have a Base Type ?
            string entityBaseType = EdmxHelper.GetEntityBaseType(entityNodeKeysOwner);
            if (entityBaseType != null)
            {
                // Find the Base Type node
                XElement entityBaseTypeNode = entityNodeElements.FirstOrDefault(
                    e => e.Attribute(EdmxNodeAttributes.EntityType_Name).Value == entityBaseType);

                if (entityBaseTypeNode == null)
                {
                    throw new ApplicationException(string.Format(Resources.Error_BaseTypeNotFound,
                        entityNodeKeysOwner.Attribute(EdmxNodeAttributes.EntityType_Name).Value, entityBaseType));
                }

                // Add the Entity Base Type keys to the resulting keys
                result.AddRange(DTOGenerator.GetEntityKeys(entityBaseTypeNode, entityNodeToSetKeys,
                    entityNodeElements, genParams));
            }

            LogManager.LogMethodEnd();

            return result;
        }

        /// <summary>
        /// Gets the Entity Navigations from the provided EDMX Document.
        /// </summary>
        /// <param name="genParams">Parameters for the generation of DTOs.</param>
        /// <returns></returns>
        private static List<EntityNavigation> GetEntitiesNavigations(GenerateDTOsParams genParams)
        {
            LogManager.LogMethodStart();

            // Get Entity Associations
            List<EntityAssociation> entitiesAssociations = DTOGenerator.GetEntitiesAssociations(genParams);

            // Get Entity Key Properties
            List<EntityKeyProperty> entitiesKeys = DTOGenerator.GetEntitiesKeyProperties(genParams);

            // Get Entities NavigationProperty Nodes
            IEnumerable<XElement> navigationNodeElements = genParams.EDMXDocument.DescendantsCSDL(EdmxNodes.NavigationProperty);
            
            var result = new List<EntityNavigation>();

            foreach (XElement navigationNode in navigationNodeElements)
            {
                result.Add(new EntityNavigation(navigationNode, 
                    entitiesAssociations, entitiesKeys, genParams));
            }

            LogManager.LogMethodEnd();

            return result;
        }

        /// <summary>
        /// Gets the Properties of an EntityDTO to use in a Constructor as parameters, recursively including the Base Class properties.
        /// </summary>
        /// <param name="entityDTO">EntityDTO to get the Properties from.</param>
        /// <returns></returns>
        private static List<DTOClassProperty> GetPropertiesForConstructor(DTOEntity entityDTO)
        {
            LogManager.LogMethodStart();

            var result = new List<DTOClassProperty>();

            result.AddRange(entityDTO.Properties);

            if (entityDTO.BaseDTO != null)
            {
                result.AddRange(DTOGenerator.GetPropertiesForConstructor(entityDTO.BaseDTO));
            }

            LogManager.LogMethodEnd();

            return result;
        }

    }
}