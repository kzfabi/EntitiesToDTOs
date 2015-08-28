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
using System.Xml.Linq;
using EnvDTE;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Domain;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to the EDMX file.
    /// </summary>
    internal class EdmxHelper
    {
        /// <summary>
        /// EDMX Documents loaded.
        /// </summary>
        private static Dictionary<string, EdmxDocument> EdmxDocuments { get; set; }



        /// <summary>
        /// Initializes <see cref="EdmxHelper"/>.
        /// </summary>
        public static void Initialize()
        {
            EdmxHelper.EdmxDocuments = new Dictionary<string, EdmxDocument>();
        }

        /// <summary>
        /// Gets the EDMX Document ProjectItem as a EdmxDocument type.
        /// </summary>
        /// <param name="edmxProjectItem">EDMX Document ProjectItem source.</param>
        /// <returns></returns>
        public static EdmxDocument GetEdmxDocument(ProjectItem edmxProjectItem)
        {
            // Get EDMX full file path (key for dictionary)
            string edmxFilePath = edmxProjectItem.Properties.Item(Resources.ProjectItem_FullPath).Value.ToString();

            if (EdmxHelper.EdmxDocuments.ContainsKey(edmxFilePath) == false)
            {
                // Load EDMX Document and add it to dictionary
                EdmxHelper.EdmxDocuments.Add(edmxFilePath, EdmxDocument.LoadEdmx(edmxFilePath));
            }

            // Return EDMX Document
            return EdmxHelper.EdmxDocuments[edmxFilePath];
        }

        /// <summary>
        /// Gets the EnumType nodes from an EDMX Document.
        /// </summary>
        /// <param name="edmxDocument">EDMX Document source.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetEnumTypeNodes(EdmxDocument edmxDocument)
        {
            return edmxDocument.DescendantsCSDL(EdmxNodes.EnumType);
        }

        /// <summary>
        /// Gets the Complex Type nodes from an EDMX Document.
        /// </summary>
        /// <param name="edmxDocument">EDMX Document source.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetComplexTypeNodes(EdmxDocument edmxDocument)
        {
            return edmxDocument.DescendantsCSDL(EdmxNodes.ComplexType);
        }

        /// <summary>
        /// Gets the Entity Type nodes from an EDMX Document.
        /// </summary>
        /// <param name="edmxDocument">EDMX Document source.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetEntityTypeNodes(EdmxDocument edmxDocument)
        {
            return edmxDocument.DescendantsCSDL(EdmxNodes.EntityType);
        }

        /// <summary>
        /// Gets the Class Base Type of an EntityType node.
        /// </summary>
        /// <param name="entityNode">EntityType node to get the Class Base Type.</param>
        /// <returns></returns>
        public static string GetEntityBaseType(XElement entityNode)
        {
            string entityBaseType = null;

            if (entityNode.Attribute(EdmxNodeAttributes.EntityType_BaseType) != null)
            {
                entityBaseType = entityNode.Attribute(EdmxNodeAttributes.EntityType_BaseType).Value;

                entityBaseType = EdmxHelper.GetNameWithoutNamespace(entityBaseType);
            }

            return entityBaseType;
        }

        /// <summary>
        /// Gets the Navigation Property nodes from an EDMX Document.
        /// </summary>
        /// <param name="edmxDocument">EDMX Document source.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetNavigationPropertyNodes(EdmxDocument edmxDocument)
        {
            return edmxDocument.DescendantsCSDL(EdmxNodes.NavigationProperty);
        }

        /// <summary>
        /// Gets the Association nodes from an EDMX Document.
        /// </summary>
        /// <param name="edmxDocument">EDMX Document source.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetAssociationNodes(EdmxDocument edmxDocument)
        {
            return edmxDocument.DescendantsCSDL(EdmxNodes.Association);
        }

        /// <summary>
        /// Gets the association name of a navigation node.
        /// </summary>
        /// <param name="navigationNode">Novigation node from where to get the association name.</param>
        /// <returns></returns>
        public static string GetNavigationAssociationName(XElement navigationNode)
        {
            string associationName = navigationNode.Attribute(
                EdmxNodeAttributes.NavigationProperty_Relationship).Value;

            return EdmxHelper.GetNameWithoutNamespace(associationName);
        }

        /// <summary>
        /// Removes namespace part of a name obtained from an edmx document.
        /// </summary>
        /// <param name="fullName">Full name obtained from an edmx document.</param>
        /// <returns></returns>
        public static string GetNameWithoutNamespace(string fullName)
        {
            string[] fullNameSplitted = fullName.Split(
                new string[] { Resources.Dot }, StringSplitOptions.RemoveEmptyEntries);

            return fullNameSplitted[(fullNameSplitted.Length - 1)];
        }

        /// <summary>
        /// Gets the Entities namspace from an EDMX ProjectItem.
        /// </summary>
        /// <param name="edmxProjectItem">EDMX ProjectItem from where to find the Entities namespace.</param>
        /// <returns></returns>
        public static string GetEntitiesNamespace(ProjectItem edmxProjectItem)
        {
            // Use dynamic to get the container of the EDMX
            // It can be a Project or ProjectItem but both have "Properties" defined
            dynamic edmxContainer = (dynamic)edmxProjectItem.Collection.Parent;

            // Get project namespace
            string entitiesNamespace = VisualStudioHelper.GetDefaultNamespaceFromProperties(edmxContainer.Properties);

            if (string.IsNullOrWhiteSpace(entitiesNamespace) == true)
            {
                throw new ApplicationException(Resources.Error_EntitiesNamespaceNotFound);
            }

            return entitiesNamespace;
        }

    }
}