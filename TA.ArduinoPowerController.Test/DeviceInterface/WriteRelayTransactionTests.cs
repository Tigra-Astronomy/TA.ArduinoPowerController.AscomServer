// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: WriteRelayTransactionTests.cs  Last modified: 2019-09-09@18:46 by Tim Long

using Machine.Specifications;
using TA.ArduinoPowerController.DeviceInterface;
using TA.ArduinoPowerController.Test.TestContexts;

namespace TA.ArduinoPowerController.Test.DeviceInterface
{
    [Subject(typeof(WriteRelayTransaction), "command")]
    internal class when_a_relay_transaction_is_processed : with_fake_communications_channel
    {
        Establish context = () => Context = Builder
            .WithFakeResponse(":W00#")
            .Build();

        Because of = () =>
            {
                Transaction = new WriteRelayTransaction(0, false);
                Context.Processor.CommitTransaction(Transaction);
                Transaction.WaitForCompletionOrTimeout();
            };

        It should_succeed = () => Transaction.Successful.ShouldBeTrue();
        static WriteRelayTransaction Transaction;
    }
}