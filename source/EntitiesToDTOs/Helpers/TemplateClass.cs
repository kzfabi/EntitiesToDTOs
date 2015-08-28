using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EntitiesToDTOs.Properties;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Manages operations relative to the TemplateFile used to create ProjectItems.
    /// </summary>
    internal class TemplateClass
    {
        /// <summary>
        /// Gets the TemplateClass file path.
        /// </summary>
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_filePath))
                {
                    _filePath = Path.GetTempPath()
                        + Resources.EntitiesToDTOsTempFolderName 
                        + Resources.TemplateClassFileName;
                }

                return _filePath;
            }
        }
        private static string _filePath;


        /// <summary>
        /// Creates the TemplateFile in the File System
        /// </summary>
        public static void CreateFile()
        {
            TemplateClass.Delete();

            using (var templateClassFile = File.CreateText(TemplateClass.FilePath))
            {
                templateClassFile.Write(string.Empty);

                templateClassFile.Flush();
                templateClassFile.Close();
                templateClassFile.Dispose();
            }
        }

        /// <summary>
        /// Deletes the TemplateFile from the File System
        /// </summary>
        public static void Delete()
        {
            if (File.Exists(TemplateClass.FilePath))
            {
                File.Delete(TemplateClass.FilePath);
            }
        }
    }
}