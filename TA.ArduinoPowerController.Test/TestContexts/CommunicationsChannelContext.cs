using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TA.Ascom.ReactiveCommunications;
using TA.NexDome.Specifications.Fakes;

namespace TA.ArduinoPowerController.Test.TestContexts
    {
    internal sealed class CommunicationsChannelContext
        {
            public ICommunicationChannel Channel { get; set; }

            public FakeCommunicationChannel FakeChannel => Channel as FakeCommunicationChannel;

            public DeviceEndpoint Endpoint => Channel.Endpoint;

            public ReactiveTransactionProcessor Processor { get; set; }

            public ChannelFactory Factory { get; set; }

            public TransactionObserver Observer { get; set; }
        }
    }
