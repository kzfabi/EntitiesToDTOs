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
    /// Issue window.
    /// </summary>
    public partial class ReportIssueWindow : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportIssueWindow"/> class.
        /// </summary>
        /// <param name="reportEx">Exception to report as issue.</param>
        public ReportIssueWindow(Exception reportEx)
        {
            try
            {
                InitializeComponent();

                this.Text = Resources.Error_Caption;

                this.lblErrorMessage.Text = string.Format(this.lblErrorMessage.Text, reportEx.Message);

                this.lblLogFilePath.Text = LogManager.LogFilePath;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowErrorMessage(ex.Message);
            }
        }

        #endregion Constructors

        #region UI Events

        /// <summary>
        /// Executed when the user clicks the Report button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Resources.CreateIssueURL);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Executed when the user clicks the See Log button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeeLog_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(LogManager.LogFilePath);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Executed when the user clicks the Close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion UI Events
    }
}