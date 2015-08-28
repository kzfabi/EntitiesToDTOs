using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Provides advertising operations.
    /// </summary>
    internal class AdvertHelper
    {
        /// <summary>
        /// Specifies adverts config file nodes used.
        /// </summary>
        private class AdvertsConfigNodes
        {
            public static string Advert = "Advert";
            public static string AdvertAttrId = "id";
            public static string AdvertAttrCompiled = "compiled";
            public static string AdvertAttrEnabled = "enabled";

            public static string Link = "Link";
            public static string LinkAttrUrl = "url";

        }



        /// <summary>
        /// Indicates if advert logic is initialized.
        /// </summary>
        private static bool IsInitialized { get; set; }

        /// <summary>
        /// Folder where advert images are stored once downloaded.
        /// </summary>
        private static string AdvertsLocalFolder
        {
            get
            {
                return (Path.GetTempPath() + Resources.EntitiesToDTOsTempFolderName + Resources.AdvertsFolderName);
            }
        }

        /// <summary>
        /// Advert-xml config file path.
        /// </summary>
        private static string AdvertsConfigFilePath
        {
            get
            {
                return (AdvertHelper.AdvertsLocalFolder + Resources.AdvertsConfigFileName);
            }
        }

        /// <summary>
        /// Default advert.
        /// </summary>
        private static Advert DefaultAdvert
        {
            get
            {
                if (AdvertHelper.defaultAdvert == null)
                {
                    AdvertHelper.defaultAdvert = new Advert();
                    AdvertHelper.defaultAdvert.Image = Resources.AdTemplate;
                    AdvertHelper.defaultAdvert.LinkURL = Resources.AdvertDefaultLinkURL;
                }

                return AdvertHelper.defaultAdvert;
            }
        }
        private static Advert defaultAdvert = null;

        /// <summary>
        /// Adverts that are compiled with the tool.
        /// </summary>
        private static List<Advert> CompiledAdverts
        {
            get
            {
                if (AdvertHelper.compiledAdverts == null)
                {
                    AdvertHelper.compiledAdverts = new List<Advert>();

                    // LinkURL could be updated by advert-xml obtained from repository

                    AdvertHelper.compiledAdverts.Add(new Advert()
                    {
                        AdvertID = 1,
                        Image = Resources.Advert_1,
                        LinkURL = Resources.AdvertDefaultLinkURL
                    });
                }

                return AdvertHelper.compiledAdverts;
            }
        }
        private static List<Advert> compiledAdverts = null;

        /// <summary>
        /// Available adverts.
        /// </summary>
        public static List<Advert> AvailablesAdverts { get; set; }



        /// <summary>
        /// Initializes advert logic.
        /// </summary>
        public static void Init()
        {
            if (AdvertHelper.IsInitialized == false)
            {
                AdvertHelper.IsInitialized = true;
                AdvertHelper.AvailablesAdverts = new List<Advert>();

                try
                {
                    // Create advert local folder if not exists
                    if (Directory.Exists(AdvertHelper.AdvertsLocalFolder) == false)
                    {
                        Directory.CreateDirectory(AdvertHelper.AdvertsLocalFolder);
                    }

                    // Get advert-xml from repository
                    XDocument xdoc = XDocument.Load(Resources.AdvertsBaseURL + Resources.AdvertsConfigFileName);

                    // Update latest advert-xml downloaded with the new one
                    xdoc.Save(AdvertHelper.AdvertsConfigFilePath);
                }
                catch (Exception ex)
                {
                    AdvertHelper.IsInitialized = false;

                    // Log error
                    LogManager.LogError(ex);
                }

                if (File.Exists(AdvertHelper.AdvertsConfigFilePath) == true)
                {
                    try
                    {
                        // Load adverts config file
                        XDocument xdoc = XDocument.Load(AdvertHelper.AdvertsConfigFilePath);

                        // Get enabled adverts
                        IEnumerable<XElement> advertsFromConfig = xdoc.Descendants(AdvertsConfigNodes.Advert)
                            .Where(n => n.Attribute(AdvertsConfigNodes.AdvertAttrEnabled)
                                .Value.ToLower() == (true).ToString().ToLower());

                        // Loop through adverts from config
                        foreach (XElement advertNode in advertsFromConfig)
                        {
                            bool downloadAdvertImage = false;

                            var advert = new Advert();

                            advert.AdvertID = Convert.ToInt32(advertNode.Attribute(AdvertsConfigNodes.AdvertAttrId).Value);

                            advert.LinkURL = advertNode.Descendants(AdvertsConfigNodes.Link).First()
                                .Attribute(AdvertsConfigNodes.LinkAttrUrl).Value;

                            // Get compiled attribute value
                            string compiledValue = advertNode.Attribute(AdvertsConfigNodes.AdvertAttrCompiled).Value;

                            if (compiledValue.ToLower() == (true).ToString().ToLower())
                            {
                                // Advert is compiled, get image from CompiledAdverts
                                Advert advertCompiled = AdvertHelper.CompiledAdverts.FirstOrDefault(a => a.AdvertID == advert.AdvertID);

                                if (advertCompiled != null)
                                {
                                    advert.Image = advertCompiled.Image;
                                }
                                else
                                {
                                    // Advert is marked as compiled but was not found in resources
                                    downloadAdvertImage = true;
                                }
                            }
                            else
                            {
                                downloadAdvertImage = true;
                            }

                            if (downloadAdvertImage == true)
                            {
                                AdvertHelper.DownloadAdvertImage(advert);

                                if (advert.Image == null)
                                {
                                    // Image could not be downloaded, continue with next advert
                                    continue;
                                }
                            }

                            // Add to available adverts
                            AdvertHelper.AvailablesAdverts.Add(advert);

                        } // END foreach
                    }
                    catch (Exception ex)
                    {
                        AdvertHelper.IsInitialized = false;

                        // Log error
                        LogManager.LogError(ex);
                    }
                } // END if
                
                if (AdvertHelper.AvailablesAdverts.Count == 0)
                {
                    // Indicate all compiled adverts as available adverts
                    AdvertHelper.AvailablesAdverts = new List<Advert>();
                    foreach (Advert compiledAd in AdvertHelper.CompiledAdverts)
                    {
                        AdvertHelper.AvailablesAdverts.Add(compiledAd);
                    }

                    if (AdvertHelper.AvailablesAdverts.Count == 0)
                    {
                        // No available adverts, show default advert
                        AdvertHelper.AvailablesAdverts.Add(AdvertHelper.DefaultAdvert);
                    }
                }

            } // END if (AdvertHelper.IsInitialized == false)
        }

        /// <summary>
        /// Downloads the image of an advert.
        /// </summary>
        /// <param name="advert">Advert to download the image.</param>
        private static void DownloadAdvertImage(Advert advert)
        {
            // Compose advert image name
            string advertImageName =
                string.Format(Resources.AdvertImageFileName, advert.AdvertID.ToString());

            bool imageWasDownloaded = false;
            bool imageExistsInFileSystem = File.Exists(AdvertHelper.AdvertsLocalFolder + advertImageName);

            if (imageExistsInFileSystem == false)
            {
                try
                {
                    // Download image
                    (new WebClient()).DownloadFile(Resources.AdvertsBaseURL + advertImageName,
                        AdvertHelper.AdvertsLocalFolder + advertImageName);

                    imageWasDownloaded = true;
                }
                catch (Exception ex)
                {
                    // Log error
                    LogManager.LogError(ex);

                    // Image could not be downloaded
                    advert.Image = null;
                }
            }

            if (imageExistsInFileSystem || imageWasDownloaded)
            {
                // Get image from file system
                using (FileStream imgStream = File.OpenRead(AdvertHelper.AdvertsLocalFolder + advertImageName))
                {
                    advert.Image = new Bitmap(imgStream);
                }
            }
        }

        /// <summary>
        /// Shuffle available adverts.
        /// </summary>
        public static void ShuffleAdverts()
        {
            // Shuffle available adverts
            AdvertHelper.AvailablesAdverts = AdvertHelper.AvailablesAdverts.OrderBy(a => Guid.NewGuid()).ToList();
        }


    }
}