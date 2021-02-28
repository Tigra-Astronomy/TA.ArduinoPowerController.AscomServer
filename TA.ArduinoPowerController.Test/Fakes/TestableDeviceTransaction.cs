// This file is part of the TA.NexDome.AscomServer project
// Copyright © 2019-2019 Tigra Astronomy, all rights reserved.

namespace TA.NexDome.Specifications.Fakes
    {
    using System;

    using TA.Ascom.ReactiveCommunications;
    using TA.Utils.Core;

    class TestableDeviceTransaction : DeviceTransaction
        {
        readonly DeviceTransaction sourceTransaction;

        public TestableDeviceTransaction(DeviceTransaction sourceTransaction)
            : base(sourceTransaction.Command)
            {
            this.sourceTransaction = sourceTransaction;
            }

        public override void ObserveResponse(IObservable<char> source)
            {
            throw new NotImplementedException();
            }

        void SetResponse(string response)
            {
            Response = Maybe<string>.From(response);
            }

        internal void SignalCompletion(string fakeResponse)
            {
            SetResponse(fakeResponse);
            OnCompleted();
            }

        public void SignalError(string error)
            {
            OnError(new TimeoutException(error));
            }
        }
    }