using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Helpers.Domain
{
    /// <summary>
    /// Specifies all the AddIn Configuration nodes used.
    /// </summary>
    internal class AddInConfigNodes
    {
        public const string Root = "EntitiesToDTOs";

        /// <summary>
        /// Parent: Root.
        /// </summary>
        public const string Updates = "Updates";
        public const string UpdatesAttrStable = "stable";
        public const string UpdatesAttrBeta = "beta";

        /// <summary>
        /// Parent: Updates.
        /// </summary>
        public const string Skipped = "Skipped";
        
        /// <summary>
        /// Parent: Skipped.
        /// </summary>
        public const string Release = "Release";
        public const string ReleaseAttrID = "id";

        /// <summary>
        /// Parent: Root.
        /// </summary>
        public const string RateInfo = "RateInfo";
        public const string RateInfoAttrReleaseID = "releaseID";
        public const string RateInfoAttrIsRated = "isRated";
        public const string RateInfoAttrLastAskedDate = "lastAskedDate";
    }
}