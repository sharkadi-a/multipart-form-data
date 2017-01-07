using System.Collections.Generic;
using System.Linq;

namespace MultipartFormParser.ContentTransferDecoders
{
    /// <summary>
    /// This class decodes data from binary content transfer encoding into array of bytes. Naturally, there is no difference between 7bit, 8bit or binary transfer encodings for transfer environment which is working with data as s set of bytes
    /// </summary>
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
