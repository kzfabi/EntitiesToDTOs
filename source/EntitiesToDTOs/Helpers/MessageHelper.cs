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
using System.Windows.Forms;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.UI;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to showing messages in the UI.
    /// </summary>
    internal class MessageHelper
    {
        /// <summary>
        /// Shows an exception message. If it is an ApplicationException it shows a common message box, 
        /// if no it shows the <see cref="ReportIssueWindow"/>.
        /// </summary>
        /// <param name="ex">Exception occurred.</param>
        public static void ShowExceptionMessage(Exception ex)
        {
            if (ex is ApplicationException)
            {
                MessageHelper.ShowErrorMessage(ex.Message);
            }
            else
            {
                var reportIssueWindow = new ReportIssueWindow(ex);
                reportIssueWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">Message to show.</param>
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        /// <param name="message">Message to show.</param>
        public static void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, Resources.Info_Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}