// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2016 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: StringExtensions.cs  Last modified: 2016-07-27@17:32 by Tim Long

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace ASCOM.K8056
    {
    public static class StringExtensions
        {
        private static readonly string[] AsciiEncoding =
            {
            "", "", "", "", "", "", "", "\a", "\b", "\t", "\n", "\v", "\f",
            "\r", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", " ", "!", "\"", "#", "$", "%",
            "&", "\'", "(", ")", "*", "+", ",", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":",
            ";", "<", "=", ">", "?", "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
            "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_", "`", "a", "b", "c", "d", "e",
            "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{",
            "|", "}", "~", ""
            };

        private static readonly IDictionary<int, string> asciiMnemonics = new Dictionary<int, string>
            {
            {0x00, "<NULL>"},
            {0x01, "<SOH>"},
            {0x02, "<STH>"},
            {0x03, "<ETX>"},
            {0x04, "<EOT>"},
            {0x05, "<ENQ>"},
            {0x06, "<ACK>"},
            {0x07, "<BELL>"},
            {0x08, "<BS>"},
            {0x09, "<HT>"},
            {0x0A, "<LF>"},
            {0x0B, "<VT>"},
            {0x0C, "<FF>"},
            {0x0D, "<CR>"},
            {0x0E, "<SO>"},
            {0x0F, "<SI>"},
            {0x11, "<DC1>"},
            {0x12, "<DC2>"},
            {0x13, "<DC3>"},
            {0x14, "<DC4>"},
            {0x15, "<NAK>"},
            {0x16, "<SYN>"},
            {0x17, "<ETB>"},
            {0x18, "<CAN>"},
            {0x19, "<EM>"},
            {0x1A, "<SUB>"},
            {0x1B, "<ESC>"},
            {0x1C, "<FS>"},
            {0x1D, "<GS>"},
            {0x1E, "<RS>"},
            {0x1F, "<US>"},
            //{ 0x20, "<SP>" },
            {0x7F, "<DEL>"}
            };

        public static string GetString(this byte[] bytes)
            {
            Contract.Requires(bytes != null);
            Contract.Ensures(Contract.Result<string>() != null);
            var builder = new StringBuilder(bytes.Length);
            for (var i = 0; i < bytes.Length; i++)
                builder.Append(AsciiEncoding[bytes[i] & 0x7F]);
            return builder.ToString();
            }

        /// <summary>
        ///     Utility function. Expands non-printable ASCII characters into mnemonic human-readable form.
        /// </summary>
        /// <returns>
        ///     Returns a new string with non-printing characters replaced by human-readable mnemonics.
        /// </returns>
        public static string ExpandASCII(this string inputString)
            {
            Contract.Requires(inputString != null);
            Contract.Ensures(Contract.Result<string>() != null);
            var expanded = new StringBuilder(inputString.Length);
            foreach (var c in inputString)
                expanded.Append(c.ExpandASCII());
            return expanded.ToString();
            }

        /// <summary>
        ///     Utility function. Expands non-printable ASCII characters into mnemonic human-readable form.
        ///     printable characters are returned unmodified.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///     If the original character is a non-printing ASCII character, then returns a string containing a
        ///     human-readable mnemonic for that ASCII character,
        ///     <example>
        ///         0x07 returns &lt;BELL&gt;
        ///     </example>
        ///     .
        ///     Otherwise, returns the original character converted to a string.
        /// </returns>
        public static string ExpandASCII(this char c)
            {
            Contract.Ensures(Contract.Result<string>() != null);
            int asciiCode = c;
            return asciiMnemonics.ContainsKey(asciiCode) ? asciiMnemonics[asciiCode] : c.ToString();
            }

        public static bool CaseInsensitiveEquals(this string lhs, string rhs)
            {
            Contract.Requires(lhs != null);
            Contract.Requires(rhs != null);
            return string.Equals(lhs.ToLower(), rhs.ToLower());
            }

        public static bool EndsWith(this string s, string value)
            {
            Contract.Requires(s != null);
            Contract.Requires(value != null);
            return s.IndexOf(value) == s.Length - value.Length;
            }

        public static bool StartsWith(this string s, string value)
            {
            Contract.Requires(s != null);
            Contract.Requires(value != null);
            return s.IndexOf(value) == 0;
            }

        public static bool Contains(this string s, string value)
            {
            Contract.Requires(s != null);
            Contract.Requires(value != null);
            return s.IndexOf(value) >= 0;
            }

        /// <summary>
        ///     Returns the specified number of characters from the head of a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">The number of characters to be returned, must not be greater than the length of the string.</param>
        /// <returns>The specified number of characters from the head of the source string, as a new string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the requested number of characters exceeds the string length.</exception>
        public static string Head(this string source, int length)
            {
            if (length > source.Length)
                {
                throw new ArgumentOutOfRangeException("source",
                    "The specified length is greater than the length of the string.");
                }
            return source.Substring(0, length);
            }

        /// <summary>
        ///     Returns the specified number of characters from the tail of a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">The number of characters to be returned, must not be greater than the length of the string.</param>
        /// <returns>The specified number of characters from the tail of the source string, as a new string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the requested number of characters exceeds the string length.</exception>
        public static string Tail(this string source, int length)
            {
            var srcLength = source.Length;
            if (length > srcLength)
                {
                throw new ArgumentOutOfRangeException("source",
                    "The specified length is greater than the length of the string.");
                }
            return source.Substring(srcLength - length, length);
            }

        /// <summary>
        ///     Keeps only the wanted (that is, removes all unwanted characters) from the string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="keep">A list of the wanted characters. All other characters will be removed.</param>
        /// <returns>
        ///     A new string with all of the unwanted characters deleted. Returns <see cref="string.Empty" /> if all
        ///     the characters were deleted or if the source string was null or empty.
        /// </returns>
        /// <seealso cref="Clean" />
        public static string Keep(this string source, string keep)
            {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var cleanString = new StringBuilder(source.Length);
            foreach (var ch in source)
                {
                if (keep.Contains(ch))
                    cleanString.Append(ch);
                }
            return cleanString.ToString();
            }

        /// <summary>
        ///     Removes all unwanted characters from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="clean">A list of the unwanted characters. All other characters will be preserved.</param>
        /// <returns>
        ///     A new string with all of the unwanted characters deleted. Returns <see cref="string.Empty" />
        ///     if all of the characters were deleted or if the source string was null or empty.
        /// </returns>
        /// <remarks>
        ///     Contrast with <see cref="Keep" />
        /// </remarks>
        /// <seealso cref="Keep" />
        public static string Clean(this string source, string clean)
            {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var cleanString = new StringBuilder(source.Length);
            foreach (var ch in source)
                {
                if (!clean.Contains(ch))
                    cleanString.Append(ch);
                }
            return cleanString.ToString();
            }

        /// <summary>
        ///     Remove the head of the string, leaving the tail.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">Number of characters to remove from the head.</param>
        /// <returns>A new string containing the old string with <paramref name="length" /> characters removed from the head.</returns>
        public static string RemoveHead(this string source, int length)
            {
            if (length < 1)
                return source;
            return source.Tail(source.Length - length);
            }

        /// <summary>
        ///     Remove the tail of the string, leaving the head.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="length">Number of characters to remove from the tail.</param>
        /// <returns>A new string containing the old string with <paramref name="length" /> characters removed from the tail.</returns>
        public static string RemoveTail(this string source, int length)
            {
            if (length < 1)
                return source;
            return source.Head(source.Length - length);
            }

        /// <summary>
        ///     Converts a tring to a hex representation, suitable for display in a debugger.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>A new string showing each character of the source string as a hex digit.</returns>
        public static string ToHex(this string source)
            {
            const string formatWithSeperator = ", {0,2:x}";
            const string formatNoSeperator = "{0,2:x}";
            var hexString = new StringBuilder(source.Length * 7);
            hexString.Append('{');
            var seperator = false;
            foreach (var ch in source)
                {
                hexString.AppendFormat(seperator ? formatWithSeperator : formatNoSeperator, (int) ch);
                seperator = true;
                }
            hexString.Append('}');
            return hexString.ToString();
            }
        }
    }