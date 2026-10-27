// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;

namespace ValueGenerator.Uri
{
    public class HexEscapeRequest : IComputeRequest
    {
        public byte[] Result { get; set; } = [];

        public string Description => "Hex escaped char";

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        private string DataToEscape { get; set; }

        public HexEscapeRequest(string dataToEscape)
        {
            DataToEscape = dataToEscape ?? throw new ArgumentNullException(nameof(dataToEscape));

            // Validate that we have only one character
            if (dataToEscape.Length != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dataToEscape));
            }
        }

        public bool Compute()
        {
            IsSuccessful = true;
            try
            {
                char charToEscape = DataToEscape[0];
                Result = Encoding.UTF8.GetBytes(System.Uri.HexEscape(charToEscape));
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                IsSuccessful = false;
            }

            return IsSuccessful;
        }

        public string ResultToString()
        {
            return Encoding.UTF8.GetString(Result);
        }
    }
}
