// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: Octet.cs  Last modified: 2017-03-06@18:13 by Tim Long

using System;
using System.Diagnostics.Contracts;
using System.Text;
using PostSharp.Patterns.Contracts;

namespace TA.VellemanK8056.DeviceInterface
    {
    /// <summary>
    ///     An immutable representation of an 8 bit byte, with each bit individually addressable. In
    ///     most cases an Octet is interchangeable with a <see cref="byte" /> (implicit conversion
    ///     operators are provided). An Octet can be explicitly converted (cast) to or from an
    ///     integer.
    /// </summary>
    public struct Octet : IEquatable<Octet>
        {
        private readonly bool[] bits;

        [ContractInvariantMethod]
        private void ObjectInvariant()
            {
            Contract.Invariant(bits != null);
            Contract.Invariant(bits.Length == 8, "Consider using Octet.FromInt() instead of new Octet()");
            }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Octet" /> struct from an array of at least 8 booleans.
        /// </summary>
        /// <param name="bits">The bits; there must be exactly 8.</param>
        private Octet([Required] bool[] bits)
            {
            Contract.Requires(bits != null);
            Contract.Requires(bits.Length == 8);
            this.bits = bits;
            }

        /// <summary>
        ///     Gets an Octet with all the bits set to zero.
        /// </summary>
        public static Octet Zero { get; } = FromInt(0);

        /// <summary>
        ///     Gets an Octet set to the maximum value (i.e. all the bits set to one).
        /// </summary>
        public static Octet Max { get; } = FromInt(0xFF);

        public bool this[int bit]
            {
            get
                {
                Contract.Requires(bit >= 0 && bit < 8);
                return bits[bit];
                }
            }

        /// <summary>
        ///     Factory method: create an Octet from an integer.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Octet.</returns>
        public static Octet FromInt(int source)
            {
            var bits = new bool[8];
            for (var i = 0; i < 8; i++)
                {
                var bit = source & 0x01;
                bits[i] = bit != 0;
                source >>= 1;
                }
            return new Octet(bits);
            }

        /// <summary>
        ///     Factory method: create an Octet from an unisgned integer.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Octet.</returns>
        public static Octet FromUnsignedInt(uint source)
            {
            return FromInt((int) source);
            }

        /// <summary>
        ///     Returns a new octet with the specified bit number set to the specified value.
        ///     Other bits are duplicated.
        /// </summary>
        /// <param name="bit">The bit number to be modified.</param>
        /// <param name="value">The value of the specified bit number.</param>
        /// <returns>A new octet instance with the specified bit number set to the specified value.</returns>
        public Octet WithBitSetTo(ushort bit, bool value)
            {
            Contract.Requires(bit < 8);
            var newBits = new bool[8];
            bits.CopyTo(newBits, 0);
            newBits[bit] = value;
            return new Octet(newBits);
            }

        /// <summary>
        ///     Returns a new octet with the specified bit number set to the specified value.
        ///     Other bits are duplicated.
        /// </summary>
        /// <param name="bit">The bit number to be modified.</param>
        /// <param name="value">The value of the specified bit number.</param>
        /// <returns>A new octet instance with the specified bit number set to the specified value.</returns>
        public Octet WithBitSetTo(int bit, bool value)
            {
            Contract.Requires(bit >= 0 && bit < 8);
            return WithBitSetTo((ushort) bit, value);
            }

        public override string ToString()
            {
            var builder = new StringBuilder();
            for (var i = 7; i >= 0; i--)
                {
                builder.Append(bits[i] ? '1' : '0');
                builder.Append(' ');
                }
            builder.Length -= 1;
            return builder.ToString();
            }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="uint" /> to <see cref="Octet" />.
        ///     This conversion is explicit because there is potential loss of information.
        /// </summary>
        /// <param name="integer">The integer.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Octet(uint integer)
            {
            return FromUnsignedInt(integer);
            }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="int" /> to <see cref="Octet" />.
        ///     This conversion is explicit because there is potential loss of information.
        /// </summary>
        /// <param name="integer">The integer.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Octet(int integer)
            {
            return FromInt(integer);
            }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Octet" /> to <see cref="byte" />.
        /// </summary>
        /// <param name="octet">The octet.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte(Octet octet)
            {
            var sum = 0;
            for (var i = 0; i < 8; i++)
                {
                if (octet[i]) sum += 1 << i;
                }
            return (byte) sum;
            }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="byte" /> to <see cref="Octet" />.
        /// </summary>
        /// <param name="b">The input byte.</param>
        /// <returns>The result of the conversion in a new Octet.</returns>
        public static implicit operator Octet(byte b)
            {
            return FromUnsignedInt(b);
            }

        /// <summary>
        ///     Indicates whether this octet is equal to another octet, using value semantics.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Octet other)
            {
            for (var i = 0; i < bits.Length; i++)
                {
                if (bits[i] != other[i]) return false;
                }
            return true;
            }

        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Octet && Equals((Octet) obj);
            }

        public override int GetHashCode()
            {
            return bits.GetHashCode();
            }

        public static bool operator ==(Octet left, Octet right)
            {
            return left.Equals(right);
            }

        public static bool operator !=(Octet left, Octet right)
            {
            return !left.Equals(right);
            }

        public static Octet operator &(Octet left, Octet right)
            {
            var result = (bool[]) left.bits.Clone();
            for (var i = 0; i < 8; i++)
                {
                result[i] &= right[i];
                }
            return new Octet(result);
            }

        public static Octet operator |(Octet left, Octet right)
            {
            var result = (bool[]) left.bits.Clone();
            for (var i = 0; i < 8; i++)
                {
                result[i] |= right[i];
                }
            return new Octet(result);
            }
        }
    }