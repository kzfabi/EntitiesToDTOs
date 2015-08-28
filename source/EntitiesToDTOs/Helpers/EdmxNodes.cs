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
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Specifies all the EDMX Document nodes used in this AddIn.
    /// </summary>
    internal class EdmxNodes
    {
        /// <summary>
        /// Schema node.
        /// </summary>
        public static string Schema = "Schema";

        /// <summary>
        /// EntityType node.
        /// </summary>
        public static string EntityType = "EntityType";

        /// <summary>
        /// ComplexType node.
        /// </summary>
        public static string ComplexType = "ComplexType";

        /// <summary>
        /// EnumType node.
        /// </summary>
        public static string EnumType = "EnumType";

        /// <summary>
        /// Key node.
        /// </summary>
        public static string Key = "Key";

        /// <summary>
        /// Property node.
        /// </summary>
        public static string Property = "Property";

        /// <summary>
        /// PropertyRef node.
        /// </summary>
        public static string PropertyRef = "PropertyRef";

        /// <summary>
        /// NavigationProperty node.
        /// </summary>
        public static string NavigationProperty = "NavigationProperty";

        /// <summary>
        /// Association node.
        /// </summary>
        public static string Association = "Association";

        /// <summary>
        /// End node.
        /// </summary>
        public static string End = "End";
    }
}
