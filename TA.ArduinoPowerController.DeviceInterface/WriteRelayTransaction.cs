// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: WriteRelayTransaction.cs  Last modified: 2019-09-08@04:17 by Tim Long

using TA.Ascom.ReactiveCommunications;

namespace TA.ArduinoPowerController.DeviceInterface
{
    /// <summary>
    ///     Class WriteRelayTransaction. A transaction type used to write a relay setting.
    ///     The transaction checks that the received response contains the expected relay number and value.
    ///     No value is returned by this transaction (it is a write-only transaction) but the received
    ///     response string may be found in <see cref="DeviceTransaction.Response" />.
    /// </summary>
    /// <seealso cref="ArduinoSwitchTransaction" />
    /// <seealso cref="DeviceTransaction" />
    internal class WriteRelayTransaction : ArduinoSwitchTransaction
    {
        private readonly ushort relay;
        private readonly bool value;

        public WriteRelayTransaction(ushort relay, bool value) : base(CreateWriteCommand(relay, value))
        {
            this.relay = relay;
            this.value = value;
        }

        private static string CreateWriteCommand(ushort relay, bool value)
        {
            var onOrOff = value ? '1' : '0';
            return $":S{relay}{onOrOff}#";
        }

        protected override void OnNext(string response)
        {
            // Expecting :Srv#
            var relayNumber = int.Parse(response.Substring(2, 1));
            var relayValue = response[3] == '1';
            if (response[0] != 'S')
            {
                OnError(
                    new TransactionException(
                        $"Response appears to be of the wrong type. Expected 'S' but got '{response[0]}'"));
                return;
            }
            if (relayNumber != relay)
            {
                OnError(new TransactionException(
                    $"Response contained an unexpected relay number. Expected={relay}, Actual={relayNumber}"));
                return;
            }
            if (relayValue != value)
            {
                OnError(
                    new TransactionException(
                        $"Response contained an unexpected relay value. Expected={value}, Actual={relayValue}"));
                return;
            }
            base.OnNext(response);
        }
    }
}