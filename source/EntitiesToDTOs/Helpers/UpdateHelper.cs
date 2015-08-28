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
using System.Xml.Linq;
using EntitiesToDTOs.Properties;
using EntitiesToDTOs.Domain.Enums;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Provides operations related to AddIn updates.
    /// </summary>
    internal class UpdateHelper
    {
        // Based on Brice Lambson's approach for "Automatic Update for CodePlex Projects".
        // See: http://brice-lambson.blogspot.com/2011/03/automatic-update-for-codeplex-projects.html

        /// <summary>
        /// Specifies AvailableReleases (releases info file) nodes used.
        /// </summary>
        private class ReleasesNodes
        {
            public const string Release = "Release";
            public const string ReleaseAttrID = "id";
            public const string ReleaseAttrVersion = "version";
            public const string ReleaseAttrStatus = "status";

            public const string Link = "Link";
            public const string LinkAttrHref = "href";

            public const string ChangeList = "ChangeList";
            
            public const string Change = "Change";

        }



        /// <summary>
        /// Skips the received release.
        /// </summary>
        /// <param name="releaseID">Release ID to skip.</param>
        public static void SkipRelease(int releaseID)
        {
            // Get AddIn configuration
            AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

            // Check if the release was not already skipped
            if (addInConfig.SkippedReleases.Contains(releaseID) == false)
            {
                // Add the release to the skipped releases collection
                addInConfig.SkippedReleases.Add(releaseID);

                // Save AddIn configuration
                ConfigurationHelper.SaveAddInConfig(addInConfig);
            }
        }

        /// <summary>
        /// Checks if a new release exists based on current version.
        /// </summary>
        /// <param name="statusFilter">Release status filter.</param>
        /// <returns></returns>
        public static Release CheckNewRelease(List<ReleaseStatus> statusFilter)
        {
            try
            {
                // Get current version
                string currentVersion = AssemblyHelper.Version;

                // Get available releases document
                XDocument xdoc = XDocument.Load(Resources.ReleasesURL);

                // Transform release status filter to string
                IEnumerable<string> statusFilterValues = statusFilter.Select(s => s.ToString().ToLower());

                // Get releases filtered by release status
                IEnumerable<XElement> releases = xdoc.Descendants(ReleasesNodes.Release)
                    .Where(n => statusFilterValues.Contains(
                        n.Attribute(ReleasesNodes.ReleaseAttrStatus).Value.ToLower())
                    );

                // Get latest release node
                XElement latestReleaseNode = releases.LastOrDefault();

                if (latestReleaseNode == null)
                {
                    // No releases using the specified status filter
                    return null;
                }

                // Get latest release ID
                int latestReleaseID = Convert.ToInt32(latestReleaseNode
                    .Attribute(ReleasesNodes.ReleaseAttrID).Value);

                // Check if it is a newer release
                if (latestReleaseID <= AssemblyHelper.VersionInfo.ReleaseID)
                {
                    // Same or old release
                    return null;
                }
                else
                {
                    // Get AddIn configuration
                    AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

                    // Check if the new release has not been previously skipped by the user
                    if (addInConfig.SkippedReleases.Contains(latestReleaseID) == true)
                    {
                        // The release is newer but was previously skipped by the user
                        return null;
                    }
                    else
                    {
                        // New release
                        return new Release()
                        {
                            ReleaseID = Convert.ToInt32(latestReleaseNode.Attribute(ReleasesNodes.ReleaseAttrID).Value),

                            Version = latestReleaseNode.Attribute(ReleasesNodes.ReleaseAttrVersion).Value,

                            Status = UpdateHelper.GetReleaseStatusFromText(
                                latestReleaseNode.Attribute(ReleasesNodes.ReleaseAttrID).Value),

                            Link = latestReleaseNode.Descendants(ReleasesNodes.Link).First()
                                .Attribute(ReleasesNodes.LinkAttrHref).Value,

                            Changes = UpdateHelper.GetChanges(latestReleaseNode)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                return null;
            }
        }

        /// <summary>
        /// Gets the change list of a release node.
        /// </summary>
        /// <param name="releaseNode">Release node.</param>
        /// <returns></returns>
        private static List<string> GetChanges(XElement releaseNode)
        {
            var changes = new List<string>();

            foreach (XElement changeNode in releaseNode.Descendants(ReleasesNodes.Change))
            {
                changes.Add(changeNode.Value);
            }

            return changes;
        }

        /// <summary>
        /// Gets the release status value from the text received.
        /// </summary>
        /// <param name="releaseStatusText">Release status text representing the value.</param>
        /// <returns></returns>
        private static ReleaseStatus GetReleaseStatusFromText(string releaseStatusText)
        {
            if (ReleaseStatus.Stable.ToString().ToLower() == releaseStatusText.ToLower())
            {
                return ReleaseStatus.Stable;
            }
            else
            {
                return ReleaseStatus.Beta;
            }
        }

    }
}