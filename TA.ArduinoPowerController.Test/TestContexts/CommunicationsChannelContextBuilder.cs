using TA.Ascom.ReactiveCommunications;
using TA.NexDome.Specifications.Fakes;

namespace TA.ArduinoPowerController.Test.TestContexts
    {
    class CommunicationsChannelContextBuilder
        {
        CommunicationsChannelContext context = new CommunicationsChannelContext();
        string connectionString = "Fake";
        string fakeResponse = string.Empty;

        internal CommunicationsChannelContextBuilder WithFakeResponse(string response)
            {
            connectionString = $"Fake:{response}";
            return this;
            }

        internal CommunicationsChannelContext Build()
            {
            var logService = new TA.Utils.Logging.NLog.LoggingService();
            context.Factory = new ChannelFactory(logService);
            context.Factory.RegisterChannelType(FakeEndpoint.IsConnectionStringValid, FakeEndpoint.FromConnectionString, FakeCommunicationChannel.FromEndpoint);
            context.Channel = context.Factory.FromConnectionString(connectionString);
            context.Observer = new TransactionObserver(context.Channel);
            var processor = new ReactiveTransactionProcessor();
            processor.SubscribeTransactionObserver(context.Observer);
            context.Processor = processor;
            context.Channel.Open();
            return context;
            }
        }
    }
