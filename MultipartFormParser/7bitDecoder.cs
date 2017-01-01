using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipartFormParser
{
    internal class _7BitDecoder : IContentTransferDecoder<char>
    {
        [ContentTransferDecoderType("7bit", typeof(char))]
        public string ContentTransferEncodingName
        {
            get { return "7bit"; }
        }

        public IEnumerable<char> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            if (multipartFormDataItem == null) throw new ArgumentNullException("multipartFormDataItem");
            if (multipartFormDataItem.Content == null) return Enumerable.Empty<char>();
            return Encoding.ASCII.GetChars(multipartFormDataItem.Content);
        }
    }
}
