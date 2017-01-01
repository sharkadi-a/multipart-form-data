using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    internal class _8BitDecoder : IContentTransferDecoder<char>
    {
        [ContentTransferDecoderType("8bit", typeof(char))]
        public string ContentTransferEncodingName { get { return "8bit"; } }
        public IEnumerable<char> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            if (multipartFormDataItem.Content == null) return Enumerable.Empty<char>();
            return Encoding.GetEncoding(28591).GetChars(multipartFormDataItem.Content);
        }
    }
}
