// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: CommunicationSettingsControl.cs  Last modified: 2017-03-08@15:47 by Tim Long

using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using TA.VellemanK8056.Server.Properties;

namespace TA.VellemanK8056.Server
    {
    public partial class CommunicationSettingsControl : UserControl
        {
        public CommunicationSettingsControl()
            {
            InitializeComponent();
            var currentSelection = Settings.Default.CommPortName;
            var ports = new SortedSet<string>(SerialPort.GetPortNames());
            if (!ports.Contains(currentSelection))
                {
                ports.Add(currentSelection);
                }
            CommPortName.Items.Clear();
            CommPortName.Items.AddRange(ports.ToArray());
            var currentIndex = CommPortName.Items.IndexOf(currentSelection);
            CommPortName.SelectedIndex = currentIndex;
            }

        public void Save()
            {
            Settings.Default.ConnectionString = $"{Settings.Default.CommPortName}:2400";
            Settings.Default.Save();
            }
        }
    }