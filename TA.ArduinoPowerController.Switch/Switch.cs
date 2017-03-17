// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: Switch.cs  Last modified: 2017-03-17@17:37 by Tim Long

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using ASCOM;
using ASCOM.DeviceInterface;
using JetBrains.Annotations;
using NLog;
using TA.ArduinoPowerController.DeviceInterface;
using TA.ArduinoPowerController.Server;
using TA.ArduinoPowerController.Server.Properties;
using TA.PostSharp.Aspects;
using NotImplementedException = System.NotImplementedException;

#if DEBUG_IN_EXTERNAL_APP
using System.Windows.Forms;

#endif

namespace TA.ArduinoPowerController.AscomSwitch
    {
    [ProgId(SharedResources.SwitchDriverId)]
    [Guid("A864F06E-B5CC-4566-BCBF-59FAC56E6DDB")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [UsedImplicitly]
    [ServedClassName(SharedResources.SwitchDriverName)]
    [NLogTraceWithArguments]
    public class Switch : ReferenceCountedObjectBase, ISwitchV2, IDisposable, IAscomDriver
        {
        private readonly Guid clientId;
        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        private DeviceController device;

        private bool disposed;
        private Octet shadow = Octet.Zero;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Switch" /> class.
        /// </summary>
        public Switch()
            {
#if DEBUG_IN_EXTERNAL_APP
            MessageBox.Show("Attach debugger now");
#endif
            //HandleAssemblyResolveEvents();
            //device = CompositionRoot.GetDeviceLayer();
            clientId = SharedResources.ConnectionManager.RegisterClient(SharedResources.SwitchDriverId);
            }

        internal bool IsOnline => device?.IsOnline ?? false;

        [MustBeConnected]
        public string Action(string ActionName, string ActionParameters)
            {
            throw new NotImplementedException();
            }

        public bool CanWrite(short id) => true;

        public void CommandBlind(string Command, bool Raw = false)
            {
            throw new NotImplementedException();
            }

        public bool CommandBool(string Command, bool Raw = false)
            {
            throw new NotImplementedException();
            }

        public string CommandString(string Command, bool Raw = false)
            {
            throw new NotImplementedException();
            }

        /// <summary>
        ///     Gets or sets the connection state.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
            {
            get { return IsOnline; }
            set
                {
                if (value)
                    Connect();
                else
                    Disconnect();
                }
            }

        /// <summary>
        ///     Returns a description of the device, such as manufacturer and modelnumber. Any ASCII characters may be used.
        /// </summary>
        public string Description => "Velleman K8056 Relay Card";

        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        /// <summary>
        ///     Descriptive and version information about this ASCOM driver.
        /// </summary>
        public string DriverInfo => @"ASCOM Switch driver for Velleman K8056 8-port relay card
Developed by Tigra Astronomy
in collaboration with Oxford University Department of Astrophysics.
Project page: http://tigra-astronomy.com/oss/k8056-switch-driver
Licensed under the MIT License: http://tigra.mit-license.org/";

        /// <summary>
        ///     A string containing only the major and minor version of the driver.
        /// </summary>
        public string DriverVersion => "0.0";

        [MustBeConnected]
        public bool GetSwitch(short id) => shadow[id];

        public string GetSwitchDescription(short id) => $"Relay {id}";


        public string GetSwitchName(short id) => Settings.Default.SwitchNames[id] ?? $"Relay {id}";

        /// <summary>
        ///     Returns the value for switch device id as a double
        /// </summary>
        [MustBeConnected]
        public double GetSwitchValue(short id) => shadow[id] ? 1.0 : 0.0;

        /// <summary>
        ///     The ASCOM interface version number that this device supports.
        /// </summary>
        public short InterfaceVersion => 2;

        public short MaxSwitch => 8;

        /// <summary>
        ///     Returns the maximum value for this switch device.
        /// </summary>
        public double MaxSwitchValue(short id) => 1.0;

        /// <summary>
        ///     Returns the minimum value for this switch device.
        /// </summary>
        public double MinSwitchValue(short id) => 0.0;

        /// <summary>
        ///     The short name of the driver, for display purposes
        /// </summary>
        public string Name => SharedResources.SwitchDriverName;

        [MustBeConnected]
        public void SetSwitch(short id, bool state)
            {
            var relay = (ushort) id;
            if (state) device.SetRelay(relay);
            else device.ClearRelay(relay);
            shadow = shadow.WithBitSetTo(id, state);
            }

        public void SetSwitchName(short id, string name)
            {
            Settings.Default.SwitchNames[id] = string.IsNullOrWhiteSpace(name) ? $"Relay {id}" : name;
            Settings.Default.Save();
            }

        /// <summary>
        ///     Set the value for this device as a double.
        /// </summary>
        [MustBeConnected]
        public void SetSwitchValue(short id, double value)
            {
            SetSwitch(id, value > 0.0);
            }

        public void SetupDialog()
            {
            SharedResources.DoSetupDialog(clientId);
            }

        /// <summary>
        ///     Returns the list of action names supported by this driver (currently none supported).
        /// </summary>
        public ArrayList SupportedActions => new ArrayList();

        /// <summary>
        ///     Returns the step size that this device supports (the difference between successive values of the device).
        /// </summary>
        public double SwitchStep(short id) => 1.0;

        /// <summary>
        ///     Connects to the device.
        /// </summary>
        /// <exception cref="ASCOM.DriverException">
        ///     Failed to connect. Open apparently succeeded but then the device reported that
        ///     is was offline.
        /// </exception>
        private void Connect()
            {
            device = SharedResources.ConnectionManager.GoOnline(clientId);
            if (!device.IsOnline)
                {
                log.Error("Connect failed - device reported offline");
                throw new DriverException(
                    "Failed to connect. Open apparently succeeded but then the device reported that is was offline.");
                }
            device.PerformOnConnectTasks();
            }

        private void CreateSwitchNames()
            {
            Settings.Default.SwitchNames = new StringCollection();
            for (var i = 0; i < MaxSwitch; i++)
                {
                Settings.Default.SwitchNames.Add($"Relay {i}");
                }
            }

        /// <summary>
        ///     Disconnects from the device.
        /// </summary>
        private void Disconnect()
            {
            SharedResources.ConnectionManager.GoOffline(clientId);
            device = null; //[Sentinel]
            }

        protected virtual void Dispose(bool fromUserCode)
            {
            if (!disposed)
                {
                if (fromUserCode)
                    {
                    SharedResources.ConnectionManager.UnregisterClient(clientId);
                    // Do not dispose of any objects that may be referenced elsewhere.
                    }

                // ToDo - Release unmanaged resources here, if necessary.
                }
            disposed = true;

            // ToDo: Call the base class's Dispose(Boolean) method, if available.
            // base.Dispose(fromUserCode);
            }


        /// <summary>
        ///     Finalizes this instance (called prior to garbage collection by the CLR)
        /// </summary>
        ~Switch()
            {
            Dispose(false);
            }

        /// <summary>
        ///     Installs a custom assembly resolver into the AppDomain so that the driver can find its
        ///     referenced assemblies.
        /// </summary>
        private void HandleAssemblyResolveEvents()
            {
            AppDomain.CurrentDomain.AssemblyResolve += AscomDriverAssemblyResolver.ResolveSupportAssemblies;
            }
        }
    }