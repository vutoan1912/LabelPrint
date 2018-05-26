namespace LabelPrint.Inventory
{
    partial class Split
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.lblAttention = new DevExpress.XtraEditors.LabelControl();
            this.txtQuantity = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblReport = new DevExpress.XtraEditors.LabelControl();
            this.ttError = new DevExpress.Utils.ToolTipController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(203, 113);
            this.btnCancel.LookAndFeel.SkinName = "Blue";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(83, 113);
            this.btnOK.LookAndFeel.SkinName = "Blue";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblAttention
            // 
            this.lblAttention.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblAttention.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblAttention.Location = new System.Drawing.Point(14, 75);
            this.lblAttention.Name = "lblAttention";
            this.lblAttention.Size = new System.Drawing.Size(190, 14);
            this.lblAttention.TabIndex = 9;
            this.lblAttention.Text = "Số lượng của cuộn mới phải < 100";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(199, 42);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtQuantity.Properties.Appearance.Options.UseFont = true;
            this.txtQuantity.Properties.LookAndFeel.SkinName = "Blue";
            this.txtQuantity.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.txtQuantity.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtQuantity.Size = new System.Drawing.Size(152, 22);
            this.txtQuantity.TabIndex = 8;
            this.txtQuantity.Validating += new System.ComponentModel.CancelEventHandler(this.txtQuantity_Validating);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.labelControl1.Location = new System.Drawing.Point(14, 43);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(167, 18);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Enter the number to split:";
            // 
            // lblReport
            // 
            this.lblReport.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblReport.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblReport.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblReport.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblReport.Location = new System.Drawing.Point(14, 12);
            this.lblReport.Name = "lblReport";
            this.lblReport.Size = new System.Drawing.Size(250, 16);
            this.lblReport.TabIndex = 6;
            this.lblReport.Text = "labelControl1";
            // 
            // ttError
            // 
            this.ttError.Appearance.ForeColor = System.Drawing.Color.Red;
            this.ttError.Appearance.Options.UseForeColor = true;
            this.ttError.CloseOnClick = DevExpress.Utils.DefaultBoolean.True;
            this.ttError.Rounded = true;
            this.ttError.ShowBeak = true;
            this.ttError.ToolTipLocation = DevExpress.Utils.ToolTipLocation.TopCenter;
            // 
            // Split
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 153);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblAttention);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.lblReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Split";
            this.Text = "SPLIT";
            this.Load += new System.EventHandler(this.Split_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.LabelControl lblAttention;
        private DevExpress.XtraEditors.TextEdit txtQuantity;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblReport;
        private DevExpress.Utils.ToolTipController ttError;
    }
}