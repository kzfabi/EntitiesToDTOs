/* EntitiesToDTOs. Copyright (c) 2012. Fabian Fernandez.
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
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents a release.
    /// </summary>
    internal class Release
    {
        /// <summary>
        /// Identifier of the release.
        /// </summary>
        public int ReleaseID { get; set; }

        /// <summary>
        /// Version of the release.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Release status.
        /// </summary>
        public ReleaseStatus Status { get; set; }

        /// <summary>
        /// Link to release page.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// List of changes.
        /// </summary>
        public List<string> Changes { get; set; }

    }
}