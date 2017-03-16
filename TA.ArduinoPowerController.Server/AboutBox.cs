// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: AboutBox.cs  Last modified: 2017-03-07@20:02 by Tim Long

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace TA.VellemanK8056.Server
    {
    public partial class AboutBox : Form
        {
        public AboutBox()
            {
            InitializeComponent();
            }

        private void AboutBox_Load(object sender, EventArgs e)
            {
            var me = Assembly.GetExecutingAssembly();
            var name = me.GetName();
            var driverVersion = name.Version;
            DriverVersion.Text = driverVersion.ToString();
            }

        private void DriverVersion_Click(object sender, EventArgs e) {}

        private void label1_Click(object sender, EventArgs e) {}

        private void NavigateToWebPage(object sender, EventArgs e)
            {
            var control = sender as Control;
            if (control == null)
                return;
            var url = control.Tag.ToString();
            if (!url.StartsWith("http:"))
                return;
            try
                {
                Process.Start(url);
                }
            catch (Exception)
                {
                // Just fail silently
                }
            }

        private void OkCommand_Click(object sender, EventArgs e)
            {
            Close();
            }
        }
    }