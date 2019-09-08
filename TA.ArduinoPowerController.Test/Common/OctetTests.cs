// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
//
// File: OctetTests.cs  Last modified: 2019-09-08@19:01 by Tim Long

using Machine.Specifications;
using TA.ArduinoPowerController.Common;

namespace TA.ArduinoPowerController.Test.Common
{
    [Subject(typeof(Octet), "Initialization")]
    internal class when_an_octet_is_initialized_to_zero
    {
        Establish context;
        Because of = () => zero = Octet.Zero;

        It should_have_all_bits_clear = () =>
            {
                for (var i = 0; i < 8; i++)
                {
                    zero[i].ShouldBeFalse();
                }
            };

        static Octet zero;
    }

    [Subject(typeof(Octet), "bit clear")]
    internal class when_a_bit_is_cleared
    {
        Establish context = () => immutableOctet = Octet.Max;

        It should_produce_a_new_octet_with_that_bit_cleared = () =>
            {
                var octet = Octet.Max; // All bits set
                for (int i = 0; i < 8; ++i)
                {
                    var actual = octet.WithBitSetTo(i, false);
                    expected[i].ShouldEqual((byte) actual);
                }
                immutableOctet.ShouldEqual(Octet.Max);
            };

        static byte[] expected = {0xFE, 0xFD, 0xFB, 0xF7, 0xEF, 0xDF, 0xBF, 0x7F};
        static Octet immutableOctet;
    }

    [Subject(typeof(Octet), "bit set")]
    internal class when_a_bit_is_set
    {
        Establish context = () => immutableOctet = Octet.Zero;

        It should_produce_a_new_octet_with_that_bit_set = () =>
            {
                for (int i = 0; i < 8; ++i)
                {
                    var actual = immutableOctet.WithBitSetTo(i, true);
                    expected[i].ShouldEqual((byte) actual);
                }
                immutableOctet.ShouldEqual(Octet.Zero);
            };

        static byte[] expected = {0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80};
        static Octet immutableOctet;
    }

    [Subject(typeof(Octet), "bitwise AND")]
    internal class when_a_bitwise_and_is_applied
    {
        Establish context = () => mask = 0xAA;
        Because of = () => actual = Octet.Max & mask;
        It should_mask_the_false_bits = () => actual.ShouldEqual(mask);
        static Octet actual;
        static Octet mask;
    }

    [Subject(typeof(Octet), "bitwise OR")]
    internal class when_a_bitwise_or_is_applied
    {
        Establish context = () => mask = 0xAA;
        Because of = () => actual = Octet.Zero | mask;
        It should_set_the_true_bits = () => actual.ShouldEqual(mask);
        static Octet actual;
        static Octet mask;
    }
}