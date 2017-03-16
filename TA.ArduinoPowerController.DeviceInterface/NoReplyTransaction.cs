// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: NoReplyTransaction.cs  Last modified: 2017-03-06@18:12 by Tim Long

using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Patterns.Contracts;
using TA.Ascom.ReactiveCommunications;

namespace TA.VellemanK8056.DeviceInterface
    {
    public class NoReplyTransaction : DeviceTransaction
        {
        public NoReplyTransaction([Required] string command) : base(command)
            {
            Timeout = TimeSpan.FromMilliseconds(1); // Very short, as no response is expected.
            }

        public override void ObserveResponse([Required] IObservable<char> source)
            {
            // No reply is expected, so instead of observing the response stream, we immediately complete.
            OnCompleted();
            }

        /// <summary>
        ///     Computes the checksum of the supplied bytes.
        /// </summary>
        /// <param name="bytes">The bytes in the data packet.</param>
        protected internal static byte ComputeChecksum([Required] IEnumerable<byte> bytes)
            {
            byte total = 0;
            foreach (var item in bytes)
                {
                unchecked
                    {
                    total += item;
                    }
                }
            var compliment = (byte) (0x0100 - total);
            return compliment;
            }

        /// <summary>
        ///     Builds a command string for a relay operation complete with checksum.
        /// </summary>
        /// <param name="cardAddress">The card address.</param>
        /// <param name="instruction">The single letter instruction code.</param>
        /// <param name="relayNumber">The relay number, counting from zero.</param>
        /// <returns>A string containing a packet ready to be sent to the serial port (including checksum).</returns>
        protected static string CreateCommand(byte cardAddress, char instruction, ushort relayNumber)
            {
            var command = new List<byte> {0x0D, cardAddress, (byte) instruction, (byte) ('1' + relayNumber)};
            command.Add(ComputeChecksum(command));
            return Encoding.ASCII.GetString(command.ToArray());
            }
        }
    }