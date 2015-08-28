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

namespace EntitiesToDTOs.Domain
{
    internal static class XElementExtensions
    {
        /// <summary>
        /// Returns a filtered collection of the CSDL descendant elements for this element in document order. 
        /// Only elements that have a matching System.Xml.Linq.XName are included in the collection.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> DescendantsCSDL(this XElement element, XName name)
        {
            return element.Descendants("{" + (element.Document as EdmxDocument).CsdlSchemaNamespace + "}" + name);
        }

    }
}