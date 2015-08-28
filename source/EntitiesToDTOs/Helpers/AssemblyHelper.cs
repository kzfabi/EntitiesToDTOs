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
using System.Reflection;
using EntitiesToDTOs.Properties;
using System.IO;
using EntitiesToDTOs.Domain;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to the Assembly.
    /// </summary>
    internal class AssemblyHelper
    {
        #region Properties

        /// <summary>
        /// Assembly that contains the code that is currently executing.
        /// </summary>
        public static Assembly ExecutingAssembly
        {
            get
            {
                if (_executingAssembly == null)
                {
                    _executingAssembly = Assembly.GetExecutingAssembly();
                }

                return _executingAssembly;
            }
        }
        private static Assembly _executingAssembly = null;

        /// <summary>
        /// AssemblyName of the Assembly that contains the code that is currently executing.
        /// </summary>
        public static AssemblyName ExecutingAssemblyName
        {
            get
            {
                if (_executingAssemblyName == null)
                {
                    _executingAssemblyName = AssemblyHelper.ExecutingAssembly.GetName();
                }

                return _executingAssemblyName;
            }
        }
        private static AssemblyName _executingAssemblyName = null;

        /// <summary>
        /// Executing Assembly Version in the form of "Major.Minor{.BetaSuffix}".
        /// </summary>
        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    _version = (AssemblyHelper.ExecutingAssemblyName.Version.Major.ToString() 
                        + Resources.Dot 
                        + AssemblyHelper.ExecutingAssemblyName.Version.Minor.ToString()
                        + AssemblyHelper.VersionInfo.BetaSuffix);
                }

                return _version;
            }
        }
        private static string _version = null;

        /// <summary>
        /// Gets the assembly version info.
        /// </summary>
        public static AssemblyVersionInfo VersionInfo
        {
            get
            {
                if (versionInfo == null)
                {
                    versionInfo = AssemblyHelper.GetAssemblyVersionInfo();
                }

                return versionInfo;
            }
        }
        private static AssemblyVersionInfo versionInfo = null;

        /// <summary>
        /// Executing assembly location.
        /// </summary>
        public static string AssemblyLocation
        {
            get
            {
                return AssemblyHelper.ExecutingAssembly.Location;
            }
        }

        /// <summary>
        /// Executing assembly location.
        /// </summary>
        public static string AssemblyLocationPath
        {
            get
            {
                if (_assemblyLocationPath == null)
                {
                    _assemblyLocationPath = Path.GetDirectoryName(AssemblyHelper.ExecutingAssembly.Location) + "\\";
                }

                return _assemblyLocationPath;
            }
        }
        private static string _assemblyLocationPath = null;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the assembly version info.
        /// </summary>
        private static AssemblyVersionInfo GetAssemblyVersionInfo()
        {
            var assemblyVersionInfo = new AssemblyVersionInfo();

            var versionInfoAttr = (Attribute.GetCustomAttribute(AssemblyHelper.ExecutingAssembly,
                typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute);

            if (versionInfoAttr == null)
            {
                throw new InvalidOperationException(Resources.Error_MissingVersionInfo);
            }
            else
            {
                string[] versionInfoSplitted = versionInfoAttr.InformationalVersion
                    .Split(new string[] { Resources.VersionInfoSplitter }, StringSplitOptions.RemoveEmptyEntries);

                assemblyVersionInfo.ReleaseID = Convert.ToInt32(
                    versionInfoSplitted[0].Split(new string[] { Resources.VersionInfoReleaseID }, StringSplitOptions.None)[1]);

                assemblyVersionInfo.DownloadID = Convert.ToInt32(
                    versionInfoSplitted[1].Split(new string[] { Resources.VersionInfoDownloadID }, StringSplitOptions.None)[1]);

                // Beta suffix can be empty
                assemblyVersionInfo.BetaSuffix =
                    versionInfoSplitted[2].Split(new string[] { Resources.VersionInfoBetaSuffix }, StringSplitOptions.None)[1];
            }

            return assemblyVersionInfo;
        }

        #endregion Methods
    }
}