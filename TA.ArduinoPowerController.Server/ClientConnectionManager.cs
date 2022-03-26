﻿// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ClientConnectionManager.cs  Last modified: 2017-03-16@23:33 by Tim Long

using JetBrains.Annotations;
using Ninject;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using TA.ArduinoPowerController.DeviceInterface;
using TA.Ascom.ReactiveCommunications;
using TA.PostSharp.Aspects;
using TA.Utils.Core;
using TA.Utils.Core.Diagnostics;

namespace TA.ArduinoPowerController.Server
    {
    /// <summary>
    ///     Manages client (driver) connections to the shared device controller. Uses the Reader
    ///     Writer Lock pattern to ensure thread safety.
    /// </summary>
    [ReaderWriterSynchronized]
    [NLogTraceWithArguments]
    public class ClientConnectionManager
        {
        [Reference] private readonly ILog log;
        private readonly bool performActionsOnOpen;
        [Reference] private Maybe<DeviceController> controllerInstance;
        [Reference] private ITransactionProcessorFactory factory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConnectionManager" /> class.
        /// </summary>
        /// <param name="factory">
        ///     A factory class that can create and destroy transaction processors (and by implication,
        ///     the entire communications stack).
        /// </param>
        public ClientConnectionManager(ITransactionProcessorFactory factory, ILog logService) : this(factory,
            performActionsOnOpen: true)
            {
            this.log = logService;
            }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConnectionManager" /> class and allows
        ///     control of whether an On Connected actions will be performed. This is primarily intended
        ///     for use in unit testing so is not visible to clients.
        /// </summary>
        /// <param name="factory">
        ///     A factory class that can construct instances of a trnasaction processor (and by
        ///     implication, the entire communications stack).
        /// </param>
        /// <param name="performActionsOnOpen">
        ///     if set to <c>true</c> [perform actions on open].
        /// </param>
        internal ClientConnectionManager(ITransactionProcessorFactory factory, bool performActionsOnOpen)
            {
            this.factory = factory;
            this.performActionsOnOpen = performActionsOnOpen;
            Clients = new List<ClientStatus>();
            controllerInstance = Maybe<DeviceController>.Empty;
            }

        [Reference]
        internal List<ClientStatus> Clients { get; }

        /// <summary>
        ///     Gets the controller instance if it has been created.
        /// </summary>
        /// <value>The controller instance.</value>
        internal Maybe<DeviceController> MaybeControllerInstance => controllerInstance;

        public int OnlineClientCount => Clients.Count(p => p.Online);

        /// <summary>
        ///     Gets the number of connected clients.
        /// </summary>
        /// <value>The connected client count.</value>
        public int RegisteredClientCount => Clients.Count();

        /// <summary>
        ///     Gets or sets the transaction processor factory.
        /// </summary>
        /// <value>The transaction processor factory.</value>
        /// <exception cref="InvalidOperationException">
        ///     Cannot change or set the Transaction Processor Factory while there are connected clients
        /// </exception>
        internal ITransactionProcessorFactory TransactionProcessorFactory
            {
            get { return factory; }
            set
                {
                log.Warn().Message("Setting the TransactionProcessorFactory").Write();
                if (OnlineClientCount > 0)
                    throw new InvalidOperationException(
                        "Cannot change or set the Transaction Processor Factory while there are connected clients");
                // We have no online clients, so destroy any existing transaction processors.
                factory?.DestroyTransactionProcessor();
                factory = value;
                }
            }

        internal event EventHandler<EventArgs> ClientStatusChanged;

        [Writer]
        private void DestroyControllerInstance()
            {
            if (controllerInstance.Any())
                controllerInstance.Single().Close();
            controllerInstance = Maybe<DeviceController>.Empty;
            }

        [Writer]
        private void EnsureControllerInstanceCreatedAndOpen()
            {
            if (!controllerInstance.Any())
                {
                CompositionRoot.BeginSessionScope();    // Starting a new session
                var controller = CompositionRoot.Kernel.Get<DeviceController>();
                controllerInstance = Maybe<DeviceController>.From(controller);
                }
            var instance = controllerInstance.Single();
            if (!instance.IsOnline)
                {
                instance.Open();
                }
            }

        [Writer]
        public void GoOffline(Guid clientId)
            {
            log.Info().Message("Go offline for client {clientId}", clientId).Write();
            ClientStatus client = null;
            try
                {
                client = Clients.Single(p => p.Equals(clientId));
                }
            catch (InvalidOperationException e)
                {
                log.Error()
                    .Exception(e)
                    .Message("Attempt to go offline by unecognized client {clientId}", clientId)
                    .Write();
                }
            client.Online = false;
            RaiseClientStatusChanged();
            if (OnlineClientCount == 0)
                {
                log.Warn().Message("The last client has gone offline - closing connection").Write();
                if (controllerInstance.Any())
                    {
                    controllerInstance.Single().Close();
                    controllerInstance = Maybe<DeviceController>.Empty;
                    }
                }
            }

        /// <summary>
        ///     Gets the controller instance, ensuring that it is open and ready for use.
        /// </summary>
        /// <param name="clientId">
        ///     The client must provide it's ID which has previously been obtained by calling
        ///     <see cref="RegisterClient" />.
        /// </param>
        /// <returns>An instance of <see cref="DeviceController"/> which is online and ready to accept commands.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///     Clients must release previous controller instances before requesting another
        /// </exception>
        [Writer]
        public DeviceController GoOnline(Guid clientId)
            {
            log.Info().Message("Go online for client {clientId}", clientId).Write();
            ClientStatus client = null;
            try
                {
                client = Clients.Single(p => p.Equals(clientId));
                }
            catch (InvalidOperationException e)
                {
                log.Error()
                    .Exception(e)
                    .Message("Attempt to go online with unregistered client {clientId}", clientId)
                    .Write();
                //ThrowOnUnrecognizedClient(clientId, e, message);
                }
            try
                {
                EnsureControllerInstanceCreatedAndOpen();
                }
            catch (TransactionException trex)
                {
                log.Error()
                    .Exception(trex)
                    .Message("NOT CONNECTED due to transaction exception in {transaction}", trex.Transaction)
                    .Write();
                DestroyControllerInstance();
                return null;
                }
            var clientOnline = controllerInstance.Single().IsOnline;
            client.Online = clientOnline;
            if (!clientOnline)
                DestroyControllerInstance();
            RaiseClientStatusChanged();
            return clientOnline ? controllerInstance.Single() : null;
            }


        /// <summary>
        ///     Determines whether the client with the specified ID is registered.
        /// </summary>
        /// <param name="clientId">The client unique identifier.</param>
        /// <returns><c>true</c> if the client is connected; otherwise, <c>false</c>.</returns>
        [Reader]
        public bool IsClientRegistered(Guid clientId) => Clients.Any(p => p.Equals(clientId));

        protected void RaiseClientStatusChanged()
            {
            ClientStatusChanged?.Invoke(this, EventArgs.Empty);
            }

        /// <summary>
        ///     Gets a new unique client identifier.
        /// </summary>
        /// <returns>Guid.</returns>
        [Writer]
        public Guid RegisterClient(string name = null)
            {
            var id = Guid.NewGuid();
            var status = new ClientStatus { ClientId = id, Name = name ?? id.ToString(), Online = false };
            Clients.Add(status);
            RaiseClientStatusChanged();
            return id;
            }

        /// <summary>
        ///     Throws an ASCOM.<see cref="T:ASCOM.InvalidOperationException" /> with information about registered clients.
        /// </summary>
        /// <param name="clientId">The client identifier causing the original exception.</param>
        /// <param name="e">The original (inner) exception.</param>
        /// <param name="message">The error message.</param>
        [ContractAnnotation("=>halt")]
        private void ThrowOnUnrecognizedClient(Guid clientId, Exception e, string message)
            {
            var ex = new ASCOM.InvalidOperationException($"Connection Manager: {message}", e);
            ex.Data["RegisteredClients"] = Clients;
            ex.Data["UnknownClient"] = clientId;
            log.Error()
                .Exception(ex).Message("Client not found: {clientId}", clientId)
                .Property("registeredClients", Clients)
                .Write();
            throw ex;
            }

        [Writer]
        public void UnregisterClient(Guid clientId)
            {
            log.Info().Message("Unregistering client {clientId}", clientId).Write();
            var previousClientCount = RegisteredClientCount;
            try
                {
                Clients.Remove(Clients.Single(p => p.Equals(clientId)));
                RaiseClientStatusChanged();
                }
            catch (InvalidOperationException)
                {
                var message = $"Attempt to unregister unknown client {clientId}";
                log.Error()
                    .Message("Attempt to unregister unknown client {clientId}", clientId)
                    .Write();
                }
            if (previousClientCount == 1 && RegisteredClientCount == 0)
                {
                DestroyControllerInstance();
                Server.TerminateLocalServer();
                }
            }
        }
    }