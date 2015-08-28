using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Specifies all the EDMX Document Node Attributes used in this AddIn.
    /// </summary>
    internal class EdmxNodeAttributes
    {
        #region Schema node

        /// <summary>
        /// Schema node, Namespace attribute.
        /// </summary>
        public static string Schema_Namespace = "Namespace";

        #endregion Schema node

        #region EntityType node

        /// <summary>
        /// EntityType node, Name attribute.
        /// </summary>
        public static string EntityType_Name = "Name";
        
        /// <summary>
        /// EntityType node, BaseType attribute.
        /// </summary>
        public static string EntityType_BaseType = "BaseType";
        
        /// <summary>
        /// EntityType node, Abstract attribute.
        /// </summary>
        public static string EntityType_Abstract = "Abstract";

        #endregion EntityType node

        #region EnumType node

        public static string EnumType_ExternalTypeName = "{http://schemas.microsoft.com/ado/2006/04/codegeneration}ExternalTypeName";

        #endregion EnumType node

        #region Property node

        /// <summary>
        /// Property node, Type attribute.
        /// </summary>
        public static string Property_Type = "Type";
        /// <summary>
        /// Property node, Name attribute.
        /// </summary>
        public static string Property_Name = "Name";
        /// <summary>
        /// Property node, Nullable attribute.
        /// </summary>
        public static string Property_Nullable = "Nullable";

        #endregion Property node

        #region PropertyRef node

        /// <summary>
        /// PropertyRef node, Name attribute.
        /// </summary>
        public static string PropertyRef_Name = "Name";

        #endregion PropertyRef node

        #region NavigationProperty node

        /// <summary>
        /// NavigationProperty node, Name attribute.
        /// </summary>
        public static string NavigationProperty_Name = "Name";

        /// <summary>
        /// NavigationProperty node, FromRole attribute.
        /// </summary>
        public static string NavigationProperty_FromRole = "FromRole";

        /// <summary>
        /// NavigationProperty node, ToRole attribute.
        /// </summary>
        public static string NavigationProperty_ToRole = "ToRole";

        /// <summary>
        /// NavigationProperty node, Relationship attribute.
        /// </summary>
        public static string NavigationProperty_Relationship = "Relationship";

        #endregion NavigationProperty node

        #region Association node

        /// <summary>
        /// Association node, Name attribute.
        /// </summary>
        public static string Association_Name = "Name";

        #endregion Association node

        #region End node

        /// <summary>
        /// End node, Type attribute.
        /// </summary>
        public static string End_Type = "Type";

        /// <summary>
        /// End node, Role attribute.
        /// </summary>
        public static string End_Role = "Role";

        /// <summary>
        /// End node, Multiplicity attribute.
        /// </summary>
        public static string End_Multiplicity = "Multiplicity";

        #endregion End node
    }
}