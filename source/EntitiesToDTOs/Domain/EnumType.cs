﻿/* EntitiesToDTOs. Copyright (c) 2013. Fabian Fernandez.
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

namespace EntitiesToDTOs.Domain
{
    /// <summary>
    /// Represents an EnumType available in the model.
    /// </summary>
    internal class EnumType
    {
        public string Name { get; set; }

        public string ExternalTypeName { get; set; }

        public bool IsExternal
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ExternalTypeName) == false;
            }
        }

        public string Namespace
        {
            get
            {
                string ns = null;

                if (this.IsExternal)
                {
                    ns = this.ExternalTypeName;
                    
                    int dotLastIndex = ns.LastIndexOf('.');
                    if (dotLastIndex > 0)
                    {
                        ns = ns.Substring(0, dotLastIndex);
                    }
                }

                return ns;
            }
        }
    }
}