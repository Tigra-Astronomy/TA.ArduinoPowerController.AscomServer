// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: TransactionException.cs  Last modified: 2017-03-07@19:18 by Tim Long

using System;
using System.Runtime.Serialization;
using TA.Ascom.ReactiveCommunications;

namespace TA.VellemanK8056.DeviceInterface
    {
    [Serializable]
    public class TransactionException : Exception
        {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TransactionException() {}

        public TransactionException(string message) : base(message) {}

        public TransactionException(string message, Exception inner) : base(message, inner) {}

        protected TransactionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}

        public DeviceTransaction Transaction { get; set; }
        }
    }