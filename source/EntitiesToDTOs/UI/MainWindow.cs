using EnvDTE;
using EnvDTE80;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using EntitiesToDTOs.Events;
using EntitiesToDTOs.Generators;
using EntitiesToDTOs.Generators.Parameters;
using EntitiesToDTOs.Helpers;
using EntitiesToDTOs.Properties;
using Microsoft.VisualStudio.Shell;
using EntitiesToDTOs.Domain;
using EntitiesToDTOs.Forms;
using EntitiesToDTOs.Domain.Enums;

// TODO: ffernandez, how to get project language (for feature to support multiple languages)
// EnvDTE.Project.CodeModel.Language (string)
// EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB
// EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp

namespace EntitiesToDTOs.UI
{
    /// <summary>
    /// Main Window of the AddIn
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Represents the IDE (Visual Studio)
        /// </summary>
        private DTE2 _applicationObject;

        /// <summary>
        /// Reference to the ProgressWindow used to show the process progress
        /// </summary>
        private GenerationProgressWindow ProgressWindow { get; set; }

        // Column indexes of types grid
        private int TypeCellWarnings = 0;
        private int TypeCellName = 1;
        private int TypeCellType = 2;
        private int TypeCellGenerate = 3;

        /// <summary>
        /// Transparent image.
        /// </summary>
        private Bitmap TransparentImage
        {
            get
            {
                if (_transparentImage == null)
                {
                    _transparentImage = new Bitmap(22, 22);
                    Graphics graphics = Graphics.FromImage(_transparentImage);
                    graphics.FillRectangle(Brushes.Transparent, 0, 0, 22, 22);
                    graphics.Dispose();
                }

                return _transparentImage;
            }
        }
        private Bitmap _transparentImage = null;

        /// <summary>
        /// Current EDMX source.
        /// </summary>
        private EdmxDocument CurrentSourceEdmx
        {
            get
            {
                // Validate EDMX
                if (this.rbSourceEdmx.Checked == true && this.lstEntitySources.SelectedItem != null)
                {
                    // Get EDMX Document as ProjectItem
                    var edmxProjectItem = (this.lstEntitySources.SelectedItem as ProjectItem);

                    if (edmxProjectItem.Name != _currentSourceEdmxName)
                    {
                        _currentSourceEdmxName = edmxProjectItem.Name;

                        // Get EDMX Document
                        _currentSourceEdmx = EdmxHelper.GetEdmxDocument(edmxProjectItem);
                    }
                }
                else
                {
                    _currentSourceEdmx = null;
                    _currentSourceEdmxName = null;
                }

                return _currentSourceEdmx;
            }
        }
        private EdmxDocument _currentSourceEdmx = null;
        private string _currentSourceEdmxName = null;

        /// <summary>
        /// Current project source.
        /// </summary>
        private Project CurrentSourceProject
        {
            get
            {
                // Validate project
                if (this.rbSourceProject.Checked == true && this.lstEntitySources.SelectedItem != null)
                {
                    // Get selected item as Project
                    return (this.lstEntitySources.SelectedItem as Project);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// EDMX items found in the solution.
        /// </summary>
        private List<ProjectItem> EdmxItems { get; set; }

        /// <summary>
        /// Projects found in the solution.
        /// </summary>
        private List<Project> Projects { get; set; }

        /// <summary>
        /// Classes obtained from projects.
        /// </summary>
        private Dictionary<string, IEnumerable<CodeClass>> ProjectCodeClasses { get; set; }

        /// <summary>
        /// Indicates if new release check is already done. 
        /// Static field so the check is done only one time per Visual Studio session.
        /// </summary>
        private static bool IsNewReleaseCheckDone = false;

        /// <summary>
        /// Indicates if MainWindow form is closing.
        /// </summary>
        private bool IsMainWindowClosing { get; set; }



        /// <summary>
        /// Creates a new instance of MainWindow
        /// </summary>
        /// <param name="applicationObject"></param>
        public MainWindow(DTE2 applicationObject)
        {
            InitializeComponent();

            try
            {
                // Set Window Caption
                this.Text = string.Format(Resources.MainWindow_Caption, AssemblyHelper.Version);

                // Set the object that represents the IDE (Visual Studio)
                _applicationObject = applicationObject;

                // Check if there is an open Solution
                if (VisualStudioHelper.IsSolutionOpen(_applicationObject) == false)
                {
                    MessageHelper.ShowErrorMessage(Resources.Error_NoSolution);
                }
                else
                {
                    GeneratorManager.ClearEventHandlers();
                    this.Advertising_Init();
                    EdmxHelper.Initialize();
                    this.InitCustomHeaderCommentControl();
                    this.AssignUIEventHandlers();

                    // Initialize classes obtained from projects
                    this.ProjectCodeClasses = new Dictionary<string, IEnumerable<CodeClass>>();

                    // Get solution items
                    List<ProjectItem> solutionItems = 
                        VisualStudioHelper.GetSolutionProjectItems(_applicationObject.Solution);

                    // Get solution EDMX items
                    this.EdmxItems = solutionItems
                        .Where(i => i.Name.EndsWith(Resources.EdmxFileExtension))
                        .OrderBy(i => i.Name).ToList();

                    // Get solution projects
                    this.Projects = VisualStudioHelper.GetProjectsFromSolution(_applicationObject.Solution);

                    // lstEntitySources
                    this.lstEntitySources.DisplayMember = Resources.ProjectItem_Name;
                    this.lstEntitySources.DataSource = this.EdmxItems;
                    if (this.lstEntitySources.Items.Count > 0)
                    {
                        this.lstEntitySources.SelectedIndex = 0;
                    }

                    // Show target options
                    this.ShowTargetOptions(_applicationObject.Solution);

                    // Attach to GeneratorManager events
                    GeneratorManager.OnComplete += this.GeneratorManager_OnComplete;
                    GeneratorManager.OnCancel += this.GeneratorManager_OnCancel;
                    GeneratorManager.OnException += this.GeneratorManager_OnException;

                    this.LoadAddInConfig();
                    this.LoadLatestConfiguration();
                    this.CheckIfUserHasToRateThisRelease();

                    // Check if there is a new release available
                    this.CheckNewRelease_Init();
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        #region UI Events

        /// <summary>
        /// Executed when the window is about to be closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            VisualStudioHelper.SetStatusBarMessage(Resources.Info_Ready);

            this.IsMainWindowClosing = true;
        }

        /// <summary>
        /// Executed when the selected index of lstEntitySources changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstEntitySources_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCustomizePanel();
        }

        /// <summary>
        /// Executed when the User checks the Custom Header Comment checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbCustomHeaderComment_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomHeaderComment.Enabled = this.cbCustomHeaderComment.Checked;
        }

        /// <summary>
        /// Executed when the User checks the Generate Assemblers checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbGenerateAssemblers_CheckedChanged(object sender, EventArgs e)
        {
            this.ToggleGenerateAssemblers(this.cbGenerateAssemblers.Checked);
        }

        /// <summary>
        /// Executed when the User checks the Use Default Namespace checkbox of DTOs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbUseDefaultNamespaceDTOs_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbUseDefaultNamespaceDTOs.Checked == true)
            {
                this.txtNamespaceDTOs.Enabled = false;

                this.ShowTargetDefaultNamespace(this.treeTargetDTOs, this.txtNamespaceDTOs);
            }
            else
            {
                this.txtNamespaceDTOs.Enabled = true;
            }
        }

        /// <summary>
        /// Executed when the User checks the Use Default Namespace checkbox of Assemblers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbUseDefaultNamespaceAssemblers_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbUseDefaultNamespaceAssemblers.Checked == true)
            {
                this.txtNamespaceAssemblers.Enabled = false;

                this.ShowTargetDefaultNamespace(this.treeTargetAssemblers, this.txtNamespaceAssemblers);
            }
            else
            {
                this.txtNamespaceAssemblers.Enabled = true;
            }
        }

        /// <summary>
        /// Executed when the User clicks the Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Executed when the User clicks the Generate button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnGenerate_Click(object sender, EventArgs e)
        {
            this.Generate();
        }

        /// <summary>
        /// Executed when the User clicks the Help button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnHelp_Click(object sender, EventArgs e)
        {
            var helpWin = new HelpWindow();
            helpWin.ShowDialog(this);
        }

        /// <summary>
        /// Executed when the User enters the Help button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnHelp_MouseEnter(object sender, EventArgs e)
        {
            this.btnHelp.Image = Resources.img_help_hover;
        }

        /// <summary>
        /// Executed when the User leaves the Help button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnHelp_MouseLeave(object sender, EventArgs e)
        {
            this.btnHelp.Image = Resources.img_help;
        }

        /// <summary>
        /// Executed before the User selects a different target for DTOs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeTargetDTOs_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.treeTargetDTOs.SelectedNode != null)
            {
                var regularFont = new Font(this.treeTargetDTOs.Font, FontStyle.Regular);
                this.treeTargetDTOs.SelectedNode.NodeFont = regularFont;
            }
        }

        /// <summary>
        /// Executed before the User selects a different target for Assemblers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeTargetAssemblers_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.treeTargetAssemblers.SelectedNode != null)
            {
                var regularFont = new Font(this.treeTargetAssemblers.Font, FontStyle.Regular);
                this.treeTargetAssemblers.SelectedNode.NodeFont = regularFont;
            }
        }

        /// <summary>
        /// Executed when the User selects a different target for DTOs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeTargetDTOs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.cbUseDefaultNamespaceDTOs.Checked)
            {
                this.ShowTargetDefaultNamespace(this.treeTargetDTOs, this.txtNamespaceDTOs);
            }

            this.HighlightSelectedNode(this.treeTargetDTOs);
        }

        /// <summary>
        /// Executed when the User selects a different target for Assemblers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeTargetAssemblers_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.cbUseDefaultNamespaceAssemblers.Checked)
            {
                this.ShowTargetDefaultNamespace(this.treeTargetAssemblers, this.txtNamespaceAssemblers);
            }

            this.HighlightSelectedNode(this.treeTargetAssemblers);
        }

        /// <summary>
        /// Executed when the User checks one of the Source File Generation Type radio buttons for Assemblers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rbSourceFileGenerationTypeAssemblers_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbOneSourceFileAssemblers.Checked)
            {
                this.txtSourceFileNameAssemblers.Enabled = true;
                this.txtSourceFileNameAssemblers.Text = string.Empty;
            }
            else
            {
                this.txtSourceFileNameAssemblers.Enabled = false;
                this.txtSourceFileNameAssemblers.Text = Resources.Text_DefaultSourceFileName;
            }
        }

        /// <summary>
        /// Executed when the User checks one of the Source File Generation Type radio buttons for DTOs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rbSourceFileGenerationTypeDTOs_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbOneSourceFileDTOs.Checked)
            {
                this.txtSourceFileNameDTOs.Enabled = true;
                this.txtSourceFileNameDTOs.Text = string.Empty;
            }
            else
            {
                this.txtSourceFileNameDTOs.Enabled = false;
                this.txtSourceFileNameDTOs.Text = Resources.Text_DefaultSourceFileName;
            }
        }

        /// <summary>
        /// Executed when the User clicks the Config Export menu button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate generation process
                this.ValidateGeneration();

                // Show Save File Dialog
                var dialog = new SaveFileDialog();
                dialog.Filter = Resources.GenConfigFileFilter;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Export configuration
                    ConfigurationHelper.Export(this.GetGeneratorManagerParams(), dialog.FileName);

                    // Show OK message
                    VisualStudioHelper.SetStatusBarMessage(Resources.Info_ConfigExportComplete);
                }
            }
            catch (Exception ex)
            {
                this.ManageException(ex);
            }
        }

        /// <summary>
        /// Executed when the User clicks the Config Import menu button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Show Open File Dialog
                var dialog = new OpenFileDialog();
                dialog.Filter = Resources.GenConfigFileFilter;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the configuration
                    bool showWarnings = true;
                    this.LoadConfiguration(dialog.FileName, showWarnings);

                    // Validate generation process
                    this.ValidateGeneration();

                    // Shoe OK message
                    VisualStudioHelper.SetStatusBarMessage(Resources.Info_ConfigImportComplete);
                }
            }
            catch (Exception ex)
            {
                this.ManageException(ex);
            }
        }

        /// <summary>
        /// Executed when the user clicks the generate all types checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbGenerateAllTypes_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = this.chbGenerateAllTypes.Checked;

            this.chbGenerateComplexTypes.CheckedChanged -= this.chbGenerateComplexTypes_CheckedChanged;
            this.chbGenerateComplexTypes.Checked = isChecked;
            this.chbGenerateComplexTypes.CheckedChanged += this.chbGenerateComplexTypes_CheckedChanged;

            this.chbGenerateEntityTypes.CheckedChanged -= this.chbGenerateEntityTypes_CheckedChanged;
            this.chbGenerateEntityTypes.Checked = isChecked;
            this.chbGenerateEntityTypes.CheckedChanged += this.chbGenerateEntityTypes_CheckedChanged;

            this.UpdateCheckedTypes("Complex", isChecked);
            this.UpdateCheckedTypes("Entity", isChecked);

            this.UpdateGenerateAllTypesCheckedState();

            // Checks warnings in types to generate
            this.CheckTypesWarnings();
        }
        
        /// <summary>
        /// Executed when the user clicks the generate complex types checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbGenerateComplexTypes_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateGenerateAllTypesCheckedState();

            this.UpdateCheckedTypes("Complex", this.chbGenerateComplexTypes.Checked);

            // Checks warnings in types to generate
            this.CheckTypesWarnings();
        }

        /// <summary>
        /// Executed when the user clicks the generate entity types checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbGenerateEntityTypes_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateGenerateAllTypesCheckedState();

            this.UpdateCheckedTypes("Entity", this.chbGenerateEntityTypes.Checked);

            // Checks warnings in types to generate
            this.CheckTypesWarnings();
        }

        /// <summary>
        /// Executed when cell content of types grid is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == TypeCellGenerate)
            {
                this.UpdateGenerateTypesCheckedState();

                // Checks warnings in types to generate
                this.CheckTypesWarnings();
            }
        }

        /// <summary>
        /// Executed when dto identifier "none" option checked status changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDTOIdentifierNone_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDTOIdentifierWord.Enabled = (rbDTOIdentifierNone.Checked == false);
        }

        /// <summary>
        /// Executed when assembler identifier "none" option checked status changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbAssemblerIdentifierNone_CheckedChanged(object sender, EventArgs e)
        {
            this.txtAssemblerIdentifierWord.Enabled = (rbAssemblerIdentifierNone.Checked == false);
        }

        /// <summary>
        /// Executed when the user clicks one of notify new versions option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCheckReleases_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReleaseNotifications();
        }

        /// <summary>
        /// Executed when the user checks the EDMX source option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSourceEdmx_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbSourceEdmx.Checked == true)
            {
                this.lstEntitySources.DataSource = this.EdmxItems;
            }
        }

        /// <summary>
        /// Executed when the user checks the Project source option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSourceProject_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbSourceProject.Checked == true)
            {
                this.lstEntitySources.DataSource = this.Projects;
            }
        }

        /// <summary>
        /// Executed when the user clicks the Donate button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDonate_Click(object sender, EventArgs e)
        {
            try
            {
                // Open donate URL
                System.Diagnostics.Process.Start(Resources.DonateURL);
            }
            catch (Exception ex)
            {
                MessageHelper.ShowErrorMessage(ex.Message + Environment.NewLine + Resources.DonateURL);
            }
        }

        #endregion UI Events

        #region GeneratorManager Events

        /// <summary>
        /// Executed when an Exception is raised by the GeneratorManager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorManager_OnException(object sender, GeneratorOnExceptionEventArgs e)
        {
            this.ManageException(e.Exception);
        }

        /// <summary>
        /// Executed when a Cancel event is raised by the GeneratorManager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorManager_OnCancel(object sender, GeneratorOnCancelEventArgs e)
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Executed when the GeneratorManager finishes the process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorManager_OnComplete(object sender, GeneratorOnCompleteEventArgs e)
        {
            // Close progress window
            this.ProgressWindow.Close();

            // Check Warnings
            if (VisualStudioHelper.ErrorTaskCollectionHasItems())
            {
                // Show Warning message
                MessageBox.Show(Resources.Info_GenerationCompleteWithWarnings, Resources.Info_Caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Show ErrorList pane
                VisualStudioHelper.ShowErrorList();
            }
            else
            {
                // Show OK message
                MessageBox.Show(Resources.Info_GenerationComplete, Resources.Info_Caption, 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Release COM Object (use ReleaseComObject only if it is absolutely required)
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(_applicationObject);

            // Close AddIn
            this.Close();
        }

        #endregion GeneratorManager Events

        #region CheckNewRelease

        /// <summary>
        /// Checks if a new release exists (initializes a background worker).
        /// </summary>
        private void CheckNewRelease_Init()
        {
            try
            {
                // Check only one time per Visual Studio session
                if (MainWindow.IsNewReleaseCheckDone == false)
                {
                    // Indicate that the new release check has been done
                    MainWindow.IsNewReleaseCheckDone = true;

                    // Get AddIn general config
                    AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

                    // Process in a background thread
                    var worker = new BackgroundWorker();

                    worker.WorkerReportsProgress = true;
                    worker.WorkerSupportsCancellation = true;

                    worker.DoWork += new DoWorkEventHandler(this.CheckNewRelease_DoWork);
                    worker.ProgressChanged += new ProgressChangedEventHandler(this.CheckNewRelease_ProgressChanged);

                    // Send release status filter as parameter
                    worker.RunWorkerAsync(addInConfig.ReleaseStatusFilter);
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);

                MessageHelper.ShowExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Checks if a new release exists (in a background worker).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckNewRelease_DoWork(object sender, DoWorkEventArgs args)
        {
            var worker = (sender as BackgroundWorker);
            var releaseStatusFilter = (args.Argument as List<ReleaseStatus>);

            try
            {
                // Check if a new release exists
                Release newRelease = UpdateHelper.CheckNewRelease(releaseStatusFilter);

                if (newRelease != null)
                {
                    // Report new release
                    worker.ReportProgress(100, newRelease);
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
        private void CheckNewRelease_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
            else if (e.UserState is Release)
            {
                // Get the new release
                var newRelease = (e.UserState as Release);

                // Notify the user
                var updateWindow = new UpdateWindow(newRelease);
                updateWindow.ShowDialog();
            }
        }

        #endregion CheckNewRelease

        #region Methods

        /// <summary>
        /// Assigns UI event handlers.
        /// </summary>
        private void AssignUIEventHandlers()
        {
            this.chbGenerateAllTypes.CheckedChanged += this.chbGenerateAllTypes_CheckedChanged;
            this.chbGenerateComplexTypes.CheckedChanged += this.chbGenerateComplexTypes_CheckedChanged;
            this.chbGenerateEntityTypes.CheckedChanged += this.chbGenerateEntityTypes_CheckedChanged;

            this.dgvTypes.CellContentClick += this.dgvTypes_CellContentClick;

            this.cbCheckReleasesStable.CheckedChanged += this.cbCheckReleases_CheckedChanged;
            this.cbCheckReleasesBeta.CheckedChanged += this.cbCheckReleases_CheckedChanged;

            this.treeTargetDTOs.BeforeSelect += this.treeTargetDTOs_BeforeSelect;
            this.treeTargetAssemblers.BeforeSelect += this.treeTargetAssemblers_BeforeSelect;
            this.treeTargetDTOs.AfterSelect += this.treeTargetDTOs_AfterSelect;
            this.treeTargetAssemblers.AfterSelect += this.treeTargetAssemblers_AfterSelect;

            this.lstEntitySources.SelectedIndexChanged += this.lstEntitySources_SelectedIndexChanged;
        }

        /// <summary>
        /// Highlights the selected node of the received TreeView.
        /// </summary>
        /// <param name="treeView">Selected node's TreeView.</param>
        private void HighlightSelectedNode(TreeView treeView)
        {
            if (treeView.SelectedNode != null)
            {
                var underlineFont = new Font(treeView.Font, FontStyle.Underline);
                treeView.SelectedNode.NodeFont = underlineFont;
            }
        }

        /// <summary>
        /// Initializes the custom header comment control.
        /// </summary>
        private void InitCustomHeaderCommentControl()
        {
            // Get IDE text editor comment colors
            ColorableItems ci = VisualStudioHelper.GetTextEditorCommentColorableItems();

            // Set background color
            int oleColor = Convert.ToInt32(ci.Background);
            Color color = ColorTranslator.FromOle(oleColor);
            this.txtCustomHeaderComment.BackColor = color;

            // Set foreground color
            oleColor = Convert.ToInt32(ci.Foreground);
            color = ColorTranslator.FromOle(oleColor);
            this.txtCustomHeaderComment.ForeColor = color;

            // Set normal or bold text
            this.txtCustomHeaderComment.Font = new Font(this.txtCustomHeaderComment.Font, 
                ci.Bold ? FontStyle.Bold : FontStyle.Regular);
        }

        /// <summary>
        /// Shows target's default namespace.
        /// </summary>
        /// <param name="treeView">TreeView where the target is.</param>
        /// <param name="textBox">TextBox where to show default namespace.</param>
        private void ShowTargetDefaultNamespace(TreeView treeView, TextBox textBox)
        {
            if (treeView.SelectedNode != null)
            {
                // Get node value
                object nodeValue = (treeView.SelectedNode as TreeNodeExtended).Value;

                // Continue if it is not the solution
                if ((nodeValue is Solution) == false)
                {
                    EnvDTE.Properties properties;

                    if (nodeValue is Project)
                    {
                        properties = (nodeValue as Project).Properties;
                    }
                    else
                    {
                        properties = (nodeValue as ProjectItem).Properties;
                    }

                    // Set namespace
                    textBox.Text = VisualStudioHelper.GetDefaultNamespaceFromProperties(properties);
                }
            }
        }

        /// <summary>
        /// Updates the checked state of the generate types filters.
        /// </summary>
        private void UpdateGenerateTypesCheckedState()
        {
            bool generateComplexTypesAll = true;
            bool generateComplexTypesSome = false;
            bool generateEntityTypesAll = true;
            bool generateEntityTypesSome = false;

            foreach (DataGridViewRow row in this.dgvTypes.Rows)
            {
                if ((string)row.Cells[TypeCellType].Value == "Complex")
                {
                    if ((bool)row.Cells[TypeCellGenerate].EditedFormattedValue == false)
                    {
                        generateComplexTypesAll = false;
                    }
                    else
                    {
                        generateComplexTypesSome = true;
                    }
                }
                else
                {
                    if ((bool)row.Cells[TypeCellGenerate].EditedFormattedValue == false)
                    {
                        generateEntityTypesAll = false;
                    }
                    else
                    {
                        generateEntityTypesSome = true;
                    }
                }
            }

            this.chbGenerateComplexTypes.CheckedChanged -= this.chbGenerateComplexTypes_CheckedChanged;
            this.chbGenerateComplexTypes.CheckState =
                (generateComplexTypesAll == true ? CheckState.Checked : CheckState.Unchecked);
            if (generateComplexTypesAll == false && generateComplexTypesSome == true)
            {
                this.chbGenerateComplexTypes.CheckState = CheckState.Indeterminate;
            }
            this.chbGenerateComplexTypes.CheckedChanged += this.chbGenerateComplexTypes_CheckedChanged;

            this.chbGenerateEntityTypes.CheckedChanged -= this.chbGenerateEntityTypes_CheckedChanged;
            this.chbGenerateEntityTypes.CheckState =
                (generateEntityTypesAll == true ? CheckState.Checked : CheckState.Unchecked);
            if (generateEntityTypesAll == false && generateEntityTypesSome == true)
            {
                this.chbGenerateEntityTypes.CheckState = CheckState.Indeterminate;
            }
            this.chbGenerateEntityTypes.CheckedChanged += this.chbGenerateEntityTypes_CheckedChanged;

            this.UpdateGenerateAllTypesCheckedState();
        }

        /// <summary>
        /// Updates the checked state of the generate all types option.
        /// </summary>
        private void UpdateGenerateAllTypesCheckedState()
        {
            this.chbGenerateAllTypes.CheckedChanged -= this.chbGenerateAllTypes_CheckedChanged;

            if (this.chbGenerateComplexTypes.CheckState == CheckState.Checked
                && this.chbGenerateEntityTypes.CheckState == CheckState.Checked)
            {
                this.chbGenerateAllTypes.CheckState = CheckState.Checked;
            }
            else if (this.chbGenerateComplexTypes.CheckState == CheckState.Checked
                || this.chbGenerateEntityTypes.CheckState == CheckState.Checked
                || this.chbGenerateComplexTypes.CheckState == CheckState.Indeterminate
                || this.chbGenerateEntityTypes.CheckState == CheckState.Indeterminate)
            {
                this.chbGenerateAllTypes.CheckState = CheckState.Indeterminate;
            }
            else
            {
                this.chbGenerateAllTypes.CheckState = CheckState.Unchecked;
            }

            this.chbGenerateAllTypes.CheckedChanged += this.chbGenerateAllTypes_CheckedChanged;
        }

        /// <summary>
        /// Updates the checked state of the types to generate which are from the type indicated.
        /// </summary>
        /// <param name="type">Type filter.</param>
        /// <param name="isChecked">Indicates the checked state.</param>
        private void UpdateCheckedTypes(string type, bool isChecked)
        {
            foreach (DataGridViewRow row in this.dgvTypes.Rows)
            {
                if ((string)row.Cells[TypeCellType].Value == type)
                {
                    row.Cells[TypeCellGenerate].Value = isChecked;
                }
            }
        }

        /// <summary>
        /// In case of Exception, this method must be call.
        /// </summary>
        private void ManageException(Exception ex)
        {
            LogManager.LogError(ex);

            if (this.ProgressWindow != null)
            {
                // Close progress window
                this.ProgressWindow.Close();
            }

            MessageHelper.ShowExceptionMessage(ex);

            this.Enabled = true;
        }

        /// <summary>
        /// Updates customize panel based on the current source selected.
        /// </summary>
        private void UpdateCustomizePanel()
        {
            try
            {
                // Clear types to generate
                this.dgvTypes.Rows.Clear();

                // Validate source
                if (this.lstEntitySources.SelectedItem != null)
                {
                    int rowIndex = -1;

                    if (this.rbSourceEdmx.Checked == true)
                    {
                        // Source is an EDMX

                        // Get complex types
                        List<XElement> allTypes = EdmxHelper.GetComplexTypeNodes(this.CurrentSourceEdmx).ToList();

                        // Get entity types
                        allTypes.AddRange(EdmxHelper.GetEntityTypeNodes(this.CurrentSourceEdmx));

                        // Order types by name
                        allTypes = allTypes.OrderBy(e => e.Attribute(EdmxNodeAttributes.EntityType_Name).Value).ToList();

                        // Add types to panel
                        foreach (XElement type in allTypes)
                        {
                            // Add row
                            rowIndex = this.dgvTypes.Rows.Add(new DataGridViewRowExtended());
                            var row = (this.dgvTypes.Rows[rowIndex] as DataGridViewRowExtended);

                            // Set value
                            row.Value = type;
                            
                            // Set type name
                            row.Cells[TypeCellName].Value = type.Attribute(EdmxNodeAttributes.EntityType_Name).Value;

                            // Set type
                            row.Cells[TypeCellType].Value = (type.Name.LocalName == EdmxNodes.EntityType ? "Entity" : "Complex");

                            // Set generate value
                            row.Cells[TypeCellGenerate].Value = true;

                            // Set warnings cell, no image
                            row.Cells[TypeCellWarnings].Value = this.TransparentImage;
                        }
                    }
                    else
                    {
                        // Source is a project

                        // If code classes were not loaded from this project, then load them
                        if (this.ProjectCodeClasses.ContainsKey(this.CurrentSourceProject.FullName) == false)
                        {
                            this.ProjectCodeClasses.Add(this.CurrentSourceProject.FullName, 
                                VisualStudioHelper.GetProjectCodeClasses(this.CurrentSourceProject));
                        }

                        // Get project code classes
                        IEnumerable<CodeClass> projectCodeClasses = 
                            this.ProjectCodeClasses[this.CurrentSourceProject.FullName];

                        // Add types to panel
                        foreach (CodeClass codeClass in projectCodeClasses)
                        {
                            // Add row
                            rowIndex = this.dgvTypes.Rows.Add(new DataGridViewRowExtended());
                            var row = (this.dgvTypes.Rows[rowIndex] as DataGridViewRowExtended);

                            // Set value
                            row.Value = codeClass;

                            // Set type name
                            row.Cells[TypeCellName].Value = codeClass.Name;

                            // Set type
                            row.Cells[TypeCellType].Value = "Entity";
                            // TODO: ffernandez, take in count attribute complex of code first classes

                            // Set generate value
                            row.Cells[TypeCellGenerate].Value = true;

                            // Set warnings cell, no image
                            row.Cells[TypeCellWarnings].Value = this.TransparentImage;
                        }
                    }

                    // Update filter checked states and check warnings
                    this.UpdateGenerateTypesCheckedState();

                } // END if...
            }
            catch (Exception ex)
            {
                this.ManageException(ex);
            }
        }

        /// <summary>
        /// Checks warnings in types to generate.
        /// </summary>
        private void CheckTypesWarnings()
        {
            if (this.rbSourceEdmx.Checked == true)
            {
                this.CheckTypesWarningsSourceEdmx();
            }
            else
            {
                this.CheckTypesWarningsSourceProject();
            }
        }

        /// <summary>
        /// Checks warnings in types to generate from EDMX source.
        /// </summary>
        private void CheckTypesWarningsSourceEdmx()
        {
            // Get navigation nodes
            IEnumerable<XElement> navigationNodes =
                EdmxHelper.GetNavigationPropertyNodes(this.CurrentSourceEdmx);

            // Get association nodes
            IEnumerable<XElement> associationNodes =
                EdmxHelper.GetAssociationNodes(this.CurrentSourceEdmx);

            // Get the rows collection to query
            IEnumerable<DataGridViewRowExtended> rowsCollection =
                this.dgvTypes.Rows.OfType<DataGridViewRowExtended>();

            // Loop through rows
            foreach (DataGridViewRowExtended row in this.dgvTypes.Rows)
            {
                bool showWarnings = false;
                string warningText = string.Empty;

                // Check for warnings only if it is an entity and is going to be generated
                if ((string)row.Cells[TypeCellType].Value == "Entity"
                    && (bool)row.Cells[TypeCellGenerate].EditedFormattedValue == true)
                {
                    // Get the entity node
                    XElement rowEntityNode = (row.Value as XElement);

                    // Get the base type name (if exists)
                    string baseTypeName = EdmxHelper.GetEntityBaseType(rowEntityNode);

                    if (string.IsNullOrWhiteSpace(baseTypeName) == false)
                    {
                        // Check if the base type is going to be generated
                        DataGridViewRow rowBase = rowsCollection.First(r =>
                            (string)r.Cells[TypeCellName].Value == baseTypeName);

                        if ((bool)rowBase.Cells[TypeCellGenerate].EditedFormattedValue == false)
                        {
                            // Base type is not generated, add warning
                            showWarnings = true;
                            warningText += Environment.NewLine
                                + string.Format(Resources.Warning_BaseTypeNotGenerated,
                                rowBase.Cells[TypeCellName].Value);
                        }
                    }

                    // Get navigations of this entity
                    IEnumerable<XElement> entityNavigations = navigationNodes.Where(n =>
                        n.Parent.Attribute(EdmxNodeAttributes.EntityType_Name).Value
                        == (string)row.Cells[TypeCellName].Value);

                    foreach (XElement navNode in entityNavigations)
                    {
                        // Get toRole attribute value
                        string toRole = navNode.Attribute(
                            EdmxNodeAttributes.NavigationProperty_ToRole).Value;

                        // Get the association name
                        string associationName = EdmxHelper.GetNavigationAssociationName(navNode);

                        // Get the association node
                        XElement assocNode = associationNodes.First(n =>
                            n.Attribute(EdmxNodeAttributes.Association_Name).Value == associationName);

                        // Get destination type name
                        string destinationTypeName = assocNode.DescendantsCSDL(EdmxNodes.End).First(n =>
                            n.Attribute(EdmxNodeAttributes.End_Role).Value == toRole)
                            .Attribute(EdmxNodeAttributes.End_Type).Value;

                        // Remove namespace from destination type name
                        destinationTypeName = EdmxHelper.GetNameWithoutNamespace(destinationTypeName);

                        // Check if destination type is going to be generated
                        DataGridViewRow rowDestination = rowsCollection.First(r =>
                            (string)r.Cells[TypeCellName].Value == destinationTypeName);

                        if ((bool)rowDestination.Cells[TypeCellGenerate].EditedFormattedValue == false)
                        {
                            // Destination type is not generated, add warning
                            showWarnings = true;
                            warningText += Environment.NewLine
                                + string.Format(Resources.Warning_DestTypeNotGenerated,
                                rowDestination.Cells[TypeCellName].Value);
                        }
                    }
                }

                if (showWarnings == true)
                {
                    // Add header to warning text and set image
                    warningText = ("Warning" + warningText);
                    row.Cells[TypeCellWarnings].Value = Resources.img_warning;
                }
                else
                {
                    // Set no image
                    row.Cells[TypeCellWarnings].Value = this.TransparentImage;
                }

                // Set warning text
                row.Cells[TypeCellWarnings].ToolTipText = warningText;
            }
        }

        /// <summary>
        /// Checks warnings in types to generate from project source.
        /// </summary>
        private void CheckTypesWarningsSourceProject()
        {
            // Get the rows collection to query
            IEnumerable<DataGridViewRowExtended> rowsCollection =
                this.dgvTypes.Rows.OfType<DataGridViewRowExtended>();

            // Get object class full name
            string objectClassFullName = typeof(object).FullName;

            // Loop through rows
            foreach (DataGridViewRowExtended row in this.dgvTypes.Rows)
            {
                bool showWarnings = false;
                string warningText = string.Empty;

                // Check for warnings only if it is going to be generated
                if ((bool)row.Cells[TypeCellGenerate].EditedFormattedValue == true)
                {
                    // Get CodeClass
                    CodeClass codeClass = (row.Value as CodeClass);

                    // Get base type full names
                    var baseTypeFullNames = new List<string>();
                    if (codeClass.Bases.Item(1).FullName != objectClassFullName)
                    {
                        baseTypeFullNames = VisualStudioHelper.GetFullNamesFromFullName(codeClass.Bases.Item(1).FullName);
                    }

                    if (baseTypeFullNames.Count > 0)
                    {
                        var notGeneratedBases = new List<string>();

                        foreach (string baseTypeFN in baseTypeFullNames)
                        {
                            // Check if the base type is going to be generated
                            DataGridViewRowExtended rowBase = rowsCollection.FirstOrDefault(r =>
                                (r.Value as CodeClass).FullName == baseTypeFN);

                            // The base type could be missing (i.e. not found in the project)
                            if (rowBase != null
                                && (bool)rowBase.Cells[TypeCellGenerate].EditedFormattedValue == false
                                && notGeneratedBases.Contains(rowBase.Cells[TypeCellName].Value) == false)
                            {
                                // Base type is not generated
                                notGeneratedBases.Add((string)rowBase.Cells[TypeCellName].Value);
                            }
                        }

                        if (notGeneratedBases.Count > 0)
                        {
                            // There are base types that are not checked to generate
                            showWarnings = true;

                            // Add warning text
                            foreach (string notGeneratedBaseName in notGeneratedBases)
                            {
                                warningText += Environment.NewLine
                                    + string.Format(Resources.Warning_BaseTypeNotGenerated, notGeneratedBaseName);
                            }
                        }

                    } // END if

                    // Get class public properties
                    IEnumerable<CodeProperty> publicProperties = codeClass.Members.OfType<CodeElement>()
                        .Where(c => c.Kind == vsCMElement.vsCMElementProperty)
                        .Select(p => p as CodeProperty)
                        .Where(p => p.Access == vsCMAccess.vsCMAccessPublic);

                    var notGeneratedTypes = new List<string>();

                    foreach (CodeProperty property in publicProperties)
                    {
                        // Get property types involved
                        List<string> typesInvolved = VisualStudioHelper.GetTypesFullNamesFromTypeRef(property.Type);

                        foreach (string typeFullName in typesInvolved)
                        {
                            // Check if the type is going to be generated
                            DataGridViewRowExtended rowPropertyType = rowsCollection.FirstOrDefault(r =>
                                (r.Value as CodeClass).FullName == typeFullName);

                            // The type could be missing (i.e. not found in the project)
                            if (rowPropertyType != null
                                && (bool)rowPropertyType.Cells[TypeCellGenerate].EditedFormattedValue == false
                                && notGeneratedTypes.Contains((string)rowPropertyType.Cells[TypeCellName].Value) == false)
                            {
                                // Type is not generated
                                notGeneratedTypes.Add((string)rowPropertyType.Cells[TypeCellName].Value);
                            }

                        } // END foreach

                    } // END foreach

                    if (notGeneratedTypes.Count > 0)
                    {
                        // There are related types that are not checked to generate
                        showWarnings = true;

                        // Add warning text
                        foreach (string notRelatedTypeName in notGeneratedTypes)
                        {
                            warningText += Environment.NewLine
                                + string.Format(Resources.Warning_DestTypeNotGenerated, notRelatedTypeName);
                        }
                    }

                } // END if

                if (showWarnings == true)
                {
                    // Add header to warning text and set image
                    warningText = ("Warning" + warningText);
                    row.Cells[TypeCellWarnings].Value = Resources.img_warning;
                }
                else
                {
                    // Set no image
                    row.Cells[TypeCellWarnings].Value = this.TransparentImage;
                }

                // Set warning text
                row.Cells[TypeCellWarnings].ToolTipText = warningText;

            } // END foreach
        }

        /// <summary>
        /// Process validations to check if the AddIn is ready to start the generation.
        /// </summary>
        private void ValidateGeneration()
        {
            #region Entity source validations
            // Check that a source is selected
            if (this.lstEntitySources.SelectedItem == null)
            {
                throw new ApplicationException(Resources.Error_NoEDMXSource);
            }
            #endregion Entity source validations

            #region DTOs validations
            // Check if at least one type is marked to be generated
            if (this.chbGenerateAllTypes.CheckState == CheckState.Unchecked)
            {
                throw new ApplicationException(Resources.Error_NoTypesToGenerateFrom);
            }

            // Check target for DTOs and source file namespace
            if (this.treeTargetDTOs.SelectedNode == null
                || (this.treeTargetDTOs.SelectedNode as TreeNodeExtended).Value is Solution)
            {
                throw new ApplicationException(Resources.Error_NoTargetDTOs);
            }
            else if (string.IsNullOrWhiteSpace(this.GetSourceFileNamespaceForDTOs()))
            {
                throw new ApplicationException(Resources.Error_InvalidNamespaceDTOs);
            }

            // Check the Source File Generation Type for DTOs
            if (this.rbOneSourceFileDTOs.Checked)
            {
                if (string.IsNullOrWhiteSpace(this.txtSourceFileNameDTOs.Text)
                    || this.txtSourceFileNameDTOs.Text.Contains(Resources.Space))
                {
                    throw new ApplicationException(Resources.Error_InvalidSourceFileNameDTOs);
                }
            }

            // Check identifier
            if (this.rbDTOIdentifierPrefix.Checked == true || this.rbDTOIdentifierSuffix.Checked == true)
            {
                if (string.IsNullOrWhiteSpace(this.txtDTOIdentifierWord.Text) == true)
                {
                    throw new ApplicationException(Resources.Error_IdentifierWordEmpty);
                }
            }
            #endregion DTOs validations

            #region Assemblers validations
            if (this.cbGenerateAssemblers.Checked)
            {
                if (this.treeTargetAssemblers.SelectedNode == null
                    || (this.treeTargetAssemblers.SelectedNode as TreeNodeExtended).Value is Solution)
                {
                    throw new ApplicationException(Resources.Error_NoTargetAssemblers);
                }
                else if (string.IsNullOrWhiteSpace(this.GetSourceFileNamespaceForAssemblers()))
                {
                    throw new ApplicationException(Resources.Error_InvalidNamespaceAssemblers);
                }

                // Check the Source File Generation Type for Assemblers
                if (this.rbOneSourceFileAssemblers.Checked)
                {
                    if (string.IsNullOrWhiteSpace(this.txtSourceFileNameAssemblers.Text)
                        || this.txtSourceFileNameAssemblers.Text.Contains(Resources.Space))
                    {
                        throw new ApplicationException(Resources.Error_InvalidSourceFileNameAssemblers);
                    }
                }

                // Check identifier
                if (this.rbAssemblerIdentifierPrefix.Checked == true || this.rbAssemblerIdentifierSuffix.Checked == true)
                {
                    if (string.IsNullOrWhiteSpace(this.txtAssemblerIdentifierWord.Text) == true)
                    {
                        throw new ApplicationException(Resources.Error_IdentifierWordEmpty);
                    }
                }
            }
            #endregion Assemblers validations
        }

        /// <summary>
        /// Gets the Source File Namespace for DTOs.
        /// </summary>
        /// <returns></returns>
        private string GetSourceFileNamespaceForDTOs()
        {
            string sourceNamespace = null;

            if (this.cbUseDefaultNamespaceDTOs.Checked == true)
            {
                if (this.treeTargetDTOs.SelectedNode != null)
                {
                    object nodeValue = (this.treeTargetDTOs.SelectedNode as TreeNodeExtended).Value;

                    if ((nodeValue is Solution) == false)
                    {
                        EnvDTE.Properties properties;

                        if (nodeValue is Project)
                        {
                            properties = (nodeValue as Project).Properties;
                        }
                        else
                        {
                            properties = (nodeValue as ProjectItem).Properties;
                        }

                        sourceNamespace = VisualStudioHelper.GetDefaultNamespaceFromProperties(properties);
                    }
                }
            }
            else
            {
                sourceNamespace = this.txtNamespaceDTOs.Text;
            }

            return sourceNamespace;
        }

        /// <summary>
        /// Gets the Source File Namespace for Assemblers.
        /// </summary>
        /// <returns></returns>
        private string GetSourceFileNamespaceForAssemblers()
        {
            string sourceNamespace = null;

            if (this.cbUseDefaultNamespaceAssemblers.Checked == true)
            {
                if (this.treeTargetAssemblers.SelectedNode != null)
                {
                    object nodeValue = (this.treeTargetAssemblers.SelectedNode as TreeNodeExtended).Value;

                    if ((nodeValue is Solution) == false)
                    {
                        EnvDTE.Properties properties;

                        if (nodeValue is Project)
                        {
                            properties = (nodeValue as Project).Properties;
                        }
                        else
                        {
                            properties = (nodeValue as ProjectItem).Properties;
                        }

                        sourceNamespace = VisualStudioHelper.GetDefaultNamespaceFromProperties(properties);
                    }
                }
            }
            else
            {
                sourceNamespace = this.txtNamespaceAssemblers.Text;
            }

            return sourceNamespace;
        }

        /// <summary>
        /// Toggles the UI elements related to Assembler generation.
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleGenerateAssemblers(bool enabled)
        {
            this.treeTargetAssemblers.Enabled = enabled;
            this.cbUseDefaultNamespaceAssemblers.Enabled = enabled;
            this.txtNamespaceAssemblers.Enabled = enabled;
            this.groupGenerationTypeAssemblers.Enabled = enabled;
            this.groupIdentifierAssemblers.Enabled = enabled;
        }

        /// <summary>
        /// Shows DTOs target options in tree view control.
        /// </summary>
        /// <param name="solution">Source solution.</param>
        private void ShowTargetOptions(Solution solution)
        {
            // Get solution name
            string solutionName = VisualStudioHelper.GetSolutionName(solution);

            // Create solution node
            var solutionNodeDTOs = new TreeNodeExtended(solutionName);
            solutionNodeDTOs.Value = solution;
            var solutionNodeAssemblers = new TreeNodeExtended(solutionName);
            solutionNodeAssemblers.Value = solution;

            // Loop through solution projects
            foreach (Project project in this.Projects)
            {
                // Create project node
                var projectNodeDTOs = new TreeNodeExtended(project.Name);
                projectNodeDTOs.Value = project;
                var projectNodeAssemblers = new TreeNodeExtended(project.Name);
                projectNodeAssemblers.Value = project;

                // Get first level folders and add them to project node
                List<ProjectItem> folders = VisualStudioHelper.GetFirstLevelFoldersFromProject(project);

                this.AddFoldersToTreeNode(projectNodeDTOs, folders);
                this.AddFoldersToTreeNode(projectNodeAssemblers, folders);

                // Add project node to solution node
                solutionNodeDTOs.Nodes.Add(projectNodeDTOs);
                solutionNodeAssemblers.Nodes.Add(projectNodeAssemblers);
            }

            // Add solution node to tree targets and expand the node
            this.treeTargetDTOs.Nodes.Add(solutionNodeDTOs);
            this.treeTargetAssemblers.Nodes.Add(solutionNodeAssemblers);
            solutionNodeDTOs.Expand();
            solutionNodeAssemblers.Expand();
        }

        /// <summary>
        /// Adds the received folders to the received tree node.
        /// </summary>
        /// <param name="treeNode">Target tree node.</param>
        /// <param name="folders">Source folders.</param>
        private void AddFoldersToTreeNode(TreeNodeExtended treeNode, List<ProjectItem> folders)
        {
            // Loop through folders
            foreach (ProjectItem objFolder in folders)
            {
                // Create folder node
                var folderNode = new TreeNodeExtended(objFolder.Name);
                folderNode.Value = objFolder;

                // Get sub folders
                List<ProjectItem> subFolders =
                    VisualStudioHelper.GetFirstLevelFoldersFromFolder(objFolder);

                // Add sub folders
                this.AddFoldersToTreeNode(folderNode, subFolders);

                // Add folder node to target node
                treeNode.Nodes.Add(folderNode);
            }
        }

        /// <summary>
        /// Gets the types from which to generate the DTOs.
        /// </summary>
        /// <param name="onlyFilteredTypes">Indicates if only filtered types 
        /// or all types checked to be generated must be returned.</param>
        /// <returns></returns>
        private List<string> GetTypesToGenerateFilter(bool onlyFilteredTypes)
        {
            // Get types to generate
            var types = this.dgvTypes.Rows.OfType<DataGridViewRow>()
                .Where(r => (bool)r.Cells[TypeCellGenerate].Value == true)
                .Select(r => new
                {
                    Name = (string)r.Cells[TypeCellName].Value,
                    Type = (string)r.Cells[TypeCellType].Value
                });

            if (onlyFilteredTypes == false)
            {
                return types.Select(t => t.Name).ToList();
            }
            else if (this.chbGenerateAllTypes.CheckState == CheckState.Checked)
            {
                // All types must be generated, there is no filter
                return new List<string>();
            }
            else
            {
                var result = new List<string>();

                // Get complex types if filtered
                if (this.chbGenerateComplexTypes.CheckState == CheckState.Indeterminate)
                {
                    result.AddRange(types.Where(t => t.Type == "Complex").Select(t => t.Name));
                }

                // Get entity types if filtered
                if (this.chbGenerateEntityTypes.CheckState == CheckState.Indeterminate)
                {
                    result.AddRange(types.Where(t => t.Type == "Entity").Select(t => t.Name));
                }

                return result;
            }
        }

        /// <summary>
        /// Executes the generation process.
        /// </summary>
        private void Generate()
        {
            try
            {
                // Validate generation process
                this.ValidateGeneration();

                // Disable Main Window
                this.Enabled = false;

                // Clear ErrorList pane from ErrorTask produced by EntitiesToDTOs
                VisualStudioHelper.ClearErrorList();

                // Get Generator Manager parameters
                GeneratorManagerParams generatorParams = this.GetGeneratorManagerParams();

                // Show progress window
                this.ProgressWindow = new GenerationProgressWindow(generatorParams.DTOsParams.EDMXProjectItem.Name);
                this.ProgressWindow.Show(this);

                // Save latest configuration
                ConfigurationHelper.Export(generatorParams, ConfigurationHelper.GenConfigTempFilePath);

                // Get the types to generate filter (with all types)
                // after exporting since we use only filtered types to export
                generatorParams.DTOsParams.TypesToGenerateFilter = 
                    this.GetTypesToGenerateFilter(onlyFilteredTypes: false);
                
                // Start the generation
                GeneratorManager.Generate(generatorParams);
            }
            catch (Exception ex)
            {
                this.ManageException(ex);
            }
        }

        /// <summary>
        /// Gets the parameters for GeneratorManager.
        /// </summary>
        /// <returns></returns>
        private GeneratorManagerParams GetGeneratorManagerParams()
        {
            #region Entity source

            dynamic entitySource = null;
            if (this.rbSourceEdmx.Checked == true)
            {
                // Get entity source as ProjectItem (it is an EDMX Document)
                entitySource = (this.lstEntitySources.SelectedItem as ProjectItem);
            }
            else
            {
                // Get entity source as Project
                entitySource = this.CurrentSourceProject;
            }

            #endregion Entity source

            #region General

            // Source File Header Comment
            string sourceFileHeaderComment = null;
            if (this.cbCustomHeaderComment.Checked)
            {
                sourceFileHeaderComment = this.txtCustomHeaderComment.Text;
            }

            #endregion General

            #region DTOs
            // Get the types to generate filter
            List<string> typesToGenerateFilter = this.GetTypesToGenerateFilter(onlyFilteredTypes: true);

            // Target
            Project targetProjectDTOs = null;
            ProjectItem targetProjectFolderDTOs = null;
            object nodeValueDTOs = (this.treeTargetDTOs.SelectedNode as TreeNodeExtended).Value;
            if (nodeValueDTOs is Project)
            {
                targetProjectDTOs = (nodeValueDTOs as Project);
            }
            else
            {
                targetProjectDTOs = (nodeValueDTOs as ProjectItem).ContainingProject;
                targetProjectFolderDTOs = (nodeValueDTOs as ProjectItem);
            }
            
            string sourceNamespaceDTOs = this.GetSourceFileNamespaceForDTOs();

            string sourceFileNameDTOs = this.txtSourceFileNameDTOs.Text.Trim();

            // Get source file generation type for DTOs
            SourceFileGenerationType sourceFileGenerationTypeDTOs = SourceFileGenerationType.SourceFilePerClass;
            if (this.rbOneSourceFileDTOs.Checked)
            {
                sourceFileGenerationTypeDTOs = SourceFileGenerationType.OneSourceFile;
            }

            AssociationType associationType = AssociationType.KeyProperty;
            if (this.rbAssociationConfigUseClassTypes.Checked)
            {
                associationType = AssociationType.ClassType;
            }

            // Set generate types flags
            bool generateAllTypes = (this.chbGenerateAllTypes.CheckState == CheckState.Checked);
            bool generateAllComplexTypes = (this.chbGenerateComplexTypes.CheckState == CheckState.Checked);
            bool generateAllEntityTypes = (this.chbGenerateEntityTypes.CheckState == CheckState.Checked);

            // Set Identifier
            ClassIdentifierUse dtosClassIdentifierUse = ClassIdentifierUse.None;
            string dtosClassIdentifierWord = string.Empty;
            if (this.rbDTOIdentifierPrefix.Checked == true)
            {
                dtosClassIdentifierUse = ClassIdentifierUse.Prefix;
                dtosClassIdentifierWord = this.txtDTOIdentifierWord.Text.Trim();
            }
            else if (this.rbDTOIdentifierSuffix.Checked == true)
            {
                dtosClassIdentifierUse = ClassIdentifierUse.Suffix;
                dtosClassIdentifierWord = this.txtDTOIdentifierWord.Text.Trim();
            }

            // Set DTOs parameters
            var dtosParams = new GenerateDTOsParams(targetProjectDTOs, targetProjectFolderDTOs, entitySource,
                typesToGenerateFilter, generateAllTypes, generateAllComplexTypes, generateAllEntityTypes,
                sourceFileHeaderComment, this.cbUseDefaultNamespaceDTOs.Checked, sourceNamespaceDTOs,
                this.cbServiceReadyDTOs.Checked, dtosClassIdentifierUse, dtosClassIdentifierWord,
                sourceFileGenerationTypeDTOs, sourceFileNameDTOs, associationType,
                this.cbGenerateDTOConstructors.Checked);
            #endregion DTOs

            #region Assemblers
            GenerateAssemblersParams assemblersParams = null;

            if (this.cbGenerateAssemblers.Checked)
            {
                // Target
                Project targetProjectAssemblers = null;
                ProjectItem targetProjectFolderAssemblers = null;
                object nodeValueAssemblers = (this.treeTargetAssemblers.SelectedNode as TreeNodeExtended).Value;
                if (nodeValueAssemblers is Project)
                {
                    targetProjectAssemblers = (nodeValueAssemblers as Project);
                }
                else
                {
                    targetProjectAssemblers = (nodeValueAssemblers as ProjectItem).ContainingProject;
                    targetProjectFolderAssemblers = (nodeValueAssemblers as ProjectItem);
                }

                string sourceNamespaceAssemblers = this.GetSourceFileNamespaceForAssemblers();

                string sourceFileNameAssemblers = this.txtSourceFileNameAssemblers.Text.Trim();

                // Get the source file generation type for Assemblers
                SourceFileGenerationType sourceFileGenerationTypeAssemblers = SourceFileGenerationType.SourceFilePerClass;
                if (this.rbOneSourceFileAssemblers.Checked)
                {
                    sourceFileGenerationTypeAssemblers = SourceFileGenerationType.OneSourceFile;
                }

                // Set Identifier
                ClassIdentifierUse assemblersClassIdentifierUse = ClassIdentifierUse.None;
                string assemblersClassIdentifierWord = string.Empty;
                if (this.rbAssemblerIdentifierPrefix.Checked == true)
                {
                    assemblersClassIdentifierUse = ClassIdentifierUse.Prefix;
                    assemblersClassIdentifierWord = this.txtAssemblerIdentifierWord.Text.Trim();
                }
                else if (this.rbAssemblerIdentifierSuffix.Checked == true)
                {
                    assemblersClassIdentifierUse = ClassIdentifierUse.Suffix;
                    assemblersClassIdentifierWord = this.txtAssemblerIdentifierWord.Text.Trim();
                }

                // Set Assemblers parameters
                assemblersParams = new GenerateAssemblersParams(targetProjectAssemblers, targetProjectFolderAssemblers,
                    sourceFileHeaderComment, this.cbUseDefaultNamespaceAssemblers.Checked,
                    sourceNamespaceAssemblers, assemblersClassIdentifierUse, assemblersClassIdentifierWord,
                    sourceFileGenerationTypeAssemblers, sourceFileNameAssemblers, this.cbServiceReadyDTOs.Checked,
                    sourceNamespaceDTOs, targetProjectDTOs, entitySource);
            }
            #endregion Assemblers

            // Return the result generator parameters
            return new GeneratorManagerParams(dtosParams, assemblersParams, this.cbGenerateAssemblers.Checked);
        }

        /// <summary>
        /// Loads EntitiesToDTOs AddIn configuration.
        /// </summary>
        private void LoadAddInConfig()
        {
            // Get AddIn config (force it to be loaded from file)
            AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig(forceLoad: true);

            this.cbCheckReleasesStable.CheckedChanged -= this.cbCheckReleases_CheckedChanged;
            this.cbCheckReleasesBeta.CheckedChanged -= this.cbCheckReleases_CheckedChanged;

            // Update UI
            this.cbCheckReleasesStable.Checked = (addInConfig.ReleaseStatusFilter.Contains(ReleaseStatus.Stable) == true);
            this.cbCheckReleasesBeta.Checked = (addInConfig.ReleaseStatusFilter.Contains(ReleaseStatus.Beta) == true);

            this.cbCheckReleasesStable.CheckedChanged += this.cbCheckReleases_CheckedChanged;
            this.cbCheckReleasesBeta.CheckedChanged += this.cbCheckReleases_CheckedChanged;
        }

        /// <summary>
        /// Searches the target inside the received node and is set as selected if found.
        /// </summary>
        /// <param name="node">Node where to perform the search.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="targetProjectName">Target project name.</param>
        /// <param name="targetName">Target name (if targetType is Project then it should be the same as targetProjectName).</param>
        /// <param name="targetContainer">Object where to set target reference once found.</param>
        /// <param name="currentFolderLevel">(Optional) Current folder level. Do not pass value on first invocation.</param>
        /// <returns>Boolean indicating if the target was found.</returns>
        private bool SetTarget(TreeNodeExtended node, TargetType targetType, string targetProjectName, string targetName, 
            dynamic targetContainer, int currentFolderLevel = -1)
        {
            bool found = false;

            if (node.Nodes != null && node.Nodes.Count > 0)
            {
                // Indicates the real target name to search
                string realTargetName = targetName;

                // Indicates if there are any more folder levels to search
                bool pendingFolderLevels = true;

                if (targetType == TargetType.ProjectFolder && currentFolderLevel >= 0)
                {
                    // Split target name by levels
                    string[] folderLevels = targetName.Split(
                        new string[] { Resources.ProjectFolderLevelSeparator }, StringSplitOptions.None);

                    // Tagret is a folder, get current level folder name
                    realTargetName = folderLevels[currentFolderLevel];

                    if ((currentFolderLevel + 1) == folderLevels.Count())
                    {
                        // Searched folder name reached
                        pendingFolderLevels = false;
                    }
                }

                // Loop through childs
                foreach (TreeNodeExtended childNode in node.Nodes)
                {
                    if ((targetType == TargetType.Project) && (childNode.Value is Project)
                        && (childNode.Value as Project).Name == targetProjectName)
                    {
                        // Found target project
                        childNode.TreeView.SelectedNode = childNode;
                        childNode.TreeView.SelectedNode.EnsureVisible();
                        found = true;
                        // Set project reference
                        targetContainer.TargetProject = (childNode.Value as Project);
                        break;
                    }
                    else if (targetType == TargetType.ProjectFolder)
                    {
                        // First search, project level
                        if (currentFolderLevel == -1)
                        {
                            // Is it a project and the one containing the target folder?
                            if (childNode.Value is Project
                                && (childNode.Value as Project).Name == targetProjectName)
                            {
                                // Project found, search target folder inside
                                found = this.SetTarget(childNode, targetType, targetProjectName, targetName,
                                    targetContainer, (currentFolderLevel + 1));

                                // Target folder found?
                                if (found == true)
                                {
                                    // Set project reference
                                    targetContainer.TargetProject = (childNode.Value as Project);
                                    break;
                                }
                            }
                        }
                        else if (childNode.Value is ProjectItem
                            && (childNode.Value as ProjectItem).Name == realTargetName)
                        {
                            if (pendingFolderLevels == true)
                            {
                                // Found a folder that contains at some level the target folder, keep searching
                                found = this.SetTarget(childNode, targetType, targetProjectName, targetName,
                                    targetContainer, (currentFolderLevel + 1));

                                // Target folder found?
                                if (found == true)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // Found target folder
                                childNode.TreeView.SelectedNode = childNode;
                                childNode.TreeView.SelectedNode.EnsureVisible();
                                found = true;
                                // Set folder reference
                                targetContainer.TargetProjectFolder = (childNode.Value as ProjectItem);
                                break;
                            }

                        } // END else

                    } // END else

                } // END foreach

            } // END if

            return found;
        }

        /// <summary>
        /// Loads a configuration from the specified file path.
        /// </summary>
        /// <param name="filePath">File path from where to load the configuration.</param>
        /// <param name="showWarnings">Indicates if warnings should be shown.</param>
        /// <returns>True if EDMX Source match, DTOs Target Project match, Assemblers Target Project match. False otherwise.</returns>
        private bool LoadConfiguration(string filePath, bool showWarnings)
        {
            bool edmxSourceFound = false;
            bool dtosTargetFound = false;
            bool assemblersTargetFound = false;
            string warnings = string.Empty;

            // Import configuration
            GeneratorManagerParams config = ConfigurationHelper.Import(filePath);

            // Set EDMX Source
            foreach (ProjectItem projectItem in this.lstEntitySources.Items)
            {
                if (projectItem.Name == config.DTOsParams.EDMXName)
                {
                    this.lstEntitySources.SelectedItem = projectItem;
                    edmxSourceFound = true;
                    break;
                }
            }

            if (edmxSourceFound == false)
            {
                if (warnings.Length > 0) warnings += Environment.NewLine;
                warnings += string.Format(Resources.Warning_EdmxSourceNotFound, config.DTOsParams.EDMXName);
            }
            else
            {
                // Loop through types to generate
                foreach (DataGridViewRow row in this.dgvTypes.Rows)
                {
                    if (config.DTOsParams.GenerateAllTypes == true)
                    {
                        row.Cells[TypeCellGenerate].Value = true;
                    }
                    else
                    {
                        if ((config.DTOsParams.GenerateAllComplexTypes == false && (string)row.Cells[TypeCellType].Value == "Complex")
                            || (config.DTOsParams.GenerateAllEntityTypes == false && (string)row.Cells[TypeCellType].Value == "Entity"))
                        {
                            if (config.DTOsParams.TypesToGenerateFilter.Contains((string)row.Cells[TypeCellName].Value) == true)
                            {
                                row.Cells[TypeCellGenerate].Value = true;
                            }
                            else
                            {
                                row.Cells[TypeCellGenerate].Value = false;
                            }
                        }
                    }
                }

                // Update generate types filter state
                this.UpdateGenerateTypesCheckedState();

                // Check types warnings
                this.CheckTypesWarnings();
            }

            // Set Target
            dtosTargetFound = this.SetTarget((this.treeTargetDTOs.Nodes[0] as TreeNodeExtended), 
                config.DTOsParams.TargetType, config.DTOsParams.TargetProjectName,
                config.DTOsParams.TargetName, config.DTOsParams);

            // Highlight selected node
            this.HighlightSelectedNode(this.treeTargetDTOs);

            // Target NOT found?
            if (dtosTargetFound == false)
            {
                if (warnings.Length > 0)
                {
                    warnings += Environment.NewLine;
                }

                // Add warning
                warnings += string.Format(Resources.Warning_DTOsTargetNotFound,
                    config.DTOsParams.TargetType.ToString(), config.DTOsParams.TargetName);
            }

            // Set Source File namespace for DTOs
            this.cbUseDefaultNamespaceDTOs.Checked = (config.DTOsParams.UseProjectDefaultNamespace && dtosTargetFound);
            if (this.cbUseDefaultNamespaceDTOs.Checked)
            {
                if (config.DTOsParams.TargetType == TargetType.Project)
                {
                    VisualStudioHelper.GetDefaultNamespaceFromProperties(config.DTOsParams.TargetProject.Properties);
                }
                else
                {
                    VisualStudioHelper.GetDefaultNamespaceFromProperties(config.DTOsParams.TargetProjectFolder.Properties);
                }
            }
            else
            {
                this.txtNamespaceDTOs.Text = config.DTOsParams.SourceNamespace;
            }

            // Set Source File Generation Type
            this.rbSourceFilePerClassDTOs.Checked = true;
            this.rbOneSourceFileDTOs.Checked = (config.DTOsParams.SourceFileGenerationType == SourceFileGenerationType.OneSourceFile);

            this.txtSourceFileNameDTOs.Text = config.DTOsParams.SourceFileName;

            // Set Association Type
            this.rbAssociationConfigUseKeyProperty.Checked = true;
            this.rbAssociationConfigUseClassTypes.Checked = (config.DTOsParams.AssociationType == AssociationType.ClassType);

            // Set Service-Ready
            this.cbServiceReadyDTOs.Checked = config.DTOsParams.DTOsServiceReady;

            // Set DTO identifier options
            this.rbDTOIdentifierNone.Checked = (config.DTOsParams.ClassIdentifierUse == ClassIdentifierUse.None);
            this.rbDTOIdentifierPrefix.Checked = (config.DTOsParams.ClassIdentifierUse == ClassIdentifierUse.Prefix);
            this.rbDTOIdentifierSuffix.Checked = (config.DTOsParams.ClassIdentifierUse == ClassIdentifierUse.Suffix);
            this.txtDTOIdentifierWord.Text = config.DTOsParams.ClassIdentifierWord;

            // Set Generate Assemblers option
            this.cbGenerateAssemblers.Checked = config.GenerateAssemblers;

            if (config.GenerateAssemblers)
            {
                // Set Target
                assemblersTargetFound = this.SetTarget((this.treeTargetAssemblers.Nodes[0] as TreeNodeExtended),
                    config.AssemblersParams.TargetType, config.AssemblersParams.TargetProjectName,
                    config.AssemblersParams.TargetName, config.AssemblersParams);

                // Highlight selected node
                this.HighlightSelectedNode(this.treeTargetAssemblers);

                // Target NOT found?
                if (assemblersTargetFound == false)
                {
                    if (warnings.Length > 0)
                    {
                        warnings += Environment.NewLine;
                    }

                    // Add warning
                    warnings += string.Format(Resources.Warning_AssemblersTargetNotFound,
                        config.AssemblersParams.TargetType.ToString(), config.AssemblersParams.TargetName);
                }

                // Set Source File namespace for Assemblers
                this.cbUseDefaultNamespaceAssemblers.Checked = (config.AssemblersParams.UseProjectDefaultNamespace && assemblersTargetFound);
                if (this.cbUseDefaultNamespaceAssemblers.Checked)
                {
                    if (config.AssemblersParams.TargetType == TargetType.Project)
                    {
                        VisualStudioHelper.GetDefaultNamespaceFromProperties(config.AssemblersParams.TargetProject.Properties);
                    }
                    else
                    {
                        VisualStudioHelper.GetDefaultNamespaceFromProperties(config.AssemblersParams.TargetProjectFolder.Properties);
                    }
                }
                else
                {
                    this.txtNamespaceAssemblers.Text = config.AssemblersParams.SourceNamespace;
                }

                // Set Source File Generation Type
                this.rbSourceFilePerClassAssemblers.Checked = true;
                this.rbOneSourceFileAssemblers.Checked = (config.AssemblersParams.SourceFileGenerationType == SourceFileGenerationType.OneSourceFile);

                this.txtSourceFileNameAssemblers.Text = config.AssemblersParams.SourceFileName;

                // Set Assembler identifier options
                this.rbAssemblerIdentifierNone.Checked = (config.AssemblersParams.ClassIdentifierUse == ClassIdentifierUse.None);
                this.rbAssemblerIdentifierPrefix.Checked = (config.AssemblersParams.ClassIdentifierUse == ClassIdentifierUse.Prefix);
                this.rbAssemblerIdentifierSuffix.Checked = (config.AssemblersParams.ClassIdentifierUse == ClassIdentifierUse.Suffix);
                this.txtAssemblerIdentifierWord.Text = config.AssemblersParams.ClassIdentifierWord;
            }

            // Set Source File Header Comment
            this.cbCustomHeaderComment.Checked = (string.IsNullOrWhiteSpace(config.DTOsParams.SourceFileHeaderComment) == false);
            this.txtCustomHeaderComment.Text = config.DTOsParams.SourceFileHeaderComment;

            // Check Warnings
            if ((showWarnings == true) && (warnings.Length > 0))
            {
                // Show warning message
                MessageBox.Show(string.Format(Resources.Warning_WarningsWhenLoadingConfiguration, warnings),
                    Resources.Warning_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Return. True if EDMX Source match, DTOs Target Project match, Assemblers Target Project match. False otherwise.
            return (edmxSourceFound && dtosTargetFound && assemblersTargetFound);
        }

        /// <summary>
        /// Loads the latest configuration used by the user.
        /// </summary>
        private void LoadLatestConfiguration()
        {
            try
            {
                // Temp config file exists?
                if (File.Exists(ConfigurationHelper.GenConfigTempFilePath))
                {
                    // Load latest configuration
                    bool showWarnings = false;
                    bool edmxAndTargetProjectsFound = this.LoadConfiguration(ConfigurationHelper.GenConfigTempFilePath, showWarnings);

                    if (edmxAndTargetProjectsFound)
                    {
                        // Validate generation process
                        this.ValidateGeneration();

                        // Show OK message
                        VisualStudioHelper.SetStatusBarMessage(Resources.Info_LatestConfigLoaded);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ManageException(ex);
            }
        }

        /// <summary>
        /// Updates AddIn config release notifications.
        /// </summary>
        private void UpdateReleaseNotifications()
        {
            // Get AddIn config
            AddInConfig addInConfig = ConfigurationHelper.GetAddInConfig();

            // Reset release status filter
            addInConfig.ReleaseStatusFilter = new List<ReleaseStatus>();

            // Set stable release status filter
            if (this.cbCheckReleasesStable.Checked == true)
            {
                addInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Stable);
            }

            // Set beta release status filter
            if (this.cbCheckReleasesBeta.Checked == true)
            {
                addInConfig.ReleaseStatusFilter.Add(ReleaseStatus.Beta);
            }

            // Save AddIn config
            ConfigurationHelper.SaveAddInConfig(addInConfig);
        }

        #endregion Methods
        
    }
}