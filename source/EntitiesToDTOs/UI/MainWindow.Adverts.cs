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
using System.Linq;
using System.Text;
using System.Threading;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Helpers;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Main Window of the AddIn.
    /// </summary>
    public partial class MainWindow
    {
        #region Constants

        /// <summary>
        /// Seconds that an advert is displayed.
        /// </summary>
        private const double ADVERT_SECONDS = 5;

        #endregion Constants

        #region Properties

        /// <summary>
        /// Indicates current Advert index.
        /// </summary>
        private int CurrentAdvertIndex { get; set; }

        /// <summary>
        /// Advert currently displayed.
        /// </summary>
        private Advert CurrentAdvert { get; set; }

        #endregion Properties

        #region UI Events

        /// <summary>
        /// Executed when the user clicks the current advert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picAdvert_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurrentAdvert != null && string.IsNullOrWhiteSpace(this.CurrentAdvert.LinkURL) == false)
                {
                    // Open advert URL
                    System.Diagnostics.Process.Start(this.CurrentAdvert.LinkURL);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message + Environment.NewLine + this.CurrentAdvert.LinkURL);
            }
        }

        #endregion UI Events

        #region Methods

        /// <summary>
        /// Initializes advertising.
        /// </summary>
        private void Advertising_Init()
        {
            try
            {
                // Process in a background thread
                var worker = new BackgroundWorker();

                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                worker.DoWork += new DoWorkEventHandler(this.Advertising_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(this.Advertising_ProgressChanged);

                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Performs advertising (in a background worker).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Advertising_DoWork(object sender, DoWorkEventArgs args)
        {
            var worker = (sender as BackgroundWorker);

            try
            {
                this.CurrentAdvert = null;
                this.CurrentAdvertIndex = int.MaxValue;

                // Initialize advertising
                AdvertHelper.Init();

                // Shuffle adverts
                AdvertHelper.ShuffleAdverts();

                while (true)
                {
                    if (this.IsMainWindowClosing || AdvertHelper.AvailablesAdverts.Count == 0)
                    {
                        break;
                    }

                    // Get current advert index
                    if (this.CurrentAdvertIndex >= (AdvertHelper.AvailablesAdverts.Count - 1))
                    {
                        this.CurrentAdvertIndex = 0;
                    }
                    else
                    {
                        this.CurrentAdvertIndex += 1;
                    }

                    // Get current advert
                    this.CurrentAdvert = AdvertHelper.AvailablesAdverts[this.CurrentAdvertIndex];

                    // Report progress to show current advert
                    worker.ReportProgress(0);

                    // Sleep til new advert
                    Thread.Sleep(TimeSpan.FromSeconds(MainWindow.ADVERT_SECONDS));
                }
            }
            catch (Exception ex)
            {
                worker.ReportProgress(100, ex);
            }
        }

        /// <summary>
        /// Checks if a new release exists (background worker progress changed event).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Advertising_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            if (e.UserState is Exception)
            {
                var ex = (e.UserState as Exception);

                // Log Error
                LogManager.LogError(ex);

                // Show error message
                MessageHelper.ShowExceptionMessage(ex);
            }
            else
            {
                // Show current advert
                this.picAdvert.Image = this.CurrentAdvert.Image;
            }
        }

        #endregion Methods
    }
}