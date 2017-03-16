// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: SetupDialogForm.cs  Last modified: 2017-03-16@23:34 by Tim Long

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TA.ArduinoPowerController.Server
    {
    [ComVisible(false)] // Form not registered for COM!
    public partial class SetupDialogForm : Form
        {
        public SetupDialogForm()
            {
            InitializeComponent();
            }

        private void AboutBox_Click(object sender, EventArgs e)
            {
            using (var aboutBox = new AboutBox())
                {
                aboutBox.ShowDialog();
                }
            }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
            {
            try
                {
                Process.Start("http://ascom-standards.org/");
                }
            catch (Win32Exception noBrowser)
                {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
                }
            catch (Exception other)
                {
                MessageBox.Show(other.Message);
                }
            }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
            {
            Close();
            }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
            {
            communicationSettingsControl1.Save();
            }

        private void SetupDialogForm_Load(object sender, EventArgs e)
            {
            var onlineClients = SharedResources.ConnectionManager.OnlineClientCount;
            if (onlineClients == 0)
                {
                communicationSettingsControl1.Enabled = true;
                ConnectionErrorProvider.SetError(communicationSettingsControl1, string.Empty);
                }
            else
                {
                communicationSettingsControl1.Enabled = false;
                ConnectionErrorProvider.SetError(communicationSettingsControl1,
                    "Connection settings cannot be changed while there are connected clients");
                }
            }
        }
    }