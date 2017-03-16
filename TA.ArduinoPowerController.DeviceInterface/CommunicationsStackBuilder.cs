// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: CommunicationsStackBuilder.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;
using TA.Ascom.ReactiveCommunications;

namespace TA.ArduinoPowerController.DeviceInterface
    {
    /// <summary>
    ///     Factory methods for creating various parts of the communications stack.
    /// </summary>
    public static class CommunicationsStackBuilder
        {
        public static ICommunicationChannel BuildChannel(DeviceEndpoint endpoint)
            {
            if (endpoint is SerialDeviceEndpoint)
                return new SerialCommunicationChannel(endpoint);
            throw new NotSupportedException($"There is no supported channel type for the endpoint: {endpoint}")
                {
                Data = {["endpoint"] = endpoint}
                };
            }

        public static TransactionObserver BuildTransactionObserver(ICommunicationChannel channel)
            {
            return new TransactionObserver(channel);
            }

        public static ITransactionProcessor BuildTransactionProcessor(TransactionObserver observer)
            {
            var processor = new ReactiveTransactionProcessor();
            processor.SubscribeTransactionObserver(observer);
            return processor;
            }
        }
    }