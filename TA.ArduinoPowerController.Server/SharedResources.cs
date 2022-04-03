// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: SharedResources.cs  Last modified: 2017-03-16@23:34 by Tim Long

using System;
using System.Windows.Forms;
using Ninject;
using NLog;
using TA.ArduinoPowerController.DeviceInterface;
using TA.ArduinoPowerController.Server.Properties;
using TA.Utils.Core.Diagnostics;

namespace TA.ArduinoPowerController.Server
    {
    /// <summary>
    ///     The resources shared by all drivers and devices, in this example it's a serial port with
    ///     a shared SendMessage method an idea for locking the message and handling connecting is
    ///     given. In reality extensive changes will probably be needed. Multiple drivers means that
    ///     several applications connect to the same hardware device, aka a hub. Multiple devices
    ///     means that there are more than one instance of the hardware, such as two focusers. In
    ///     this case there needs to be multiple instances of the hardware connector, each with it's
    ///     own connection count.
    /// </summary>
    public static class SharedResources
        {
        /// <summary>
        ///     ASCOM DeviceID (COM ProgID) for the rotator driver.
        /// </summary>
        public const string SwitchDriverId = "ASCOM.ArduinoPowerController.Switch";
        /// <summary>
        ///     Driver description for the rotator driver.
        /// </summary>
        public const string SwitchDriverName = "Arduino Power Controller";

        private static readonly ILog Log = CompositionRoot.Kernel.Get<ILog>();

        static SharedResources()
            {
            }

        /// <summary>
        ///     Gets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        public static ClientConnectionManager ConnectionManager => CompositionRoot.Kernel.Get<ClientConnectionManager>();



        public static void DoSetupDialog(Guid clientId)
            {
            var oldConnectionString = Settings.Default.ConnectionString;
            Log.Info().Message("SetupDialog requested by client {clientId}", clientId).Write();
            using (var dialogForm = new SetupDialogForm())
                {
                var result = dialogForm.ShowDialog();
                switch (result)
                    {
                    case DialogResult.OK:
                        Log.Info().Message("SetupDialog successful, saving settings").Write();
                        Settings.Default.Save();
                        var newConnectionString = Settings.Default.ConnectionString;
                        if (oldConnectionString != newConnectionString)
                            {
                            Log.Warn().Message(
                                "Connection string has changed from {old} to {new} - replacing the TansactionProcessorFactory")
                            .Property("old", oldConnectionString)
                            .Property("new", newConnectionString)
                            .Write();
                            ConnectionManager.TransactionProcessorFactory = CompositionRoot.Kernel.Get<ITransactionProcessorFactory>();
                            }
                        break;
                    default:
                        Log.Warn().Message("SetupDialog cancelled or failed - reverting to previous settings").Write();
                        Settings.Default.Reload();
                        break;
                    }
                }
            }
        }
    }