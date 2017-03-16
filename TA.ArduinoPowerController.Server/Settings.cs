// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: Settings.cs  Last modified: 2017-03-06@21:11 by Tim Long

using System.ComponentModel;
using System.Configuration;
using ASCOM;
using NLog;
using SettingsProvider = ASCOM.SettingsProvider;

// ReSharper disable CheckNamespace

namespace TA.VellemanK8056.Server.Properties
    {
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    [SettingsProvider(typeof(SettingsProvider))]
    [DeviceId(SharedResources.RotatorDriverId, DeviceName = SharedResources.RotatorDriverName)]
    internal sealed partial class Settings
        {
        private readonly ILogger log = LogManager.GetCurrentClassLogger();

        public Settings()
            {
            SettingChanging += SettingChangingEventHandler;
            SettingsSaving += SettingsSavingEventHandler;
            SettingsLoaded += SettingsLoadedEventHandler;
            PropertyChanged += SettingChangedEventHandler;
            }

        private void SettingChangedEventHandler(object sender, PropertyChangedEventArgs args)
            {
            log.Debug($"Setting changed: {args.PropertyName}");
            }

        private void SettingsLoadedEventHandler(object sender, SettingsLoadedEventArgs e)
            {
            log.Warn("Settings loaded");
            }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
            {
            log.Debug($"Setting changing {e.SettingName}[{e.SettingKey}] -> {e.NewValue}");
            }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
            {
            log.Warn($"Saving settings");
            }
        }
    }