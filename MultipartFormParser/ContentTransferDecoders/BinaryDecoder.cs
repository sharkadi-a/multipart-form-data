using System.Collections.Generic;
using System.Linq;

namespace MultipartFormParser.ContentTransferDecoders
{
    [ContentTransferDecoderType("binary", typeof(byte))]
    internal class BinaryDecoder : IContentTransferDecoder<byte>
    {
        public string ContentTransferEncodingName { get { return "binary"; } }
        public IEnumerable<byte> Decode(MultipartFormDataItem multipartFormDataItem)
        {
            return multipartFormDataItem.Content ?? Enumerable.Empty<byte>();
        }
    }
}
