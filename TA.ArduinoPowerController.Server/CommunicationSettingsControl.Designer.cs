namespace TA.VellemanK8056.Server
{
    partial class CommunicationSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CommPortName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "2400,n,8,1";
            // 
            // CommPortName
            // 
            this.CommPortName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TA.VellemanK8056.Server.Properties.Settings.Default, "CommPortName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CommPortName.FormattingEnabled = true;
            this.CommPortName.Location = new System.Drawing.Point(73, 0);
            this.CommPortName.Name = "CommPortName";
            this.CommPortName.Size = new System.Drawing.Size(80, 21);
            this.CommPortName.TabIndex = 0;
            this.CommPortName.Text = global::TA.VellemanK8056.Server.Properties.Settings.Default.CommPortName;
            // 
            // CommunicationSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CommPortName);
            this.Name = "CommunicationSettingsControl";
            this.Size = new System.Drawing.Size(238, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CommPortName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
