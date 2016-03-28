namespace Vega.USiteBuilder.VsIntegration
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TemplateWizard;

    using EnvDTE80;
    using Vega.USiteBuilder.VsIntegration.Properties;

    public partial class DocumentTypeWizardForm : Form, IWizard
    {
        private bool _shouldAddItem = false;

        public DocumentTypeWizardForm()
        {
            InitializeComponent();

            this.FillDocumentTypes();

            if (this.lboxDocTypes.Items.Count > 0)
            {
                this.lboxDocTypes.SelectedIndex = 0;
            }
            else
            {
                this.radioNoMasterDocType.Checked = true;
                this.radioHasMasterDocType.Checked = false;
                this.radioHasMasterDocType.Enabled = false;

                this.lblSelect.Text = "No Document Types found in solution";
            }
        }

        private void FillDocumentTypes()
        {
            VSSearch vsSearch = new VSSearch();

            List<CodeClass2> docTypeClasses = vsSearch.SearchForClassInCurrentProject(Constants.BaseTypes.DocumentTypeBaseFullName);

            foreach (CodeClass2 codeClass in docTypeClasses)
            {
                this.lboxDocTypes.Items.Add(new CodeClassListBoxItem(codeClass));
            }
        }

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                string docTypeBase;
                if (this.radioHasMasterDocType.Checked)
                {
                    docTypeBase = ((CodeClassListBoxItem)lboxDocTypes.SelectedItem).CodeClass.FullName;
                }
                else
                {
                    docTypeBase = Constants.BaseTypes.DocumentTypeBaseFullName;
                }

                string tipClass = "";
                string tipProperty = "";
                if (this.cboxGenerateComments.Checked)
                {
                    tipClass = Resources.DocumentTypeTip;
                    tipProperty = Resources.DocumentTypePropertyTip;
                }

                replacementsDictionary.Add("$documenttypebase$", docTypeBase);
                replacementsDictionary.Add(Constants.TemplateVars.ClassTipsComment, tipClass);
                replacementsDictionary.Add(Constants.TemplateVars.PropertyTipsComment, tipProperty);

                this._shouldAddItem = true;
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return this._shouldAddItem;
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void RadioMasterDocType_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.radioHasMasterDocType.Checked)
            {
                this.lblSelect.Enabled = true;
                this.lboxDocTypes.Enabled = true;
            }
            else
            {
                this.lblSelect.Enabled = false;
                this.lboxDocTypes.Enabled = false;
            }
        }
    }
}
