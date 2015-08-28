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
    /// Represents the EntitiesToDTOs AddIn general configuration.
    /// </summary>
    internal class AddInConfig
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="AddInConfig"/>.
        /// </summary>
        public AddInConfig()
        {
            this.ReleaseStatusFilter = new List<ReleaseStatus>();
            this.SkippedReleases = new List<int>();
            this.RateReleaseID = 0;
            this.IsReleaseRated = false;
            this.LastRateAskedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Release status filter to use when checking for updates.
        /// </summary>
        public List<ReleaseStatus> ReleaseStatusFilter { get; set; }

        /// <summary>
        /// IDs of skipped releases.
        /// </summary>
        public List<int> SkippedReleases { get; set; }

        /// <summary>
        /// Gets the release ID rated.
        /// </summary>
        public int RateReleaseID { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the RateReleaseID was rated by the user.
        /// </summary>
        public bool IsReleaseRated { get; set; }

        /// <summary>
        /// Gets or sets the last date when the user was asked to rate the release.
        /// </summary>
        public DateTime LastRateAskedDate { get; set; }

        #endregion Properties
    }
}