// This file is part of the TA.NexDome.AscomServer project
// Copyright © 2019-2019 Tigra Astronomy, all rights reserved.

namespace TA.NexDome.Specifications.Fakes
    {
    using TA.Ascom.ReactiveCommunications;

    class FakeEndpoint : DeviceEndpoint
        {
        public override string ToString()
            {
            return "fake device";
            }
        }
    }