namespace Vega.USiteBuilder.VsIntegration
{
    partial class WebUserControlWizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSelect = new System.Windows.Forms.Label();
            this.lboxDocTypes = new System.Windows.Forms.ListBox();
            this.radioStronglyTyped = new System.Windows.Forms.RadioButton();
            this.radioStandard = new System.Windows.Forms.RadioButton();
            this.cboxGenerateComments = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(236, 454);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(317, 454);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(28, 82);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(213, 13);
            this.lblSelect.TabIndex = 8;
            this.lblSelect.Text = "Select on of the following Document Types:";
            // 
            // lboxDocTypes
            // 
            this.lboxDocTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lboxDocTypes.FormattingEnabled = true;
            this.lboxDocTypes.Location = new System.Drawing.Point(31, 105);
            this.lboxDocTypes.Name = "lboxDocTypes";
            this.lboxDocTypes.Size = new System.Drawing.Size(360, 290);
            this.lboxDocTypes.TabIndex = 9;
            // 
            // radioStronglyTyped
            // 
            this.radioStronglyTyped.AutoSize = true;
            this.radioStronglyTyped.Checked = true;
            this.radioStronglyTyped.Location = new System.Drawing.Point(12, 47);
            this.radioStronglyTyped.Name = "radioStronglyTyped";
            this.radioStronglyTyped.Size = new System.Drawing.Size(225, 17);
            this.radioStronglyTyped.TabIndex = 7;
            this.radioStronglyTyped.TabStop = true;
            this.radioStronglyTyped.Text = "Strongly typed Umbraco Web User Control";
            this.radioStronglyTyped.UseVisualStyleBackColor = true;
            this.radioStronglyTyped.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // radioStandard
            // 
            this.radioStandard.AutoSize = true;
            this.radioStandard.Location = new System.Drawing.Point(12, 16);
            this.radioStandard.Name = "radioStandard";
            this.radioStandard.Size = new System.Drawing.Size(201, 17);
            this.radioStandard.TabIndex = 6;
            this.radioStandard.Text = "Standard Umbraco Web User Control";
            this.radioStandard.UseVisualStyleBackColor = true;
            this.radioStandard.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // cboxGenerateComments
            // 
            this.cboxGenerateComments.AutoSize = true;
            this.cboxGenerateComments.Location = new System.Drawing.Point(12, 417);
            this.cboxGenerateComments.Name = "cboxGenerateComments";
            this.cboxGenerateComments.Size = new System.Drawing.Size(388, 17);
            this.cboxGenerateComments.TabIndex = 12;
            this.cboxGenerateComments.Text = "Generate Tips comments in this control (recommented for the first time usage)";
            this.cboxGenerateComments.UseVisualStyleBackColor = true;
            // 
            // WebUserControlWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 493);
            this.Controls.Add(this.cboxGenerateComments);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblSelect);
            this.Controls.Add(this.lboxDocTypes);
            this.Controls.Add(this.radioStronglyTyped);
            this.Controls.Add(this.radioStandard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebUserControlWizardForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Web User Control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.ListBox lboxDocTypes;
        private System.Windows.Forms.RadioButton radioStronglyTyped;
        private System.Windows.Forms.RadioButton radioStandard;
        private System.Windows.Forms.CheckBox cboxGenerateComments;
    }
}