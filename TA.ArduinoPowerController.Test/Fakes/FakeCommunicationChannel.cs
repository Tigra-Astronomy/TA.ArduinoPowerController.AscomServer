// This file is part of the TA.NexDome.AscomServer project
// Copyright © 2019-2019 Tigra Astronomy, all rights reserved.

using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog.Fluent;
using TA.Ascom.ReactiveCommunications;

namespace TA.NexDome.Specifications.Fakes
    {
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    ///     A fake communication channel that logs any sent data in <see cref="SendLog" />and
    ///     receives a fake pre-programmed response passed into the constructor. The class also
    ///     keeps a count of how many times each method of <see cref="ICommunicationChannel" /> was
    ///     called.
    /// </summary>
    public class FakeCommunicationChannel : ICommunicationChannel
        {
        readonly IObservable<char> receivedCharacters;

        readonly Subject<char> receiveChannelSubject = new Subject<char>();

        readonly StringBuilder sendLog;

        /// <summary>
        ///     Dependency injection constructor.
        ///     Initializes a new instance of the <see cref="SafetyMonitorDriver" /> class.
        /// </summary>
        /// <param name="fakeResponse">Implementation of the injected dependency.</param>
        public FakeCommunicationChannel(string fakeResponse)
            {
            Contract.Requires(fakeResponse != null);
            Endpoint = new InvalidEndpoint();
            Response = fakeResponse;
            receivedCharacters = fakeResponse.ToCharArray().ToObservable();
            sendLog = new StringBuilder();
            IsOpen = false;
            }

        /// <summary>
        ///     Gets the send log.
        /// </summary>
        /// <value>The send log.</value>
        public string SendLog => sendLog.ToString();

        /// <summary>
        ///     Gets a copy of the fake pre-programmed response.
        /// </summary>
        /// <value>The response.</value>
        public string Response { get; }

        public int TimesDisposed { get; set; }

        public int TimesClosed { get; set; }

        public int TimesOpened { get; set; }

        public void Dispose()
            {
            TimesDisposed++;
            }

        public void Open()
            {
            TimesOpened++;
            IsOpen = true;
            }

        public void Close()
            {
            TimesClosed++;
            IsOpen = false;
            }

        public void Send(string txData)
            {
            Log.Info().Message($"Send: {txData}").Property(nameof(txData), txData).Write();
            sendLog.Append(txData);
            foreach (char c in Response) receiveChannelSubject.OnNext(c);
            }

        public IObservable<char> ObservableReceivedCharacters => receiveChannelSubject.AsObservable();

        public bool IsOpen { get; set; }

        public DeviceEndpoint Endpoint { get; }
        }
    }