using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Specifies all the User Configuration nodes used.
    /// </summary>
    internal class ConfigNodes
    {
        public static string Root = "EntitiesToDTOsConfiguration";

        public static string GenerateAssemblers = "GenerateAssemblers";

        public static string DTOsConfig = "DTOsConfig";

        [Obsolete("From v3.0 use Target for node and TargetAttrProject/TargetAttrName/TargetAttrType for attributes.")]
        public static string TargetProjectName = "TargetProjectName";
        public static string Target = "Target";
        public static string TargetAttrProject = "project";
        public static string TargetAttrName = "name";
        public static string TargetAttrType = "type";

        public static string EDMXName = "EDMXName";

        public static string GenerateFilter = "GenerateFilter";
        public static string GenerateFilterAttrAll = "all";
        public static string GenerateFilterAttrComplex = "complex";
        public static string GenerateFilterAttrEntities = "entities";

        public static string GenerateType = "GenerateType";
        public static string GenerateTypeAttrName = "name";

        public static string SourceFileHeaderComment = "SourceFileHeaderComment";

        public static string UseProjectDefaultNamespace = "UseProjectDefaultNamespace";

        public static string SourceNamespace = "SourceNamespace";

        public static string DTOsServiceReady = "DTOsServiceReady";

        public static string SourceFileGenerationType = "SourceFileGenerationType";

        public static string SourceFileName = "SourceFileName";

        public static string AssociationType = "AssociationType";

        public static string AssemblersConfig = "AssemblersConfig";

        public static string IsServiceReady = "IsServiceReady";

        public static string GenerateDTOConstructors = "GenerateDTOConstructors";

        public static string ClassIdentifier = "ClassIdentifier";
        public static string ClassIdentifierAttrUse = "use";
        public static string ClassIdentifierAttrWord = "word";

    }
}