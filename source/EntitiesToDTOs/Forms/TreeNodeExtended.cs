using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntitiesToDTOs.Forms
{
    /// <summary>
    /// Represents a node of a System.Windows.Forms.TreeView with custom functionality.
    /// </summary>
    internal class TreeNodeExtended : TreeNode
    {
        /// <summary>
        /// Value associated with the node.
        /// </summary>
        public object Value { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeExtended"/> class with
        /// the specified label text.
        /// </summary>
        /// <param name="text">The label System.Windows.Forms.TreeNode.Text of the new tree node.</param>
        public TreeNodeExtended(string text) : base(text) { }

    }
}