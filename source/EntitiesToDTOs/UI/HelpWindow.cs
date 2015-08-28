using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Helpers;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Help Window.
    /// </summary>
    public partial class HelpWindow : Form
    {
        /// <summary>
        /// Creates a new instance of HelpWindow.
        /// </summary>
        public HelpWindow()
        {
            InitializeComponent();

            // Set Window Caption
            this.Text = string.Format(Resources.HelpWindow_Caption, AssemblyHelper.Version);

            this.txtLicense.Text = Resources.License;
        }

        /// <summary>
        /// Executed when the user clicks the close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}