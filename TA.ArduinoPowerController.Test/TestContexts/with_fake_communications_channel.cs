using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace TA.ArduinoPowerController.Test.TestContexts
    {
    internal class with_fake_communications_channel
        {
            Establish context = () =>
                {
                    Builder = new CommunicationsChannelContextBuilder();
                };

            Cleanup after = () =>
                {
                    Context.Processor.Dispose();
                    Context.Channel?.Close();
                    Context.Channel?.Dispose();
                    Context = null;
                    Builder = null;
                };

            public static CommunicationsChannelContextBuilder Builder { get; set; }
            public static CommunicationsChannelContext Context { get; set; }
        }
    }
