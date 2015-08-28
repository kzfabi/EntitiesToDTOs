using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Provides all constants used in this AddIn
    /// </summary>
    internal class AppConstants
    {
        /// <summary>
        /// 
        /// Indicates to place a Code Element at the end.
        /// </summary>
        public const int PLACE_AT_THE_END = -1;

        /// <summary>
        /// EDMX Multiplicity Zero or One value.
        /// </summary>
        public const string EDMX_ASSOCIATION_MULTIPLICITY_ZERO_OR_ONE = "0..1";

        /// <summary>
        /// EDMX Multiplicity One value.
        /// </summary>
        public const string EDMX_ASSOCIATION_MULTIPLICITY_ONE = "1";

        /// <summary>
        /// EDMX Multiplicity Many value.
        /// </summary>
        public const string EDMX_ASSOCIATION_MULTIPLICITY_MANY = "*";

        /// <summary>
        /// Tab character.
        /// </summary>
        public const string TAB = "\t";
    }
}
