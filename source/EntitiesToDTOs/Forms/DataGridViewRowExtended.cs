using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntitiesToDTOs.Forms
{
    /// <summary>
    /// Represents a row of a System.Windows.Forms.DataGridView with custom functionality.
    /// </summary>
    internal class DataGridViewRowExtended : DataGridViewRow
    {
        /// <summary>
        /// Value associated with the node.
        /// </summary>
        public object Value { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewRowExtended"/> class without using a template.
        /// </summary>
        public DataGridViewRowExtended() : base() { }

    }
}