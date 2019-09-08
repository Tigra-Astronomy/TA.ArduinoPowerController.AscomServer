// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: ArduinoSwitchTransaction.cs  Last modified: 2019-09-08@04:07 by Tim Long

using System;
using PostSharp.Patterns.Contracts;
using TA.Ascom.ReactiveCommunications.Transactions;

namespace TA.ArduinoPowerController.DeviceInterface
{
    /// <summary>
    ///     Class ArduinoSwitchTransaction. Sets the base characteristics for all transactions
    ///     in this solution.
    /// </summary>
    /// <seealso cref="TA.Ascom.ReactiveCommunications.Transactions.TerminatedStringTransaction" />
    internal abstract class ArduinoSwitchTransaction : TerminatedStringTransaction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArduinoSwitchTransaction" /> class.
        /// </summary>
        /// <param name="command">The command to be sent to the communications channel.</param>
        /// <requires csharp="!string.IsNullOrEmpty(command)" vb="Not System.String.IsNullOrEmpty(command)">
        ///     !string.IsNullOrEmpty(command)
        /// </requires>
        /// <remarks>
        ///     This abstract class cannot be instantiated directly and is intended to be inherited by your own transaction
        ///     classes.
        /// </remarks>
        protected ArduinoSwitchTransaction([Required] string command) : base(command)
        {
            Timeout = TimeSpan.FromSeconds(2);
        }
    }
}