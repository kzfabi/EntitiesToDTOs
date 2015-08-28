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
using EntitiesToDTOs.Generators.Parameters;
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using System.IO;
using EntitiesToDTOs.Domain;
using EnvDTE;
using EntitiesToDTOs.Domain.Enums;
using EntitiesToDTOs.Helpers.Domain;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to the User Configuration.
    /// </summary>
    internal class ConfigurationHelper
    {
        /// <summary>
        /// Gets the generation configuration temporary file path.
        /// </summary>
        public static string GenConfigTempFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tempFilePath) == true)
                {
                    _tempFilePath = Path.GetTempPath()
                        + Resources.EntitiesToDTOsTempFolderName
                        + Resources.GenConfigTempFileName 
                        + Resources.GenConfigFileExtension;
                }

                return _tempFilePath;
            }
        }
        private static string _tempFilePath;

        /// <summary>
        /// Gets the AddIn configuration file path.
        /// </summary>
        private static string AddInConfigFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_addInConfigFilePath) == true)
                {
                    _addInConfigFilePath = Path.GetTempPath() 
                        + Resources.EntitiesToDTOsTempFolderName 
                        + Resources.AddInConfigFileName;
                }

                return _addInConfigFilePath;
            }
        }
        private static string _addInConfigFilePath;

        /// <summary>
        /// Represents the loaded EntitiesToDTOs AddIn general configuration.
        /// </summary>
        private static AddInConfig AddInConfig { get; set; }



        /// <summary>
        /// Gets the EntitiesToDTOs AddIn general configuration.
        /// </summary>
        /// <returns></returns>
        public static AddInConfig GetAddInConfig(bool forceLoad = false)
        {
            if (forceLoad == true)
            {
                ConfigurationHelper.AddInConfig = new AddInConfig();

                if (File.Exists(ConfigurationHelper.AddInConfigFilePath) == false)
                {
                    // Set default configuration
                    ConfigurationHelper.AddInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Stable);
                    ConfigurationHelper.AddInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Beta);
                    ConfigurationHelper.AddInConfig.RateReleaseID = AssemblyHelper.VersionInfo.ReleaseID;

                    ConfigurationHelper.SaveAddInConfig(ConfigurationHelper.AddInConfig);
                }
                else
                {
                    XDocument configDoc = XDocument.Load(ConfigurationHelper.AddInConfigFilePath);

                    XElement updatesNode = configDoc.Descendants(AddInConfigNodes.Updates).First();

                    // Set Stable release status filter if checked
                    if (Convert.ToBoolean(updatesNode.Attribute(AddInConfigNodes.UpdatesAttrStable).Value) == true)
                    {
                        ConfigurationHelper.AddInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Stable);
                    }

                    // Set Beta release status filter if checked
                    if (Convert.ToBoolean(updatesNode.Attribute(AddInConfigNodes.UpdatesAttrBeta).Value) == true)
                    {
                        ConfigurationHelper.AddInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Beta);
                    }

                    // Get skipped releases
                    IEnumerable<XElement> skippedReleases = updatesNode.Descendants(AddInConfigNodes.Release);
                    foreach (XElement release in skippedReleases)
                    {
                        ConfigurationHelper.AddInConfig.SkippedReleases.Add(
                            Convert.ToInt32(release.Attribute(AddInConfigNodes.ReleaseAttrID).Value));
                    }

                    XElement rateInfoNode = configDoc.Descendants(AddInConfigNodes.RateInfo).FirstOrDefault();
                    if (rateInfoNode == null)
                    {
                        // Old AddIn config where rate info node is missing
                        ConfigurationHelper.AddInConfig.RateReleaseID = AssemblyHelper.VersionInfo.ReleaseID;

                        ConfigurationHelper.SaveAddInConfig(ConfigurationHelper.AddInConfig);
                    }
                    else
                    {
                        ConfigurationHelper.AddInConfig.RateReleaseID = Convert.ToInt32(
                            rateInfoNode.Attribute(AddInConfigNodes.RateInfoAttrReleaseID).Value);

                        ConfigurationHelper.AddInConfig.IsReleaseRated =
                            (rateInfoNode.Attribute(AddInConfigNodes.RateInfoAttrIsRated).Value == Resources.XmlBoolTrue);

                        string lastRateAskedDateValue = rateInfoNode.Attribute(AddInConfigNodes.RateInfoAttrLastAskedDate).Value;

                        if (lastRateAskedDateValue.Length > 8)
                        {
                            // Correct previous versions invalid last rate asked date.
                            lastRateAskedDateValue = DateTime.Now.ToString("yyyyMMdd");

                            ConfigurationHelper.SaveAddInConfig(ConfigurationHelper.AddInConfig);
                        }

                        int lastRateAskedYear = Convert.ToInt32(lastRateAskedDateValue.Substring(0, 4));
                        int lastRateAskedMonth = Convert.ToInt32(lastRateAskedDateValue.Substring(4, 2));
                        int lastRateAskedDay = Convert.ToInt32(lastRateAskedDateValue.Substring(6, 2));
                        
                        ConfigurationHelper.AddInConfig.LastRateAskedDate =
                            new DateTime(lastRateAskedYear, lastRateAskedMonth, lastRateAskedDay);
                    }
                }
            }

            // Return general configuration
            return ConfigurationHelper.AddInConfig;
        }

        /// <summary>
        /// Saves the received EntitiesToDTOs AddIn general configuration.
        /// </summary>
        /// <param name="addInConfig">Configuration to save.</param>
        public static void SaveAddInConfig(AddInConfig addInConfig)
        {
            // Update config
            ConfigurationHelper.AddInConfig = addInConfig;

            // Generate general configuration file
            var configDoc = new XDocument();
            configDoc.Add(
                new XElement(AddInConfigNodes.Root,
                    new XElement(AddInConfigNodes.Updates, 
                        new XAttribute(AddInConfigNodes.UpdatesAttrStable, addInConfig.ReleaseStatusFilter.Contains(ReleaseStatus.Stable)),
                        new XAttribute(AddInConfigNodes.UpdatesAttrBeta, addInConfig.ReleaseStatusFilter.Contains(ReleaseStatus.Beta)), 
                        new XElement(AddInConfigNodes.Skipped)
                    ),
                    new XElement(AddInConfigNodes.RateInfo,
                        new XAttribute(AddInConfigNodes.RateInfoAttrReleaseID, addInConfig.RateReleaseID),
                        new XAttribute(AddInConfigNodes.RateInfoAttrIsRated, addInConfig.IsReleaseRated),
                        new XAttribute(AddInConfigNodes.RateInfoAttrLastAskedDate, addInConfig.LastRateAskedDate.ToString("yyyyMMdd"))
                    )
                )
            );

            if (addInConfig.SkippedReleases.Count > 0)
            {
                // Get skipped releases node
                XElement skippedReleasesNode = configDoc.Descendants(AddInConfigNodes.Skipped).First();

                // Add skipped releases
                foreach (int releaseID in addInConfig.SkippedReleases)
                {
                    skippedReleasesNode.Add(new XElement(AddInConfigNodes.Release,
                        new XAttribute(AddInConfigNodes.ReleaseAttrID, releaseID)));
                }
            }

            // Save general configuration file
            configDoc.Save(ConfigurationHelper.AddInConfigFilePath);
        }

        /// <summary>
        /// Gets target name of <see cref="GenerateDTOsParams"/>.
        /// </summary>
        /// <param name="dtosParams">DTO parameters from where to obtain the target name.</param>
        /// <returns></returns>
        private static string GetTargetName(GenerateDTOsParams dtosParams)
        {
            return ConfigurationHelper.GetTargetName(dtosParams.TargetType,
                dtosParams.TargetProject, dtosParams.TargetProjectFolder);
        }

        /// <summary>
        /// Gets target name of <see cref="GenerateAssemblersParams"/>.
        /// </summary>
        /// <param name="assemblersParams">Assembler parameters from where to obtain the target name.</param>
        /// <returns></returns>
        private static string GetTargetName(GenerateAssemblersParams assemblersParams)
        {
            return ConfigurationHelper.GetTargetName(assemblersParams.TargetType,
                assemblersParams.TargetProject, assemblersParams.TargetProjectFolder);
        }

        /// <summary>
        /// Gets the target name based on the parameters received.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="targetProject">Target project.</param>
        /// <param name="targetProjectFolder">Target project folder (can be null if target type is project).</param>
        /// <returns></returns>
        private static string GetTargetName(TargetType targetType, 
            Project targetProject, ProjectItem targetProjectFolder)
        {
            if (targetType == TargetType.Project)
            {
                return targetProject.Name;
            }
            else
            {
                return ConfigurationHelper.GetTargetName(targetProjectFolder);
            }
        }

        /// <summary>
        /// Gets the target name of a project folder.
        /// </summary>
        /// <param name="projectFolder">Project folder from where to obtain the target name.</param>
        /// <returns></returns>
        private static string GetTargetName(ProjectItem projectFolder)
        {
            if (projectFolder.Collection.Parent is ProjectItem)
            {
                return ConfigurationHelper.GetTargetName(projectFolder.Collection.Parent as ProjectItem)
                    + Resources.ProjectFolderLevelSeparator
                    + projectFolder.Name;
            }
            else
            {
                return projectFolder.Name;
            }
        }

        /// <summary>
        /// Exports the User Configuration to the specified file path.
        /// </summary>
        /// <param name="generatorParams">GeneratorManagerParams containing necessary information.</param>
        /// <param name="filePath">File path to export.</param>
        public static void Export(GeneratorManagerParams generatorParams, string filePath)
        {
            if (filePath.EndsWith(Resources.GenConfigFileExtension) == false)
            {
                filePath += Resources.GenConfigFileExtension;
            }

            var exportDoc = new XDocument();
            exportDoc.Add(
                new XElement(ConfigNodes.Root,
                    new XElement(ConfigNodes.GenerateAssemblers, generatorParams.GenerateAssemblers),
                    new XElement(ConfigNodes.DTOsConfig,
                        new XElement(ConfigNodes.Target,
                            new XAttribute(ConfigNodes.TargetAttrProject, generatorParams.DTOsParams.TargetProject.Name),
                            new XAttribute(ConfigNodes.TargetAttrName, ConfigurationHelper.GetTargetName(generatorParams.DTOsParams)),
                            new XAttribute(ConfigNodes.TargetAttrType, generatorParams.DTOsParams.TargetType.ToString())),
                        new XElement(ConfigNodes.EDMXName, generatorParams.DTOsParams.EDMXProjectItem.Name),
                        new XElement(ConfigNodes.GenerateFilter,
                            new XAttribute(ConfigNodes.GenerateFilterAttrAll, generatorParams.DTOsParams.GenerateAllTypes),
                            new XAttribute(ConfigNodes.GenerateFilterAttrComplex, generatorParams.DTOsParams.GenerateAllComplexTypes),
                            new XAttribute(ConfigNodes.GenerateFilterAttrEntities, generatorParams.DTOsParams.GenerateAllEntityTypes)),
                        new XElement(ConfigNodes.SourceFileHeaderComment, generatorParams.DTOsParams.SourceFileHeaderComment),
                        new XElement(ConfigNodes.UseProjectDefaultNamespace, generatorParams.DTOsParams.UseProjectDefaultNamespace),
                        new XElement(ConfigNodes.SourceNamespace, generatorParams.DTOsParams.SourceNamespace),
                        new XElement(ConfigNodes.DTOsServiceReady, generatorParams.DTOsParams.DTOsServiceReady),
                        new XElement(ConfigNodes.SourceFileGenerationType, generatorParams.DTOsParams.SourceFileGenerationType.ToString()),
                        new XElement(ConfigNodes.SourceFileName, generatorParams.DTOsParams.SourceFileName),
                        new XElement(ConfigNodes.AssociationType, generatorParams.DTOsParams.AssociationType.ToString()),
                        new XElement(ConfigNodes.GenerateDTOConstructors, generatorParams.DTOsParams.GenerateDTOConstructors),
                        new XElement(ConfigNodes.ClassIdentifier, 
                            new XAttribute(ConfigNodes.ClassIdentifierAttrUse, generatorParams.DTOsParams.ClassIdentifierUse.ToString()),
                            new XAttribute(ConfigNodes.ClassIdentifierAttrWord, generatorParams.DTOsParams.ClassIdentifierWord)))));

            XElement generateFilterNode = exportDoc.Descendants(ConfigNodes.GenerateFilter).First();
            foreach (string typeName in generatorParams.DTOsParams.TypesToGenerateFilter)
            {
                generateFilterNode.Add(new XElement(ConfigNodes.GenerateType,
                    new XAttribute(ConfigNodes.GenerateTypeAttrName, typeName))
                );
            }

            if (generatorParams.GenerateAssemblers)
            {
                exportDoc.Descendants(ConfigNodes.Root).First().Add(
                    new XElement(ConfigNodes.AssemblersConfig,
                        new XElement(ConfigNodes.Target,
                            new XAttribute(ConfigNodes.TargetAttrProject, generatorParams.AssemblersParams.TargetProject.Name),
                            new XAttribute(ConfigNodes.TargetAttrName, ConfigurationHelper.GetTargetName(generatorParams.AssemblersParams)),
                            new XAttribute(ConfigNodes.TargetAttrType, generatorParams.AssemblersParams.TargetType.ToString())),
                        new XElement(ConfigNodes.SourceFileHeaderComment, generatorParams.AssemblersParams.SourceFileHeaderComment),
                        new XElement(ConfigNodes.UseProjectDefaultNamespace, generatorParams.AssemblersParams.UseProjectDefaultNamespace),
                        new XElement(ConfigNodes.SourceNamespace, generatorParams.AssemblersParams.SourceNamespace),
                        new XElement(ConfigNodes.SourceFileGenerationType, generatorParams.AssemblersParams.SourceFileGenerationType.ToString()),
                        new XElement(ConfigNodes.SourceFileName, generatorParams.AssemblersParams.SourceFileName),
                        new XElement(ConfigNodes.IsServiceReady, generatorParams.AssemblersParams.IsServiceReady),
                        new XElement(ConfigNodes.ClassIdentifier,
                            new XAttribute(ConfigNodes.ClassIdentifierAttrUse, generatorParams.AssemblersParams.ClassIdentifierUse.ToString()),
                            new XAttribute(ConfigNodes.ClassIdentifierAttrWord, generatorParams.AssemblersParams.ClassIdentifierWord))));
            }

            exportDoc.Save(filePath);
        }

        /// <summary>
        /// Imports a User Configuration from the specified file path.
        /// </summary>
        /// <param name="filePath">File path from where to read the User Configuration.</param>
        /// <returns></returns>
        public static GeneratorManagerParams Import(string filePath)
        {
            // Prepare result
            var result = new GeneratorManagerParams();
            result.DTOsParams = new GenerateDTOsParams();
            result.AssemblersParams = new GenerateAssemblersParams();

            // Load the config file
            var configXML = XDocument.Load(filePath);

            // Get general config
            result.GenerateAssemblers = (configXML.Descendants(ConfigNodes.GenerateAssemblers).First().Value == Resources.XmlBoolTrue);

            #region DTOs
            // Get the DTOs config node
            XElement dtosConfigNode = configXML.Descendants(ConfigNodes.DTOsConfig).First();
            result.DTOsParams.EDMXName = dtosConfigNode.Descendants(ConfigNodes.EDMXName).First().Value;
            result.DTOsParams.SourceFileHeaderComment = dtosConfigNode.Descendants(ConfigNodes.SourceFileHeaderComment).First().Value;
            result.DTOsParams.UseProjectDefaultNamespace = (dtosConfigNode.Descendants(ConfigNodes.UseProjectDefaultNamespace).First().Value == Resources.XmlBoolTrue);
            result.DTOsParams.SourceNamespace = dtosConfigNode.Descendants(ConfigNodes.SourceNamespace).First().Value;
            result.DTOsParams.DTOsServiceReady = (dtosConfigNode.Descendants(ConfigNodes.DTOsServiceReady).First().Value == Resources.XmlBoolTrue);
            result.DTOsParams.SourceFileName = dtosConfigNode.Descendants(ConfigNodes.SourceFileName).First().Value;
            result.DTOsParams.GenerateDTOConstructors = (dtosConfigNode.Descendants(ConfigNodes.GenerateDTOConstructors).First().Value == Resources.XmlBoolTrue);

            // Source file generation type
            XElement sourceFileGenerationTypeNodeDTOs = dtosConfigNode.Descendants(ConfigNodes.SourceFileGenerationType).First();
            if (sourceFileGenerationTypeNodeDTOs.Value == SourceFileGenerationType.SourceFilePerClass.ToString())
            {
                result.DTOsParams.SourceFileGenerationType = SourceFileGenerationType.SourceFilePerClass;
            }
            else
            {
                result.DTOsParams.SourceFileGenerationType = SourceFileGenerationType.OneSourceFile;
            }

            // Association type
            XElement associationTypeNode = dtosConfigNode.Descendants(ConfigNodes.AssociationType).First();
            if (associationTypeNode.Value == AssociationType.KeyProperty.ToString())
            {
                result.DTOsParams.AssociationType = AssociationType.KeyProperty;
            }
            else
            {
                result.DTOsParams.AssociationType = AssociationType.ClassType;
            }

            // Set generate filter defaults
            result.DTOsParams.GenerateAllTypes = true;
            result.DTOsParams.GenerateAllComplexTypes = true;
            result.DTOsParams.GenerateAllEntityTypes = true;
            result.DTOsParams.TypesToGenerateFilter = new List<string>();

            // Generate filter node exists?
            XElement genFilterNode = dtosConfigNode.Descendants(ConfigNodes.GenerateFilter).FirstOrDefault();
            if (genFilterNode != null)
            {
                // Get generation flags
                result.DTOsParams.GenerateAllTypes = 
                    (genFilterNode.Attribute(ConfigNodes.GenerateFilterAttrAll).Value == Resources.XmlBoolTrue);

                result.DTOsParams.GenerateAllComplexTypes = 
                    (genFilterNode.Attribute(ConfigNodes.GenerateFilterAttrComplex).Value == Resources.XmlBoolTrue);

                result.DTOsParams.GenerateAllEntityTypes = 
                    (genFilterNode.Attribute(ConfigNodes.GenerateFilterAttrEntities).Value == Resources.XmlBoolTrue);

                // Get filter types
                foreach (XElement generateTypeNode in genFilterNode.Descendants(ConfigNodes.GenerateType))
                {
                    result.DTOsParams.TypesToGenerateFilter.Add(
                        generateTypeNode.Attribute(ConfigNodes.GenerateTypeAttrName).Value
                    );
                }
            }

            // Set DTOs identifier defaults
            result.DTOsParams.ClassIdentifierUse = ClassIdentifierUse.Suffix;
            result.DTOsParams.ClassIdentifierWord = "DTO";

            // DTO identifier node exists?
            XElement dtosIdentifierNode = dtosConfigNode.Descendants(ConfigNodes.ClassIdentifier).FirstOrDefault();
            if (dtosIdentifierNode != null)
            {
                string dtoIdentifierUseValue = dtosIdentifierNode.Attribute(ConfigNodes.ClassIdentifierAttrUse).Value;

                if (dtoIdentifierUseValue == ClassIdentifierUse.None.ToString())
                {
                    result.DTOsParams.ClassIdentifierUse = ClassIdentifierUse.None;
                }
                else if (dtoIdentifierUseValue == ClassIdentifierUse.Prefix.ToString())
                {
                    result.DTOsParams.ClassIdentifierUse = ClassIdentifierUse.Prefix;
                }

                result.DTOsParams.ClassIdentifierWord = dtosIdentifierNode.Attribute(ConfigNodes.ClassIdentifierAttrWord).Value;
            }

            // Get target info
            XElement targetNodeDTOs = dtosConfigNode.Descendants(ConfigNodes.Target).FirstOrDefault();
            if (targetNodeDTOs == null)
            {
                // Legacy config (v2.* and older)
                targetNodeDTOs = dtosConfigNode.Descendants(ConfigNodes.TargetProjectName).First();
                result.DTOsParams.TargetType = TargetType.Project;
                result.DTOsParams.TargetProjectName = targetNodeDTOs.Value;
                result.DTOsParams.TargetName = targetNodeDTOs.Value;
            }
            else
            {
                if (targetNodeDTOs.Attribute(ConfigNodes.TargetAttrType).Value == TargetType.Project.ToString())
                {
                    result.DTOsParams.TargetType = TargetType.Project;
                }
                else
                {
                    result.DTOsParams.TargetType = TargetType.ProjectFolder;
                }

                result.DTOsParams.TargetProjectName = targetNodeDTOs.Attribute(ConfigNodes.TargetAttrProject).Value;
                result.DTOsParams.TargetName = targetNodeDTOs.Attribute(ConfigNodes.TargetAttrName).Value;
            }
            #endregion DTOs

            #region Assemblers
            if (result.GenerateAssemblers)
            {
                // Get the Assemblers config node
                XElement assemblersConfigNode = configXML.Descendants(ConfigNodes.AssemblersConfig).First();
                result.AssemblersParams.SourceFileHeaderComment = assemblersConfigNode.Descendants(ConfigNodes.SourceFileHeaderComment).First().Value;
                result.AssemblersParams.UseProjectDefaultNamespace = (assemblersConfigNode.Descendants(ConfigNodes.UseProjectDefaultNamespace).First().Value == Resources.XmlBoolTrue);
                result.AssemblersParams.SourceNamespace = assemblersConfigNode.Descendants(ConfigNodes.SourceNamespace).First().Value;
                result.AssemblersParams.SourceFileName = assemblersConfigNode.Descendants(ConfigNodes.SourceFileName).First().Value;
                result.AssemblersParams.IsServiceReady = (assemblersConfigNode.Descendants(ConfigNodes.IsServiceReady).First().Value == Resources.XmlBoolTrue);

                // Source file generation type
                XElement sourceFileGenerationTypeNodeAssemblers = assemblersConfigNode.Descendants(ConfigNodes.SourceFileGenerationType).First();
                if (sourceFileGenerationTypeNodeAssemblers.Value == SourceFileGenerationType.SourceFilePerClass.ToString())
                {
                    result.AssemblersParams.SourceFileGenerationType = SourceFileGenerationType.SourceFilePerClass;
                }
                else
                {
                    result.AssemblersParams.SourceFileGenerationType = SourceFileGenerationType.OneSourceFile;
                }

                // Set Assemblers identifier defaults
                result.AssemblersParams.ClassIdentifierUse = ClassIdentifierUse.Suffix;
                result.AssemblersParams.ClassIdentifierWord = "Assembler";

                // Assembler identifier node exists?
                XElement assemblersIdentifierNode = assemblersConfigNode.Descendants(ConfigNodes.ClassIdentifier).FirstOrDefault();
                if (assemblersIdentifierNode != null)
                {
                    string assemblersIdentifierUseValue = assemblersIdentifierNode.Attribute(ConfigNodes.ClassIdentifierAttrUse).Value;

                    if (assemblersIdentifierUseValue == ClassIdentifierUse.None.ToString())
                    {
                        result.AssemblersParams.ClassIdentifierUse = ClassIdentifierUse.None;
                    }
                    else if (assemblersIdentifierUseValue == ClassIdentifierUse.Prefix.ToString())
                    {
                        result.AssemblersParams.ClassIdentifierUse = ClassIdentifierUse.Prefix;
                    }

                    result.AssemblersParams.ClassIdentifierWord = assemblersIdentifierNode.Attribute(ConfigNodes.ClassIdentifierAttrWord).Value;
                }

                // Get target info
                XElement targetNodeAssemblers = assemblersConfigNode.Descendants(ConfigNodes.Target).FirstOrDefault();
                if (targetNodeAssemblers == null)
                {
                    // Legacy config (v2.* and older)
                    targetNodeAssemblers = assemblersConfigNode.Descendants(ConfigNodes.TargetProjectName).First();
                    result.AssemblersParams.TargetType = TargetType.Project;
                    result.AssemblersParams.TargetProjectName = targetNodeAssemblers.Value;
                    result.AssemblersParams.TargetName = targetNodeAssemblers.Value;
                }
                else
                {
                    if (targetNodeAssemblers.Attribute(ConfigNodes.TargetAttrType).Value == TargetType.Project.ToString())
                    {
                        result.AssemblersParams.TargetType = TargetType.Project;
                    }
                    else
                    {
                        result.AssemblersParams.TargetType = TargetType.ProjectFolder;
                    }

                    result.AssemblersParams.TargetProjectName = targetNodeAssemblers.Attribute(ConfigNodes.TargetAttrProject).Value;
                    result.AssemblersParams.TargetName = targetNodeAssemblers.Attribute(ConfigNodes.TargetAttrName).Value;
                }
            }
            #endregion Assemblers

            // Return the configuration
            return result;
        }
    }
}