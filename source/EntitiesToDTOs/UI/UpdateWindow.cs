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
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// New update available window.
    /// </summary>
    internal partial class UpdateWindow : Form
    {
        /// <summary>
        /// New release available.
        /// </summary>
        private Release NewRelease { get; set; }



        /// <summary>
        /// Creates an instance of <see cref="UpdateWindow"/>.
        /// </summary>
        /// <param name="newRelease">New release available.</param>
        public UpdateWindow(Release newRelease)
        {
            try
            {
                InitializeComponent();

                this.NewRelease = newRelease;

                this.Text = Resources.UpdateWindow_Caption;

                this.txtMessage.Text = string.Format(Resources.Info_NewRelease, this.NewRelease.Version);

                this.ShowReleaseChanges();
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        #region UI Events

        /// <summary>
        /// Executed when the user clicks the learn more button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLearnMore_Click(object sender, EventArgs e)
        {
            try
            {
                // Navigate to release URL
                System.Diagnostics.Process.Start(this.NewRelease.Link);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message + Environment.NewLine + this.NewRelease.Link);
            }
        }

        /// <summary>
        /// Executed when the user clicks the remind me later button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemindMeLater_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the user clicks the no thanks button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNoThanks_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateHelper.SkipRelease(this.NewRelease.ReleaseID);

                this.Close();
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        #endregion UI Events

        #region Methods

        /// <summary>
        /// Shows release changes.
        /// </summary>
        private void ShowReleaseChanges()
        {
            foreach (string change in this.NewRelease.Changes)
            {
                var label = new Label();
                label.Text = (Resources.UpdateWindow_ChangePrefix + change);
                label.AutoSize = true;

                this.flpReleaseChanges.Controls.Add(label);
            }
        }

        #endregion Methods

    }
}