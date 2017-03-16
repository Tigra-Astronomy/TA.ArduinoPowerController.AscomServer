// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2016 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: Settings.cs  Last modified: 2016-07-31@00:46 by Tim Long

using System.ComponentModel;
using System.Configuration;

namespace ASCOM.K8056.Properties
    {
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    [SettingsProvider(typeof(SettingsProvider))]
    [DeviceId("ASCOM.K8056.Switch", DeviceName = "Velleman K8056 Relay Module")]
    internal sealed partial class Settings
        {
        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
            {
            // Add code to handle the SettingChangingEvent event here.
            }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
            {
            // Add code to handle the SettingsSaving event here.
            }
        }
    }