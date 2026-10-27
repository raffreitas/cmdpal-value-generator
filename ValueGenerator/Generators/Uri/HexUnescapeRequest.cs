// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace ValueGenerator.Uri
{
    public class HexUnescapeRequest : IComputeRequest
    {
        public byte[] Result { get; set; } = [];

        public string Description => "Hex char unescaped";

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        private string DataToUnescape { get; set; }

        public HexUnescapeRequest(string dataToUnescape)
        {
            DataToUnescape = dataToUnescape ?? throw new ArgumentNullException(nameof(dataToUnescape));
        }

        public bool Compute()
        {
            IsSuccessful = true;
            try
            {
                int index = 0;
                if (System.Uri.IsHexEncoding(DataToUnescape, index))
                {
                    Result = Encoding.UTF8.GetBytes(System.Uri.HexUnescape(DataToUnescape, ref index).ToString());
                }
                else
                {
                    ErrorMessage = "Invalid hexadecimal escape. Example: uesc:hex %7A";
                    IsSuccessful = false;
                }
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
            if (Result != null)
            {
                return Encoding.UTF8.GetString(Result);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
