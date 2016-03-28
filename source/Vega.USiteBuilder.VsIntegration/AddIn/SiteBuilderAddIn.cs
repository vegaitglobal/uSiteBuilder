namespace Vega.USiteBuilder.VsIntegration.AddIn
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using EnvDTE;
    using EnvDTE80;
    using Extensibility;
    using Microsoft.VisualStudio.CommandBars;

    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class SiteBuilderAddIn : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        //private const string AddInName = "Vega.USiteBuilder";
        //private const string AddInShortName = "USiteBuilder";
        private OutputWindowPane _outputPane;

        private const string PaneUSiteBuilder = "USiteBuilder";

        #region [Commands]
        private const string CmdDeployItem = "DeployToUmbracoSite";
        private const string CmdDeployAll = "DeployToUmbracoSite";
        #endregion

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public SiteBuilderAddIn()
        {
            //MessageBox.Show("test");
            //System.Diagnostics.Debugger.Break();
        }

        public SelectedItem FirstSelectedItem
        {
            get
            {
                SelectedItem item = null;
                if (this._applicationObject.SelectedItems.Count > 0)
                {
                    item = this._applicationObject.SelectedItems.Item(1);
                }
                return item;
            }
        }

        public Project SelectedProject
        {
            get
            {
                return this.GetProject(FirstSelectedItem);
            }
        }

        public Project GetProject(SelectedItem item)
        {
            Project result = null;
            if (item != null)
            {
                if (item.ProjectItem != null)
                {
                    result = item.ProjectItem.ContainingProject;
                }
                else
                {
                    result = item.Project;
                }
            }
            return result;
        }

        private void AddToOutputWindowTextLine(string line)
        {
            this._outputPane.Activate();
            this._outputPane.OutputString(line + "\n");
        }

        private void AddCommand(string cmdName, string cmdText, string cmdToolTip, string cmdBarName)
        {
            Command command = null;
            CommandBar cmdBar;
            object[] contextGUIDS = new object[] { };

            // First try to get an existing command
            foreach (Command cmd in this._applicationObject.Commands)
            {
                if (this.IsCommandTheSame(cmd.Name, cmdName))
                {
                    command = cmd;
                    break;
                }
            }

            // create the command if it not exists
            if (command == null)
            {
                // Create a Command with name SiteBuilderAddIn and then add it to the "Item" menubar for the SolutionExplorer
                Commands2 commands = this._applicationObject.Commands as Commands2;

                command = commands.AddNamedCommand2(this._addInInstance,
                    cmdName, cmdText, cmdToolTip, true, 230, ref contextGUIDS,
                    (int)vsCommandStatus.vsCommandStatusEnabled + (int)vsCommandStatus.vsCommandStatusSupported,
                    (int)vsCommandStyle.vsCommandStyleText, vsCommandControlType.vsCommandControlTypeButton);

                cmdBar = ((CommandBars)_applicationObject.CommandBars)[cmdBarName];
                command.AddControl(cmdBar, 1);
            }
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom)
        {
            this._applicationObject = (DTE2)Application;
            this._addInInstance = (AddIn)AddInInst;

            OutputWindow outputWindow = this._applicationObject.ToolWindows.OutputWindow;
            foreach (OutputWindowPane pane in outputWindow.OutputWindowPanes)
            {
                if (pane.Name == SiteBuilderAddIn.PaneUSiteBuilder)
                {
                    this._outputPane = pane;
                }
            }

            if (this._outputPane == null)
            {
                this._outputPane = outputWindow.OutputWindowPanes.Add(SiteBuilderAddIn.PaneUSiteBuilder);
            }

            //this.AddCommand(SiteBuilderAddIn.CmdDeployItem, "Deploy this Item", "Deploys this item only to Umbraco website", "Item");
            //this.AddCommand(SiteBuilderAddIn.CmdDeployAll, "Deploy to Umbraco website", "Deploys this project to Umbraco website", "Item");
            this.AddCommand(SiteBuilderAddIn.CmdDeployAll, "Deploy to Umbraco website", "Deploys this project to Umbraco website", "Project");
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        #region IDTCommandTarget Members
        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='CmdName'>The name of the command to execute.</param>
        /// <param term='ExecuteOption'>Describes how the command should be run.</param>
        /// <param term='VariantIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='VariantOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='Handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            try
            {
                Handled = false;

                OutputWindow outputWindow = this._applicationObject.ToolWindows.OutputWindow;
                if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    if (this.IsCommandTheSame(CmdName, SiteBuilderAddIn.CmdDeployAll))
                    {
                        //UIHierarchyItem item;
                        //UIHierarchy UIH = _applicationObject.ToolWindows.SolutionExplorer;
                        //item = (UIHierarchyItem)((System.Array)UIH.SelectedItems).GetValue(0);
                        //ProjectItem projItem = (ProjectItem)item.Object;
                        //string inputFileName = (string)projItem.Properties.Item("FullPath").Value;

                        ///////////////////////////////////////////
                        // 
                        //MessageBox.Show(CmdName, "Works");

                        this._applicationObject.StatusBar.Text = "Deploy done!";

                        this.AddToOutputWindowTextLine("Copying file 1...");
                        System.Threading.Thread.Sleep(500);
                        this.AddToOutputWindowTextLine("Copying file 2...");
                        System.Threading.Thread.Sleep(500);
                        this.AddToOutputWindowTextLine("Copying file 3...");
                        System.Threading.Thread.Sleep(500);
                        this.AddToOutputWindowTextLine("Copying file 4...");
                        System.Threading.Thread.Sleep(500);
                        this.AddToOutputWindowTextLine("Copying file 5...");
                        System.Threading.Thread.Sleep(500);
                        this.AddToOutputWindowTextLine("Finished\n");
                        ///////////////////////////////////////////

                        Handled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsCommandTheSame(string cmdNameToCompare, string cmdName)
        {
            return cmdNameToCompare.IndexOf(cmdName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='CmdName'>The name of the command to determine state for.</param>
        /// <param term='NeededText'>Text that is needed for the command.</param>
        /// <param term='StatusOption'>The state of the command in the user interface.</param>
        /// <param term='CommandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (this.IsCommandTheSame(CmdName, SiteBuilderAddIn.CmdDeployAll))
                {
                    try
                    {
                        //Dynamically enable & disable the command.
                        //UIHierarchyItem item;
                        //UIHierarchy UIH = _applicationObject.ToolWindows.SolutionExplorer;
                        //item = (UIHierarchyItem)((System.Array)UIH.SelectedItems).GetValue(0);

                        SelectedItem item = this.FirstSelectedItem;
                        
                        
                        StatusOption = vsCommandStatus.vsCommandStatusSupported;
                        StatusOption |= vsCommandStatus.vsCommandStatusEnabled;

                        /*
                        if (!this.FirstSelectedItem.Name.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
                        {
                            StatusOption |= vsCommandStatus.vsCommandStatusInvisible;
                        }
                        else
                        {
                            StatusOption |= vsCommandStatus.vsCommandStatusEnabled;
                        }
                        */
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Exception: " + exc.Message, "Error in QueryStatus method");
                    }
                }
                else
                {
                    StatusOption = vsCommandStatus.vsCommandStatusUnsupported;
                }
            }
        }
        #endregion
    }
}
