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
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an Edmx Document.
    /// </summary>
    internal class EdmxDocument : XDocument
    {
        /// <summary>
        /// CSDL Schema namespace.
        /// </summary>
        public string CsdlSchemaNamespace
        {
            get
            {
                if (this.csdlSchemaNamespace == null)
                {
                    this.csdlSchemaNamespace = this.GetCsdlNamespace();
                }

                return this.csdlSchemaNamespace;
            }
        }
        private string csdlSchemaNamespace = null;


        private EdmxDocument(XDocument xdoc)
            : base(xdoc) { }


        /// <summary>
        /// Creates a new <see cref="EntitiesToDTOs.Domain.EdmxDocument"/> from a file.
        /// </summary>
        /// <param name="filePath">A URI string that references the file to load into a new EdmxDocument.</param>
        /// <returns></returns>
        public static EdmxDocument LoadEdmx(string filePath)
        {
            return new EdmxDocument(XDocument.Load(filePath));
        }


        /// <summary>
        /// Gets the CSDL Schema node namespace.
        /// </summary>
        /// <returns></returns>
        private string GetCsdlNamespace()
        {
            try
            {
                // Get edmx namespace
                XNamespace edmxNamespace = this.Root.GetNamespaceOfPrefix("edmx");

                // Get ConceptualModels node
                XElement cmNode = this.Descendants("{" + edmxNamespace.NamespaceName + "}ConceptualModels").First();

                // Get Schema node
                XElement schemaNode = cmNode.Descendants().First();

                // Get CSDL Schema node namespace
                return schemaNode.Attribute("xmlns").Value;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                throw new ApplicationException(Resources.Error_CsdlSchemaNamespaceMissing, ex);
            }
        }

        /// <summary>
        /// Returns a filtered collection of the CSDL descendant elements for this EDMX document in document order. 
        /// Only elements that have a matching System.Xml.Linq.XName are included in the collection.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable[T] of System.Xml.Linq.XElement containing the 
        /// descendant elements of the System.Xml.Linq.XContainer that match the specified System.Xml.Linq.XName.
        /// </returns>
        public IEnumerable<XElement> DescendantsCSDL(XName name)
        {
            return this.Descendants("{" + this.CsdlSchemaNamespace + "}" + name);
        }

    }
}