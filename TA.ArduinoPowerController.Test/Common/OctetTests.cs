// This file is part of the TA.UWP.IoTUtilities project
//
// Copyright © 2015-2015 Tigra Astronomy., all rights reserved.
//
// File: OctetTests.cs  Last modified: 2015-11-10@15:27 by Tim Long

using FluentAssertions;
using TA.ArduinoPowerController.Common;
using Xunit;

namespace TA.ArduinoPowerController.Test.Common
{
    public class OctetTests
    {
        [Fact]
        private void OctetZeroContainsAllBitsFalse()
        {
            var zero = Octet.Zero;
            for (var i = 0; i < 8; i++)
            {
                zero[i].Should().BeFalse("Zero instance should always be false");
            }
        }

        [Theory]
        [InlineData(0, 0xFE)]
        [InlineData(1, 0xFD)]
        [InlineData(2, 0xFB)]
        [InlineData(3, 0xF7)]
        [InlineData(4, 0xEF)]
        [InlineData(5, 0xDF)]
        [InlineData(6, 0xBF)]
        [InlineData(7, 0x7F)]
        public void ClearingABitProducesANewObjectWithThatBitCleared(ushort bit, byte expected)
        {
            var allBitsSet = Octet.Max;
            var oneBitClear = allBitsSet.WithBitSetTo(bit, false);
            var expectedOctet = Octet.FromUnsignedInt(expected);
            oneBitClear.Should().NotBeSameAs(allBitsSet, "octets are immutable");
            oneBitClear.Should().Be(expectedOctet, "clearing a bit should only affect that bit");
        }

        [Fact]
        public void BitwiseAndOperatorShouldMaskBits()
        {
            var mask = Octet.FromInt(0xAA);
            var andResult = Octet.Max & mask;
            andResult.Should().Be(mask, "bitwise AND should clear set");
        }

        [Fact]
        public void BitwiseOrOperatorShouldSetBits()
        {
            var mask = Octet.FromInt(0x0F);
            var orResult = Octet.Zero | mask;
            orResult.Should().Be(mask, "bitwise OR should set bits");
        }
    }
}