// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: RelayStateChangedEventArgs.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;

namespace TA.ArduinoPowerController.DeviceInterface
    {
    public class RelayStateChangedEventArgs : EventArgs
        {
        public RelayStateChangedEventArgs(int relayNumber, bool newState)
            {
            RelayNumber = relayNumber;
            NewState = newState;
            }

        public bool NewState { get; }

        public int RelayNumber { get; }
        }
    }