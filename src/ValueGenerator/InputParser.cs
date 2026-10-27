// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Text;

using ValueGenerator.Base64;
using ValueGenerator.GUID;
using ValueGenerator.Hashing;
using ValueGenerator.Uri;

namespace ValueGenerator
{
    public class InputParser
    {
        private readonly Func<string>? _readClipboard;

        public InputParser(Func<string>? readClipboard = null)
        {
            _readClipboard = readClipboard;
        }

        private string ReadClipboardText()
        {
            var text = _readClipboard?.Invoke();
            if (string.IsNullOrEmpty(text))
            {
                throw new FormatException("Clipboard is empty, unavailable, or does not contain text.");
            }

            return text;
        }

        public IComputeRequest ParseInput(string query)
        {
            IComputeRequest request;

            ArgumentNullException.ThrowIfNull(query);
            query = query.TrimStart();
            var terms = query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (terms.Length == 0)
            {
                throw new FormatException("Empty request");
            }

            string command = terms[0];

            if (command.Equals("md5", StringComparison.OrdinalIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();

                if (content == string.Empty)
                {
                    content = ReadClipboardText();
                }

                request = new HashRequest(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(content));
            }
            else if (command.StartsWith("sha", StringComparison.InvariantCultureIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();
                HashAlgorithmName algorithmName;

                switch (command.Substring(3))
                {
                    case "1":
                        algorithmName = HashAlgorithmName.SHA1;
                        break;

                    case "256":
                        algorithmName = HashAlgorithmName.SHA256;
                        break;

                    case "384":
                        algorithmName = HashAlgorithmName.SHA384;
                        break;

                    case "512":
                        algorithmName = HashAlgorithmName.SHA512;
                        break;
                    default:
                        throw new FormatException("Unknown SHA variant. Supported variants: SHA1, SHA256, SHA384, SHA512");
                }

                if (content == string.Empty)
                {
                    content = ReadClipboardText();
                }

                request = new HashRequest(algorithmName, Encoding.UTF8.GetBytes(content));
            }
            else if (command.StartsWith("guid", StringComparison.InvariantCultureIgnoreCase) ||
                     command.StartsWith("uuid", StringComparison.InvariantCultureIgnoreCase))
            {
                string content = query.Substring(command.Length).Trim();

                // Default to version 4
                int version = 4;
                string versionQuery = command.Substring(4);

                if (versionQuery.Length > 0)
                {
                    if (versionQuery.StartsWith("v", StringComparison.InvariantCultureIgnoreCase))
                    {
                        versionQuery = versionQuery.Substring(1);
                    }

                    if (!int.TryParse(versionQuery, null, out version))
                    {
                        throw new FormatException("Could not determine requested GUID version. Supported versions are 1, 3, 4, 5, and 7");
                    }
                }

                if (version == 3 || version == 5)
                {
                    string[] sParameters = content.Split(" ");

                    if (sParameters.Length != 2)
                    {
                        throw new ArgumentException($"GUID version {version} require 2 parameters - a namespace GUID and a name.\nExample: uuidv{version} ns:<DNS, URL, OID, or X500> <your input>");
                    }

                    string namespaceParameter = sParameters[0];
                    string nameParameter = sParameters[1];

                    request = new GUIDRequest(version, namespaceParameter, nameParameter);
                }
                else
                {
                    request = new GUIDRequest(version);
                }
            }
            else if (command.Equals("ulid", StringComparison.OrdinalIgnoreCase))
            {
                request = new Generators.Ulid.UlidRequest();
            }
            else if (command.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();
                request = new Base64Request(Encoding.UTF8.GetBytes(content));
            }
            else if (command.Equals("base64d", StringComparison.OrdinalIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();
                request = new Base64DecodeRequest(content);
            }
            else if (command.StartsWith("esc:", StringComparison.OrdinalIgnoreCase))
            {
                // Escape things
                if (command.Equals("esc:data", StringComparison.OrdinalIgnoreCase))
                {
                    int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                    string content = query.Substring(commandIndex + command.Length).Trim();
                    request = new DataEscapeRequest(content);
                }
                else if (command.Equals("esc:hex", StringComparison.OrdinalIgnoreCase))
                {
                    int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                    string content = query.Substring(commandIndex + command.Length).Trim();

                    // This is only for single chars
                    if (content.Length > 1)
                    {
                        throw new ArgumentException($"Invalid Query: {query} (Too many characters.)");
                    }
                    else if (content.Length == 0)
                    {
                        throw new FormatException($"Invalid Query: {query}");
                    }

                    request = new HexEscapeRequest(content);
                }
                else
                {
                    throw new FormatException($"Invalid Query: {query}");
                }
            }
            else if (command.StartsWith("uesc:", StringComparison.OrdinalIgnoreCase))
            {
                // Unescape things
                if (command.Equals("uesc:data", StringComparison.OrdinalIgnoreCase))
                {
                    int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                    string content = query.Substring(commandIndex + command.Length).Trim();
                    request = new DataUnescapeRequest(content);
                }
                else if (command.Equals("uesc:hex", StringComparison.OrdinalIgnoreCase))
                {
                    int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                    string content = query.Substring(commandIndex + command.Length).Trim();
                    request = new HexUnescapeRequest(content);
                }
                else
                {
                    throw new FormatException($"Invalid Query: {query}");
                }
            }
            else if (command.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();
                request = new UrlEncodeRequest(content);
            }
            else if (command.Equals("urld", StringComparison.OrdinalIgnoreCase))
            {
                int commandIndex = query.IndexOf(command, StringComparison.InvariantCultureIgnoreCase);
                string content = query.Substring(commandIndex + command.Length).Trim();
                request = new UrlDecodeRequest(content);
            }
            else
            {
                throw new FormatException($"Invalid Query: {query}");
            }

            return request;
        }
    }
}
