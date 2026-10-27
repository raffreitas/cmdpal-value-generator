// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

using System;

namespace ValueGenerator.Helpers
{
    /// <summary>
    /// Helper class to easier work with queries
    /// </summary>
    internal static class CommandCatalog
    {
        /// <summary>
        /// a list of all value generator descriptions
        /// </summary>
        private const string GeneratorDescriptionUuid = GeneratorStrings.generator_description_uuid;
        private const string GeneratorDescriptionUuidv1 = GeneratorStrings.generator_description_uuidv1;
        private const string GeneratorDescriptionUuidv3 = GeneratorStrings.generator_description_uuidv3;
        private const string GeneratorDescriptionUuidv4 = GeneratorStrings.generator_description_uuidv4;
        private const string GeneratorDescriptionUuidv5 = GeneratorStrings.generator_description_uuidv5;
        private const string GeneratorDescriptionUuidv7 = GeneratorStrings.generator_description_uuidv7;
        private const string GeneratorDescriptionHash = GeneratorStrings.generator_description_hash;
        private const string GeneratorDescriptionBase64 = GeneratorStrings.generator_description_base64;
        private const string GeneratorDescriptionBase64d = GeneratorStrings.generator_description_base64d;
        private const string GeneratorDescriptionUrl = GeneratorStrings.generator_description_url;
        private const string GeneratorDescriptionUrld = GeneratorStrings.generator_description_urld;
        private const string GeneratorDescriptionEscData = GeneratorStrings.generator_description_esc_data;
        private const string GeneratorDescriptionUescData = GeneratorStrings.generator_description_uesc_data;
        private const string GeneratorDescriptionEscHex = GeneratorStrings.generator_description_esc_hex;
        private const string GeneratorDescriptionUescHex = GeneratorStrings.generator_description_uesc_hex;
        private const string GeneratorDescriptionYourInput = GeneratorStrings.generator_description_your_input;
        private const string GeneratorExample = GeneratorStrings.generator_example;
        private const string Or = GeneratorStrings.or;

        private static string GetStringFormat(string value, string arg)
        {
            return string.Format(CultureInfo.CurrentCulture, value, arg);
        }

        private static string GetStringFormat(string value)
        {
            return string.Format(CultureInfo.CurrentCulture, value);
        }

        internal static string GetResultTitle(GeneratorData generatorData)
        {
            return $"{generatorData.Keyword} - {generatorData.Description}";
        }

        internal static string GetResultSubtitle(GeneratorData generatorData)
        {
            return GetStringFormat(GeneratorExample, generatorData.Example);
        }

        /// <summary>
        /// A list that contain all of the value generators and its descriptions
        /// </summary>
        internal static readonly List<GeneratorData> GeneratorDataList =
        [
            new()
            {
                Keyword = "guid",
                Description = GetStringFormat(GeneratorDescriptionUuid),
                Example = "guid (aliases: uuid, guid4, guidv4, uuid4, uuidv4)",
            },
            new()
            {
                Keyword = "guid1",
                Description = GetStringFormat(GeneratorDescriptionUuidv1),
                Example = "guid1 (aliases: guidv1, uuid1, uuidv1)",
            },
            new()
            {
                Keyword = "guid3",
                Description = GetStringFormat(GeneratorDescriptionUuidv3),
                Example = $"guid3 ns:<DNS, URL, OID, {GetStringFormat(Or)} X500> <{GetStringFormat(GeneratorDescriptionYourInput)}>",
            },
            new()
            {
                Keyword = "guid4",
                Description = GetStringFormat(GeneratorDescriptionUuidv4),
                Example = "guid4 (aliases: guidv4, uuid4, uuidv4)",
            },
            new()
            {
                Keyword = "guid5",
                Description = GetStringFormat(GeneratorDescriptionUuidv5),
                Example = $"guid5 ns:<DNS, URL, OID, {GetStringFormat(Or)} X500> <{GetStringFormat(GeneratorDescriptionYourInput)}>",
            },
            new()
            {
                Keyword = "guid7",
                Description = GetStringFormat(GeneratorDescriptionUuidv7),
                Example = "guid7 (aliases: guidv7, uuid7, uuidv7)",
            },
            new()
            {
                Keyword = "md5",
                Description = GetStringFormat(GeneratorDescriptionHash, "MD5"),
                Example = "md5 <your input>",
            },
            new()
            {
                Keyword = "sha1",
                Description = GetStringFormat(GeneratorDescriptionHash, "SHA1"),
                Example = "sha1 <your input>",
            },
            new()
            {
                Keyword = "sha256",
                Description = GetStringFormat(GeneratorDescriptionHash, "SHA256"),
                Example = "sha256 <your input>",
            },
            new()
            {
                Keyword = "sha384",
                Description = GetStringFormat(GeneratorDescriptionHash, "SHA384"),
                Example = "sha384 <your input>",
            },
            new()
            {
                Keyword = "sha512",
                Description = GetStringFormat(GeneratorDescriptionHash, "SHA512"),
                Example = "sha512 <your input>",
            },
            new()
            {
                Keyword = "base64",
                Description = GetStringFormat(GeneratorDescriptionBase64),
                Example = $"base64 <{GetStringFormat(GeneratorDescriptionYourInput)}>",
            },
            new()
            {
                Keyword = "base64d",
                Description = GetStringFormat(GeneratorDescriptionBase64d),
                Example = $"base64d <{GetStringFormat(GeneratorDescriptionYourInput)}>",
            },
            new()
            {
                Keyword = "url",
                Description = GetStringFormat(GeneratorDescriptionUrl),
                Example = "url https://bing.com/?q=My Test query",
            },
            new()
            {
                Keyword = "urld",
                Description = GetStringFormat(GeneratorDescriptionUrld),
                Example = "urld https://bing.com/?q=My+Test+query",
            },
            new()
            {
                Keyword = "esc:data",
                Description = GetStringFormat(GeneratorDescriptionEscData),
                Example = "esc:data C:\\Program Files\\PowerToys\\PowerToys.exe",
            },
            new()
            {
                Keyword = "uesc:data",
                Description = GetStringFormat(GeneratorDescriptionUescData),
                Example = "uesc:data C%3A%5CProgram%20Files%5CPowerToys%5CPowerToys.exe",
            },
            new()
            {
                Keyword = "esc:hex",
                Description = GetStringFormat(GeneratorDescriptionEscHex),
                Example = "esc:hex z",
            },
            new()
            {
                Keyword = "uesc:hex",
                Description = GetStringFormat(GeneratorDescriptionUescHex),
                Example = "uesc:hex %7A",
            },
        ];
    }
}
