using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This class decodes data from Base64 content transfer encoding type into enumerable of bytes
    /// </summary>
    [ContentTransferDecoderType("base64", typeof(byte))]
    internal class Base64Decoder : IContentTransferDecoder<byte>
    {
        public string ContentTransferEncodingName
        {
            get { return "base64"; }
        }

        public IEnumerable<byte> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            if (multipartFormDataItem.Content == null) return Enumerable.Empty<byte>();
            var chars = Encoding.ASCII.GetChars(multipartFormDataItem.Content).ToArray();
            return Convert.FromBase64CharArray(chars, 0, chars.Length);
        }
    }
}
