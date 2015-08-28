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
using EntitiesToDTOs.Helpers;

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Main Window of the AddIn.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Checks if the user has to rate this release, it shows the <see cref="RateReleaseWindow"/> if true.
        /// </summary>
        private void CheckIfUserHasToRateThisRelease()
        {
            if (RateReleaseHelper.IsReleaseRatePending())
            {
                var rateReleaseWindow = new RateReleaseWindow();
                rateReleaseWindow.ShowDialog();
            }
        }
    }
}