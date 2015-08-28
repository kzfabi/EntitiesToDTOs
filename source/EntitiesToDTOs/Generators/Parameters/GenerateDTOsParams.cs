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
using EnvDTE;
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Generators.Parameters
{
    /// <summary>
    /// Parameters for the DTOGenerator GenerateDTOs method.
    /// This is because it is used with BackgroundWorker, we need an object to provide all typed parameters.
    /// </summary>
    internal class GenerateDTOsParams
    {
        /// <summary>
        /// Indicates the target type, where to generate DTO's.
        /// </summary>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Project where the DTOs are going to be generated. 
        /// This is the containing project if <see cref="TargetType"/> indicates to generate inside a project folder, 
        /// </summary>
        public Project TargetProject { get; set; }

        /// <summary>
        /// Name of the target project where DTOs are going to be generated.
        /// </summary>
        public string TargetProjectName { get; set; }

        /// <summary>
        /// Project folder where DTO's are going to be generated. 
        /// Null value indicates to generate at project level.
        /// </summary>
        public ProjectItem TargetProjectFolder { get; set; }

        /// <summary>
        /// Name of the target where DTOs are going to be generated.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// EDMX Document ProjectItem containing the Entities definitions.
        /// </summary>
        public ProjectItem EDMXProjectItem { get; set; }

        /// <summary>
        /// [Used only for configuration management] 
        /// Name of the EDMX containing the Entities definitions.
        /// </summary>
        public string EDMXName { get; set; }

        /// <summary>
        /// EDMX Document containing the Entities definitions.
        /// </summary>
        public EdmxDocument EDMXDocument { get; set; }

        /// <summary>
        /// Project containing the EDMX file.
        /// </summary>
        public Project EDMXProject { get; set; }

        /// <summary>
        /// Namespace used for the entities.
        /// </summary>
        public string EntitiesNamespace { get; set; }

        /// <summary>
        /// Types from which to generate the DTOs.
        /// </summary>
        public List<string> TypesToGenerateFilter { get; set; }

        /// <summary>
        /// Indicates if all types must be generated.
        /// </summary>
        public bool GenerateAllTypes { get; set; }

        /// <summary>
        /// Indicates if all complex types must be generated.
        /// </summary>
        public bool GenerateAllComplexTypes { get; set; }

        /// <summary>
        /// Indicates if all entity types must be generated.
        /// </summary>
        public bool GenerateAllEntityTypes { get; set; }

        /// <summary>
        /// Source File Header Comment (optional).
        /// </summary>
        public string SourceFileHeaderComment { get; set; }

        /// <summary>
        /// Indicates if Target Project default namespace is going to be used.
        /// </summary>
        public bool UseProjectDefaultNamespace { get; set; }

        /// <summary>
        /// Namespace used for the DTOs.
        /// </summary>
        public string SourceNamespace { get; set; }

        /// <summary>
        /// Specifies if the DTOs are Service-Ready (this means if they can be serialized or not).
        /// </summary>
        public bool DTOsServiceReady { get; set; }

        /// <summary>
        /// Indicates use of identifier in class names.
        /// </summary>
        public ClassIdentifierUse ClassIdentifierUse { get; set; }

        /// <summary>
        /// Identifier to use in class names.
        /// </summary>
        public string ClassIdentifierWord { get; set; }

        /// <summary>
        /// Specifies the Source File Generation Type desired.
        /// </summary>
        public SourceFileGenerationType SourceFileGenerationType { get; set; }

        /// <summary>
        /// Source File Name to use if the Source File Generation Type is OneSourceFile.
        /// </summary>
        public string SourceFileName { get; set; }

        /// <summary>
        /// Class Association Type desired.
        /// </summary>
        public AssociationType AssociationType { get; set; }

        /// <summary>
        /// Indicates if DTO's Constructor methods should be created.
        /// </summary>
        public bool GenerateDTOConstructors { get; set; }

        /// <summary>
        /// All complex types.
        /// </summary>
        public IEnumerable<XElement> AllComplexTypes
        {
            get
            {
                if (_allComplexTypes == null)
                {
                    _allComplexTypes = EdmxHelper.GetComplexTypeNodes(this.EDMXDocument);
                }

                return _allComplexTypes;
            }
        }
        private IEnumerable<XElement> _allComplexTypes = null;

        /// <summary>
        /// All complex types names.
        /// </summary>
        public IEnumerable<string> AllComplexTypesNames
        {
            get
            {
                if (_allComplexTypesNames == null)
                {
                    _allComplexTypesNames = this.AllComplexTypes.Select(t =>
                        t.Attribute(EdmxNodeAttributes.EntityType_Name).Value);
                }

                return _allComplexTypesNames;
            }
        }
        private IEnumerable<string> _allComplexTypesNames = null;

        /// <summary>
        /// All entity types.
        /// </summary>
        public IEnumerable<XElement> AllEntityTypes
        {
            get
            {
                if (_allEntityTypes == null)
                {
                    _allEntityTypes = EdmxHelper.GetEntityTypeNodes(this.EDMXDocument);
                }

                return _allEntityTypes;
            }
        }
        private IEnumerable<XElement> _allEntityTypes = null;

        /// <summary>
        /// All entity types names.
        /// </summary>
        public IEnumerable<string> AllEntityTypesNames
        {
            get
            {
                if (_allEntityTypesNames == null)
                {
                    _allEntityTypesNames = this.AllComplexTypes.Select(t =>
                        t.Attribute(EdmxNodeAttributes.EntityType_Name).Value);
                }

                return _allEntityTypesNames;
            }
        }
        private IEnumerable<string> _allEntityTypesNames = null;

        /// <summary>
        /// All enum types.
        /// </summary>
        public IEnumerable<XElement> AllEnumTypes
        {
            get
            {
                if (allEnumTypes == null)
                {
                    allEnumTypes = EdmxHelper.GetEnumTypeNodes(this.EDMXDocument);
                }

                return allEnumTypes;
            }
        }
        private IEnumerable<XElement> allEnumTypes = null;

        /// <summary>
        /// All enum types names.
        /// </summary>
        public IEnumerable<string> AllEnumTypesNames
        {
            get
            {
                if (allEnumTypesNames == null)
                {
                    allEnumTypesNames = this.AllEnumTypes.Select(t =>
                        t.Attribute(EdmxNodeAttributes.EntityType_Name).Value);
                }

                return allEnumTypesNames;
            }
        }
        private IEnumerable<string> allEnumTypesNames = null;



        public GenerateDTOsParams() { }

        /// <summary>
        /// Creates a new instance of <see cref="GenerateDTOsParams"/>.
        /// </summary>
        /// <param name="targetProject">Project where the DTOs are going to be generated.</param>
        /// <param name="targetProjectFolder">Project folder where DTO's are going to be generated. 
        /// Null value indicates to generate at project level.</param>
        /// <param name="entitySource">Entity source (can be a Project or a ProjectItem representing an EDMX).</param>
        /// <param name="typesToGenerateFilter">Types from which to generate the DTOs.</param>
        /// <param name="generateAllTypes">Indicates if all types must be generated.</param>
        /// <param name="generateAllComplexTypes">Indicates if all complex types must be generated.</param>
        /// <param name="generateAllEntityTypes">Indicates if all complex types must be generated.</param>
        /// <param name="sourceFileHeaderComment">Source File Header Comment (optional).</param>
        /// <param name="useProjectDefaultNamespace">Indicates if Target Project default namespace is going to be used.</param>
        /// <param name="sourceNamespace">Namespace used for the DTOs.</param>
        /// <param name="dtosServiceReady">Specifies if the DTOs are Service-Ready (this means if they can be serialized or not).</param>
        /// <param name="classIdentifierUse">Indicates use of identifier in class names.</param>
        /// <param name="classIdentifierWord">Identifier to use in class names.</param>
        /// <param name="sourceFileGenerationType">Specifies the Source File Generation Type desired.</param>
        /// <param name="sourceFileName">Source File Name to use if the Source File Generation Type is OneSourceFile.</param>
        /// <param name="associationType">Class Association Type desired.</param>
        /// <param name="generateDTOConstructors">Indicates if DTO's Constructor methods should be created.</param>
        public GenerateDTOsParams(Project targetProject, ProjectItem targetProjectFolder, dynamic entitySource, 
            List<string> typesToGenerateFilter, bool generateAllTypes, bool generateAllComplexTypes, 
            bool generateAllEntityTypes, string sourceFileHeaderComment, bool useProjectDefaultNamespace, 
            string sourceNamespace, bool dtosServiceReady, ClassIdentifierUse classIdentifierUse,
            string classIdentifierWord, SourceFileGenerationType sourceFileGenerationType, string sourceFileName,
            AssociationType associationType, bool generateDTOConstructors)
        {
            this.TargetProject = targetProject;
            this.TargetProjectFolder = targetProjectFolder;

            this.TargetType = (this.TargetProjectFolder == null ? TargetType.Project : TargetType.ProjectFolder);

            this.EDMXProject = (entitySource is ProjectItem ? entitySource.ContainingProject : null);

            // TODO: ffernandez, use EntitySource (enum), add new property for the type and for Project
            this.EDMXProjectItem = entitySource;

            this.EDMXDocument = EdmxHelper.GetEdmxDocument(this.EDMXProjectItem);

            // TODO: ffernandez, review Project case
            this.EntitiesNamespace = EdmxHelper.GetEntitiesNamespace(entitySource);

            this.TypesToGenerateFilter = typesToGenerateFilter;
            this.GenerateAllTypes = generateAllTypes;
            this.GenerateAllComplexTypes = generateAllComplexTypes;
            this.GenerateAllEntityTypes = generateAllEntityTypes;

            this.SourceFileHeaderComment = sourceFileHeaderComment;
            this.UseProjectDefaultNamespace = useProjectDefaultNamespace;
            this.SourceNamespace = sourceNamespace;
            this.DTOsServiceReady = dtosServiceReady;
            
            this.ClassIdentifierUse = classIdentifierUse;
            this.ClassIdentifierWord = classIdentifierWord;

            if (this.ClassIdentifierUse == ClassIdentifierUse.None)
            {
                this.ClassIdentifierWord = string.Empty;
            }

            this.SourceFileGenerationType = sourceFileGenerationType;
            this.SourceFileName = sourceFileName;
            this.AssociationType = associationType;
            this.GenerateDTOConstructors = generateDTOConstructors;
        }

    }
}