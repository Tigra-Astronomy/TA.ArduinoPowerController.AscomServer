// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: TransactionException.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;
using TA.Ascom.ReactiveCommunications;

namespace TA.ArduinoPowerController.DeviceInterface
    {
    [Serializable]
    public sealed class TransactionException : Exception
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

        public DeviceTransaction Transaction { get; set; }
        }
    }