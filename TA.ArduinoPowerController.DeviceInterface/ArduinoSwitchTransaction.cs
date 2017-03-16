// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ArduinoSwitchTransaction.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;
using PostSharp.Patterns.Contracts;
using TA.Ascom.ReactiveCommunications.Transactions;

namespace TA.ArduinoPowerController.DeviceInterface
    {
    internal abstract class ArduinoSwitchTransaction : TerminatedStringTransaction
        {
        protected ArduinoSwitchTransaction([Required] string command) : base(command)
            {
            Timeout = TimeSpan.FromSeconds(2);
            }
        }
    }