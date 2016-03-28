namespace Vega.USiteBuilder.VsIntegration
{
    partial class DocumentTypeWizardForm
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
            this.radioNoMasterDocType = new System.Windows.Forms.RadioButton();
            this.radioHasMasterDocType = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lboxDocTypes = new System.Windows.Forms.ListBox();
            this.lblSelect = new System.Windows.Forms.Label();
            this.cboxGenerateComments = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // radioNoMasterDocType
            // 
            this.radioNoMasterDocType.AutoSize = true;
            this.radioNoMasterDocType.Location = new System.Drawing.Point(14, 20);
            this.radioNoMasterDocType.Name = "radioNoMasterDocType";
            this.radioNoMasterDocType.Size = new System.Drawing.Size(153, 17);
            this.radioNoMasterDocType.TabIndex = 0;
            this.radioNoMasterDocType.Text = "No Master Document Type";
            this.radioNoMasterDocType.UseVisualStyleBackColor = true;
            this.radioNoMasterDocType.CheckedChanged += new System.EventHandler(this.RadioMasterDocType_CheckedChanged);
            // 
            // radioHasMasterDocType
            // 
            this.radioHasMasterDocType.AutoSize = true;
            this.radioHasMasterDocType.Checked = true;
            this.radioHasMasterDocType.Location = new System.Drawing.Point(14, 54);
            this.radioHasMasterDocType.Name = "radioHasMasterDocType";
            this.radioHasMasterDocType.Size = new System.Drawing.Size(158, 17);
            this.radioHasMasterDocType.TabIndex = 1;
            this.radioHasMasterDocType.TabStop = true;
            this.radioHasMasterDocType.Text = "Has Master Document Type";
            this.radioHasMasterDocType.UseVisualStyleBackColor = true;
            this.radioHasMasterDocType.CheckedChanged += new System.EventHandler(this.RadioMasterDocType_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(317, 458);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(236, 458);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // lboxDocTypes
            // 
            this.lboxDocTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lboxDocTypes.FormattingEnabled = true;
            this.lboxDocTypes.Location = new System.Drawing.Point(34, 104);
            this.lboxDocTypes.Name = "lboxDocTypes";
            this.lboxDocTypes.Size = new System.Drawing.Size(358, 290);
            this.lboxDocTypes.TabIndex = 2;
            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(31, 82);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(213, 13);
            this.lblSelect.TabIndex = 5;
            this.lblSelect.Text = "Select on of the following Document Types:";
            // 
            // cboxGenerateComments
            // 
            this.cboxGenerateComments.AutoSize = true;
            this.cboxGenerateComments.Location = new System.Drawing.Point(14, 413);
            this.cboxGenerateComments.Name = "cboxGenerateComments";
            this.cboxGenerateComments.Size = new System.Drawing.Size(250, 30);
            this.cboxGenerateComments.TabIndex = 13;
            this.cboxGenerateComments.Text = "Generate Tips comments in this DocumentType\r\n      (recommented for the first tim" +
                "e usage)";
            this.cboxGenerateComments.UseVisualStyleBackColor = true;
            // 
            // DocumentTypeWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 493);
            this.Controls.Add(this.cboxGenerateComments);
            this.Controls.Add(this.lblSelect);
            this.Controls.Add(this.lboxDocTypes);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.radioHasMasterDocType);
            this.Controls.Add(this.radioNoMasterDocType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocumentTypeWizardForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Document Type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioNoMasterDocType;
        private System.Windows.Forms.RadioButton radioHasMasterDocType;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ListBox lboxDocTypes;
        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.CheckBox cboxGenerateComments;
    }
}