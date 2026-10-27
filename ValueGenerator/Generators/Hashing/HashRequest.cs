// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Text;

namespace ValueGenerator.Hashing
{
    public class HashRequest : IComputeRequest
    {
        public byte[] Result { get; set; } = [];

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public string Description
        {
            get
            {
                return $"{AlgorithmName}({Encoding.UTF8.GetString(DataToHash)})";
            }
        }

        public HashAlgorithmName AlgorithmName { get; set; }

        private byte[] DataToHash { get; set; }

        public HashRequest(HashAlgorithmName algorithmName, byte[] dataToHash)
        {
            AlgorithmName = algorithmName;
            DataToHash = dataToHash ?? throw new ArgumentNullException(nameof(dataToHash));
        }

        public bool Compute()
        {
            if (DataToHash == null)
            {
                ErrorMessage = "Null data passed to hash request";

                IsSuccessful = false;
            }
            else
            {
#pragma warning disable CA5350, CA5351 // Required for the MD5/SHA1 generator commands.
                Result = AlgorithmName.Name switch
                {
                    "MD5" => MD5.HashData(DataToHash),
                    "SHA1" => SHA1.HashData(DataToHash),
                    "SHA256" => SHA256.HashData(DataToHash),
                    "SHA384" => SHA384.HashData(DataToHash),
                    "SHA512" => SHA512.HashData(DataToHash),
                    _ => throw new ArgumentException("Unsupported hash algorithm"),
                };
#pragma warning restore CA5350, CA5351
                IsSuccessful = true;
            }

            return IsSuccessful;
        }

        public string ResultToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in Result)
            {
                sb.Append(b.ToString("X2", null));
            }

            return sb.ToString();
        }
    }
}
