using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Generators.Parameters;
using System.ComponentModel;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Events;
using EntitiesToDTOs.Properties;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Generators
{
    /// <summary>
    /// Provides functionality to generate Assemblers.
    /// </summary>
    internal class AssemblerGenerator
    {
        /// <summary>
        /// Generates Assemblers for DTOs generated using the parameters received.
        /// </summary>
        /// <param name="parameters">Parameters for the generation of Assemblers.</param>
        /// <param name="worker">BackgroundWorker reference.</param>
        public static void GenerateAssemblers(GenerateAssemblersParams parameters, BackgroundWorker worker)
        {
            LogManager.LogMethodStart();

            // Variables
            EditPoint objEditPoint; // EditPoint to reuse
            CodeNamespace objNamespace = null; // Namespace item to add Classes
            ProjectItem sourceFileItem = null; // Source File Item to save
            int assemblersGenerated = 0;

            // Report Progress
            worker.ReportProgress(0, new GeneratorOnProgressEventArgs(0,
                string.Format(Resources.Text_AssemblersGenerated, 
                    assemblersGenerated, parameters.EntitiesDTOs.Count)));

            // Add Reference to System.Core
            VisualStudioHelper.AddReferenceToProject(parameters.TargetProject,
                Resources.AssemblySystemCore, Resources.AssemblySystemCore);

            // Add Reference to System.Data.Entity
            VisualStudioHelper.AddReferenceToProject(parameters.TargetProject,
                Resources.AssemblySystemDataEntity, Resources.AssemblySystemDataEntity);

            if (parameters.IsServiceReady)
            {
                // Add Reference to System.Runtime.Serialization
                VisualStudioHelper.AddReferenceToProject(parameters.TargetProject,
                    Resources.AssemblySystemRuntimeSerialization, Resources.AssemblySystemRuntimeSerialization);
            }

            if (parameters.TargetProject.UniqueName != parameters.EDMXProject.UniqueName)
            {
                // Add Reference to EDMX Project
                VisualStudioHelper.AddReferenceToProject(parameters.TargetProject, parameters.EDMXProject);
            }

            if (parameters.TargetProject.UniqueName != parameters.DTOsTargetProject.UniqueName)
            {
                // Add Reference to DTOs Project
                VisualStudioHelper.AddReferenceToProject(parameters.TargetProject, parameters.DTOsTargetProject);
            }

            // Check Cancellation Pending
            if (GeneratorManager.CheckCancellationPending()) return;

            // Create Template Class File
            TemplateClass.CreateFile();

            // Imports to add to the Source File
            var importList = new List<SourceCodeImport>();
            importList.Add(new SourceCodeImport(Resources.AssemblySystemLinq));

            if (parameters.SourceNamespace != parameters.DTOsNamespace)
            {
                // Add import of DTOs namespace
                importList.Add(new SourceCodeImport(parameters.DTOsNamespace));
            }

            if (parameters.SourceNamespace != parameters.EntitiesNamespace)
            {
                // Add import of Entities namespace
                importList.Add(new SourceCodeImport(parameters.EntitiesNamespace));
            }

            // Generate Source File if type is One Source File
            if (parameters.SourceFileGenerationType == SourceFileGenerationType.OneSourceFile)
            {
                sourceFileItem = null;

                // Generate Source and Get the Namespace item
                objNamespace = VisualStudioHelper.GenerateSourceAndGetNamespace(
                    parameters.TargetProject, parameters.TargetProjectFolder, 
                    parameters.SourceFileName, parameters.SourceFileHeaderComment, 
                    parameters.SourceNamespace, parameters.IsServiceReady, out sourceFileItem);

                // Add import of System.Data.Objects.DataClasses (necessary for AssemblerBase)
                importList.Add(new SourceCodeImport(Resources.AssemblySystemDataObjectsDataClasses));

                // Add Imports to Source File
                VisualStudioHelper.AddImportsToSourceCode(ref sourceFileItem, importList);
            }

            // Check Cancellation Pending
            if (GeneratorManager.CheckCancellationPending()) return;

            // Set Assembler for all DTOs
            foreach (DTOEntity dto in parameters.EntitiesDTOs)
            {
                dto.SetAssembler(parameters);
            }

            // Check Cancellation Pending
            if (GeneratorManager.CheckCancellationPending()) return;

            // Loop through Entities DTOs
            foreach (DTOEntity dto in parameters.EntitiesDTOs)
            {
                if (dto.IsAbstract == true && (dto.DTOChilds == null || dto.DTOChilds.Count == 0))
                {
                    // DTO is abstract and does not have childs

                    // Get the source file name if it is one source file generation type
                    string sourceFileName = null;
                    if (parameters.SourceFileGenerationType == SourceFileGenerationType.OneSourceFile)
                    {
                        sourceFileName = sourceFileItem.Name;
                    }

                    // Add warning
                    VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                        string.Format(Resources.Warning_AbstractWithoutChildsAssembler, dto.Name),
                        parameters.TargetProject, sourceFileName, null, null);
                }
                else
                {
                    // Generate Source File if type is Source File per Class
                    if (parameters.SourceFileGenerationType == SourceFileGenerationType.SourceFilePerClass)
                    {
                        sourceFileItem = null;

                        // Generate Source and Get the Namespace item
                        objNamespace = VisualStudioHelper.GenerateSourceAndGetNamespace(
                            parameters.TargetProject, parameters.TargetProjectFolder, 
                            dto.Assembler.Name, parameters.SourceFileHeaderComment, 
                            parameters.SourceNamespace, parameters.IsServiceReady, out sourceFileItem);

                        // Add Imports to Source File
                        VisualStudioHelper.AddImportsToSourceCode(ref sourceFileItem, importList);
                    }

                    // Instance creation code variables
                    var toDTOInstanceCode = string.Empty;
                    var toEntityInstanceCode = string.Empty;

                    #region Generate instance creation code
                    if (dto.DTOChilds != null && dto.DTOChilds.Count > 0)
                    {
                        bool firstIfStatement = true;

                        // Add null assignment for DTO instance code
                        toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceNull, dto.NameDTO);
                        toDTOInstanceCode += Environment.NewLine + Environment.NewLine;

                        // Add null assignment for Entity instance code
                        toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceNull, dto.Name);
                        toEntityInstanceCode += Environment.NewLine + Environment.NewLine;

                        // Loop DTO Child classes
                        foreach (DTOEntity dtoChild in dto.DTOChilds)
                        {
                            // Set if statement checking if the entity is of this type, if it is then invoke DTO Assembler ToDTO method
                            if (firstIfStatement)
                            {
                                firstIfStatement = false;

                                toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceIfChild, dtoChild.Name);

                                toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceIfChild, dtoChild.NameDTO);
                            }
                            else
                            {
                                toDTOInstanceCode += Environment.NewLine;
                                toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceElseIfChild, dtoChild.Name);

                                toEntityInstanceCode += Environment.NewLine;
                                toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceElseIfChild, dtoChild.NameDTO);
                            }

                            toDTOInstanceCode += Environment.NewLine;
                            toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceChild, dtoChild.Name);

                            toEntityInstanceCode += Environment.NewLine;
                            toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceChild, dtoChild.NameDTO);
                        }

                        if (dto.IsAbstract == false)
                        {
                            toDTOInstanceCode += Environment.NewLine + Resources.AssemblerToDTOInstanceElse + Environment.NewLine;
                            toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceNew, dto.NameDTO);

                            toEntityInstanceCode += Environment.NewLine + Resources.AssemblerToEntityInstanceElse + Environment.NewLine;
                            toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceNew, dto.Name);
                        }
                    }
                    else if (dto.IsAbstract == false)
                    {
                        // No childs, simple DTO instance

                        toDTOInstanceCode = Resources.CSharpCodeVar + Resources.Space;
                        toDTOInstanceCode += string.Format(Resources.AssemblerToDTOInstanceNew, dto.NameDTO);

                        toEntityInstanceCode = Resources.CSharpCodeVar + Resources.Space;
                        toEntityInstanceCode += string.Format(Resources.AssemblerToEntityInstanceNew, dto.Name);
                    }
                    #endregion Generate instance creation code

                    // Property assignments code variables
                    var toDTOAssignmentsCode = string.Empty;
                    var toEntityAssignmentsCode = string.Empty;

                    #region Generate property assignments code
                    if ((dto.DTOChilds != null && dto.DTOChilds.Count > 0)
                        || dto.IsAbstract == false)
                    {
                        foreach (var property in dto.Properties)
                        {
                            bool includeAssignments = true;
                            string toDTOAssignment = null;
                            string toEntityAssignment = null;

                            if (property.IsNavigation == true)
                            {
                                // Do not map navigation properties.
                                // The developer has to map navigations manually on partial methods if needed.
                                includeAssignments = false;
                            }
                            else if (property.IsComplex == true)
                            {
                                toDTOAssignment = string.Format(Resources.AssemblerToDTOAssignmentEntityComplexProp, property.PropertyNameEDMX);
                                toEntityAssignment = string.Format(Resources.AssemblerToEntityAssignmentDTOComplexProp, property.PropertyName);
                            }
                            else
                            {
                                toDTOAssignment = string.Format(Resources.AssemblerToDTOAssignmentEntityProp, property.PropertyNameEDMX);
                                toEntityAssignment = string.Format(Resources.AssemblerToEntityAssignmentDTOProp, property.PropertyName);
                            }

                            if (includeAssignments)
                            {
                                // ToDTO assignment => dto.prop = ...
                                toDTOAssignmentsCode += Environment.NewLine
                                    + string.Format(Resources.AssemblerToDTOAssignment, property.PropertyName, toDTOAssignment);

                                // ToEntity assignment => entity.prop = ...
                                toEntityAssignmentsCode += Environment.NewLine
                                    + string.Format(Resources.AssemblerToEntityAssignment, property.PropertyNameEDMX, toEntityAssignment);
                            }
                        }
                    }
                    #endregion Generate property assignments code

                    // Get the start point where to insert the Assembler code
                    objEditPoint = objNamespace.EndPoint.CreateEditPoint();
                    objEditPoint.LineUp();
                    objEditPoint.EndOfLine();
                    objEditPoint.Insert(Environment.NewLine);

                    // Get the Assembler's code
                    string assemblerCode = dto.Assembler.GetAssemblerCode(toDTOInstanceCode, toEntityInstanceCode,
                        toDTOAssignmentsCode, toEntityAssignmentsCode);

                    // Insert Assembler's code
                    objEditPoint.Insert(assemblerCode);

                    // Format code
                    objEditPoint.StartOfDocument();
                    EditPoint endOfDocument = objNamespace.EndPoint.CreateEditPoint();
                    endOfDocument.EndOfDocument();
                    objEditPoint.SmartFormat(endOfDocument);
                
                    // Save changes to Source File Item
                    sourceFileItem.Save();

                } // END (else) if dto is abstract and does not have childs

                // Count Assemblers generated
                assemblersGenerated++;

                // Report Progress
                int progress = ((assemblersGenerated * 100) / parameters.EntitiesDTOs.Count);
                if (progress < 100)
                {
                    worker.ReportProgress(progress, new GeneratorOnProgressEventArgs(progress,
                        string.Format(Resources.Text_AssemblersGenerated, assemblersGenerated, parameters.EntitiesDTOs.Count)));
                }

                // Check Cancellation Pending
                if (GeneratorManager.CheckCancellationPending()) return;

            } // END Loop through Entities DTOs

            // Save Target Project
            parameters.TargetProject.Save();

            // Delete Template Class File
            TemplateClass.Delete();

            // Report Progress
            worker.ReportProgress(100, new GeneratorOnProgressEventArgs(100,
                string.Format(Resources.Text_AssemblersGenerated, 
                    assemblersGenerated, parameters.EntitiesDTOs.Count)));

            LogManager.LogMethodEnd();
        }
    }
}