namespace Vega.USiteBuilder.VsIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using EnvDTE80;

    using Microsoft.VisualStudio.TemplateWizard;
    using Vega.USiteBuilder.VsIntegration.Properties;

    public partial class WebUserControlWizardForm : Form, IWizard
    {
        private bool _shouldAddItem = false;

        public WebUserControlWizardForm()
        {
            InitializeComponent();

            VSSearch vsSearch = new VSSearch();

            foreach (CodeClass2 codeClass in vsSearch.SearchForClassInCurrentProject(Constants.BaseTypes.DocumentTypeBaseFullName))
            {
                this.lboxDocTypes.Items.Add(new CodeClassListBoxItem(codeClass));
            }

            if (this.lboxDocTypes.Items.Count > 0)
            {
                this.lboxDocTypes.SelectedIndex = 0;
            }
            else
            {
                this.radioStandard.Checked = true;
                this.radioStronglyTyped.Checked = false;
                this.radioStronglyTyped.Enabled = false;

                this.lblSelect.Text = "No Document Types found in solution";
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioStronglyTyped.Checked)
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


        #region IWizard Members

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
                string controlBase;
                if (this.radioStronglyTyped.Checked)
                {
                    string docType = ((CodeClassListBoxItem)lboxDocTypes.SelectedItem).CodeClass.FullName;

                    controlBase = string.Format("{0}<{1}>", Constants.BaseTypes.WebUserControlBaseFullName, docType);
                }
                else
                {
                    controlBase = Constants.BaseTypes.WebUserControlBaseFullName;
                }
                replacementsDictionary.Add("$usercontrolbase$", controlBase);

                string tip = "";
                if (this.cboxGenerateComments.Checked)
                {
                    tip = Resources.WebUserControlTip;
                }

                replacementsDictionary.Add(Constants.TemplateVars.ClassTipsComment, tip);

                this._shouldAddItem = true;
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return this._shouldAddItem;
        }

        #endregion
    }
}
