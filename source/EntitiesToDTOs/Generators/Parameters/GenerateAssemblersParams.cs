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
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;
using EnvDTE;

namespace EntitiesToDTOs.Generators.Parameters
{
    /// <summary>
    /// Parameters for the AssemblerGenerator GenerateAssemblers method.
    /// This is because it is used with BackgroundWorker, we need an object to provide all typed parameters.
    /// </summary>
    internal class GenerateAssemblersParams
    {
        /// <summary>
        /// Indicates the target type, where to generate Assemblers.
        /// </summary>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Project where the Assemblers are going to be generated. 
        /// This is the containing project if <see cref="TargetType"/> indicates to generate inside a project folder, 
        /// </summary>
        public Project TargetProject { get; set; }

        /// <summary>
        /// Name of the target project where Assemblers are going to be generated.
        /// </summary>
        public string TargetProjectName { get; set; }

        /// <summary>
        /// Project folder where Assemblers are going to be generated. 
        /// Null value indicates to generate at project level.
        /// </summary>
        public ProjectItem TargetProjectFolder { get; set; }

        /// <summary>
        /// Name of the target where Assemblers are going to be generated.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Source File Header Comment (optional).
        /// </summary>
        public string SourceFileHeaderComment { get; set; }

        /// <summary>
        /// Indicates if Target Project default namespace is going to be used.
        /// </summary>
        public bool UseProjectDefaultNamespace { get; set; }

        /// <summary>
        /// Namespace used for the Assemblers.
        /// </summary>
        public string SourceNamespace { get; set; }

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
        /// Specifies if it is Service-Ready (serialization is going to be used).
        /// </summary>
        public bool IsServiceReady { get; set; }

        /// <summary>
        /// List of <see cref="DTOEntity"/> to generate the Assemblers from.
        /// </summary>
        public List<DTOEntity> EntitiesDTOs { get; set; }

        /// <summary>
        /// DTO's namespace.
        /// </summary>
        public string DTOsNamespace { get; set; }

        /// <summary>
        /// DTO's target Project.
        /// </summary>
        public Project DTOsTargetProject { get; set; }

        /// <summary>
        /// Project containing the EDMX file.
        /// </summary>
        public Project EDMXProject { get; set; }

        /// <summary>
        /// Namespace used for the entities.
        /// </summary>
        public string EntitiesNamespace { get; set; }



        public GenerateAssemblersParams() { }

        /// <summary>
        /// Creates a new instance of <see cref="GenerateAssemblersParams"/>.
        /// </summary>
        /// <param name="targetProject">Project where the Assemblers are going to be generated.</param>
        /// <param name="targetProjectFolder">Project folder where Assemblers are going to be generated. 
        /// Null value indicates to generate at project level.</param>
        /// <param name="sourceFileHeaderComment">Source File Header Comment (optional).</param>
        /// <param name="useProjectDefaultNamespace">Indicates if Target Project default namespace is going to be used.</param>
        /// <param name="sourceNamespace">Namespace used for the Assemblers.</param>
        /// <param name="classIdentifierUse">Indicates use of identifier in class names.</param>
        /// <param name="classIdentifierWord">Identifier to use in class names.</param>
        /// <param name="sourceFileGenerationType">Specifies the Source File Generation Type desired.</param>
        /// <param name="sourceFileName">Source File Name to use if the Source File Generation Type is OneSourceFile.</param>
        /// <param name="isServiceReady">Specifies if the DTOs are Service-Ready (this means if they can be serialized or not).</param>
        /// <param name="dtosNamespace">DTO's namespace.</param>
        /// <param name="dtosTargetProject">DTO's target Project.</param>
        /// <param name="edmxProjectItem">EDMX ProjectItem.</param>
        public GenerateAssemblersParams(Project targetProject, ProjectItem targetProjectFolder, 
            string sourceFileHeaderComment, bool useProjectDefaultNamespace, string sourceNamespace, 
            ClassIdentifierUse classIdentifierUse, string classIdentifierWord,
            SourceFileGenerationType sourceFileGenerationType, string sourceFileName, bool isServiceReady, 
            string dtosNamespace, Project dtosTargetProject, ProjectItem edmxProjectItem)
        {
            this.TargetProject = targetProject;
            this.TargetProjectFolder = targetProjectFolder;

            // Indicate target type
            this.TargetType = (this.TargetProjectFolder == null ? TargetType.Project : TargetType.ProjectFolder);

            this.SourceFileHeaderComment = sourceFileHeaderComment;
            this.UseProjectDefaultNamespace = useProjectDefaultNamespace;
            this.SourceNamespace = sourceNamespace;

            this.ClassIdentifierUse = classIdentifierUse;
            this.ClassIdentifierWord = classIdentifierWord;

            if (this.ClassIdentifierUse == ClassIdentifierUse.None)
            {
                this.ClassIdentifierWord = string.Empty;
            }

            this.SourceFileGenerationType = sourceFileGenerationType;
            this.SourceFileName = sourceFileName;
            this.IsServiceReady = isServiceReady;
            this.DTOsNamespace = dtosNamespace;
            this.DTOsTargetProject = dtosTargetProject;
            this.EDMXProject = edmxProjectItem.ContainingProject;

            this.EntitiesNamespace = EdmxHelper.GetEntitiesNamespace(edmxProjectItem);
        }
    }
}