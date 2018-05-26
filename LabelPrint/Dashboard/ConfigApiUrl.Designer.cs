namespace LabelPrint.Dashboard
{
    partial class ConfigApiUrl
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
            this.txtApiUrl = new DevExpress.XtraEditors.TextEdit();
            this.lblApiUrl = new DevExpress.XtraEditors.LabelControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtApiUrl.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtApiUrl
            // 
            this.txtApiUrl.Location = new System.Drawing.Point(91, 25);
            this.txtApiUrl.Name = "txtApiUrl";
            this.txtApiUrl.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtApiUrl.Properties.Appearance.Options.UseFont = true;
            this.txtApiUrl.Size = new System.Drawing.Size(312, 22);
            this.txtApiUrl.TabIndex = 49;
            // 
            // lblApiUrl
            // 
            this.lblApiUrl.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblApiUrl.Appearance.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblApiUrl.Location = new System.Drawing.Point(19, 26);
            this.lblApiUrl.LookAndFeel.SkinName = "Blue";
            this.lblApiUrl.Name = "lblApiUrl";
            this.lblApiUrl.Size = new System.Drawing.Size(60, 19);
            this.lblApiUrl.TabIndex = 48;
            this.lblApiUrl.Text = "API URL";
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(91, 65);
            this.btnSave.LookAndFeel.SkinName = "Blue";
            this.btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(224, 65);
            this.btnCancel.LookAndFeel.SkinName = "Blue";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 23);
            this.btnCancel.TabIndex = 51;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ConfigApiUrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 111);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtApiUrl);
            this.Controls.Add(this.lblApiUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigApiUrl";
            this.Text = "CONFIG API URL";
            this.Load += new System.EventHandler(this.ConfigApiUrl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtApiUrl.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtApiUrl;
        private DevExpress.XtraEditors.LabelControl lblApiUrl;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}