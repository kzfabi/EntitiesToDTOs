/* EntitiesToDTOs. Copyright (c) 2012. Fabian Fernandez.
 * http://entitiestodtos.codeplex.com
 * Licensed by Common Development and Distribution License (CDDL).
 * http://entitiestodtos.codeplex.com/license
 * Fabian Fernandez. 
 * http://www.linkedin.com/in/fabianfernandezb/en
 * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Rate release window.
    /// </summary>
    internal partial class RateReleaseWindow : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RateReleaseWindow"/> class.
        /// </summary>
        public RateReleaseWindow()
        {
            try
            {
                InitializeComponent();

                this.Text = Resources.RateReleaseWindow_Caption;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        #endregion Constructors

        #region UI Events

        /// <summary>
        /// Executed when the user clicks the Rate button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRate_Click(object sender, EventArgs e)
        {
            string rateReleaseURL = string.Format(Resources.RateReleaseURL, AssemblyHelper.VersionInfo.DownloadID);

            try
            {
                RateReleaseHelper.MarkReleaseAsRated();

                // Navigate to rate release URL
                System.Diagnostics.Process.Start(rateReleaseURL);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message + Environment.NewLine + rateReleaseURL);
            }
        }

        /// <summary>
        /// Executed when the user clicks the Remind me later button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemindMeLater_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the user clicks the Already did button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAlreadyDid_Click(object sender, EventArgs e)
        {
            try
            {
                RateReleaseHelper.MarkReleaseAsRated();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        #endregion UI Events
    }
}