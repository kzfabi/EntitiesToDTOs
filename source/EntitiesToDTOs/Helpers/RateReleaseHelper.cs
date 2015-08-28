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
using EntitiesToDTOs.Domain;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to rate the release.
    /// </summary>
    internal class RateReleaseHelper
    {
        /// <summary>
        /// Checks if a release rate is pending. If a rate is pending, it updates the LastRateAskedDate of the 
        /// AddIn config assuming the user will be asked to rate the release.
        /// </summary>
        /// <returns></returns>
        public static bool IsReleaseRatePending()
        {
            try
            {
                bool isRatePending = false;

                AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

                if (addInConfig.RateReleaseID != AssemblyHelper.VersionInfo.ReleaseID)
                {
                    // User has changed the AddIn version
                    addInConfig.RateReleaseID = AssemblyHelper.VersionInfo.ReleaseID;
                    addInConfig.IsReleaseRated = false;
                    addInConfig.LastRateAskedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    ConfigurationHelper.SaveAddInConfig(addInConfig);
                }

                if (addInConfig.IsReleaseRated == false)
                {
                    // Have we waited to ask again?
                    if ((DateTime.Now - addInConfig.LastRateAskedDate).TotalDays >= 1)
                    {
                        isRatePending = true;

                        addInConfig.LastRateAskedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        ConfigurationHelper.SaveAddInConfig(addInConfig);
                    }
                }

                return isRatePending;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Marks this release as rated.
        /// </summary>
        public static void MarkReleaseAsRated()
        {
            try
            {
                AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

                addInConfig.IsReleaseRated = true;

                ConfigurationHelper.SaveAddInConfig(addInConfig);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw;
            }
        }
    }
}