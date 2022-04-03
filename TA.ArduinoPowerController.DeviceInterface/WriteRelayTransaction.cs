// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: WriteRelayTransaction.cs  Last modified: 2019-09-08@12:50 by Tim Long

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using TA.Ascom.ReactiveCommunications;
using TA.Ascom.ReactiveCommunications.Diagnostics;

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
    internal class WriteRelayTransaction : DeviceTransaction
    {
        private const string ResponsePattern = @"^:(?<Operation>[SsRr])(?<Relay>\d)(?<Value>[01])#$";

        private const RegexOptions ParseOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant |
                                                  RegexOptions.ExplicitCapture | RegexOptions.Singleline;

        private static readonly Regex responseParser = new Regex(ResponsePattern, ParseOptions);
        private readonly ushort relay;
        private readonly bool value;

        public WriteRelayTransaction(ushort relay, bool value) : base(CreateWriteCommand(relay, value))
        {
            this.relay = relay;
            this.value = value;
            Timeout=TimeSpan.FromSeconds(2);
        }

        /// <summary>
        ///     Upon successful transaction completion, contains the received relay operation.
        /// </summary>
        public RelayCommand Value { get; private set; }

        private static string CreateWriteCommand(ushort relay, bool value)
        {
            var onOrOff = value ? '1' : '0';
            return $":S{relay}{onOrOff}#";
        }

        /// <summary>
        ///     Observes the character sequence from the communications channel until a satisfactory
        ///     response has been received. Every transaction class must have an <c>ObserveResponse</c>
        ///     override that picks out exactly one response that is valid for the type of transaction.
        ///     This behaviour is a key design pillar of Reactive Communications for ASCOM.
        /// </summary>
        /// <param name="source">The source <see cref="IObservable{char}" /> sequence.</param>
        /// <remarks>
        ///     This method is contractually required to guarantee that only valid responses are accepted.
        /// </remarks>
        public override void ObserveResponse(IObservable<char> source)
        {
            // Filter the response stream into a sequence of valid responses using a regular expression match
            var query = from message in source.DelimitedMessageStrings(':','#')
                        where responseParser.IsMatch(message)
                        select message;
            // Then subscribe to the valid response sequence and only take the first element.
            // Note that OnNext and OnError are provided by the DeviceTransaction base class,
            // but OnCompleted is overridden here.
            query.Trace("Relay")
                .Take(1)
                .Subscribe(OnNext, OnError, OnCompleted);
        }

        /// <summary>
        ///     Called when the response sequence completes. This indicates a successful
        ///     transaction and provides an opportunity for further analysis of a known-good response.
        /// </summary>
        /// <remarks>
        ///     If this method is called, we know that a response has been received and that it is
        ///     guaranteed to be valid, since the <see cref="ObserveResponse" /> method is
        ///     constructed to only respond to valid responses. If we never receive a response, then
        ///     the transaction will time out and <see cref="DeviceTransaction.OnError" /> would be
        ///     called instead. Here we can perform further analysis on our response string and
        ///     transform it into a more meaningful type.
        /// </remarks>
        protected override void OnCompleted()
        {
            if (Response.Any())
            {
                var matches = responseParser.Match(Response.Single());
                var operation = matches.Groups["Operation"].Value.ToUpperInvariant();
                int relay = int.Parse(matches.Groups["Relay"].Value);
                bool state = matches.Groups["Value"].Value == "1";
                Value = new RelayCommand {Operation = operation, Relay = this.relay, State = state};
                //Log.Debug().Message("Successfully received RelayCommand {response}", Value).Write();
            }
            base.OnCompleted(); //  This is critical - it marks the transaction as complete.
        }
    }
}