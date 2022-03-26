// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ServerStatusDisplay.cs  Last modified: 2017-03-16@23:34 by Tim Long

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using TA.ArduinoPowerController.DeviceInterface;
using TA.ArduinoPowerController.Server.Properties;
using TA.Ascom.ReactiveCommunications.Diagnostics;
using TA.WinFormsControls;

namespace TA.ArduinoPowerController.Server
    {
    public partial class ServerStatusDisplay : Form
        {
        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        private List<Annunciator> annunciators;
        private IDisposable clientStatusSubscription;
        private IDisposable relayStateChangedSubscription;

        public ServerStatusDisplay()
            {
            InitializeComponent();
            }

        private void CalibrateCommand_Click(object sender, EventArgs e)
            {
            if (!SharedResources.ConnectionManager.MaybeControllerInstance.Any())
                return;
            //SharedResources.ConnectionManager.MaybeControllerInstance.Single().CalibrateFocuserAsync();
            }

        /// <summary>
        ///     Begins or terminates UI updates depending on the number of online clients.
        /// </summary>
        private void ConfigureUiPropertyNotifications()
            {
            var clientsOnline = SharedResources.ConnectionManager.OnlineClientCount;
            if (clientsOnline > 0)
                SubscribePropertyChangeNotifications();
            else
                UnsubscribePropertyChangeNotifications();
            }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
            {
            clientStatusSubscription?.Dispose();
            UnsubscribePropertyChangeNotifications();
            var clients = SharedResources.ConnectionManager.Clients;
            foreach (var client in clients)
                {
                SharedResources.ConnectionManager.GoOffline(client.ClientId);
                }
            Application.Exit();
            }

        private void frmMain_Load(object sender, EventArgs e)
            {
            annunciators = new List<Annunciator>
                {
                Relay0Annunciator, Relay1Annunciator, Relay2Annunciator, Relay3Annunciator,
                Relay4Annunciator, Relay5Annunciator, Relay6Annunciator, Relay7Annunciator
                };
            foreach (var annunciator in annunciators)
                {
                annunciator.Enabled = false;
                }
            var clientStatusObservable = Observable.FromEventPattern<EventHandler<EventArgs>, EventArgs>(
                handler => SharedResources.ConnectionManager.ClientStatusChanged += handler,
                handler => SharedResources.ConnectionManager.ClientStatusChanged -= handler);
            clientStatusSubscription = clientStatusObservable
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(ObserveClientStatusChanged);
            ObserveClientStatusChanged(null); // This sets the initial UI state before any notifications arrive
            }

        private void frmMain_LocationChanged(object sender, EventArgs e)
            {
            Settings.Default.Save();
            }


        private void ObserveClientStatusChanged(EventPattern<EventArgs> eventPattern)
            {
            SetUiButtonState();
            SetUiDeviceConnectedState();
            var clientStatus = SharedResources.ConnectionManager.Clients;
            registeredClientCount.Text = clientStatus.Count().ToString();
            OnlineClients.Text = clientStatus.Count(p => p.Online).ToString();
            ConfigureUiPropertyNotifications();
            }


        private void SetPaCommand_Click(object sender, EventArgs e)
            {
            if (!SharedResources.ConnectionManager.MaybeControllerInstance.Any())
                return;
            //SharedResources.ConnectionManager.MaybeControllerInstance.Single().SetRotatorPositionAngle(0.0);
            }

        private void SetUiButtonState() {}

        /// <summary>
        ///     Enables each device activity annunciator if there are any clients connected to that
        ///     device.
        /// </summary>
        private void SetUiDeviceConnectedState() {}

        private void SetupCommand_Click(object sender, EventArgs e)
            {
            SharedResources.DoSetupDialog(default(Guid));
            }

        /// <summary>
        ///     Creates subscriptions that observe property change notifications and update the UI as changes occur.
        /// </summary>
        private void SubscribePropertyChangeNotifications()
            {
            if (!SharedResources.ConnectionManager.MaybeControllerInstance.Any())
                return;
            var controller = SharedResources.ConnectionManager.MaybeControllerInstance.Single();
            var relayStateChangeEvents = Observable
                .FromEventPattern<EventHandler<RelayStateChangedEventArgs>, RelayStateChangedEventArgs>(
                    handler => controller.RelayStateChanged += handler,
                    handler => controller.RelayStateChanged -= handler);

            relayStateChangedSubscription = relayStateChangeEvents
                .Select(element => element.EventArgs)
                .ObserveOn(SynchronizationContext.Current)
                .Trace("Annunciators")
                .Subscribe(UpdateRelayAnnunciator);
            }

        /// <summary>
        ///     Stops observing the controller property change notifications.
        /// </summary>
        private void UnsubscribePropertyChangeNotifications()
            {
            // Dispose any observable subscriptions
            relayStateChangedSubscription?.Dispose();
            }

        private void UpdateRelayAnnunciator(RelayStateChangedEventArgs args)
            {
            log.Info($"Relay {args.RelayNumber} changed to {args.NewState}");
            if (annunciators == null || annunciators.Count == 0)
                {
                log.Warn($"Relay {args.RelayNumber} changed to {args.NewState} but there are no annunciators to update");
                return;
                }
            var annunciator = annunciators[args.RelayNumber];
            annunciator.Enabled = args.NewState;
            }
        }
    }