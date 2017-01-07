using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This class decodes data from 8bit content transfer encoding into array of characters. Such decoder should be used by transfer environments, which treat data as a stream of characters, not bytes
    /// </summary>
    [ContentTransferDecoderType("8bit", typeof(char))]
    internal class Char8BitDecoder : IContentTransferDecoder<char>
    {
        public string ContentTransferEncodingName { get { return "8bit"; } }
        public IEnumerable<char> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            if (multipartFormDataItem.Content == null) return Enumerable.Empty<char>();
            return Encoding.GetEncoding(28591).GetChars(multipartFormDataItem.Content);
        }
    }
}
