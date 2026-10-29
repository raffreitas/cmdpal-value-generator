// Copyright (c) 2026 raffreitas
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;

namespace ValueGenerator.Generators.Ulid;

public sealed class UlidRequest : IComputeRequest
{
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

    public byte[] Result { get; set; } = [];
    public string Description => "ULID: Time-ordered randomly generated identifier";
    public bool IsSuccessful { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public bool Compute()
    {
        Result = RandomNumberGenerator.GetBytes(16);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        for (var index = 5; index >= 0; index--)
        {
            Result[index] = (byte)timestamp;
            timestamp >>= 8;
        }

        IsSuccessful = true;
        return true;
    }

    public string ResultToString()
    {
        if (!IsSuccessful)
        {
            return ErrorMessage;
        }

        // A ULID has 128 bits; its 26 Base32 characters have two leading zero bits.
        Span<char> encoded = stackalloc char[26];
        for (var digit = 0; digit < encoded.Length; digit++)
        {
            var value = 0;
            for (var bit = 0; bit < 5; bit++)
            {
                var sourceBit = (digit * 5) + bit - 2;
                value <<= 1;
                if (sourceBit >= 0)
                {
                    value |= (Result[sourceBit / 8] >> (7 - (sourceBit % 8))) & 1;
                }
            }

            encoded[digit] = Alphabet[value];
        }

        return new string(encoded);
    }
}
