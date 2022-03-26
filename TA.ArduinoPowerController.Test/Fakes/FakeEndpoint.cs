// This file is part of the TA.NexDome.AscomServer project
// Copyright © 2019-2019 Tigra Astronomy, all rights reserved.

using System;
using System.Text.RegularExpressions;

namespace TA.NexDome.Specifications.Fakes
    {
    using TA.Ascom.ReactiveCommunications;

    class FakeEndpoint : DeviceEndpoint
        {
            public string FakeResponse { get; }

            public FakeEndpoint(string fakeResponse = null)
            {
                this.FakeResponse = fakeResponse;
            }

        public override string ToString()
            {
            return "fake device";
            }

        public static bool IsConnectionStringValid(string connectionString) =>
            connectionString.StartsWith("Fake", StringComparison.InvariantCultureIgnoreCase);

        public static DeviceEndpoint FromConnectionString(string connectionString)
        {
            const string endpointParsePattern = "^Fake(:(?<Response>.*))?$";
            var parser = new Regex(endpointParsePattern,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture |
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = parser.Match(connectionString);
            if (!match.Success)
                throw new ArgumentException("unable to parse the connection string");
            var fakeResponse = match.Groups["Response"].Value;
            return new FakeEndpoint(fakeResponse);
        }
        }
    }