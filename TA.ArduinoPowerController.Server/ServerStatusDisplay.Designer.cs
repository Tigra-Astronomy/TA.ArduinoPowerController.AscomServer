namespace TA.ArduinoPowerController.Server
{
    partial class ServerStatusDisplay
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
            this.label1 = new System.Windows.Forms.Label();
            this.registeredClientCount = new System.Windows.Forms.Label();
            this.OnlineClients = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ClientStatus = new System.Windows.Forms.ListBox();
            this.annunciatorPanel1 = new ASCOM.Controls.AnnunciatorPanel();
            this.Relay0Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay1Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay2Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay3Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay4Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay5Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay6Annunciator = new ASCOM.Controls.Annunciator();
            this.Relay7Annunciator = new ASCOM.Controls.Annunciator();
            this.SetupCommand = new System.Windows.Forms.Button();
            this.annunciatorPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Registered clients:";
            // 
            // registeredClientCount
            // 
            this.registeredClientCount.AutoSize = true;
            this.registeredClientCount.Location = new System.Drawing.Point(122, 10);
            this.registeredClientCount.Name = "registeredClientCount";
            this.registeredClientCount.Size = new System.Drawing.Size(13, 13);
            this.registeredClientCount.TabIndex = 1;
            this.registeredClientCount.Text = "0";
            // 
            // OnlineClients
            // 
            this.OnlineClients.AutoSize = true;
            this.OnlineClients.Location = new System.Drawing.Point(226, 10);
            this.OnlineClients.Name = "OnlineClients";
            this.OnlineClients.Size = new System.Drawing.Size(13, 13);
            this.OnlineClients.TabIndex = 3;
            this.OnlineClients.Text = "0";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(166, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Online:";
            // 
            // ClientStatus
            // 
            this.ClientStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClientStatus.FormattingEnabled = true;
            this.ClientStatus.Location = new System.Drawing.Point(13, 35);
            this.ClientStatus.Name = "ClientStatus";
            this.ClientStatus.Size = new System.Drawing.Size(514, 108);
            this.ClientStatus.TabIndex = 4;
            // 
            // annunciatorPanel1
            // 
            this.annunciatorPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.annunciatorPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.annunciatorPanel1.Controls.Add(this.Relay0Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay1Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay2Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay3Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay4Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay5Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay6Annunciator);
            this.annunciatorPanel1.Controls.Add(this.Relay7Annunciator);
            this.annunciatorPanel1.Location = new System.Drawing.Point(346, 7);
            this.annunciatorPanel1.Name = "annunciatorPanel1";
            this.annunciatorPanel1.Size = new System.Drawing.Size(181, 19);
            this.annunciatorPanel1.TabIndex = 5;
            // 
            // Relay0Annunciator
            // 
            this.Relay0Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay0Annunciator.AutoSize = true;
            this.Relay0Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay0Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay0Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay0Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay0Annunciator.Location = new System.Drawing.Point(3, 0);
            this.Relay0Annunciator.Mute = false;
            this.Relay0Annunciator.Name = "Relay0Annunciator";
            this.Relay0Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay0Annunciator.TabIndex = 0;
            this.Relay0Annunciator.Text = "0";
            // 
            // Relay1Annunciator
            // 
            this.Relay1Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay1Annunciator.AutoSize = true;
            this.Relay1Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay1Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay1Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay1Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay1Annunciator.Location = new System.Drawing.Point(25, 0);
            this.Relay1Annunciator.Mute = false;
            this.Relay1Annunciator.Name = "Relay1Annunciator";
            this.Relay1Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay1Annunciator.TabIndex = 0;
            this.Relay1Annunciator.Text = "1";
            // 
            // Relay2Annunciator
            // 
            this.Relay2Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay2Annunciator.AutoSize = true;
            this.Relay2Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay2Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay2Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay2Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay2Annunciator.Location = new System.Drawing.Point(47, 0);
            this.Relay2Annunciator.Mute = false;
            this.Relay2Annunciator.Name = "Relay2Annunciator";
            this.Relay2Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay2Annunciator.TabIndex = 0;
            this.Relay2Annunciator.Text = "2";
            // 
            // Relay3Annunciator
            // 
            this.Relay3Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay3Annunciator.AutoSize = true;
            this.Relay3Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay3Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay3Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay3Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay3Annunciator.Location = new System.Drawing.Point(69, 0);
            this.Relay3Annunciator.Mute = false;
            this.Relay3Annunciator.Name = "Relay3Annunciator";
            this.Relay3Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay3Annunciator.TabIndex = 0;
            this.Relay3Annunciator.Text = "3";
            // 
            // Relay4Annunciator
            // 
            this.Relay4Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay4Annunciator.AutoSize = true;
            this.Relay4Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay4Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay4Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay4Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay4Annunciator.Location = new System.Drawing.Point(91, 0);
            this.Relay4Annunciator.Mute = false;
            this.Relay4Annunciator.Name = "Relay4Annunciator";
            this.Relay4Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay4Annunciator.TabIndex = 0;
            this.Relay4Annunciator.Text = "4";
            // 
            // Relay5Annunciator
            // 
            this.Relay5Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay5Annunciator.AutoSize = true;
            this.Relay5Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay5Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay5Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay5Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay5Annunciator.Location = new System.Drawing.Point(113, 0);
            this.Relay5Annunciator.Mute = false;
            this.Relay5Annunciator.Name = "Relay5Annunciator";
            this.Relay5Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay5Annunciator.TabIndex = 0;
            this.Relay5Annunciator.Text = "5";
            // 
            // Relay6Annunciator
            // 
            this.Relay6Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay6Annunciator.AutoSize = true;
            this.Relay6Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay6Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay6Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay6Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay6Annunciator.Location = new System.Drawing.Point(135, 0);
            this.Relay6Annunciator.Mute = false;
            this.Relay6Annunciator.Name = "Relay6Annunciator";
            this.Relay6Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay6Annunciator.TabIndex = 0;
            this.Relay6Annunciator.Text = "6";
            // 
            // Relay7Annunciator
            // 
            this.Relay7Annunciator.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay7Annunciator.AutoSize = true;
            this.Relay7Annunciator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Relay7Annunciator.Font = new System.Drawing.Font("Consolas", 10F);
            this.Relay7Annunciator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay7Annunciator.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.Relay7Annunciator.Location = new System.Drawing.Point(157, 0);
            this.Relay7Annunciator.Mute = false;
            this.Relay7Annunciator.Name = "Relay7Annunciator";
            this.Relay7Annunciator.Size = new System.Drawing.Size(16, 17);
            this.Relay7Annunciator.TabIndex = 0;
            this.Relay7Annunciator.Text = "7";
            // 
            // SetupCommand
            // 
            this.SetupCommand.Location = new System.Drawing.Point(265, 5);
            this.SetupCommand.Name = "SetupCommand";
            this.SetupCommand.Size = new System.Drawing.Size(75, 23);
            this.SetupCommand.TabIndex = 8;
            this.SetupCommand.Text = "Setup...";
            this.SetupCommand.UseVisualStyleBackColor = true;
            this.SetupCommand.Click += new System.EventHandler(this.SetupCommand_Click);
            // 
            // ServerStatusDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 160);
            this.Controls.Add(this.SetupCommand);
            this.Controls.Add(this.annunciatorPanel1);
            this.Controls.Add(this.ClientStatus);
            this.Controls.Add(this.OnlineClients);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.registeredClientCount);
            this.Controls.Add(this.label1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::TA.ArduinoPowerController.Server.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = global::TA.ArduinoPowerController.Server.Properties.Settings.Default.MainFormLocation;
            this.Name = "ServerStatusDisplay";
            this.Text = "Arduino Power Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
            this.annunciatorPanel1.ResumeLayout(false);
            this.annunciatorPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label registeredClientCount;
        private System.Windows.Forms.Label OnlineClients;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox ClientStatus;
        private ASCOM.Controls.AnnunciatorPanel annunciatorPanel1;
        private System.Windows.Forms.Button SetupCommand;
        private ASCOM.Controls.Annunciator Relay0Annunciator;
        private ASCOM.Controls.Annunciator Relay1Annunciator;
        private ASCOM.Controls.Annunciator Relay2Annunciator;
        private ASCOM.Controls.Annunciator Relay3Annunciator;
        private ASCOM.Controls.Annunciator Relay4Annunciator;
        private ASCOM.Controls.Annunciator Relay5Annunciator;
        private ASCOM.Controls.Annunciator Relay6Annunciator;
        private ASCOM.Controls.Annunciator Relay7Annunciator;
    }
}

