// This file is part of the TA.NexDome.AscomServer project
// Copyright © 2019-2019 Tigra Astronomy, all rights reserved.

namespace TA.NexDome.Specifications.Fakes
    {
    using System.Collections.Generic;
    using System.Linq;

    using TA.Ascom.ReactiveCommunications;

    class FakeTransactionProcessor : ITransactionProcessor
        {
        readonly IEnumerable<string> fakeResponses;

        readonly IEnumerator<string> responseEnumerator;

        public FakeTransactionProcessor(IEnumerable<string> fakeResponses)
            {
            var enumerable = fakeResponses.ToList();
            this.fakeResponses = enumerable;
            responseEnumerator = enumerable.GetEnumerator();
            }

        public List<DeviceTransaction> ProcessedTransactions { get; } = new List<DeviceTransaction>();

        public void CommitTransaction(DeviceTransaction transaction)
            {
            bool moreResponses = responseEnumerator.MoveNext();
            if (moreResponses)
                transaction.SimulateCompletionWithResponse(responseEnumerator.Current);
            else
                transaction.TimedOut("Timeout");
            ProcessedTransactions.Add(transaction);
            }
        }
    }