using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Properties;
using System.Windows.Forms;

namespace EntitiesToDTOs.Helpers
{
    /// <summary>
    /// Helps in the tasks relative to Visual Studio manipulation (Solution, Projects, ProjectItems, etc.).
    /// </summary>
    internal class VisualStudioHelper
    {
        /// <summary>
        /// List of NOT supported project types.
        /// </summary>
        private static List<string> NotSupportedProjectTypes
        {
            get
            {
                if (_notSupportedProjectTypes == null)
                {
                    _notSupportedProjectTypes = new List<string>();
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectSetup);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectJSharp);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDatabase);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDatabaseOther);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDeploymentCab);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDeploymentMergeModule);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDeploymentSetup);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectDeploymentSmartDeviceCab);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectSharepointVB);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectSmartDeviceVB);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectWebSite);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectWindowsCPlusPlus);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectWindowsVB);
                    _notSupportedProjectTypes.Add(Resources.VSGuidProjectWorkflowVB);
                }

                return _notSupportedProjectTypes;
            }
        }
        private static List<string> _notSupportedProjectTypes = null;

        /// <summary>
        /// List of NOT supported project folders.
        /// </summary>
        private static List<string> NotSupportedProjectFolders
        {
            get
            {
                if (_notSupportedProjectFolders == null)
                {
                    _notSupportedProjectFolders = new List<string>();
                    _notSupportedProjectFolders.Add(Resources.VSProjectFolderProperties);
                    _notSupportedProjectFolders.Add(Resources.VSProjectFolderServiceReferences);
                }

                return _notSupportedProjectFolders;
            }
        }
        private static List<string> _notSupportedProjectFolders = null;

        /// <summary>
        /// DTE object reference.
        /// </summary>
        private static DTE2 _DTE = null;

        /// <summary>
        /// Indicates if VisualStudioHelper has been initialized.
        /// </summary>
        private static bool _initialized = false;

        /// <summary>
        /// Collection of ErrorTask items.
        /// </summary>
        private static List<ErrorTask> ErrorTaskCollection
        {
            get
            {
                if (_errorTaskCollection == null)
                {
                    _errorTaskCollection = new List<ErrorTask>();
                }

                return _errorTaskCollection;
            }
        }
        private static List<ErrorTask> _errorTaskCollection = null;

        /// <summary>
        /// ErrorList Provider.
        /// </summary>
        private static ErrorListProvider _errorListProvider = null;

        /// <summary>
        /// Provides top-level manipulation or maintenance of the solution. 
        /// </summary>
        private static IVsSolution _solutionService = null;



        /// <summary>
        /// Initializes VisualStudioHelper class.
        /// </summary>
        /// <param name="objDTE">DTE object.</param>
        public static void Initialize(DTE2 objDTE)
        {
            if (VisualStudioHelper._initialized == false)
            {
                VisualStudioHelper._initialized = true;

                // Set DTE reference
                VisualStudioHelper._DTE = objDTE;
                
                // Set event handlers
                VisualStudioHelper._DTE.Events.SolutionEvents.BeforeClosing += VisualStudioHelper.SolutionEvents_BeforeClosing;
                VisualStudioHelper._DTE.Events.SolutionEvents.ProjectRemoved += VisualStudioHelper.SolutionEvents_ProjectRemoved;

                // Get ServiceProvider
                Microsoft.VisualStudio.Shell.ServiceProvider sp = 
                    new Microsoft.VisualStudio.Shell.ServiceProvider(
                        (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)objDTE);

                // Get the Solution service from the ServiceProvider
                VisualStudioHelper._solutionService = (sp.GetService(typeof(IVsSolution)) as IVsSolution);
                
                // Get the ErrorList provider
                VisualStudioHelper._errorListProvider = new Microsoft.VisualStudio.Shell.ErrorListProvider(sp);
                VisualStudioHelper._errorListProvider.ProviderName = Resources.ErrorList_ProviderName;
                VisualStudioHelper._errorListProvider.ProviderGuid = new Guid(Resources.ErrorList_ProviderGUID);
            }
        }
        


        /// <summary>
        /// Checks if a Solution is open.
        /// </summary>
        /// <returns></returns>
        public static bool IsSolutionOpen(DTE2 application)
        {
            if (application.Solution == null
                || application.Solution.IsOpen == false
                || string.IsNullOrWhiteSpace(application.Solution.FullName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the name of the solution.
        /// </summary>
        /// <param name="solution">Solution from where to obtain the name.</param>
        /// <returns>Solution name, if not found for some reason then default solution name is returned.</returns>
        public static string GetSolutionName(Solution solution)
        {
            object solutionNameValue = null;

            foreach (Property prop in solution.Properties)
            {
                if (prop.Name == "Name")
                {
                    solutionNameValue = prop.Value;
                }
            }

            if (string.IsNullOrWhiteSpace((string)solutionNameValue) == true)
            {
                return Resources.DefaultSolutionName;
            }
            else
            {
                return (string)solutionNameValue;
            }
        }

        /// <summary>
        /// Gets the IDE text editor comment colorable items.
        /// </summary>
        /// <returns></returns>
        public static ColorableItems GetTextEditorCommentColorableItems()
        {
            var faci = (VisualStudioHelper._DTE.Properties["FontsAndColors", "TextEditor"]
                    .Item("FontsAndColorsItems").Object as FontsAndColorsItems);

            return (faci.Item("Comment") as ColorableItems);
        }

        /// <summary>
        /// Gets default namespace from properties (obtained from a Project or ProjectItem).
        /// </summary>
        /// <param name="properties">Properties from where to get the Default Namespace.</param>
        /// <returns></returns>
        public static string GetDefaultNamespaceFromProperties(EnvDTE.Properties properties)
        {
            return properties.Item(Resources.Properties_DefaultNamespace).Value.ToString();
        }

        /// <summary>
        /// Gets all items of the projects of a provided Solution.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public static List<ProjectItem> GetSolutionProjectItems(Solution solution)
        {
            var result = new List<ProjectItem>();

            foreach (Project item in solution.Projects)
            {
                if (item.ProjectItems != null)
                {
                    result.AddRange(VisualStudioHelper.GetProjectItems(item.ProjectItems));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the items of the provided ProjectItems collection.
        /// </summary>
        /// <param name="itemsCollection"></param>
        /// <returns></returns>
        public static List<ProjectItem> GetProjectItems(ProjectItems itemsCollection)
        {
            var result = new List<ProjectItem>();

            foreach (ProjectItem item in itemsCollection)
            {
                result.Add(item);

                if (item.SubProject != null && item.SubProject.ProjectItems != null)
                {
                    result.AddRange(VisualStudioHelper.GetProjectItems(item.SubProject.ProjectItems));
                }

                if (item.ProjectItems != null)
                {
                    result.AddRange(VisualStudioHelper.GetProjectItems(item.ProjectItems));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the Projects of the provided Solution.
        /// </summary>
        /// <param name="solution">Solution to get the Projects from.</param>
        /// <returns></returns>
        public static List<Project> GetProjectsFromSolution(Solution solution)
        {
            var result = new List<Project>();

            IEnumerator item = solution.Projects.GetEnumerator();

            while (item.MoveNext())
            {
                var project = (item.Current as Project);

                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    result.AddRange(VisualStudioHelper.GetSolutionFolderProjects(project));
                }
                else if (VisualStudioHelper.NotSupportedProjectTypes.Contains(project.Kind) == false)
                {
                    result.Add(project);
                }
            }

            return result.OrderBy(i => i.Name).ToList();
        }

        /// <summary>
        /// Gets the Projects of a a Solution Folder.
        /// </summary>
        /// <param name="solutionFolder">Solution Folder to get projects from.</param>
        /// <returns></returns>
        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        {
            var result = new List<Project>();

            IEnumerator item = solutionFolder.ProjectItems.GetEnumerator();

            while (item.MoveNext())
            {
                var subProject = (item.Current as ProjectItem).SubProject;

                if (subProject == null)
                {
                    continue;
                }

                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    result.AddRange(VisualStudioHelper.GetSolutionFolderProjects(subProject));
                }
                else if (VisualStudioHelper.NotSupportedProjectTypes.Contains(subProject.Kind) == false)
                {
                    result.Add(subProject);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets only first level folders from a project.
        /// </summary>
        /// <param name="project">Project from where to obtain first level folders.</param>
        /// <returns></returns>
        public static List<ProjectItem> GetFirstLevelFoldersFromProject(Project project)
        {
            var result = new List<ProjectItem>();

            if (project.ProjectItems != null)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    if (item.Kind == Resources.VSGuidPhysicalFolder
                        && VisualStudioHelper.NotSupportedProjectFolders.Contains(item.Name) == false)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets only first level folders from a folder.
        /// </summary>
        /// <param name="folder">Folder from where to obtain first level folders.</param>
        /// <returns></returns>
        public static List<ProjectItem> GetFirstLevelFoldersFromFolder(ProjectItem folder)
        {
            var result = new List<ProjectItem>();

            if (folder.ProjectItems != null)
            {
                foreach (ProjectItem item in folder.ProjectItems)
                {
                    if (item.Kind == Resources.VSGuidPhysicalFolder)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the project code files.
        /// </summary>
        /// <param name="project">Project from where to obtain the code files.</param>
        /// <returns></returns>
        public static IEnumerable<ProjectItem> GetProjectCodeFiles(Project project)
        {
            return VisualStudioHelper.GetProjectItems(project.ProjectItems)
                .Where(i => i.Name.EndsWith(Resources.CSharpFileExtension))
                .OrderBy(i => i.Name);
        }

        /// <summary>
        /// Gets the code classes from a project.
        /// </summary>
        /// <param name="project">Project from where to obtain the code classes.</param>
        /// <returns></returns>
        public static IEnumerable<CodeClass> GetProjectCodeClasses(Project project)
        {
            var result = new List<CodeClass>();
            
            foreach (ProjectItem codeFile in VisualStudioHelper.GetProjectCodeFiles(project))
            {
                // Add classes from code file
                result.AddRange(VisualStudioHelper.GetCodeClasses(codeFile.FileCodeModel.CodeElements));
            }

            return result.OrderBy(c => c.Name);
        }

        /// <summary>
        /// Gets the code classes form a CodeElements collection.
        /// </summary>
        /// <param name="codeElements">CodeElements collection from where to get the code classes.</param>
        /// <returns></returns>
        private static List<CodeClass> GetCodeClasses(CodeElements codeElements)
        {
            var result = new List<CodeClass>();

            CodeClass codeClass;

            // Loop through code elements
            foreach (CodeElement code in codeElements)
            {
                if (code.Kind == vsCMElement.vsCMElementNamespace)
                {
                    // Get classes from namespace
                    result.AddRange(VisualStudioHelper.GetCodeClasses((code as CodeNamespace).Members));
                }
                else if (code.Kind == vsCMElement.vsCMElementClass)
                {
                    // Case code to CodeClass
                    codeClass = (code as CodeClass);
                    
                    // Add class to the result
                    result.Add(codeClass);

                    // Add nested classes
                    result.AddRange(VisualStudioHelper.GetCodeClasses(codeClass.Members));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the full names of the types involved with the received type reference.
        /// </summary>
        /// <param name="typeRef">TypeRef from where to obtained the type full names involved with it.</param>
        /// <returns></returns>
        public static List<string> GetTypesFullNamesFromTypeRef(CodeTypeRef typeRef)
        {
            var result = new List<string>();

            if (typeRef.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
            {
                result.AddRange(VisualStudioHelper.GetTypesFullNamesFromTypeRef(typeRef.ElementType));
            }
            else
            {
                result.AddRange(VisualStudioHelper.GetFullNamesFromFullName(typeRef.AsFullName));
            }

            return result;
        }

        /// <summary>
        /// Gets the full names involved with the received full name.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        /// <returns></returns>
        public static List<string> GetFullNamesFromFullName(string fullName)
        {
            var result = new List<string>();

            string[] types = fullName
                .Replace("[", string.Empty).Replace("]", string.Empty)
                .Replace(">", string.Empty).Replace(',', '<')
                .Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string type in types)
            {
                if (result.Contains(type) == false)
                {
                    result.Add(type);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the provided Reference to the received Project.
        /// </summary>
        /// <param name="targetProject">Project to add the Reference.</param>
        /// <param name="referenceIdentity">Reference Identity (Name).</param>
        /// <param name="browseUrl">URL of the Reference (same as the Identity if it is in the GAC).</param>
        public static void AddReferenceToProject(Project targetProject,
            string referenceIdentity, string browseUrl)
        {
            string path = string.Empty;

            if (browseUrl.StartsWith(referenceIdentity) == false)
            {
                // It is a path
                path = browseUrl;
            }

            if (targetProject.Object is VSLangProj.VSProject)
            {
                VSLangProj.VSProject vsproject = (VSLangProj.VSProject)targetProject.Object;
                VSLangProj.Reference reference = null;

                try
                {
                    reference = vsproject.References.Find(referenceIdentity);
                }
                catch (Exception)
                {
                    // It failed to find one, so it must not exist.
                }

                if (reference == null)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(path))
                        {
                            vsproject.References.Add(browseUrl);
                        }
                        else
                        {
                            vsproject.References.Add(path);
                        }
                    }
                    catch (Exception)
                    {
                        // Add warning to ErrorList pane
                        VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                            string.Format(Resources.Warning_ManuallyAddReference, referenceIdentity, targetProject.Name),
                            targetProject, null, null, null);
                    }
                }
                else
                {
                    // Reference already exists.
                }
            }
            else if (targetProject.Object is VsWebSite.VSWebSite)
            {
                VsWebSite.VSWebSite vswebsite = (VsWebSite.VSWebSite)targetProject.Object;
                VsWebSite.AssemblyReference reference = null;

                try
                {
                    foreach (VsWebSite.AssemblyReference r in vswebsite.References)
                    {
                        if (r.Name == referenceIdentity)
                        {
                            reference = r;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // It failed to find one, so it must not exist.
                }

                if (reference == null)
                {
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        vswebsite.References.AddFromGAC(browseUrl);
                    }
                    else
                    {
                        vswebsite.References.AddFromFile(path);
                    }
                }
                else
                {
                    // Reference already exists.
                }
            }
            else
            {
                // Add warning to ErrorList pane
                VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                    string.Format(Resources.Warning_ManuallyAddReference, referenceIdentity, targetProject.Name),
                    targetProject, null, null, null);
            }
        }

        /// <summary>
        /// Adds the provided Reference to the received Project.
        /// </summary>
        /// <param name="targetProject">Project to add the Reference.</param>
        /// <param name="referencedProject">Project to be referenced.</param>
        public static void AddReferenceToProject(Project targetProject, Project referencedProject)
        {
            if (targetProject.FullName == referencedProject.FullName)
            {
                return;
            }

            if (targetProject.Object is VSLangProj.VSProject)
            {
                VSLangProj.VSProject vsproject = (VSLangProj.VSProject)targetProject.Object;
                VSLangProj.Reference reference = null;

                try
                {
                    reference = vsproject.References.Find(referencedProject.Name);
                }
                catch (Exception)
                {
                    // It failed to find one, so it must not exist.
                }

                if (reference == null)
                {
                    try
                    {
                        vsproject.References.AddProject(referencedProject);
                    }
                    catch (Exception)
                    {
                        // Add warning to ErrorList pane
                        VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                            string.Format(Resources.Warning_ManuallyAddReference, referencedProject.Name, targetProject.Name),
                            targetProject, null, null, null);
                    }
                }
                else
                {
                    // Reference already exists.
                }
            }
            else if (targetProject.Object is VsWebSite.VSWebSite)
            {
                VsWebSite.VSWebSite vswebsite = (VsWebSite.VSWebSite)targetProject.Object;
                VsWebSite.AssemblyReference reference = null;

                try
                {
                    foreach (VsWebSite.AssemblyReference r in vswebsite.References)
                    {
                        if (r.Name == referencedProject.Name)
                        {
                            reference = r;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // It failed to find one, so it must not exist.
                }

                if (reference == null)
                {
                    try
                    {
                        vswebsite.References.AddFromProject(referencedProject);
                    }
                    catch (Exception)
                    {
                        // Add warning to ErrorList pane
                        VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning, 
                            string.Format(Resources.Warning_ManuallyAddReference, referencedProject.Name, targetProject.Name),
                            targetProject, null, null, null);
                    }
                }
                else
                {
                    // Reference already exists.
                }
            }
            else
            {
                // Add warning to ErrorList pane
                VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                    string.Format(Resources.Warning_ManuallyAddReference, referencedProject.Name, targetProject.Name),
                    targetProject, null, null, null);
            }
        }

        /// <summary>
        /// Adds imports to a ProjectItem.
        /// </summary>
        /// <param name="sourceFileItem">ProjectItem to add the imports.</param>
        /// <param name="importList">List of imports to add.</param>
        public static void AddImportsToSourceCode(ref ProjectItem sourceFileItem, List<SourceCodeImport> importList)
        {
            // Cast to FileCodeModel2 for AddImport method
            var objFileCodeModel2 = (sourceFileItem.FileCodeModel as FileCodeModel2);

            // Add imports (using ...)
            foreach (SourceCodeImport import in importList)
            {
                objFileCodeModel2.AddImport(import.ImportNamespace);
            }
        }

        /// <summary>
        /// Generates the specified Source File in the received Project with the options 
        /// provided and gets the Namespace ready to add code in it.
        /// </summary>
        /// <param name="targetProject">Project where the Source File is going to be placed.</param>
        /// <param name="targetProjectFolder">Project folder where the source file is going to be placed. 
        /// Null indicates to place the source file as child of targetProject.</param>
        /// <param name="sourceFileName">Source File name to use.</param>
        /// <param name="sourceFileHeaderComment">Source File Header Comment (optional).</param>
        /// <param name="sourceNamespace">Namespace used in the Source File.</param>
        /// <param name="isServiceReady">Specifies if it is Service-Ready (serialization is going to be used).</param>
        /// <param name="sourceFileItem">(out parameter) Source File ProjectItem.</param>
        /// <returns></returns>
        public static CodeNamespace GenerateSourceAndGetNamespace(Project targetProject, ProjectItem targetProjectFolder, 
            string sourceFileName, string sourceFileHeaderComment, string sourceNamespace,
            bool isServiceReady, out ProjectItem sourceFileItem)
        {
            // Validate source file name
            if (sourceFileName.EndsWith(Resources.CSharpFileExtension) == false)
            {
                sourceFileName += Resources.CSharpFileExtension;
            }

            // Validate source file header comment
            if (string.IsNullOrWhiteSpace(sourceFileHeaderComment) == false)
            {
                if (sourceFileHeaderComment.IndexOf("*/") >= 0)
                {
                    throw new ApplicationException(Resources.Error_HeaderCommentInvalidChars);
                }
            }

            // ProjectItems collection where to place the source file
            ProjectItems projectItems = targetProject.ProjectItems;
            if (targetProjectFolder != null)
            {
                // Place inside received project folder
                projectItems = targetProjectFolder.ProjectItems;
            }

            // Properties collection of the target
            EnvDTE.Properties targetProperties = targetProject.Properties;
            if (targetProjectFolder != null)
            {
                targetProperties = targetProjectFolder.Properties;
            }

            // Source file
            sourceFileItem = null;

            #region If source file exists in the target, clear it and get the reference
            foreach (ProjectItem projItem in projectItems)
            {
                string projItemFileName = projItem.Properties.Item(Resources.ProjectItem_FileName).Value.ToString();

                if (sourceFileName.ToLower() == projItemFileName.ToLower())
                {
                    // Source file already exists
                    sourceFileItem = projItem;

                    if (sourceFileItem.FileCodeModel.CodeElements != null
                        && sourceFileItem.FileCodeModel.CodeElements.Count > 0)
                    {
                        // Clear source file

                        CodeElement firstElement = sourceFileItem.FileCodeModel.CodeElements.Item(1);

                        CodeElement lastElement = sourceFileItem.FileCodeModel.CodeElements.Item(
                            sourceFileItem.FileCodeModel.CodeElements.Count);

                        EditPoint startPoint = firstElement.StartPoint.CreateEditPoint();
                        EditPoint endPoint = lastElement.EndPoint.CreateEditPoint();

                        while (startPoint.AtStartOfDocument != true)
                        {
                            startPoint.LineUp();
                        }

                        while (endPoint.AtEndOfDocument != true)
                        {
                            endPoint.LineDown();
                        }

                        startPoint.Delete(endPoint);
                    }

                    break;
                }
            }
            #endregion

            #region If source file NOT exists in the target, create it and get the reference
            if (sourceFileItem == null)
            {
                // New source file, get target path
                string targetPath = targetProperties.Item(Resources.Properties_LocalPath).Value.ToString();

                // Check if the new source file already exists in the file system (and it is not added to the solution)
                if (File.Exists(targetPath + sourceFileName))
                {
                    // Rename the existent source file
                    string backupSourceFileName = (sourceFileName + Resources.BackupFileExtension);
                    File.Move((targetPath + sourceFileName), (targetPath + backupSourceFileName));

                    // Add warning
                    VisualStudioHelper.AddToErrorList(TaskErrorCategory.Warning,
                        string.Format(Resources.Warning_SourceFileAlreadyExists, sourceFileName, backupSourceFileName),
                        targetProject, sourceFileName, null, null);
                }

                // Add source file to target
                sourceFileItem = projectItems.AddFromTemplate(TemplateClass.FilePath, sourceFileName);
            }
            #endregion

            #region Generate imports
            var importList = new List<SourceCodeImport>();
            importList.Add(new SourceCodeImport(Resources.NamespaceSystem));
            importList.Add(new SourceCodeImport(Resources.NamespaceSystemCollectionsGeneric));
            importList.Add(new SourceCodeImport(Resources.NamespaceSystemText));

            if (isServiceReady)
            {
                importList.Add(new SourceCodeImport(Resources.NamespaceSystemRuntimeSerialization));
            }

            importList = importList.OrderBy(d => d.ImportNamespace).ToList();
            #endregion Generate imports

            // Add imports to the source code
            VisualStudioHelper.AddImportsToSourceCode(ref sourceFileItem, importList);

            // Get Source file code start
            EditPoint objEditPoint = sourceFileItem.FileCodeModel.CodeElements.Item(1).StartPoint.CreateEditPoint();
            objEditPoint.StartOfDocument();

            // Add header comment
            if (string.IsNullOrWhiteSpace(sourceFileHeaderComment) == false)
            {
                sourceFileHeaderComment = (Environment.NewLine + sourceFileHeaderComment + Environment.NewLine);

                objEditPoint.Insert(
                    string.Format(Resources.CSharpCommentMultiline, sourceFileHeaderComment) +
                    Environment.NewLine);
            }

            // Add EntitiesToDTOs signature
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
            objEditPoint.Insert(string.Format(Resources.EntitiesToDTOsSignature, AssemblyHelper.Version, timestamp));
            objEditPoint.Insert(Environment.NewLine);

            // Add blank line before source file namespace
            objEditPoint.EndOfDocument();
            objEditPoint.Insert(Environment.NewLine);

            // Add namespace
            CodeNamespace objNamespace = sourceFileItem.FileCodeModel
                .AddNamespace(sourceNamespace, AppConstants.PLACE_AT_THE_END);

            return objNamespace;
        }

        /// <summary>
        /// Sets the message displayed on Visual Studio status bar.
        /// </summary>
        /// <param name="message">Message to display.</param>
        public static void SetStatusBarMessage(string message)
        {
            VisualStudioHelper._DTE.StatusBar.Text = message;
        }

        #region Error List pane manipulation

        // ErrorList manipulation based on: http://www.mztools.com/articles/2008/MZ2008022.aspx

        /// <summary>
        /// Checks if ErrorTask collection has Items which means that the ErrorList pane has messages created by us.
        /// </summary>
        /// <returns></returns>
        public static bool ErrorTaskCollectionHasItems()
        {
            return (VisualStudioHelper.ErrorTaskCollection.Count > 0);
        }

        /// <summary>
        /// Shows the ErrorList pane of Visual Studio.
        /// </summary>
        public static void ShowErrorList()
        {
            VisualStudioHelper._errorListProvider.Show();

            VisualStudioHelper._DTE.ToolWindows.ErrorList.ShowWarnings = true;
        }

        /// <summary>
        /// Removes all the ErrorTasks from the ErrorList pane and our collection.
        /// </summary>
        public static void ClearErrorList()
        {
            try
            {
                if (VisualStudioHelper.ErrorTaskCollection.Count > 0)
                {
                    for (int i = (VisualStudioHelper.ErrorTaskCollection.Count - 1); i >= 0; i--)
                    {
                        // Get the ErrorTask
                        ErrorTask objErrorTask = VisualStudioHelper.ErrorTaskCollection.ElementAt(i);

                        // Remove the ErrorTask from the ErorList pane and our collection
                        VisualStudioHelper.RemoveTask(objErrorTask);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adds a Error/Warning/Message to the Error List pane of Visual Studio.
        /// </summary>
        /// <param name="objProject">Project to reference. Can be null.</param>
        /// <param name="fileName">File name to reference. Can be null.</param>
        /// <param name="lineNumber">Line number to reference. Can be null.</param>
        /// <param name="columnNumber">Column number to reference. Can be null.</param>
        /// <param name="category">Category of the message.</param>
        /// <param name="text">Message text.</param>
        public static void AddToErrorList(TaskErrorCategory category, string text, 
            Project objProject, string fileName, int? lineNumber, int? columnNumber)
        {
            try
            {
                // Create the Error Task
                var objErrorTask = new Microsoft.VisualStudio.Shell.ErrorTask();

                // Set category and text
                objErrorTask.ErrorCategory = category;
                objErrorTask.Text = text;

                // If Project is not null then get the Hierarchy object to reference
                if (objProject != null)
                {
                    IVsHierarchy objVsHierarchy;

                    ErrorHandler.ThrowOnFailure(
                        VisualStudioHelper._solutionService.GetProjectOfUniqueName(objProject.UniqueName, out objVsHierarchy));

                    // Set HierarchyItem reference
                    objErrorTask.HierarchyItem = objVsHierarchy;

                    // Set Navigate event handler
                    objErrorTask.Navigate += VisualStudioHelper.ErrorTaskNavigate;
                }

                // If fileName is not null or whitespace then set the name of the Document
                if (string.IsNullOrWhiteSpace(fileName) == false)
                {
                    objErrorTask.Document = fileName;

                    // Can be obtained from a ProjectItem instance like shown below:
                    // objProjectItem.FileNames[0];
                }

                // Set Column if columnNumber has value
                if (columnNumber.HasValue)
                {
                    objErrorTask.Column = columnNumber.Value;
                }

                // Set Line if lineNumber has value
                if (lineNumber.HasValue)
                {
                    // VS uses indexes starting at 0 while the automation model uses indexes starting at 1
                    objErrorTask.Line = lineNumber.Value - 1;
                }

                // Add the ErrorTask to our collection
                VisualStudioHelper.ErrorTaskCollection.Add(objErrorTask);

                // Add the ErrorTask to the ErrorList pane of Visual Studio
                VisualStudioHelper._errorListProvider.Tasks.Add(objErrorTask);
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executed when the User double clicks an ErrorTask from the ErrorList pane if Visual Studio.
        /// Only if we created the ErrorTask.
        /// </summary>
        /// <param name="sender">ErrorTask item.</param>
        /// <param name="e">Event arguments.</param>
        private static void ErrorTaskNavigate(object sender, EventArgs e)
        {
            try
            {
                // Cast the ErrorTask
                var objErrorTask = (sender as ErrorTask);

                if ((objErrorTask.HierarchyItem != null)
                    && (string.IsNullOrWhiteSpace(objErrorTask.Document) == false))
                {
                    // Fix the index start
                    objErrorTask.Line += 1;

                    bool bResult = VisualStudioHelper._errorListProvider.Navigate(objErrorTask, new Guid(EnvDTE.Constants.vsViewKindCode));

                    // Restore the index start
                    objErrorTask.Line -= 1;

                    if (bResult == false)
                    {
                        // Show error message
                        MessageBox.Show(string.Format(Resources.Error_ErrorNavigatingTo, objErrorTask.Text),
                            Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executed before closing the Solution.
        /// </summary>
        private static void SolutionEvents_BeforeClosing()
        {
            // Remove Error Tasks from the Error List pane and our collection
            VisualStudioHelper.ClearErrorList();
        }

        /// <summary>
        /// Executed when a Project is removed from the Solution.
        /// </summary>
        /// <param name="projectRemoved">Project removed.</param>
        private static void SolutionEvents_ProjectRemoved(Project projectRemoved)
        {
            try
            {
                if (VisualStudioHelper.ErrorTaskCollection != null)
                {
                    // Loop through ErrorTask collection
                    for (int i = (VisualStudioHelper.ErrorTaskCollection.Count - 1); i >= 0; i--)
                    {
                        // Get the ErrorTask
                        ErrorTask objErrorTask = VisualStudioHelper.ErrorTaskCollection.ElementAt(i);

                        if (objErrorTask.HierarchyItem != null)
                        {
                            // Try to get the Project of the ErrorTask
                            object errorTaskProject;
                            ErrorHandler.ThrowOnFailure(objErrorTask.HierarchyItem.GetProperty(
                                VSConstants.VSITEMID_ROOT, Convert.ToInt32(__VSHPROPID.VSHPROPID_ExtObject), out errorTaskProject));

                            if (errorTaskProject != null && errorTaskProject is Project)
                            {
                                // Cast the Project of the ErrorTask
                                var objErrorTaskProject = (errorTaskProject as Project);

                                if (objErrorTaskProject.UniqueName == projectRemoved.UniqueName)
                                {
                                    // If it is the same Project then remove the ErrorTask from the Error List pane and our collection
                                    VisualStudioHelper.RemoveTask(objErrorTask);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Removes a ErrorTask from the ErrorList pane and our collection.
        /// </summary>
        /// <param name="errorTaskToRemove">ErrorTask to remove.</param>
        private static void RemoveTask(ErrorTask errorTaskToRemove)
        {
            try
            {
                // Stop refreshing the task list until ResumeRefresh is called
                VisualStudioHelper._errorListProvider.SuspendRefresh();

                // Remove event handlers
                errorTaskToRemove.Navigate -= VisualStudioHelper.ErrorTaskNavigate;

                // Remove the ErrorTask from our collection
                VisualStudioHelper.ErrorTaskCollection.Remove(errorTaskToRemove);

                // Remove the ErrorTask from the ErrorList pane
                VisualStudioHelper._errorListProvider.Tasks.Remove(errorTaskToRemove);
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show(ex.Message, Resources.Error_Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restart the refreshing of the task list after it has been suspended
                VisualStudioHelper._errorListProvider.ResumeRefresh();
            }
        }

        #endregion Error List pane manipulation
    }

    /// <summary>
    /// Visual Studio extension methods.
    /// </summary>
    internal static class VisualStudioExtensions
    {
        /// <summary>
        /// Adds a class to the <see cref="CodeNamespace"/> and supports adding a partial class.
        /// </summary>
        /// <param name="codeNamespace"><see cref="CodeNamespace"/> instance.</param>
        /// <param name="name">Name of the class.</param>
        /// <param name="baseName">Name of the base class. Null indicates no base class.</param>
        /// <param name="access">Class access.</param>
        /// <param name="classKind">Class kind.</param>
        /// <returns><see cref="CodeClass"/> instance.</returns>
        internal static CodeClass AddClassWithPartialSupport(this CodeNamespace codeNamespace, string name, string baseName, 
            vsCMAccess access, vsCMClassKind classKind)
        {
            // Workaround to support existing partial class, set name after.
            CodeClass codeClass = codeNamespace.AddClass(Resources.TempClassName, AppConstants.PLACE_AT_THE_END, baseName, null, access);

            (codeClass as CodeClass2).ClassKind = classKind;

            codeClass.Name = name;

            return codeClass;
        }
    }
}