using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Events;
using EntitiesToDTOs.Generators;
using EntitiesToDTOs.Helpers;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Progress Window, shows the progress of the generation.
    /// </summary>
    public partial class GenerationProgressWindow : Form
    {
        /// <summary>
        /// Creates a new instance of GenerationProgressWindow.
        /// </summary>
        /// <param name="edmxName">Name of the EDMX File that is used to generate the DTOs.</param>
        public GenerationProgressWindow(string edmxName)
        {
            InitializeComponent();

            // Set Window Caption
            this.Text = string.Format(Resources.GenerationProgressWindow_Caption, edmxName, AssemblyHelper.Version);

            // Set initial status message
            this.lblStatus.Text = Resources.Text_PleaseWait;

            // Attach to GeneratorManager events
            GeneratorManager.OnProgress += new EventHandler<GeneratorOnProgressEventArgs>(GeneratorManager_OnProgress);
            GeneratorManager.OnException += new EventHandler<GeneratorOnExceptionEventArgs>(GeneratorManager_OnException);
        }

        /// <summary>
        /// Executed when the User clicks the Cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel the current generation
            GeneratorManager.CancelGeneration();
            
            // Close this Window
            this.Close();
        }

        /// <summary>
        /// Executed when <see cref="GeneratorManager"/> fires an OnException event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorManager_OnException(object sender, GeneratorOnExceptionEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the <see cref="GeneratorManager"/> fires an OnProgress event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorManager_OnProgress(object sender, GeneratorOnProgressEventArgs e)
        {
            this.lblStatus.Text = e.StatusMessage;
            this.progressBar.Value = e.Progress;
        }
    }
}