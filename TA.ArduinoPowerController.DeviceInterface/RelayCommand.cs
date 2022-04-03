// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: RelayCommand.cs  Last modified: 2019-09-08@12:35 by Tim Long
namespace TA.ArduinoPowerController.DeviceInterface
{
    /// <summary>
    /// A simple data transfer object (DTO) for storing relay operations
    /// </summary>
    class RelayCommand
    {
        public string Operation { get; set; }

        public ushort Relay { get; set; }

        public bool State { get; set; }

        /// Debugging is always easier if there is a ToString method.
        public override string ToString()
        {
            return $"{nameof(Operation)}: {Operation}, {nameof(Relay)}: {Relay}, {nameof(State)}: {State}";
        }
    }
}