using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Assembly version info found in the AssemblyInformationalVersion attribute of AssemblyInfo.cs.
    /// </summary>
    internal class AssemblyVersionInfo
    {
        /// <summary>
        /// Gets or sets the release ID.
        /// </summary>
        public int ReleaseID { get; set; }

        /// <summary>
        /// Gets or sets the download ID.
        /// </summary>
        public int DownloadID { get; set; }

        /// <summary>
        /// Gets or sets the beta suffix. Empty if it isn't a beta.
        /// </summary>
        public string BetaSuffix { get; set; }
    }
}