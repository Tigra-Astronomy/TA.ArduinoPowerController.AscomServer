// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2016 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: EnergizeRelayTransaction.cs  Last modified: 2016-06-27@22:11 by Tim Long

namespace TA.VellemanK8056.DeviceInterface
    {
    internal class EnergizeRelayTransaction : NoReplyTransaction
        {
        public EnergizeRelayTransaction(ushort relayNumber) : base(CreateCommand(1, 'S', relayNumber)) {}
        }
    }