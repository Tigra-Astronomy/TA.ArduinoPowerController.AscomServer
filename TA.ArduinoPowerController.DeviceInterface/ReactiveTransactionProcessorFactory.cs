// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ReactiveTransactionProcessorFactory.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;
using System.Threading;
using TA.Ascom.ReactiveCommunications;

namespace TA.ArduinoPowerController.DeviceInterface
    {
    public class ReactiveTransactionProcessorFactory : ITransactionProcessorFactory
        {
        private readonly string connectionString;
        private TransactionObserver observer;
        private ReactiveTransactionProcessor processor;

        public ReactiveTransactionProcessorFactory(string connectionString)
            {
            this.connectionString = connectionString;
            Endpoint = DeviceEndpoint.FromConnectionString(connectionString);
            }

        public ICommunicationChannel Channel { get; private set; }

        /// <summary>
        ///     Creates the transaction processor ready for use. Also creates and initialises the
        ///     device endpoint and the communications channel and opens the channel.
        /// </summary>
        /// <returns>ITransactionProcessor.</returns>
        public ITransactionProcessor CreateTransactionProcessor()
            {
            Endpoint = DeviceEndpoint.FromConnectionString(connectionString);
            Channel = CommunicationsStackBuilder.BuildChannel(Endpoint);
            observer = new TransactionObserver(Channel);
            processor = new ReactiveTransactionProcessor();
            processor.SubscribeTransactionObserver(observer, TimeSpan.FromSeconds(2));
            Channel.Open();
            //Task.Delay(TimeSpan.FromSeconds(2)).Wait(); // Arduino needs 2 seconds to initialize
            Thread.Sleep(TimeSpan.FromSeconds(3));
            return processor;
            }

        /// <summary>
        ///     Destroys the transaction processor and its dependencies. Ensures that the
        ///     <see cref="Channel" /> is closed. Once this method has been called, the
        ///     <see cref="Channel" /> and <see cref="Endpoint" /> properties will be null. A new
        ///     connection to the same endpoint can be created by calling
        ///     <see cref="CreateTransactionProcessor" /> again.
        /// </summary>
        public void DestroyTransactionProcessor()
            {
            processor?.Dispose();
            processor = null; // [Sentinel]
            observer = null;
            if (Channel?.IsOpen ?? false)
                Channel.Close();
            Channel?.Dispose();
            Channel = null; // [Sentinel]
            GC.Collect(3, GCCollectionMode.Forced, blocking: true);
            }

        public DeviceEndpoint Endpoint { get; set; }
        }
    }